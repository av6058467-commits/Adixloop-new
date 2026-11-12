using System;
using UnityEngine;
using UnityEngine.Events;

#if ADIX_FIREBASE_AUTH
using Firebase;
using Firebase.Auth;
using Google;
#endif

namespace Adix.BattleRoyale.Auth
{
    [Serializable] public class GoogleLoginResult : UnityEvent<string, string, string> { }
    [Serializable] public class GoogleLoginError : UnityEvent<string> { }

    /// <summary>
    /// Google/Gmail sign-in bridge for the Android game.
    /// Enable ADIX_FIREBASE_AUTH only after importing Firebase Auth and Google Sign-In
    /// Unity plugins and completing the setup in GOOGLE_LOGIN_SETUP.md.
    /// </summary>
    public sealed class GoogleLoginManager : MonoBehaviour
    {
        [Header("Google OAuth Web client ID from Firebase project")]
        [SerializeField] private string webClientId;
        [Header("Events: uid, email, display name")]
        public GoogleLoginResult onLoginSuccess = new GoogleLoginResult();
        public GoogleLoginError onLoginFailed = new GoogleLoginError();

#if ADIX_FIREBASE_AUTH
        private FirebaseAuth auth;
        private bool ready;

        private async void Start()
        {
            try
            {
                var dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();
                if (dependencyStatus != DependencyStatus.Available)
                    throw new Exception("Firebase dependencies unavailable: " + dependencyStatus);

                auth = FirebaseAuth.DefaultInstance;
                GoogleSignIn.Configuration = new GoogleSignInConfiguration
                {
                    WebClientId = webClientId,
                    RequestIdToken = true,
                    RequestEmail = true
                };
                ready = true;

                if (auth.CurrentUser != null)
                    onLoginSuccess.Invoke(auth.CurrentUser.UserId,
                        auth.CurrentUser.Email ?? "", auth.CurrentUser.DisplayName ?? "");
            }
            catch (Exception e)
            {
                onLoginFailed.Invoke("Google login setup error: " + e.Message);
            }
        }

        public void SignInWithGoogle()
        {
            if (!ready)
            {
                onLoginFailed.Invoke("Login is not ready. Check Firebase and Google Sign-In setup.");
                return;
            }
            SignInAsync();
        }

        private async void SignInAsync()
        {
            try
            {
                var googleUser = await GoogleSignIn.DefaultInstance.SignIn();
                if (googleUser == null || string.IsNullOrEmpty(googleUser.IdToken))
                    throw new Exception("Google did not return an ID token.");

                var credential = GoogleAuthProvider.GetCredential(googleUser.IdToken, null);
                var result = await auth.SignInWithCredentialAsync(credential);
                var user = result.User;
                onLoginSuccess.Invoke(user.UserId, user.Email ?? "", user.DisplayName ?? "");
            }
            catch (Exception e)
            {
                onLoginFailed.Invoke("Google sign-in failed: " + e.Message);
            }
        }

        public async void SignOut()
        {
            try
            {
                if (auth != null) await auth.SignOutAsync();
                GoogleSignIn.DefaultInstance.SignOut();
            }
            catch (Exception e)
            {
                onLoginFailed.Invoke("Sign-out failed: " + e.Message);
            }
        }
#else
        public void SignInWithGoogle()
        {
            onLoginFailed.Invoke("Google login SDK is not installed yet. Follow GOOGLE_LOGIN_SETUP.md.");
        }

        public void SignOut()
        {
            onLoginFailed.Invoke("Google login SDK is not installed yet.");
        }
#endif
    }
}
