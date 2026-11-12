# Google / Gmail Login — ADIX BATTLE ROYALE

This adds Google account sign-in through **Firebase Authentication**. Players use their own Google/Gmail account; the game receives a Firebase user ID, verified email and display name. Do not ask players to type their Gmail password into the game.

## Current repository state

- `GoogleLoginManager.cs` is a compile-safe integration bridge.
- It deliberately compiles without third-party SDKs. Until the SDKs and Firebase project configuration are added, pressing Google Login reports that setup is pending.
- This is not yet live/verified authentication and does not create an APK.

## Required setup (one-time, project owner)

1. Create or open a Firebase project at https://console.firebase.google.com/.
2. Add an **Android app** using the exact Unity Android package ID. Set that ID in Unity Player Settings first; do not guess it.
3. Download that app's `google-services.json` and place it at the Unity project root or the location required by the Firebase Unity SDK version being used.
4. In Firebase Console → Authentication → Sign-in method, enable **Google**.
5. Add the Android app's SHA-1 and SHA-256 signing certificate fingerprints in Firebase Project Settings. Debug and release keystores have different fingerprints.
6. Download/import the official Firebase Unity SDK modules **Firebase App** and **Firebase Auth**.
7. Import a compatible Google Sign-In Unity plugin (Google Sign-In for Unity) and configure its OAuth **Web client ID**. Use the Web client ID, not the Android client ID, in the script.
8. Add the scripting define `ADIX_FIREBASE_AUTH` in Unity Player Settings → Other Settings → Scripting Define Symbols for the Android build target.
9. Add `GoogleLoginManager` to the login scene. Connect the Google button's OnClick to `SignInWithGoogle()`; connect the success/error UnityEvents to the login UI.
10. Test with a real Android build using the same package ID and signing certificate registered in Firebase.

## Security and account behavior

- Never collect or store a Gmail password.
- Never commit `google-services.json`, OAuth client secrets, private keys, or keystore files to a public repository.
- Google ID token is exchanged with Firebase Auth; use Firebase UID as the stable player account key.
- A Google sign-in does not automatically sync offline game progress across devices. Cloud save/database rules must be designed separately.
- Provide a guest/offline option so the offline single-player game remains usable without internet.

## UI event contract

- Success event arguments: `uid, email, displayName`.
- Error event argument: readable error message.
- The script restores an existing Firebase session on startup when one is present.
