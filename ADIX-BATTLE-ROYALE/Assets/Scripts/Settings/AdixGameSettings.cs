using UnityEngine;
using AdixBattleRoyale.Player;

namespace AdixBattleRoyale.Settings
{
    public enum AdixGraphicsPreset { UltraSmooth, Standard, Ultra }
    public enum AdixAimMode { Hold, Toggle }
    public enum AdixGyroMode { Off, ScopeOnly, AlwaysOn }

    [DisallowMultipleComponent]
    public sealed class AdixGameSettings : MonoBehaviour
    {
        public static AdixGameSettings Instance { get; private set; }

        [Header("Graphics")]
        [SerializeField] private AdixGraphicsPreset graphicsPreset = AdixGraphicsPreset.UltraSmooth;
        [SerializeField] private int targetFps = 60;
        [SerializeField, Range(0.5f, 1f)] private float renderScale = 0.85f;
        [SerializeField] private bool shadows;
        [SerializeField] private bool motionEffects = true;

        [Header("Sensitivity")]
        [SerializeField, Range(0.1f, 2f)] private float cameraSensitivity = 0.65f;
        [SerializeField, Range(0.1f, 2f)] private float redDotSensitivity = 0.8f;
        [SerializeField, Range(0.1f, 2f)] private float scope2xSensitivity = 0.7f;
        [SerializeField, Range(0.1f, 2f)] private float scope4xSensitivity = 0.55f;
        [SerializeField, Range(0.1f, 2f)] private float sniperSensitivity = 0.35f;
        [SerializeField] private AdixGyroMode gyroMode = AdixGyroMode.Off;
        [SerializeField] private AdixAimMode aimMode = AdixAimMode.Hold;

        [Header("Scene References")]
        [SerializeField] private ThirdPersonCameraRig cameraRig;
        [SerializeField] private TouchLookPad touchLookPad;

        public AdixGraphicsPreset GraphicsPreset => graphicsPreset;
        public int TargetFps => targetFps;
        public float CameraSensitivity => cameraSensitivity;
        public float RedDotSensitivity => redDotSensitivity;
        public float Scope2xSensitivity => scope2xSensitivity;
        public float Scope4xSensitivity => scope4xSensitivity;
        public float SniperSensitivity => sniperSensitivity;
        public AdixGyroMode GyroMode => gyroMode;
        public AdixAimMode AimMode => aimMode;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Load();
            ApplyAll();
        }

        public void SetGraphicsPreset(AdixGraphicsPreset preset)
        {
            graphicsPreset = preset;
            switch (preset)
            {
                case AdixGraphicsPreset.UltraSmooth:
                    targetFps = 60; renderScale = 0.75f; shadows = false; motionEffects = false;
                    break;
                case AdixGraphicsPreset.Standard:
                    targetFps = 60; renderScale = 0.9f; shadows = true; motionEffects = true;
                    break;
                case AdixGraphicsPreset.Ultra:
                    targetFps = 60; renderScale = 1f; shadows = true; motionEffects = true;
                    break;
            }
            ApplyGraphics();
            Save();
        }

        public void SetTargetFps(int fps)
        {
            targetFps = Mathf.Clamp(fps, 30, 120);
            ApplyGraphics();
            Save();
        }

        public void SetCameraSensitivity(float value)
        {
            cameraSensitivity = Mathf.Clamp(value, 0.1f, 2f);
            ApplySensitivity();
            Save();
        }

        public void SetScopeSensitivity(int scope, float value)
        {
            value = Mathf.Clamp(value, 0.1f, 2f);
            switch (scope)
            {
                case 0: redDotSensitivity = value; break;
                case 1: scope2xSensitivity = value; break;
                case 2: scope4xSensitivity = value; break;
                case 3: sniperSensitivity = value; break;
                default: return;
            }
            Save();
        }

