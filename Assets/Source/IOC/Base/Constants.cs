using System.Reflection;

namespace IOC
{
    public class Constants
    {
        public const BindingFlags DependencyAttributeFlags =
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic;

        public const BindingFlags SingletonFlag = BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic;
        public const string IOCSettingsPath = "Assets/Resources/IOCSettings.asset";
    }
}