#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;

namespace AdixBattleRoyale.Editor
{
    /// <summary>
    /// Creates editable, modular operator blockout prefabs for the first playable art pass.
    /// These are genuine Unity meshes/materials, not 2D character pictures.
    /// Replace/upgrade the generated geometry with final sculpted FBX assets as they are authored.
    /// </summary>
    public static class OperatorPrefabGenerator
    {
        private const string Root = "Assets/Art/Generated/Operators";
        private const string Materials = Root + "/Materials";

        [MenuItem("ADIX Battle Royale/Art/Create Operator Prefabs")]
        public static void GenerateAll()
        {
            Ensure("Assets/Art");
            Ensure("Assets/Art/Generated");
            Ensure(Root);
            Ensure(Materials);
            CreateOperator("Frost", new Color(.78f,.82f,.86f), new Color(.22f,.27f,.32f), new Color(.85f,.88f,.91f), true, false);
            CreateOperator("Shadow", new Color(.10f,.13f,.17f), new Color(.24f,.28f,.32f), new Color(.08f,.09f,.11f), false, false);
            CreateOperator("Ember", new Color(.25f,.12f,.12f), new Color(.34f,.18f,.17f), new Color(.12f,.10f,.12f), false, true);
            CreateOperator("Volt", new Color(.88f,.66f,.12f), new Color(.12f,.15f,.17f), new Color(.22f,.24f,.25f), false, false);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("ADIX: four modular operator prefabs generated in " + Root);
        }

        private static void Ensure(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            string parent = Path.GetDirectoryName(path).Replace("\\", "/");
            string name = Path.GetFileName(path);
            if (!AssetDatabase.IsValidFolder(parent)) Ensure(parent);
            AssetDatabase.CreateFolder(parent, name);
        }

        private static Material Mat(string name, Color color, float metallic = .12f, float smooth = .34f)
        {
            string path = Materials + "/" + name + ".mat";
            Material m = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (m == null)
            {
                Shader shader = Shader.Find("Universal Render Pipeline/Lit");
                if (shader == null) shader = Shader.Find("Standard");
                m = new Material(shader);
                AssetDatabase.CreateAsset(m, path);
            }
            if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", color);
            if (m.HasProperty("_Color")) m.SetColor("_Color", color);
            if (m.HasProperty("_Metallic")) m.SetFloat("_Metallic", metallic);
            if (m.HasProperty("_Smoothness")) m.SetFloat("_Smoothness", smooth);
            return m;
        }

        private static GameObject Part(Transform parent, string name, PrimitiveType type, Vector3 pos, Vector3 scale, Material mat)
        {
            GameObject go = GameObject.CreatePrimitive(type);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = pos;
            go.transform.localScale = scale;
            Renderer r = go.GetComponent<Renderer>();
            r.sharedMaterial = mat;
            Collider c = go.GetComponent<Collider>();
            if (c != null) Object.DestroyImmediate(c);
            return go;
        }

