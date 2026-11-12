#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Unity.AI.Navigation;
using AdixBattleRoyale.AI;
using AdixBattleRoyale.Combat;
using AdixBattleRoyale.Match;
using AdixBattleRoyale.Player;
using AdixBattleRoyale.UI;

namespace AdixBattleRoyale.Editor
{
    public static class DemoSceneBuilder
    {
        private const string SceneFolder = "Assets/Scenes";
        private const string PrefabFolder = "Assets/Prefabs";
        private const string ScenePath = SceneFolder + "/ADIX_Demo.unity";
        private const string BotPrefabPath = PrefabFolder + "/DemoBot.prefab";

        [MenuItem("ADIX Battle Royale/Create Playable Demo Scene")]
        public static void CreateDemoScene()
        {
            EnsureFolder("Assets", "Scenes");
            EnsureFolder("Assets", "Prefabs");

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            CreateWorld();
            GameObject player = CreatePlayer();
            GameObject botPrefab = CreateBotPrefab();
            CreateMatch(player, botPrefab);
            CreateHud(player);
            CreateLighting();

            EditorSceneManager.SaveScene(scene, ScenePath);
            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(ScenePath, true)
            };
            AssetDatabase.SaveAssets();
            Debug.Log("ADIX demo scene created. It contains a training island, city blocks, player, combat, bot match and touch HUD.");
        }

        private static void EnsureFolder(string parent, string child)
        {
            string path = parent + "/" + child;
            if (!AssetDatabase.IsValidFolder(path))
                AssetDatabase.CreateFolder(parent, child);
        }

        private static void CreateWorld()
        {
            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Training Island Ground";
            ground.transform.localScale = new Vector3(18f, 1f, 18f);
            ground.isStatic = true;
            ground.GetComponent<Renderer>().sharedMaterial = MakeMaterial("Island Ground", new Color(0.24f, 0.34f, 0.22f));

            Material cityMat = MakeMaterial("City Concrete", new Color(0.38f, 0.42f, 0.46f));
            Material roofMat = MakeMaterial("Roof", new Color(0.22f, 0.25f, 0.29f));
            for (int x = -3; x <= 3; x++)
            for (int z = -3; z <= 3; z++)
            {
                if (Mathf.Abs(x) < 2 && Mathf.Abs(z) < 2) continue;
                GameObject building = GameObject.CreatePrimitive(PrimitiveType.Cube);
                building.name = "City Building";
                building.transform.position = new Vector3(x * 17f, Random.Range(3f, 7f), z * 17f);
                building.transform.localScale = new Vector3(Random.Range(7f, 11f), Random.Range(6f, 14f), Random.Range(7f, 11f));
                building.isStatic = true;
                building.GetComponent<Renderer>().sharedMaterial = cityMat;
                GameObject roof = GameObject.CreatePrimitive(PrimitiveType.Cube);
                roof.name = "Roof Detail";
                roof.transform.SetParent(building.transform, false);
                roof.transform.localPosition = new Vector3(0f, 0.52f, 0f);
                roof.transform.localScale = new Vector3(1.02f, 0.08f, 1.02f);
                roof.GetComponent<Renderer>().sharedMaterial = roofMat;
                roof.isStatic = true;
            }

            for (int i = 0; i < 18; i++)
            {
                GameObject cover = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cover.name = "Combat Cover";
                cover.transform.position = new Vector3(Random.Range(-65f, 65f), 1f, Random.Range(-65f, 65f));
                cover.transform.localScale = new Vector3(Random.Range(2f, 5f), 2f, Random.Range(2f, 5f));
                cover.isStatic = true;
                cover.GetComponent<Renderer>().sharedMaterial = roofMat;
            }

            var surface = ground.AddComponent<NavMeshSurface>();
            surface.collectObjects = CollectObjects.All;
            surface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
            surface.BuildNavMesh();
        }

