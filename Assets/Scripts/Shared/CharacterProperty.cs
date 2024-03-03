
using System.Collections.Generic;

namespace Game.Shared
{
   public enum CharacterProperty
    {
        Speed,
        FlyHeight,
    }
    
    [System.Serializable]
    public class PropertyData
    {
        public CharacterProperty property;
        public float value;
    }

    
    [System.Serializable]
    public class PropertiesData
    {
        public List<PropertyData> properties;
    }
}