        private static void CreateOperator(string id, Color clothColor, Color armorColor, Color accentColor, bool paleHair, bool redHair)
        {
            GameObject root = new GameObject("ADIX_Operator_" + id);
            root.transform.position = Vector3.zero;
            Material cloth = Mat(id+"_Cloth", clothColor, .04f, .25f);
            Material armor = Mat(id+"_Armor", armorColor, .48f, .3f);
            Material accent = Mat(id+"_Accent", accentColor, .22f, .42f);
            Material skin = Mat("Operator_Skin", new Color(.72f,.53f,.42f), 0f, .28f);
            Material hair = Mat(id+"_Hair", paleHair ? new Color(.83f,.86f,.88f) : redHair ? new Color(.48f,.09f,.07f) : new Color(.055f,.045f,.04f), .02f, .2f);
            Material dark = Mat("Gear_Rubber", new Color(.035f,.045f,.055f), .05f, .2f);

            Part(root.transform,"Torso",PrimitiveType.Capsule,new Vector3(0,1.18f,0),new Vector3(.58f,.48f,.34f),cloth);
            Part(root.transform,"Armour Vest",PrimitiveType.Cube,new Vector3(0,1.28f,.19f),new Vector3(.53f,.52f,.16f),armor);
            Part(root.transform,"Chest Plate",PrimitiveType.Cube,new Vector3(0,1.43f,.285f),new Vector3(.34f,.22f,.035f),accent);
            Part(root.transform,"Utility Belt",PrimitiveType.Cube,new Vector3(0,.91f,.015f),new Vector3(.57f,.09f,.37f),dark);
            Part(root.transform,"Shoulder L",PrimitiveType.Sphere,new Vector3(-.37f,1.48f,0),new Vector3(.25f,.23f,.25f),armor);
            Part(root.transform,"Shoulder R",PrimitiveType.Sphere,new Vector3(.37f,1.48f,0),new Vector3(.25f,.23f,.25f),armor);
            Part(root.transform,"Upper Arm L",PrimitiveType.Capsule,new Vector3(-.42f,1.22f,0),new Vector3(.17f,.34f,.17f),cloth);
            Part(root.transform,"Upper Arm R",PrimitiveType.Capsule,new Vector3(.42f,1.22f,0),new Vector3(.17f,.34f,.17f),cloth);
            Part(root.transform,"Forearm L",PrimitiveType.Capsule,new Vector3(-.44f,.94f,.04f),new Vector3(.15f,.28f,.15f),armor);
            Part(root.transform,"Forearm R",PrimitiveType.Capsule,new Vector3(.44f,.94f,.04f),new Vector3(.15f,.28f,.15f),armor);
            Part(root.transform,"Glove L",PrimitiveType.Sphere,new Vector3(-.44f,.73f,.08f),new Vector3(.16f,.13f,.18f),dark);
            Part(root.transform,"Glove R",PrimitiveType.Sphere,new Vector3(.44f,.73f,.08f),new Vector3(.16f,.13f,.18f),dark);
            Part(root.transform,"Head",PrimitiveType.Sphere,new Vector3(0,1.88f,0),new Vector3(.29f,.34f,.28f),skin);
            Part(root.transform,"Hair Cap",PrimitiveType.Sphere,new Vector3(0,2.06f,-.015f),new Vector3(.32f,.22f,.31f),hair);
            Part(root.transform,"Hair Back",PrimitiveType.Sphere,new Vector3(0,1.91f,-.16f),new Vector3(.30f,.32f,.16f),hair);
            Part(root.transform,"Thigh L",PrimitiveType.Capsule,new Vector3(-.17f,.63f,0),new Vector3(.22f,.42f,.23f),cloth);
            Part(root.transform,"Thigh R",PrimitiveType.Capsule,new Vector3(.17f,.63f,0),new Vector3(.22f,.42f,.23f),cloth);
            Part(root.transform,"Knee Pad L",PrimitiveType.Sphere,new Vector3(-.17f,.38f,.13f),new Vector3(.21f,.15f,.12f),armor);
            Part(root.transform,"Knee Pad R",PrimitiveType.Sphere,new Vector3(.17f,.38f,.13f),new Vector3(.21f,.15f,.12f),armor);
            Part(root.transform,"Shin L",PrimitiveType.Capsule,new Vector3(-.17f,.22f,0),new Vector3(.17f,.28f,.18f),cloth);
            Part(root.transform,"Shin R",PrimitiveType.Capsule,new Vector3(.17f,.22f,0),new Vector3(.17f,.28f,.18f),cloth);
            Part(root.transform,"Boot L",PrimitiveType.Cube,new Vector3(-.17f,.055f,.07f),new Vector3(.22f,.12f,.34f),dark);
            Part(root.transform,"Boot R",PrimitiveType.Cube,new Vector3(.17f,.055f,.07f),new Vector3(.22f,.12f,.34f),dark);
            Part(root.transform,"Backpack",PrimitiveType.Cube,new Vector3(0,1.18f,-.27f),new Vector3(.42f,.52f,.22f),armor);
            Part(root.transform,"Pack Top",PrimitiveType.Cube,new Vector3(0,1.47f,-.27f),new Vector3(.34f,.12f,.2f),accent);
            Part(root.transform,"Radio",PrimitiveType.Cube,new Vector3(.29f,1.02f,-.18f),new Vector3(.11f,.2f,.1f),dark);
            Part(root.transform,"Thigh Pouch L",PrimitiveType.Cube,new Vector3(-.29f,.67f,.04f),new Vector3(.13f,.2f,.13f),armor);
            Part(root.transform,"Thigh Pouch R",PrimitiveType.Cube,new Vector3(.29f,.67f,.04f),new Vector3(.13f,.2f,.13f),armor);

            // Simple original weapon silhouette attached to right hand for the first visual blockout.
            GameObject rifle = Part(root.transform,"Rifle Receiver",PrimitiveType.Cube,new Vector3(.37f,1.0f,.34f),new Vector3(.12f,.12f,.52f),dark);
            rifle.transform.localRotation = Quaternion.Euler(8f,0f,0f);
            GameObject barrel = Part(root.transform,"Rifle Barrel",PrimitiveType.Cylinder,new Vector3(.37f,1.0f,.66f),new Vector3(.035f,.18f,.035f),armor);
            barrel.transform.localRotation = Quaternion.Euler(90f,0f,0f);

            string prefabPath = Root + "/ADIX_" + id + ".prefab";
            PrefabUtility.SaveAsPrefabAsset(root,prefabPath);
            Object.DestroyImmediate(root);
        }
    }
}
#endif