        private static GameObject CreatePlayer()
        {
            GameObject root = new GameObject("Player");
            root.tag = "Player";
            root.transform.position = new Vector3(0f, 1.1f, -8f);
            var controller = root.AddComponent<CharacterController>();
            controller.height = 2f;
            controller.radius = 0.42f;
            controller.center = new Vector3(0f, 0f, 0f);
            root.AddComponent<MobilePlayerController>();
            root.AddComponent<PlayerHealth>();

            GameObject body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            body.name = "Player Visual";
            body.transform.SetParent(root.transform, false);
            body.transform.localPosition = Vector3.zero;
            body.transform.localScale = new Vector3(0.72f, 0.95f, 0.72f);
            Object.DestroyImmediate(body.GetComponent<Collider>());
            body.GetComponent<Renderer>().sharedMaterial = MakeMaterial("Player Suit", new Color(0.12f, 0.42f, 0.72f));

            GameObject cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            cameraObject.AddComponent<Camera>();
            cameraObject.AddComponent<AudioListener>();
            var rig = cameraObject.AddComponent<ThirdPersonCameraRig>();
            SetObjectReference(rig, "target", root.transform);
            cameraObject.transform.position = root.transform.position + new Vector3(0f, 2.2f, -4.5f);

            var weapon = root.AddComponent<PlayerWeapon>();
            SetObjectReference(weapon, "aimCamera", cameraObject.GetComponent<Camera>());
            return root;
        }

