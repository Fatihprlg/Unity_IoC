using UnityEditor;
using UnityEngine;

namespace IOC.Editor
{
    
[CustomEditor(typeof(Builder))]
public class BuilderEditor : UnityEditor.Editor
{
    private Builder _builder;
    private GUIContent _mapClassesContent;

    private void OnEnable()
    {
        _builder = target as Builder;
        _mapClassesContent = new GUIContent("Map Classes On Scene");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        DrawUILine(Color.white);
        EditorGUILayout.BeginVertical();
        if (GUILayout.Button(_mapClassesContent))
        {
            _builder.MapClasses();
        }

        DrawUILine(Color.white);
        GUILayout.Label("Classes on Scene");
        GUILayout.Space(5);
        if (_builder.AllSceneInstances != null)
        {
            for (int index = 0; index < _builder.AllSceneInstances.Count; index++)
            {
                var @class = _builder.AllSceneInstances[index];
                GUIContent className = new((index + 1) + ". " + @class.implementation.GetType().Name);
                if (GUILayout.Button(className,
                        EditorStyles.linkLabel))
                {
                    Selection.SetActiveObjectWithContext(@class.implementation, null);
                }

                var rect = GUILayoutUtility.GetLastRect();
                rect.width = EditorStyles.linkLabel.CalcSize(className).x;
                EditorGUIUtility.AddCursorRect(rect, MouseCursor.Link);
            }
        }


        DrawUILine(Color.white);
        EditorGUILayout.EndVertical();
    }

    private static void DrawUILine(Color color, int thickness = 2, int padding = 10)
    {
        var r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
        r.height = thickness;
        r.y += padding / 2;
        r.x -= 2;
        r.width += 6;
        EditorGUI.DrawRect(r, color);
    }
}
}