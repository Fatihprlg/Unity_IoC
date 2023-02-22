using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Reflection;
using System.Text.RegularExpressions;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Builder : MonoBehaviour, IBuilder
{
    public List<ClassInfo> AllSceneInstances { get; private set; }

    public void Build(Container container)
    {
        if (AllSceneInstances == null || AllSceneInstances.Count < 1)
        {
            MapClasses();
        }

        foreach (var instance in AllSceneInstances)
        {
            if (instance.implementation == null) continue;
            var type = instance.implementation.GetType();
            container.Register(type.BaseType, type, instance);
        }
    }

#if UNITY_EDITOR
    public void OnValidate()
    {
        AssemblyReloadEvents.afterAssemblyReload -= MapClasses;
        AssemblyReloadEvents.afterAssemblyReload += MapClasses;
    }
#endif

    public void MapClasses()
    {
        AllSceneInstances = GetSceneInstanceReferences();

    }
    private List<ClassInfo> GetSceneInstanceReferences()
    {
        var wrappedSceneObjects= new List<ClassInfo>();
        var singletonRegex = new Regex("[_iI]nstance");
        var sceneObjects = FindObjectsOfType<MonoBehaviour>(true);
        foreach (var sceneObject in sceneObjects)
        {
            if (sceneObject == null) continue;
            if (sceneObject.GetType().Assembly != Assembly.GetAssembly(GetType())) continue;

            var singletonFlag = BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic;

            var isHaveSingletonField = sceneObject.GetType().GetFields(singletonFlag)
                .Any(info => singletonRegex.IsMatch(info.Name));
            var isHaveSingletonProperty = sceneObject.GetType().GetProperties(singletonFlag)
                .Any(info => singletonRegex.IsMatch(info.Name));
            ClassInfo info = new()
            {
                implementation = sceneObject,
                isSingleton = isHaveSingletonField || isHaveSingletonProperty,
            };
            wrappedSceneObjects.Add(info);
        }

        return wrappedSceneObjects;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(Builder))]
public class BuilderEditor : Editor
{
    private Builder builder;
    private GUIContent mapClassesContent;

    private void OnEnable()
    {
        builder = target as Builder;
        mapClassesContent = new GUIContent("Map Classes On Scene");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        DrawUILine(Color.white);
        EditorGUILayout.BeginVertical();
        if (GUILayout.Button(mapClassesContent))
        {
            builder.MapClasses();
        }

        DrawUILine(Color.white);
        GUILayout.Label("Classes on Scene");
        GUILayout.Space(5);
        if (builder.AllSceneInstances != null)
        {
            for (int index = 0; index < builder.AllSceneInstances.Count; index++)
            {
                ClassInfo @class = builder.AllSceneInstances[index];
                GUIContent name = new((index + 1) + ". " + @class.implementation.GetType().Name);
                if (GUILayout.Button(name,
                        EditorStyles.linkLabel))
                {
                    Selection.SetActiveObjectWithContext(@class.implementation, null);
                }

                var rect = GUILayoutUtility.GetLastRect();
                rect.width = EditorStyles.linkLabel.CalcSize(name).x;
                EditorGUIUtility.AddCursorRect(rect, MouseCursor.Link);
            }
        }


        DrawUILine(Color.white);
        EditorGUILayout.EndVertical();
    }

    private void DrawUILine(Color color, int thickness = 2, int padding = 10)
    {
        Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
        r.height = thickness;
        r.y += padding / 2;
        r.x -= 2;
        r.width += 6;
        EditorGUI.DrawRect(r, color);
    }
}
#endif