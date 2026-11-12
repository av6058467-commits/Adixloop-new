#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace AdixBattleRoyale.Editor
{
    public static class AndroidDemoBuild
    {
        private const string OutputDirectory = "Builds/Android";

        [MenuItem("ADIX Battle Royale/Build Android Demo APK")]
        public static void BuildAndroidDemo()
        {
            Directory.CreateDirectory(OutputDirectory);
            string output = Path.Combine(OutputDirectory, "ADIX-BATTLE-ROYALE-Demo.apk");

            EditorUserBuildSettings.SwitchActiveBuildTarget(
                BuildTargetGroup.Android, BuildTarget.Android);

            PlayerSettings.productName = "ADIX BATTLE ROYALE";
            PlayerSettings.applicationIdentifier = "com.adix.battleroyale";
            PlayerSettings.bundleVersion = "0.1.0";
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel26;
            PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevelAuto;
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;

            string[] scenes = Array.ConvertAll(
                EditorBuildSettings.scenes,
                scene => scene.path);

            if (scenes.Length == 0)
                throw new BuildFailedException(
                    "No scenes are enabled in Build Settings. Add the demo scene before building.");

            BuildPlayerOptions options = new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = output,
                target = BuildTarget.Android,
                options = BuildOptions.None
            };

            BuildReport report = BuildPipeline.BuildPlayer(options);
            if (report.summary.result != BuildResult.Succeeded)
                throw new BuildFailedException(
                    $"Android build failed: {report.summary.result}, errors: {report.summary.totalErrors}");

            Debug.Log($"ADIX BATTLE ROYALE APK created: {Path.GetFullPath(output)}");
        }
    }
}
#endif
