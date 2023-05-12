using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace IOC
{
    public class Context : IContext
    {
        private static IOCSettings _settings;
        private static bool _autoInjectDependencies = true;
        private static List<AssemblyField> _assemblies = null;
        private static Builder _mainBuilder;
        private static Container _container;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            FetchSettings();
            ManageDependencies();
            SceneManager.sceneLoaded += OnSceneLoaded;

        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            ManageDependencies();
        }

        private static void ManageDependencies()
        {
            _container?.Dispose();
            _container = new Container();
            IOCExtensions.SetDependencyInjector(_container);
            CreateBuilder();
            _mainBuilder.Build(_container, _assemblies);
            if (!_autoInjectDependencies) return;
            var dependentClasses = GetInstancesWithDependencyAttribute(_mainBuilder.AllSceneInstances).ToArray();
            foreach (var dependentClass in dependentClasses)
            {
                dependentClass.implementation.GetType().Inject(dependentClass.implementation);
            } 
        }
        
        private static void CreateBuilder()
        {
            _mainBuilder = Object.FindObjectOfType<Builder>();
            if (_mainBuilder != null) return;
            Debug.LogError("There is no builder in scene. Creating temporary instance.");
            GameObject gObj = new("TemporaryBuilder", typeof(Builder));
            _mainBuilder = gObj.GetComponent<Builder>();
        }

        private static void FetchSettings()
        {
            Debug.Log("Fetching, {0}", _settings);
            if(!_settings)
                _settings = Resources.Load<IOCSettings>(Constants.IOCSettingsPath);
            if (_settings)
            {
                _autoInjectDependencies = _settings.autoInjectDependencies;
                _assemblies = _settings.Assemblies;
            }
            
        }

        private static IEnumerable<ClassInfo> GetInstancesWithDependencyAttribute(List<ClassInfo> instances)
        {
            var types = new List<ClassInfo>();
            foreach (var type in instances)
            {
                var fields = type.implementation.GetType().GetFields(Constants.DependencyAttributeFlags);
                var props = type.implementation.GetType().GetProperties(Constants.DependencyAttributeFlags);
                if (fields.Any(fieldInfo =>
                        fieldInfo.GetCustomAttributes(typeof(DependencyAttribute), true).Length > 0) ||
                    props.Any(
                        propertyInfo => propertyInfo.GetCustomAttributes(typeof(DependencyAttribute), true).Length > 0))
                {
                    types.Add(type);
                }
            }

            return types;
        }
    }
}