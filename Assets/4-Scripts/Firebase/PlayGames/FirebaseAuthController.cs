using System;
using Cysharp.Threading.Tasks;
using Firebase;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.Nearby;
using UnityEngine;

namespace Game
{
    public class FirebaseAuthController : MonoBehaviour
    {
       
        private async void Start()
        {
            var test1 = await FirebaseApp.CheckAndFixDependenciesAsync();
            
            test();

            Test2();
        }

        private async UniTask Test2()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(3));
            
            Social.Active.Authenticate(Social.localUser, delegate(bool b)
            {
                Debug.Log($"TryAuthenticate user{Social.localUser}, bool {b}");
                        
                if (b) PlayGamesPlatform.Instance.Authenticate(CallbackLogin);
            });
        }

        void test()
        {
            PlayGamesPlatform.InitializeNearby(Callback);
            PlayGamesPlatform.Activate();
        }
        
        private void Callback(INearbyConnectionClient obj)
        {
            Debug.Log("Nearby connections initialized");
            
            Social.localUser.Authenticate((bool success) =>
            {
                Debug.Log($"Social auth {success}");

                if (success)
                {
                    
                }
            });
        }

        private void CallbackLogin(SignInStatus status)
        {
            Debug.Log($"CallbackLogin status {status}");

            if (status == SignInStatus.Success)
            {
                PlayGamesPlatform.Instance.RequestServerSideAccess(
                        /* forceRefreshToken= */ false,
                        authCode =>
                        {
                            // send code to server

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
        }
    }
}