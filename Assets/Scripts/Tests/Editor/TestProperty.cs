using Game.Properties;
using NUnit.Framework;

namespace TestProperty
{
    public class TestProperty
    {
        [Test]
        public void TestEmpty()
        {
            var test = new ModifiedProperty(1);
            Assert.AreEqual(1, test.GetValue());
        }

        [Test]
        public void TestAddConst()
        {
            var test = new ModifiedProperty(1);
            test.AddModifier(new BaseProperty(1, 0));
            Assert.AreEqual(2, test.GetValue());
        }

        [Test]
        public void TestMultiply()
        {
            var test = new ModifiedProperty(1);
            test.AddModifier(new BaseProperty(0, 0.1f));
            Assert.AreEqual(1.1f, test.GetValue());

        }
        
        [Test]
        public void TestMultiplyAndAdd()
        {
            var test = new ModifiedProperty(1);
            test.AddModifier(new BaseProperty(0, 0.1f));
            test.AddModifier(new BaseProperty(1, 0));
            Assert.AreEqual(2.2f, test.GetValue());
        }


    }
}