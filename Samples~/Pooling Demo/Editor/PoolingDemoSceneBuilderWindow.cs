using System.IO;
using TTest.UnityUtilities.Pooling;
using TTest.UnityUtilities.Samples.PoolingDemo;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TTest.UnityUtilities.Samples.PoolingDemo.Editor
{
    public sealed class PoolingDemoSceneBuilderWindow : EditorWindow
    {
        private const string DefaultFolder = "Assets/Samples/TTest Unity Utilities/0.2.0/Pooling Demo";
        private string _outputFolder = DefaultFolder;

        [MenuItem("TTest Samples/Unity Utilities/Create Top View Pooling Demo")]
        private static void Open() => GetWindow<PoolingDemoSceneBuilderWindow>("Pooling Demo Builder");

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Top View Pooling Demo", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Creates a ready-to-play 2D scene with pooled square, triangle, and circle enemies. Existing generated files are never overwritten.", MessageType.Info);
            _outputFolder = EditorGUILayout.TextField("Output Folder", _outputFolder);
            using (new EditorGUI.DisabledScope(!IsAssetFolderPath(_outputFolder)))
                if (GUILayout.Button("Create Demo Scene", GUILayout.Height(30))) CreateDemo();
            if (!IsAssetFolderPath(_outputFolder))
                EditorGUILayout.HelpBox("Use an Assets-relative folder, for example Assets/Samples/My Demo.", MessageType.Error);
        }

        private void CreateDemo()
        {
            var scenePath = $"{_outputFolder}/TopViewPoolingDemo.unity";
            if (File.Exists(scenePath))
            {
                EditorUtility.DisplayDialog("Pooling Demo", "The scene already exists. Choose another folder or remove the generated scene first.", "OK");
                return;
            }
            EnsureFolder(_outputFolder);
            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            var camera = Camera.main;
            camera.orthographic = true;
            camera.orthographicSize = 6f;
            camera.transform.position = new Vector3(0f, 0f, -10f);
            camera.backgroundColor = new Color(.06f, .08f, .12f);

            var player = new GameObject("Player");
            AddVisual(player, TopViewShapeVisual.Shape.Circle, new Color(.25f, .8f, 1f), 1.15f);
            var demo = new GameObject("Top View Pooling Demo");
            var spawner = demo.AddComponent<TopViewDemoSpawner>();
            var pools = new[]
            {
                CreatePool(demo.transform, "Square Pool", CreateEnemyPrefab("Square Enemy", TopViewShapeVisual.Shape.Square, new Color(1f, .35f, .35f))),
                CreatePool(demo.transform, "Triangle Pool", CreateEnemyPrefab("Triangle Enemy", TopViewShapeVisual.Shape.Triangle, new Color(1f, .8f, .2f))),
                CreatePool(demo.transform, "Circle Pool", CreateEnemyPrefab("Circle Enemy", TopViewShapeVisual.Shape.Circle, new Color(.65f, .4f, 1f)))
            };
            var serializedSpawner = new SerializedObject(spawner);
            serializedSpawner.FindProperty("player").objectReferenceValue = player.transform;
            serializedSpawner.FindProperty("enemyPools").arraySize = pools.Length;
            for (var index = 0; index < pools.Length; index++)
                serializedSpawner.FindProperty("enemyPools").GetArrayElementAtIndex(index).objectReferenceValue = pools[index];
            serializedSpawner.ApplyModifiedPropertiesWithoutUndo();
            EditorSceneManager.SaveScene(scene, scenePath);
            AssetDatabase.SaveAssets();
            EditorSceneManager.OpenScene(scenePath);
        }

        private PoolSpawner CreatePool(Transform parent, string poolName, GameObject prefab)
        {
            var poolObject = new GameObject(poolName);
            poolObject.transform.SetParent(parent);
            var pool = poolObject.AddComponent<PoolSpawner>();
            var serializedPool = new SerializedObject(pool);
            serializedPool.FindProperty("prefab").objectReferenceValue = prefab;
            serializedPool.FindProperty("prewarmCount").intValue = 6;
            serializedPool.FindProperty("maxSize").intValue = 32;
            serializedPool.ApplyModifiedPropertiesWithoutUndo();
            return pool;
        }

        private GameObject CreateEnemyPrefab(string name, TopViewShapeVisual.Shape shape, Color color)
        {
            var enemy = new GameObject(name);
            enemy.AddComponent<TopViewEnemy>();
            AddVisual(enemy, shape, color, .9f);
            enemy.SetActive(false);
            var prefab = PrefabUtility.SaveAsPrefabAsset(enemy, $"{_outputFolder}/{name.Replace(" ", string.Empty)}.prefab");
            DestroyImmediate(enemy);
            return prefab;
        }

        private static void AddVisual(GameObject owner, TopViewShapeVisual.Shape shape, Color color, float size)
        {
            var visual = owner.AddComponent<TopViewShapeVisual>();
            var serializedVisual = new SerializedObject(visual);
            serializedVisual.FindProperty("shape").enumValueIndex = (int)shape;
            serializedVisual.FindProperty("color").colorValue = color;
            serializedVisual.FindProperty("size").floatValue = size;
            serializedVisual.ApplyModifiedPropertiesWithoutUndo();
        }

        private static bool IsAssetFolderPath(string path) => !string.IsNullOrWhiteSpace(path) && path.Replace('\\', '/').StartsWith("Assets/");
        private static void EnsureFolder(string folder)
        {
            var parts = folder.Replace('\\', '/').Split('/');
            var current = parts[0];
            for (var index = 1; index < parts.Length; index++)
            {
                if (!AssetDatabase.IsValidFolder($"{current}/{parts[index]}")) AssetDatabase.CreateFolder(current, parts[index]);
                current = $"{current}/{parts[index]}";
            }
        }
    }
}
