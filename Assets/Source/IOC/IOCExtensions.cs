using System;
public static class IOCExtensions
{
    private static IOC.Container _injector;

    public static void SetDependencyInjector(IOC.Container dependencyInjector)
    {
        _injector = dependencyInjector;
    }

    public static void Inject<T>(this T classToInject) where T : class
    {
        _injector.Inject<T>(classToInject);
    }

    public static void Inject(this Type typeToInject, object classObject)
    {
        _injector.Inject(typeToInject, classObject);
    }
}