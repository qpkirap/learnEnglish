using Cysharp.Threading.Tasks;
using Firebase;
using Firebase.Auth;
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
        private UICanvasController canvas;
        private FirebaseApp app;
        private FirebaseAuth auth;
        private bool isInit;

        protected override void OnStartRunning()
        {
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
            if (!HasSingleton<InitAllFabricsTag>()) return;
            if (!HasSingleton<FirebaseReadyDependenciesTag>()) return;

            if (isInit) return;

            if (app == null) return;

            var appEntity = GetSingletonEntity<FirebaseReadyDependenciesTag>();
            
            if (EntityManager.HasComponent<AsyncTag>(appEntity)) return;

            var stateEntity = GetSingletonEntity<GameState>();
            
            if (stateEntity == Entity.Null) return;

            var dataState = EntityManager.GetComponentData<GameState>(stateEntity);
            
            if (dataState == null) return;

            if (!string.IsNullOrEmpty(dataState.UserState.Email))
            {
                EntityManager.AddComponentData(appEntity, new AsyncTag());

                SignInUserAsync(appEntity, dataState.UserState.Email, dataState.UserState.Pass);
                
                return;

                //TryAuthPlayServices();
            }

            TryCreateRegistrationPanel();

            Entities
                .WithAll<FirebaseRegistrationData>()
                .WithNone<AsyncTag>()
                .ForEach((Entity e, FirebaseRegistrationData registration) =>
            {
                
                EntityManager.AddComponentData(e, new AsyncTag());

                CreateUserAsync(e, registration.email, registration.pass);

                //TryAuthPlayServices();
            }).WithStructuralChanges().WithoutBurst().Run();;
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
                            Firebase.Auth.Credential credential = Firebase.Auth.PlayGamesAuthProvider.GetCredential(authCode);
                            
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

            Entities.WithAll<UIRegistrationPanelComponent, InstanceTag>().ForEach((Entity e) =>
            {
                testCount++;
            }).WithoutBurst().Run();

            if (testCount > 0) return false;
            
            PlayGamesPlatform.Activate();
                
            Entities.WithAll<FactoriesCardData>().ForEach((FactoriesCardData factories) =>
            {
                var entityPanel = EntityManager.CreateEntity();
                    
                var panel = factories.CreateRegInstance<UIRegistrationPanelFabric>(entityPanel, GetCanvas().root);
            }).WithStructuralChanges().WithoutBurst().Run();

            return true;
        }

        private async UniTask SignInUserAsync(Entity entity, string email, string pass)
        {
            auth.SignInWithEmailAndPasswordAsync(email, pass).ContinueWith(task => {
                if (task.IsCanceled) {
                    Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                    return;
                }
                if (task.IsFaulted) {
                    Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                    return;
                }
                
                var data = new FirebaseRegistrationData()
                {
                    email = email,
                    pass = pass
                };

                EntityManager.AddComponentData(entity, data);
                EntityManager.AddComponentData(entity, new FirebaseAppReadyTag());
                EntityManager.AddComponentData(entity, new FirebaseRegistrationCompleteTag());
                
                EntityManager.RemoveComponent<AsyncTag>(entity);
                
                isInit = true;

                Firebase.Auth.FirebaseUser newUser = task.Result;
                
                Debug.LogFormat("User signed in successfully: {0} ({1})",
                    newUser.DisplayName, newUser.UserId);
            });
        }

        private async UniTask CreateUserAsync(Entity entity, FixedString512Bytes email, FixedString512Bytes pass)
        {
            auth.CreateUserWithEmailAndPasswordAsync(email.Value, pass.Value).ContinueWith(task => {
                if (task.IsCanceled) {
                    Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                    return;
                }
                if (task.IsFaulted) {
                    Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                    return;
                }
                
                var stateEntity = GetSingletonEntity<GameState>();
            
                if (stateEntity == Entity.Null) return;

                var dataState = EntityManager.GetComponentData<GameState>(stateEntity);
                
                isInit = true;
                
                dataState.UserState.SetData(email, pass);
                
                dataState.UserState.SaveData();

                EntityManager.RemoveComponent<AsyncTag>(entity);
                EntityManager.AddComponentData(entity, new FirebaseRegistrationCompleteTag());
                EntityManager.AddComponentData(entity, new FirebaseAppReadyTag());

                // Firebase user has been created.
                Firebase.Auth.FirebaseUser newUser = task.Result;
                Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                    newUser.DisplayName, newUser.UserId);
            });
        }
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