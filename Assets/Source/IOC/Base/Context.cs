using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Context : MonoBase
{
    [SerializeField] List<Builder> mainBuilders;
    [SerializeField] Container container;

    public override void Initialize()
    {
        base.Initialize();
        mainBuilders.ForEach(b => b.Build(container));
        container.Initialize();
    }
}
