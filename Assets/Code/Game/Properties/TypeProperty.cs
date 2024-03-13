
using System.Collections.Generic;

namespace Game.Shared
{
    public enum TypeProperty
    {
        Speed,
        JumpForce,
        FlyHeight,
    }

    [System.Serializable]
    public class PropertyConfig
    {
        public List<PropertyInfo> properties;
    }

    [System.Serializable]
    public class PropertyInfo
    {
        public TypeProperty property;
        public float value;
    }
}