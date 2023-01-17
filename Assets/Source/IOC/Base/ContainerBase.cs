using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

[Serializable]
public class Container
{
    protected Dictionary<Type, Dictionary<string, GameObject>> implementations = new Dictionary<Type, Dictionary<string, GameObject>>();
    protected readonly Dictionary<Type, Dictionary<string, Type>> types = new Dictionary<Type, Dictionary<string, Type>>();
    protected readonly Dictionary<Type, TypeData> typeDatas = new Dictionary<Type, TypeData>();

    public virtual void Register(Type interfaceType, Type type, ClassInfo info)
    {
        try
        {
            TypeData typeData = TypeData.Create(type, info.isSingleton, type.IsSubclassOf(typeof(MonoBehaviour)) ? info.implementation : null);
            string key = "";
            object implementation;
            if (type.IsSubclassOf(typeof(MonoBehaviour)))
            {
                key = info.implementation.gameObject.name;
                implementation = info.implementation.gameObject;
            }
            else
            {
                implementation = ConstructClass(type);
            }
            if (types.ContainsKey(interfaceType ?? type))
            {
                var regex = new Regex($"{key}[0-9]*$");
                if (types[interfaceType ?? type].ContainsKey(key)) key = key + types[interfaceType ?? type].Count(typePointer => regex.IsMatch(typePointer.Key));
                types[interfaceType ?? type].Add(key, type);
            }
            else
            {
                types.Add(interfaceType ?? type, new Dictionary<string, Type> { { key, type } });
            }
            if (!typeDatas.ContainsKey(type))
            {
                typeDatas.Add(type, typeData);
            }
            if (!implementations.ContainsKey(type))
            {
                implementations.Add(type, new Dictionary<string, GameObject> { { key, implementation as GameObject} });
            }
            else
            {
                implementations[type].Add(key, implementation as GameObject);
            }

            
        }
        catch (Exception ex)
        {
            throw new Exception("Register type failed. ", ex);
        }
    }

    public T Inject<T>(object obj)
    {
        return (T)Inject(typeof(T), obj);
    }

    private object ConstructClass(Type type)
    {
        var implementation = Activator.CreateInstance(type);
        return implementation;
    } 

    private static void Guard(bool failed, string format, params object[] args)
    {
        if (failed) throw new Exception(string.Format(format, args), null);
    }

    private TypeData GetTypeData(Type type)
    {
        if (!typeDatas.ContainsKey(type))
        {
            var typeData = TypeData.Create(type);
            typeDatas.Add(type, typeData);
            if (types.ContainsKey(type))
            {
                types[type].Add(string.Empty, type);
            }
            else
            {
                types.Add(type, new Dictionary<string, Type> { { string.Empty, type } });
            }
        }

        return typeDatas[type];
    }

    public object Inject(Type type, object obj)
    {
        var typeData = GetTypeData(type);
        
        typeData.Fields.ForEach(x => x.Value.SetValue(obj, Resolve(x.Value.FieldType, string.IsNullOrEmpty(x.Key.Key) ? GetImplementation(x.Value.FieldType).gameObject.name : x.Key.Key)));
        typeData.Properties.ForEach(x => x.Value.SetValue(obj, Resolve(x.Value.PropertyType, x.Key.Key), null));

        return obj;
    }

    public object Resolve(Type type, string key = null)
    {
        Guard(!types[type.BaseType].ContainsKey(key ?? string.Empty),
            "There is no implementation registered with the key {0} for the type {1}.", key, type.Name);

        var foundType = types[type.BaseType][key ?? string.Empty];
        var implementationOfType = implementations[type][key ?? string.Empty].GetComponent(type) ?? Inject(foundType, implementations[type][key ?? string.Empty].AddComponent(foundType));
        var typeData = typeDatas[foundType];

        if (foundType.IsSubclassOf(typeof(MonoBehaviour)))
        {
            typeData.Instance = implementationOfType;
            return implementationOfType;
        }

        if (typeData.IsSingleton)
        {
            return typeData.Instance ?? (typeData.Instance = Setup(foundType));
        }

        return Setup(foundType);
    }
    private object Setup(Type type)
    {
        var instance = Activator.CreateInstance(type);
        Inject(type, instance);
        return instance;
    }

    private Component GetImplementation(Type type, string key = null)
    {
        Guard(!implementations.ContainsKey(type), "There is no implementation of {0} for the {1}", type.Name, key ?? string.Empty);
        GameObject imp;
        if (string.IsNullOrEmpty(key))
        {
            imp = implementations[type].FirstOrDefault().Value;
        }
        else
        {
            imp = implementations[type][key];
        }
        var implementationOfType = imp.GetComponent(type);
        return implementationOfType;
    }

}
[Serializable]
public struct ClassInfo
{
    public MonoBehaviour implementation;
    public bool isSingleton;
}
