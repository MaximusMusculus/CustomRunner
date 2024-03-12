using System;
using System.Collections.Generic;
using System.ComponentModel;
using Game.Properties;
using Game.Shared;
using NUnit.Framework;
using Unity.Plastic.Newtonsoft.Json;
using Unity.Plastic.Newtonsoft.Json.Linq;
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
        

        public interface ITest
        {
        }

        public class MyClass : ITest
        {
            public float a;
            public float b;

            public MyClass()
            {
            }

            public MyClass(float a, float b)
            {
                this.a = a;
                this.b = b;
            }
        }
        public class My2:ITest
        {
            public string value;

            public My2()
            {
            }

            public My2(string value)
            {
                this.value = value;
            }
        }
        
        public enum TypeTest
        {
            My2,
            MyClass,
        }
        
        public class TestConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return typeof(ITest).IsAssignableFrom(objectType);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                JObject jo = JObject.Load(reader);
                TypeTest typeTest = jo["tT"].ToObject<TypeTest>();
                switch (typeTest)
                {
                    case TypeTest.MyClass:
                        return jo.ToObject<MyClass>();
                    case TypeTest.My2:
                        return jo.ToObject<My2>();
                    default:
                        throw new Exception("Unknown type");
                }
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                JObject jo = new JObject();
                Type type = value.GetType();
                jo.Add("tT", JToken.FromObject(Enum.Parse(typeof(TypeTest), type.Name)));
                foreach (var prop in type.GetFields())
                {
                    jo.Add(prop.Name, JToken.FromObject(prop.GetValue(value)));
                }
                jo.WriteTo(writer);
            }
        }


        [Test]
        public void TestSerialize()
        {
            List<ITest> tests = new List<ITest>
            {
                new MyClass(1.0f, 2.0f),
                new My2("test")
            };

            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new TestConverter());
            settings.Formatting = Formatting.Indented; // Для удобочитаемости

            // Сериализация списка
            string json = JsonConvert.SerializeObject(tests, settings);
            Debug.Log(json);

            // Десериализация списка
            var deserializedTests = JsonConvert.DeserializeObject<List<ITest>>(json, settings);
            foreach (var test in deserializedTests)
            {
                // Обработка объектов
                Console.WriteLine(test.GetType().Name);
            }
        }

        public interface IProperties : IComponent
        {
            float GetValue(TypeProperty typeProperty);
            bool Has(TypeProperty typeProperty);

            void AddModifier(TypeProperty typeProperty, BaseProperty modifier);
            void RemoveModifier(TypeProperty typeProperty, BaseProperty modifier);
        }

        public  class Properties : IProperties
        {
            public ISite Site { get; set; }
            public event EventHandler Disposed;
            
            private readonly Dictionary<TypeProperty, ModifiedProperty> _properties = new Dictionary<TypeProperty, ModifiedProperty>();
            public void Dispose()
            {
                _properties.Clear();
            }

            public float GetValue(TypeProperty typeProperty)
            {
                return GetProperty(typeProperty).GetValue();
            }

            public bool Has(TypeProperty typeProperty)
            {
                return _properties.ContainsKey(typeProperty);
            }

            public void AddProperty(TypeProperty typeProperty, float baseValue)
            {
                if (_properties.ContainsKey(typeProperty))
                {
                    throw new ArgumentException($"Property {typeProperty} already exists");
                }

                _properties.Add(typeProperty, new ModifiedProperty(baseValue));
            }

            private ModifiedProperty GetProperty(TypeProperty typeProperty)
            {
                if (_properties.ContainsKey(typeProperty))
                {
                    return _properties[typeProperty];
                }

                throw new ArgumentException($"Can't find property {typeProperty}");
            }

            public void AddModifier(TypeProperty typeProperty, BaseProperty modifier)
            {
                GetProperty(typeProperty).AddModifier(modifier);
            }

            public void RemoveModifier(TypeProperty typeProperty, BaseProperty modifier)
            {
                GetProperty(typeProperty).RemoveModifier(modifier);
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