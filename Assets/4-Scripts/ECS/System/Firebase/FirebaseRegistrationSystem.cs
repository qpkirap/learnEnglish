using Cysharp.Threading.Tasks;
using Firebase;
using Game.Config;
using Game.ECS_UI.Components;
using Game.ECS.Components;
using GooglePlayGames;
using GooglePlayGames.BasicApi.Nearby;
using Unity.Entities;
using UnityEngine;

namespace Game.ECS.System
{
    [UpdateAfter(typeof(InitSystem))]
    public partial class FirebaseRegistrationSystem : InitSystemBase
    {
        private UICanvasController canvas;
        private FirebaseApp app;
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
                app = FirebaseApp.DefaultInstance;

                var entity = EntityManager.CreateEntity();

                EntityManager.AddComponentData(entity, new FirebaseReadyDependenciesTag());
            }
        } 
        
        private void Callback(INearbyConnectionClient obj)
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

        protected override void OnUpdate()
        {
            if (!HasSingleton<InitAllFabricsTag>()) return;
            if (!HasSingleton<FirebaseReadyDependenciesTag>()) return;

            if (isInit) return;

            if (app == null) return;

            var appEntity = GetSingletonEntity<FirebaseReadyDependenciesTag>();

            var stateEntity = GetSingletonEntity<GameState>();
            
            if (stateEntity == Entity.Null) return;

            var dataState = EntityManager.GetComponentData<GameState>(stateEntity);
            
            if (dataState == null) return;

            if (!string.IsNullOrEmpty(dataState.UserState.Email))
            {
                isInit = true;

                var data = new FirebaseRegistrationData()
                {
                    email = dataState.UserState.Email,
                    pass = dataState.UserState.Pass
                };

                EntityManager.AddComponentData(appEntity, data);
                EntityManager.AddComponentData(appEntity, new FirebaseAppReadyTag());
                
                TryAuth();
            }

            TryCreateRegistrationPanel();

            Entities.WithAll<FirebaseRegistrationData>().ForEach((Entity e, FirebaseRegistrationData registration) =>
            {
                isInit = true;
                
                dataState.UserState.SetData(registration.email, registration.pass);
                
                dataState.UserState.SaveData();

                EntityManager.AddComponentData(e, new FirebaseAppReadyTag());

                TryAuth();
            }).WithStructuralChanges().WithoutBurst().Run();;
        }

        private void TryAuth()
        {
            PlayGamesPlatform.InitializeNearby(Callback);
        }
        
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
    }

    public struct FirebaseReadyDependenciesTag : IComponentData
    {
    }
    
    public struct FirebaseAppReadyTag : IComponentData
    {
    }
}