using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Context : MonoBehaviour, IContext
{
    [SerializeField] protected Builder mainBuilder;
    [SerializeField] private bool initializeOnAwake = true;
    private Container container;

    private void Awake()
    {
        if(!initializeOnAwake) return;
        Initialize();
    }

    public void Initialize()
    {
        container = new Container();
        if (mainBuilder == null) CreateBuilder();
        mainBuilder.Build(container);
        IOCExtensions.SetDependencyInjector(container);
    }

    public void CreateBuilder()
    {
        mainBuilder = FindObjectOfType<Builder>();
        if(mainBuilder != null) return;
        Debug.LogError("There is no builder in scene. Creating temporary instance.");
        GameObject gObj = new ("TemporaryBuilder", typeof(Builder));
        mainBuilder = gObj.GetComponent<Builder>();
    }
}