        public void SetGyroMode(AdixGyroMode mode) { gyroMode = mode; Save(); }
        public void SetAimMode(AdixAimMode mode) { aimMode = mode; Save(); }

        public void ApplyAll()
        {
            ApplyGraphics();
            ApplySensitivity();
        }

        private void ApplyGraphics()
        {
            int requestedFps = Mathf.Clamp(targetFps, 30, 120);
            Application.targetFrameRate = requestedFps;
            QualitySettings.vSyncCount = 0;

            int qualityIndex = graphicsPreset == AdixGraphicsPreset.UltraSmooth ? 0 :
                               graphicsPreset == AdixGraphicsPreset.Standard ? Mathf.Min(2, QualitySettings.names.Length - 1) :
                               Mathf.Max(0, QualitySettings.names.Length - 1);
            if (QualitySettings.names.Length > 0)
                QualitySettings.SetQualityLevel(Mathf.Clamp(qualityIndex, 0, QualitySettings.names.Length - 1), true);

            QualitySettings.shadows = shadows ? ShadowQuality.All : ShadowQuality.Disable;
            QualitySettings.antiAliasing = graphicsPreset == AdixGraphicsPreset.Ultra ? 4 :
                                           graphicsPreset == AdixGraphicsPreset.Standard ? 2 : 0;
        }

        private void ApplySensitivity()
        {
            if (cameraRig != null) cameraRig.SetSensitivity(cameraSensitivity);
            if (touchLookPad != null) touchLookPad.SetSensitivity(cameraSensitivity);
        }

        private void Save()
        {
            PlayerPrefs.SetInt("ADIX_GFX", (int)graphicsPreset);
            PlayerPrefs.SetInt("ADIX_FPS", targetFps);
            PlayerPrefs.SetFloat("ADIX_CAM_SENS", cameraSensitivity);
            PlayerPrefs.SetFloat("ADIX_RED_DOT", redDotSensitivity);
            PlayerPrefs.SetFloat("ADIX_2X", scope2xSensitivity);
            PlayerPrefs.SetFloat("ADIX_4X", scope4xSensitivity);
            PlayerPrefs.SetFloat("ADIX_SNIPER", sniperSensitivity);
            PlayerPrefs.SetInt("ADIX_GYRO", (int)gyroMode);
            PlayerPrefs.SetInt("ADIX_AIM_MODE", (int)aimMode);
            PlayerPrefs.Save();
        }

        private void Load()
        {
            graphicsPreset = (AdixGraphicsPreset)Mathf.Clamp(PlayerPrefs.GetInt("ADIX_GFX", (int)graphicsPreset), 0, 2);
            targetFps = Mathf.Clamp(PlayerPrefs.GetInt("ADIX_FPS", targetFps), 30, 120);
            cameraSensitivity = Mathf.Clamp(PlayerPrefs.GetFloat("ADIX_CAM_SENS", cameraSensitivity), 0.1f, 2f);
            redDotSensitivity = Mathf.Clamp(PlayerPrefs.GetFloat("ADIX_RED_DOT", redDotSensitivity), 0.1f, 2f);
            scope2xSensitivity = Mathf.Clamp(PlayerPrefs.GetFloat("ADIX_2X", scope2xSensitivity), 0.1f, 2f);
            scope4xSensitivity = Mathf.Clamp(PlayerPrefs.GetFloat("ADIX_4X", scope4xSensitivity), 0.1f, 2f);
            sniperSensitivity = Mathf.Clamp(PlayerPrefs.GetFloat("ADIX_SNIPER", sniperSensitivity), 0.1f, 2f);
            gyroMode = (AdixGyroMode)Mathf.Clamp(PlayerPrefs.GetInt("ADIX_GYRO", (int)gyroMode), 0, 2);
            aimMode = (AdixAimMode)Mathf.Clamp(PlayerPrefs.GetInt("ADIX_AIM_MODE", (int)aimMode), 0, 1);
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }
    }
}
