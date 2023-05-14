using System;
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
        private static LazyInject<GameState> gameState = new();

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
                    if (!string.IsNullOrEmpty(gameState.Value.UserState.Email))
                    {
                        EntityManager.AddComponentData(appEntity, new AsyncTag());

                        SignInUserAsync(appEntity, gameState.Value.UserState.Email, gameState.Value.UserState.Pass);

                        return;

                        //TryAuthPlayServices();
                    }

                    TryCreateRegistrationPanel();
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

        private async UniTask SignInUserAsync(Entity entity, string email, string pass)
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
                        pass = pass
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
                    
                    if (gameState.Value == null) return;

                    isInit = true;

                    gameState.Value.UserState.SetData(newUser.UserId, email, pass, nick);

                    gameState.Value.UserState.SaveData();

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
           
            if (gameState.Value == null) return;

            string json = (gameState.Value.UserState.ToJson());

            var handle = reference.Child("users").Child(gameState.Value.UserState.FirebaseId).SetRawJsonValueAsync(json)
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
}