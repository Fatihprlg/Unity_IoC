using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class MonoBuilder : Builder
{
    public override void Build(Container container)
    {
        for (int i = 0; i < subBuilders.Count; i++)
        {
            subBuilders[i].Build(container);
        }

        base.Build(container);
    }

    [EditorButton]
    public void GetChildMonoBehaviours()
    {
        MonoBase[] monos = transform.GetComponentsInChildren<MonoBase>();
        
        classes.Clear();
        for (int i = 1; i < monos.Length; i++)
        {
            ClassInfo info = new ClassInfo();
            info.implementation = monos[i];
            info.isSingleton = monos[i].gameObject == singletonGameObject;
            classes.Add(info);
        }
    }

    [EditorButton]
    public void GetChildBuilders()
    {
        subBuilders.Clear();
        var builders = transform.GetComponentsInChildren<MonoBuilder>();
        for (int i = 1; i < builders.Length; i++)
        {
            subBuilders.Add(builders[i]);
        }
    }
}
