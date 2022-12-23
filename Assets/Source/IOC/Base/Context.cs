using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Context : MonoBase
{
    [SerializeField] Builder mainBuilder;
    Container container;
    public override void Initialize()
    {
        container = new();
        if (mainBuilder == null) CreateBuilder();
        mainBuilder.Build(container);
        container.Initialize();
    }

    private void CreateBuilder()
    {
        mainBuilder = FindObjectOfType<Builder>();
        if(mainBuilder != null) return;
        Debug.LogError("There is no builder in scene. Creating temporary instance.");
        var gObj = new GameObject("TemporaryBuilder", typeof(MonoBuilder));
        mainBuilder = gObj.GetComponent<MonoBuilder>();
    }
}