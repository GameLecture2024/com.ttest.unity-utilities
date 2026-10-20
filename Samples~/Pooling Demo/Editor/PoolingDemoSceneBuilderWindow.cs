using System.IO;
using TTest.UnityUtilities.Samples.PoolingDemo;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TTest.UnityUtilities.Samples.PoolingDemo.Editor
{
    public sealed class PoolingDemoSceneBuilderWindow : EditorWindow
    {
        private const string DefaultFolder = "Assets/Samples/TTest Unity Utilities/0.1.0/Pooling Demo";
        private string _outputFolder = DefaultFolder;

        [MenuItem("TTest Samples/Unity Utilities/Create Pooling Demo Scene")]
        private static void Open() => GetWindow<PoolingDemoSceneBuilderWindow>("Pooling Demo Builder");

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Pooling Demo Scene", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Creates a Cube prefab and a ready-to-play pooling scene. Existing generated files are never overwritten.", MessageType.Info);
            _outputFolder = EditorGUILayout.TextField("Output Folder", _outputFolder);

            using (new EditorGUI.DisabledScope(!IsAssetFolderPath(_outputFolder)))
            {
                if (GUILayout.Button("Create Demo Scene", GUILayout.Height(30)))
                    CreateDemo();
            }

            if (!IsAssetFolderPath(_outputFolder))
                EditorGUILayout.HelpBox("Use an Assets-relative folder, for example Assets/Samples/…", MessageType.Error);
        }

        private void CreateDemo()
        {
            var scenePath = $"{_outputFolder}/PoolingDemo.unity";
            var prefabPath = $"{_outputFolder}/PoolingDemoCube.prefab";
            if (File.Exists(scenePath) || File.Exists(prefabPath))
            {
                EditorUtility.DisplayDialog("Pooling Demo", "A generated scene or prefab already exists. Choose another folder or delete it before generating again.", "OK");
                return;
            }

            EnsureFolder(_outputFolder);
            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

            var spawnRoot = new GameObject("Spawn Root").transform;
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = "Pooling Demo Cube";
            cube.AddComponent<PoolingDemoCube>();
            cube.SetActive(false);
            var prefab = PrefabUtility.SaveAsPrefabAsset(cube, prefabPath);
            Object.DestroyImmediate(cube);

            var spawnerObject = new GameObject("Pooling Demo Spawner");
            var spawner = spawnerObject.AddComponent<PoolingDemoSpawner>();
            var serializedSpawner = new SerializedObject(spawner);
            serializedSpawner.FindProperty("cubePrefab").objectReferenceValue = prefab.GetComponent<PoolingDemoCube>();
            serializedSpawner.FindProperty("spawnRoot").objectReferenceValue = spawnRoot;
            serializedSpawner.ApplyModifiedPropertiesWithoutUndo();

            EditorSceneManager.SaveScene(scene, scenePath);
            AssetDatabase.SaveAssets();
            Selection.activeObject = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
            EditorUtility.DisplayDialog("Pooling Demo", $"Created:\n{scenePath}", "Open Scene");
            EditorSceneManager.OpenScene(scenePath);
        }

        private static bool IsAssetFolderPath(string path) => !string.IsNullOrWhiteSpace(path) && path.Replace('\\', '/').StartsWith("Assets/");

        private static void EnsureFolder(string folder)
        {
            var parts = folder.Replace('\\', '/').Split('/');
            var current = parts[0];
            for (var index = 1; index < parts.Length; index++)
            {
                var next = $"{current}/{parts[index]}";
                if (!AssetDatabase.IsValidFolder(next))
                    AssetDatabase.CreateFolder(current, parts[index]);
                current = next;
            }
        }
    }
}
