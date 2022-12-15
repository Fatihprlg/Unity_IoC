using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleClass : MonoBase
{
    public string text = "Dependency successful!!!";

    public void HelloDear(string name)
    {
        print("Hi " + name + "!");
    }
}
