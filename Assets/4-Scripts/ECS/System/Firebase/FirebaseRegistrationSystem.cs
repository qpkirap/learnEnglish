﻿using System;
using Cysharp.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Game.Config;
using Game.ECS_UI.Components;
using Game.ECS.Components;
using GooglePlayGames;
using GooglePlayGames.BasicApi.Nearby;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Game.ECS.System
{
    [UpdateAfter(typeof(InitSystem))]
    public partial class FirebaseRegistrationSystem : InitSystemBase
    {
        private DatabaseReference reference;
        
        private UICanvasController canvas;
        private FirebaseApp app;
        private FirebaseAuth auth;
        private bool isInit;
        private bool isActiveAsync;

        protected override void OnStartRunning()
        {
            reference = FirebaseDatabase.DefaultInstance.RootReference;

            CheckDependenciesAsync();
        }

        private async UniTask CheckDependenciesAsync()
        {
            var handle = await FirebaseApp.CheckDependenciesAsync();

            Debug.Log($"CheckDependenciesAsync status{handle}");

            if (handle == DependencyStatus.Available)
            {
                auth = Firebase.Auth.FirebaseAuth.DefaultInstance;

                app = FirebaseApp.DefaultInstance;

                var entity = EntityManager.CreateEntity();

                EntityManager.AddComponentData(entity, new FirebaseReadyDependenciesTag());
            }
        }

        protected override void OnUpdate()
        {
            if (isActiveAsync) return;
            
            if (!HasSingleton<InitAllFabricsTag>()) return;
            if (!HasSingleton<FirebaseReadyDependenciesTag>()) return;
            //if (!HasSingleton<UICanvasController>()) return;

            if (isInit) return;

            if (app == null) return;

            Entities
                .WithAll<FirebaseRegistrationData>()
                .WithNone<AsyncTag, FirebaseRegistrationCompleteTag>()
                .ForEach((Entity e, FirebaseRegistrationData registration) =>
                {
                    EntityManager.AddComponentData(e, new AsyncTag());

                    CreateUserAsync(e, registration.email, registration.pass, registration.nick);

                    //TryAuthPlayServices();
                }).WithStructuralChanges().WithoutBurst().Run();

            Entities
                .WithAll<FirebaseReadyDependenciesTag>()
                .WithNone<AsyncTag, FirebaseRegistrationCompleteTag>()
                .ForEach((Entity appEntity) =>
                {
                    var stateEntity = GetSingletonEntity<GameState>();

                    if (stateEntity == Entity.Null) return;

                    var dataState = EntityManager.GetComponentData<GameState>(stateEntity);

                    if (dataState == null) return;

                    if (!string.IsNullOrEmpty(dataState.UserState.Email))
                    {
                        EntityManager.AddComponentData(appEntity, new AsyncTag());

                        SignInUserAsync(appEntity, dataState.UserState.Email, dataState.UserState.Pass, dataState.UserState.Nick);

                        return;

                        //TryAuthPlayServices();
                    }

                    TryCreateRegistrationPanel();
                }).WithStructuralChanges().WithoutBurst().Run();
            
            UpdateCurrentNick();
        }

        private void UpdateCurrentNick()
        {
            Entities
                .WithAll<FirebaseReadyDependenciesTag>().WithNone<SetCurrentNickTag>()
                .ForEach((ref Entity e) =>
                {
                    var stateEntity = GetSingletonEntity<GameState>();

                    if (stateEntity == Entity.Null) return;

                    var dataState = EntityManager.GetComponentData<GameState>(stateEntity);

                    if (dataState == null) return;

                    var controllerE = GetSingletonEntity<LeaderBottomBoardController>();
                    var controller = EntityManager.GetComponentData<LeaderBottomBoardController>(controllerE);

                    controller.currentNick.text = dataState.UserState.Nick;

                    EntityManager.AddComponentData(e, new SetCurrentNickTag());

                }).WithStructuralChanges().WithoutBurst().Run();
        }

        #region PlayServicesAuth

        private void TryAuthPlayServices()
        {
            PlayGamesPlatform.InitializeNearby(CallbackPlayServices);
        }

        private void CallbackPlayServices(INearbyConnectionClient obj)
        {
            Debug.Log("Nearby connections initialized");

            Social.localUser.Authenticate((bool success) =>
            {
                Debug.Log($"Social auth {success}");

                if (success)
                {
                    PlayGamesPlatform.Instance.RequestServerSideAccess(
                        /* forceRefreshToken= */ false,
                        authCode =>
                        {
                            // send code to server

                            Debug.Log($"authCode {authCode}");

                            Firebase.Auth.FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
                            Firebase.Auth.Credential credential =
                                Firebase.Auth.PlayGamesAuthProvider.GetCredential(authCode);

                            auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
                            {
                                if (task.IsCanceled)
                                {
                                    Debug.LogError("SignInWithCredentialAsync was canceled.");
                                    return;
                                }

                                if (task.IsFaulted)
                                {
                                    Debug.LogError("SignInWithCredentialAsync encountered an error: " + task.Exception);
                                    return;
                                }

                                Firebase.Auth.FirebaseUser newUser = task.Result;
                                Debug.LogFormat("User signed in successfully: {0} ({1})",
                                    newUser.DisplayName, newUser.UserId);
                            });

                            Firebase.Auth.FirebaseUser user = auth.CurrentUser;
                            if (user != null)
                            {
                                string playerName = user.DisplayName;

                                // The user's Id, unique to the Firebase project.
                                // Do NOT use this value to authenticate with your backend server, if you
                                // have one; use User.TokenAsync() instead.
                                string uid = user.UserId;

                                Debug.Log($"name {playerName}");
                            }
                        });
                }
            });
        }

        #endregion
        

        private UICanvasController GetCanvas()
        {
            if (canvas != null) return this.canvas;

            Entities.WithAll<UICanvasController>().ForEach((Entity e, in UICanvasController canvasController) =>
            {
                canvas = canvasController;
            }).WithoutBurst().Run();

            return canvas;
        }

        private bool TryCreateRegistrationPanel()
        {
            var canvas = GetCanvas();

            if (canvas == null) return false;

            var testCount = 0;

            Entities.WithAll<UIRegistrationPanelComponent, InstanceTag>().ForEach((Entity e) => { testCount++; })
                .WithoutBurst().Run();

            if (testCount > 0) return false;

           // PlayGamesPlatform.Activate();

            Entities.WithAll<FactoriesCardData>().ForEach((FactoriesCardData factories) =>
            {
                var entityPanel = EntityManager.CreateEntity();

                var panel = factories.CreateRegInstance<UIRegistrationPanelFabric>(entityPanel, GetCanvas().CardRoot);

                panel.SwitchStateButton(RegistrationButton.State.CreateUser);
            }).WithStructuralChanges().WithoutBurst().Run();

            return true;
        }

        private async UniTask SignInUserAsync(Entity entity, string email, string pass, string nick)
        {
            isActiveAsync = true;
            
            await auth.SignInWithEmailAndPasswordAsync(email, pass).ContinueWith(async task =>
            {
                if (task.IsCanceled)
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(5));
                    
                    EntityManager.RemoveComponent<AsyncTag>(entity);

                    isActiveAsync = false;

                    Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                    return;
                }

                if (task.IsFaulted)
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(5));
                    
                    EntityManager.RemoveComponent<AsyncTag>(entity);

                    isActiveAsync = false;

                    Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                    return;
                }

                if (entity != Entity.Null)
                {
                    EntityManager.RemoveComponent<AsyncTag>(entity);

                    var data = new FirebaseRegistrationData()
                    {
                        email = email,
                        pass = pass,
                        nick = nick
                    };

                    EntityManager.AddComponentData(entity, data);
                    EntityManager.AddComponentData(entity, new FirebaseAppReadyTag());
                    EntityManager.AddComponentData(entity, new FirebaseRegistrationCompleteTag());

                    isInit = true;

                    Firebase.Auth.FirebaseUser newUser = task.Result;

                    isActiveAsync = false;

                    Debug.LogFormat("User signed in successfully: {0} ({1})",
                        newUser.DisplayName, newUser.UserId);
                    
                    await UniTask.Delay(TimeSpan.FromSeconds(.5f));
                    
                    DataBaseUpdateUser();
                }
                
            });
        }

        private async UniTask CreateUserAsync(
            Entity entity,
            FixedString512Bytes email,
            FixedString512Bytes pass,
            FixedString512Bytes nick)
        {
            isActiveAsync = true;
            
            SwitchStateRegButton(RegistrationButton.State.Process);
            
            await auth.CreateUserWithEmailAndPasswordAsync(email.Value, pass.Value).ContinueWith(async task =>
            {
                if (task.IsCanceled)
                {
                    try
                    {
                        SwitchStateRegButton(RegistrationButton.State.CreateUser);

                        if (entity != Entity.Null) EntityManager.RemoveComponent<AsyncTag>(entity);
                    }
                    catch (Exception e)
                    {
                    }

                    isActiveAsync = false;
                    
                    return;
                }

                if (task.IsFaulted)
                {
                    try
                    {
                        SwitchStateRegButton(RegistrationButton.State.CreateUser);

                        if (entity != Entity.Null) EntityManager.RemoveComponent<AsyncTag>(entity);
                    }
                    catch (Exception e)
                    {
                    }
                    
                    isActiveAsync = false;

                    return;
                }

                if (entity != Entity.Null)
                {
                    // Firebase user has been created.
                    Firebase.Auth.FirebaseUser newUser = task.Result;
                    
                    var stateEntity = GetSingletonEntity<GameState>();

                    if (stateEntity == Entity.Null) return;

                    var dataState = EntityManager.GetComponentData<GameState>(stateEntity);

                    isInit = true;

                    dataState.UserState.SetData(newUser.UserId, email, pass, nick);

                    dataState.UserState.SaveData();

                    if (entity != Entity.Null)
                    {
                        EntityManager.RemoveComponent<AsyncTag>(entity);
                        EntityManager.AddComponentData(entity, new FirebaseRegistrationCompleteTag());
                        EntityManager.AddComponentData(entity, new FirebaseAppReadyTag());
                    }

                    DestroyRegPanel();
                    
                    isActiveAsync = false;

                    Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                        newUser.DisplayName, newUser.UserId);

                    await UniTask.Delay(TimeSpan.FromSeconds(.5f));
                    
                    DataBaseUpdateUser();

                    UpdateCurrentNick();
                }
            });
        }

        private void SwitchStateRegButton(RegistrationButton.State state)
        {
            Entities.WithAll<UIRegistrationPanelComponent, InstanceTag>()
                .ForEach((UIRegistrationPanelComponent panel) => { panel.registrationPanel.SwitchStateButton(state); })
                .WithoutBurst().Run();
        }

        private void DestroyRegPanel()
        {
            Entities.WithAll<UIRegistrationPanelComponent, InstanceTag>().ForEach((Entity e) =>
            {
                EntityManager.AddComponentData(e, new DestroyTag());
            }).WithStructuralChanges().WithoutBurst().Run();
        }

        #region DataBase

        private void DataBaseUpdateUser() {
           
            var stateEntity = GetSingletonEntity<GameState>();

            if (stateEntity == Entity.Null) return;

            var dataState = EntityManager.GetComponentData<GameState>(stateEntity);
            
            string json = (dataState.UserState.ToJson());

            var handle = reference.Child("users").Child(dataState.UserState.FirebaseId).SetRawJsonValueAsync(json)
                .ContinueWith(
                    task =>
                    {
                        Debug.Log($"DataBaseUpdateUser {task.Status}");
                    });
        }

        #endregion
    }


    public struct FirebaseReadyDependenciesTag : IComponentData
    {
    }

    public struct FirebaseAppReadyTag : IComponentData
    {
    }

    public struct FirebaseRegistrationCompleteTag : IComponentData
    {
    }

    public struct AsyncTag : IComponentData
    {
    }

    public struct SetCurrentNickTag : IComponentData
    {
    }
}