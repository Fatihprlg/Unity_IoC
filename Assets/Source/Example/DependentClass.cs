using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class DependentClass : MonoBehaviour
{
    [Dependency] private ExampleClass example1;
    [Dependency] private ExampleSingleton singleton;
    public void Start()
    {
        this.Inject();
        TestIt();
        SingletonIsHere();
    }

    public void TestIt()
    {
        print(example1.text);
        example1.HelloDear("the great developer");
    }

    public void SingletonIsHere()
    {
        singleton.IAmASingleton();
    }
}