        private static GameObject CreateBotPrefab()
        {
            GameObject root = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            root.name = "DemoBot";
            root.transform.position = new Vector3(0f, -100f, 0f);
            root.GetComponent<Renderer>().sharedMaterial = MakeMaterial("Bot Armor", new Color(0.62f, 0.18f, 0.16f));
            root.AddComponent<NavMeshAgent>();
            root.AddComponent<BotHealth>();
            root.AddComponent<BattleBotBrain>();

            GameObject muzzle = new GameObject("Muzzle");
            muzzle.transform.SetParent(root.transform, false);
            muzzle.transform.localPosition = new Vector3(0f, 1.25f, 0.45f);
            var emitter = root.AddComponent<BotWeaponEmitter>();
            SetObjectReference(emitter, "muzzle", muzzle.transform);

            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, BotPrefabPath);
            Object.DestroyImmediate(root);
            return prefab;
        }

        private static void CreateMatch(GameObject player, GameObject botPrefab)
        {
            GameObject match = new GameObject("Offline Match Director");
            var director = match.AddComponent<BattleRoyaleMatchDirector>();
            SetObjectReference(director, "player", player.transform);
            SetObjectReference(director, "botPrefab", botPrefab);
            SetInt(director, "botCount", 12);

            Transform[] spawns = new Transform[8];
            for (int i = 0; i < spawns.Length; i++)
            {
                GameObject point = new GameObject("Bot Spawn " + (i + 1));
                float angle = i * Mathf.PI * 2f / spawns.Length;
                point.transform.position = new Vector3(Mathf.Sin(angle) * 62f, 0.2f, Mathf.Cos(angle) * 62f);
                spawns[i] = point.transform;
            }
            SetObjectArray(director, "spawnPoints", spawns);
        }

        private static void CreateHud(GameObject player)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();

            GameObject canvasObject = new GameObject("Mobile HUD", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            Canvas canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);

            GameObject bridgeObject = new GameObject("Mobile Input Bridge");
            var bridge = bridgeObject.AddComponent<MobileInputBridge>();
            SetObjectReference(bridge, "player", player.GetComponent<MobilePlayerController>());
            SetObjectReference(bridge, "cameraRig", Camera.main != null ? Camera.main.GetComponent<ThirdPersonCameraRig>() : Object.FindObjectOfType<ThirdPersonCameraRig>());
            SetObjectReference(bridge, "weapon", player.GetComponent<PlayerWeapon>());

            RectTransform joystickArea = CreatePanel(canvasObject.transform, "Move Joystick", new Vector2(0.12f, 0.18f), new Vector2(0.12f, 0.18f), new Vector2(180f, 180f), new Color(0.05f, 0.08f, 0.12f, 0.48f));
            RectTransform handle = CreatePanel(joystickArea, "Joystick Handle", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(68f, 68f), new Color(0.25f, 0.72f, 1f, 0.85f));
            var joystick = joystickArea.gameObject.AddComponent<VirtualJoystick>();
            SetObjectReference(joystick, "area", joystickArea);
            SetObjectReference(joystick, "handle", handle);
            SetObjectReference(bridge, "moveJoystick", joystick);

            CreateActionButton(canvasObject.transform, "FIRE", new Vector2(0.88f, 0.2f), bridge, "FirePressed", new Vector2(130f, 130f));
            CreateActionButton(canvasObject.transform, "JUMP", new Vector2(0.77f, 0.34f), bridge, "JumpPressed", new Vector2(92f, 92f));
            CreateActionButton(canvasObject.transform, "AIM / CAMERA", new Vector2(0.88f, 0.42f), bridge, "ToggleCameraPressed", new Vector2(112f, 72f));
            CreateActionButton(canvasObject.transform, "RELOAD", new Vector2(0.75f, 0.16f), bridge, "ReloadPressed", new Vector2(100f, 70f));
            CreateActionButton(canvasObject.transform, "CROUCH", new Vector2(0.65f, 0.16f), bridge, "ToggleCrouchPressed", new Vector2(100f, 70f));

            GameObject title = new GameObject("Match Title", typeof(RectTransform), typeof(Text));
            title.transform.SetParent(canvasObject.transform, false);
            RectTransform titleRect = title.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0.5f, 1f);
            titleRect.anchorMax = new Vector2(0.5f, 1f);
            titleRect.anchoredPosition = new Vector2(0f, -42f);
            titleRect.sizeDelta = new Vector2(500f, 60f);
            Text titleText = title.GetComponent<Text>();
            titleText.text = "ADIX BATTLE ROYALE  •  OFFLINE DEMO";
            titleText.alignment = TextAnchor.MiddleCenter;
            titleText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            titleText.fontSize = 26;
            titleText.color = Color.white;
        }

        private static void CreateActionButton(Transform parent, string label, Vector2 anchor, MobileInputBridge bridge, string method, Vector2 size)
        {
            GameObject go = new GameObject(label + " Button", typeof(RectTransform), typeof(Image), typeof(Button));
            go.transform.SetParent(parent, false);
            RectTransform rect = go.GetComponent<RectTransform>();
            rect.anchorMin = rect.anchorMax = anchor;
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = size;
            Image image = go.GetComponent<Image>();
            image.color = new Color(0.04f, 0.12f, 0.2f, 0.78f);
            Button button = go.GetComponent<Button>();
            button.targetGraphic = image;

            GameObject textObject = new GameObject("Label", typeof(RectTransform), typeof(Text));
            textObject.transform.SetParent(go.transform, false);
            RectTransform textRect = textObject.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = textRect.offsetMax = Vector2.zero;
            Text text = textObject.GetComponent<Text>();
            text.text = label;
            text.alignment = TextAnchor.MiddleCenter;
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.fontSize = 18;
            text.color = Color.white;

            var action = go.AddComponent<HoldActionButton>();
            var so = new SerializedObject(action);
            so.FindProperty("onPressed").FindPropertyRelative("m_PersistentCalls.m_Calls").arraySize = 0;
            so.ApplyModifiedProperties();
            UnityEventTools.AddPersistentListener(GetPressedEvent(action), ResolveAction(bridge, method));
        }

        private static UnityEngine.Events.UnityEvent GetPressedEvent(HoldActionButton action)
        {
            var field = typeof(HoldActionButton).GetField("onPressed", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            return (UnityEngine.Events.UnityEvent)field.GetValue(action);
        }

        private static UnityEngine.Events.UnityAction ResolveAction(MobileInputBridge bridge, string method)
        {
            switch (method)
            {
                case "FirePressed": return bridge.FirePressed;
                case "JumpPressed": return bridge.JumpPressed;
                case "ToggleCameraPressed": return bridge.ToggleCameraPressed;
                case "ReloadPressed": return bridge.ReloadPressed;
                default: return bridge.ToggleCrouchPressed;
            }
        }

        private static RectTransform CreatePanel(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, Vector2 size, Color color)
        {
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image));
            go.transform.SetParent(parent, false);
            RectTransform rect = go.GetComponent<RectTransform>();
            rect.anchorMin = rect.anchorMax = anchorMin;
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = size;
            go.GetComponent<Image>().color = color;
            return rect;
        }

        private static Material MakeMaterial(string name, Color color)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            Material material = new Material(shader);
            material.name = name;
            material.color = color;
            return material;
        }

        private static void CreateLighting()
        {
            GameObject lightObject = new GameObject("Sun");
            Light light = lightObject.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.15f;
            lightObject.transform.rotation = Quaternion.Euler(48f, -28f, 0f);
            RenderSettings.ambientLight = new Color(0.48f, 0.52f, 0.58f);
        }

        private static void SetObjectReference(Object target, string property, Object value)
        {
            var so = new SerializedObject(target);
            so.FindProperty(property).objectReferenceValue = value;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetInt(Object target, string property, int value)
        {
            var so = new SerializedObject(target);
            so.FindProperty(property).intValue = value;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetObjectArray(Object target, string property, Object[] values)
        {
            var so = new SerializedObject(target);
            SerializedProperty array = so.FindProperty(property);
            array.arraySize = values.Length;
            for (int i = 0; i < values.Length; i++)
                array.GetArrayElementAtIndex(i).objectReferenceValue = values[i];
            so.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
#endif
