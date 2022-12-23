using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;
#endif

public class Builder : MonoBase
{
    [SerializeField] protected List<ClassInfo> classes;

    public virtual void Build(Container container)
    {
        foreach (var item in classes)
        {
            var type = item.implementation.GetType();
            container.Register(type.BaseType, type, item);
        }
    }

#if UNITY_EDITOR
    public void OnValidate()
    {
        AssemblyReloadEvents.afterAssemblyReload -= MapClassesOnScene;
        AssemblyReloadEvents.afterAssemblyReload += MapClassesOnScene;
    }
    
    [EditorButton]
    private void MapClassesOnScene()
    {
        classes.Clear();
        var monos = FindObjectsOfType<MonoBase>();
        foreach (var mono in monos)
        {
            ClassInfo info = new ClassInfo()
            {
                implementation = mono,
                isSingleton = mono.GetType().IsSubclassOf(typeof(MonoSingleton<>))
            };
            classes.Add(info);
        }
    }
#endif
}