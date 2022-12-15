using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Builder : MonoBase
{
    [SerializeField] protected GameObject singletonGameObject;
    [SerializeField] protected List<ClassInfo> classes;
    [SerializeField] protected List<Builder> subBuilders;

    public virtual void Build(Container container)
    {
        foreach (var item in classes)
        {
            var type = item.implementation.GetType();
            container.Register(type.BaseType, type, item);
        }
    }
    
}
