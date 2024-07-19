using System.Runtime.CompilerServices;
using System.Reflection;

namespace TheCollector
{
    public static class TCEnums
    {
        public static SlugcatStats.Name TheCollector = new("TheCollector");

        public static void Init()
        {
            RuntimeHelpers.RunClassConstructor(typeof(Sound).TypeHandle);
        }

        public static void UnregisterEnums(Type type)
        {
            var extEnums = type.GetFields(BindingFlags.Static | BindingFlags.Public).Where(x => x.FieldType.IsSubclassOf(typeof(ExtEnumBase)));

            foreach (var extEnum in extEnums)
            {
                var obj = extEnum.GetValue(null);
                if (obj != null)
                {
                    obj.GetType().GetMethod("Unregister")!.Invoke(obj, null);
                    extEnum.SetValue(null, null);
                }
            }
        }

        public static void Unregister()
        {
            UnregisterEnums(typeof(Sound));
        }

        public static class Sound
        {
            public static SoundID porl = new(nameof(porl), true);
            public static SoundID flap = new(nameof(flap), true);
            public static SoundID wind = new(nameof(wind), true);
        }

        public static class Color
        {
        }
    }
}

