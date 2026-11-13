# Build ADIX Android APK from a phone (cloud build)

The Android SDK/NDK does not need to be installed on the owner's PC for this workflow. GitHub Actions runs the Unity Android build on a cloud runner.

## One-time setup

1. Sign in to GitHub and open this repository.
2. Open **Settings → Secrets and variables → Actions → New repository secret**.
3. Add the three repository secrets expected by the workflow:
   - `UNITY_LICENSE`
   - `UNITY_EMAIL`
   - `UNITY_PASSWORD`
4. Use the Unity license file/value and account credentials required by the current GameCI Unity Builder licensing instructions. Do not paste them into issues, commits, screenshots, or ChatGPT.
5. The Unity Editor version must match the project: **2022.3.62f1**. The cloud builder provisions the Android build environment.

## Run a build from Android Chrome

1. Open the repository's **Actions** tab.
2. Select **ADIX BATTLE ROYALE Android APK**.
3. Tap **Run workflow** and confirm the branch is `main`.
4. Open the new run and wait for the job to finish.
5. If successful, open **Artifacts** and download `ADIX-BATTLE-ROYALE-Android-Demo`.
6. Extract the APK from the downloaded artifact and install it on the test phone.

## If the run fails

- **Missing GitHub Secret:** the preflight step lists the missing secret names. Add them under repository Actions secrets.
- **License activation / serial / entitlement error:** verify the Unity Personal license file and account values against GameCI's current documentation. Do not send credentials or license contents to anyone.
- **Unity compile error:** open the failed Unity build step and use the first C# compiler error as the starting point.
- **No APK artifact:** the build did not finish successfully; do not treat a workflow run as a playable demo until an APK artifact exists and is tested.

## Important limits

- A cloud build does not prove gameplay works. The APK still needs installation and real-device testing.
- This workflow currently builds an ARM64 Android demo APK, not a production release.
- Online matchmaking, remote friends/invites, final art, full CS arena integration and production backend are not implied by this build.
- Never commit Unity license files, passwords, keystores, OAuth secrets or Firebase private credentials.
