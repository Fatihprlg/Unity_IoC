using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleSingleton : MonoBase
{
    public ExampleSingleton Instance => _instance;
    private ExampleSingleton _instance;
    void Start()
    {
        if (_instance == null) _instance = this;
        else Destroy(this);
    }

    public void IAmASingleton()
    {
        print("Hi I am a singleton object!");
    }
}
