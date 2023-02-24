using GooglePlayGames;
using GooglePlayGames.Android;
using GooglePlayGames.BasicApi.Nearby;
using UnityEngine;

namespace Game
{
    public class FirebaseAuthController : MonoBehaviour
    {
       
        private void Start()
        {
            test();
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
            });
        }
    }
}