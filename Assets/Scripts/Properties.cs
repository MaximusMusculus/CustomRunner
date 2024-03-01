using System;
using System.Collections.Generic;

public enum FloatProperty
{
    RunSpeed,
    JumpSpeed,
    FlyHeight,
}

public interface IProperties
{
    bool Has(FloatProperty property);
    float Get(FloatProperty floatProperty);
}

public interface IModifyingProperties : IProperties
{
    void Modify(FloatProperty floatProperty, float newValue); // +source
}


/// <summary>
/// Класс для хранения значений свойств
/// </summary>
public class Properties : IProperties
{
    private Dictionary<FloatProperty, float> _floats = new Dictionary<FloatProperty, float>();

    public bool Has(FloatProperty property)
    {
        return _floats.ContainsKey(property);
    }

    public float Get(FloatProperty floatProperty)
    {
        return _floats[floatProperty];
    }
    public void Set(FloatProperty floatProperty, float value)
    {
        _floats[floatProperty] = value;
    }
}

/// <summary>
/// Класс для изменения значений свойств,
/// </summary>
public class ModifyProperties : IModifyingProperties
{
    private readonly IProperties _baseValueProperties;
    private readonly Properties _modifiedProperties = new Properties();

    public ModifyProperties(IProperties baseValueProperties)
    {
        _baseValueProperties = baseValueProperties;
    }

    public bool Has(FloatProperty property)
    {
        return _baseValueProperties.Has(property);
    }

    public float Get(FloatProperty floatProperty)
    {
        if (_modifiedProperties.Has(floatProperty))
        {
            return _modifiedProperties.Get(floatProperty);
        }
        if (_baseValueProperties.Has(floatProperty))
        {
            return _baseValueProperties.Get(floatProperty);
        }
        throw new ArgumentException($"Property {floatProperty} not exists in base properties.");
    }

    public void Modify(FloatProperty floatProperty, float newValue)
    {
        _modifiedProperties.Set(floatProperty, newValue);
    }
}


