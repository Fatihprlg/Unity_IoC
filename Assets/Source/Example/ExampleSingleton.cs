using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleSingleton : MonoSingleton<ExampleSingleton>
{
    public void IAmASingleton()
    {
        print("Hi I am a singleton object!");
    }
}
