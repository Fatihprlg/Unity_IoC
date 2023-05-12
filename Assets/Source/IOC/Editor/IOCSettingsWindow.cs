using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace IOC.Editor
{
    public class IOCSettingsWindow : EditorWindow
    {
        [MenuItem("IoC/Show IoC Settings")]
        public static void ShowIOCSettings()
        {
            var asset = AssetDatabase.LoadAssetAtPath<Object>(Constants.IOCSettingsPath);
            if (!asset)
            {
                var p = Constants.IOCSettingsPath.Split('/').ToList();
                p.RemoveAt(p.Count - 1);
                p.RemoveAt(0);
                var path = string.Join("", p);
                if (!AssetDatabase.IsValidFolder(path))
                {
                    AssetDatabase.CreateFolder("Assets", path);
                }

                IOCSettings settings = CreateInstance<IOCSettings>();
                AssetDatabase.CreateAsset(settings, Constants.IOCSettingsPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                asset = AssetDatabase.LoadAssetAtPath<Object>(Constants.IOCSettingsPath);
            }

            EditorGUIUtility.PingObject(asset);
            Selection.activeObject = asset;
        }

        [MenuItem("IoC/Setup IoC on Scene")]
        public static void SetupIOC()
        {
            Builder builder = FindObjectOfType<Builder>();
            if (builder) DestroyImmediate(builder);

            var gameObject = new GameObject("Builder", typeof(Builder));
            EditorGUIUtility.PingObject(gameObject);
        }
    }
}