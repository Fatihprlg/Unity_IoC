using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleSingleton : MonoBehaviour
{
    public static ExampleSingleton Instance => instance;
    private static ExampleSingleton instance;

    private void Start()
    {
        if(instance != null) Destroy(this);
        instance = this;
    }

    public void IAmASingleton()
    {
        print("Hi I am a singleton object!");
    }
}
