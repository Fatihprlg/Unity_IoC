using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Reflection;
using System.Text.RegularExpressions;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace IOC
{
    public class Builder : MonoBehaviour, IBuilder
    {
        public List<ClassInfo> AllSceneInstances { get; private set; }
        [SerializeField, HideInInspector] private List<AssemblyField> _assemblies;

        public void Build(Container container, List<AssemblyField> assemblies)
        {
            if (AllSceneInstances == null || AllSceneInstances.Count < 1)
            {
                MapClasses();
            }

            if (assemblies is { Count: > 0 })
            {
                _assemblies = assemblies;
            }

            foreach (var instance in AllSceneInstances)
            {
                if (instance.implementation == null) continue;
                var type = instance.implementation.GetType();
                container.Register(type.BaseType, type, instance);
            }

        }

    #if UNITY_EDITOR
        public void OnValidate()
        {
            AssemblyReloadEvents.afterAssemblyReload -= MapClasses;
            AssemblyReloadEvents.afterAssemblyReload += MapClasses;
        }
    #endif
    
        public void MapClasses()
        {
            AllSceneInstances = GetSceneInstanceReferences();
        }
        private List<ClassInfo> GetSceneInstanceReferences()
        {
            var wrappedSceneObjects= new List<ClassInfo>();
            var singletonRegex = new Regex("[_iI]nstance");
            bool asmDefined = _assemblies is { Count: > 0 };
            if(!asmDefined) Debug.LogWarning("You did not define IOC included assemblies, it will look only its own assembly by default."); 
            var sceneObjects = FindObjectsOfType<MonoBehaviour>(true);
            foreach (var sceneObject in sceneObjects)
            {
                if (sceneObject == null) continue;
                if(asmDefined)
                    if(!_assemblies.Exists(asm => sceneObject.GetType().Assembly.GetName().Name ==  asm)) continue;
                else
                    if (sceneObject.GetType().Assembly != Assembly.GetAssembly(GetType())) continue;
    
    
                var isHaveSingletonField = sceneObject.GetType().GetFields(Constants.SingletonFlag)
                    .Any(info => singletonRegex.IsMatch(info.Name));
                var isHaveSingletonProperty = sceneObject.GetType().GetProperties(Constants.SingletonFlag)
                    .Any(info => singletonRegex.IsMatch(info.Name));
                ClassInfo info = new()
                {
                    implementation = sceneObject,
                    isSingleton = isHaveSingletonField || isHaveSingletonProperty,
                };
                wrappedSceneObjects.Add(info);
            }
    
            return wrappedSceneObjects;
        }
    }
}