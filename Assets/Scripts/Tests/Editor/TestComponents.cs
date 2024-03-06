using System;
using System.Collections.Generic;
using System.ComponentModel;
using Game.Properties;
using Game.Shared;
using NUnit.Framework;
using UnityEngine;

namespace TestEffects
{
    public class TestComponents
    {
        [Test]
        public void TestComponent()
        {
            var container = new Container();
            container.Add(new Effects());
            container.Add(new Properties());
        }

        public interface IProperties : IComponent
        {
            float GetValue(CharacterProperty characterProperty);
            bool Has(CharacterProperty characterProperty);

            void AddModifier(CharacterProperty characterProperty, BaseProperty modifier);
            void RemoveModifier(CharacterProperty characterProperty, BaseProperty modifier);
        }

        public  class Properties : IProperties
        {
            public ISite Site { get; set; }
            public event EventHandler Disposed;
            
            private readonly Dictionary<CharacterProperty, ModifiedProperty> _properties = new Dictionary<CharacterProperty, ModifiedProperty>();
            public void Dispose()
            {
                _properties.Clear();
            }

            public float GetValue(CharacterProperty characterProperty)
            {
                return GetProperty(characterProperty).GetValue();
            }

            public bool Has(CharacterProperty characterProperty)
            {
                return _properties.ContainsKey(characterProperty);
            }

            public void AddProperty(CharacterProperty characterProperty, float baseValue)
            {
                if (_properties.ContainsKey(characterProperty))
                {
                    throw new ArgumentException($"Property {characterProperty} already exists");
                }

                _properties.Add(characterProperty, new ModifiedProperty(baseValue));
            }

            private ModifiedProperty GetProperty(CharacterProperty characterProperty)
            {
                if (_properties.ContainsKey(characterProperty))
                {
                    return _properties[characterProperty];
                }

                throw new ArgumentException($"Can't find property {characterProperty}");
            }

            public void AddModifier(CharacterProperty characterProperty, BaseProperty modifier)
            {
                GetProperty(characterProperty).AddModifier(modifier);
            }

            public void RemoveModifier(CharacterProperty characterProperty, BaseProperty modifier)
            {
                GetProperty(characterProperty).RemoveModifier(modifier);
            }
        }
        
        public class Effects : IComponent
        {
            public event EventHandler Disposed;
            public ISite Site { get; set; }


            public void Dispose()
            {
                Debug.Log("effects dispose!");
            }
        }
    }


}