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
    public List<ClassInfo> classes { get; private set; }
    public void Build(Container container)
    {
        if(classes == null || classes.Count <1) MapClasses();
        foreach (var item in classes)
        {
            if (item.implementation == null) continue;
            var type = item.implementation.GetType();
            container.Register(type.BaseType, type, item);
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
        classes = new List<ClassInfo>();
        classes.Clear();
        var singletonRegex = new Regex("[_iI]nstance");
        var monos = FindObjectsOfType<MonoBehaviour>(true);
        foreach (var mono in monos)
        {
            if (mono == null) continue;
            if(mono.GetType().Assembly != Assembly.GetAssembly(GetType())) continue;
            bool isSingleton = mono.GetType().GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic)
                                   .Any(info => singletonRegex.IsMatch(info.Name))
                               || mono.GetType().GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic)
                                   .Any(info => singletonRegex.IsMatch(info.Name));
            ClassInfo info = new ()
            {
                implementation = mono,
                isSingleton = isSingleton
            };
            classes.Add(info);
        }
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
        if(builder.classes != null)
        {
            for (int index = 0; index < builder.classes.Count; index++)
            {
                ClassInfo @class = builder.classes[index];
                GUIContent name = new ((index + 1) + ". " + @class.implementation.GetType().Name);
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
        Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding+thickness));
        r.height = thickness;
        r.y+=padding/2;
        r.x-=2;
        r.width +=6;
        EditorGUI.DrawRect(r, color);
    }
}
#endif