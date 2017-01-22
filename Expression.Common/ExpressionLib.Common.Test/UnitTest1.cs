using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressionLib.Common.Test
{

    public class FancyObject<TSource, TResult>
    {
        private readonly TSource value;
        private readonly Expression<Func<TSource, TResult>> projection;

        public FancyObject(TSource value,
                           Expression<Func<TSource, TResult>> projection)
        {
            this.value = value;
            this.projection = projection;
        }
    }

    internal interface ICustomerInfo
    {
        List<string> GetCustomerList(int externalID);
    }

    class DummyClass : ICustomerInfo
    {
        public DummyClass()
        {
        }
        public DummyClass(string name)
        {
            this.Name = name;
        }

        public string Name { get; set; }

        public List<string> GetCustomerList(int externalID)
        {
            if (externalID > 0) return
                new List<string>() { "A", "B", "C" };
            return null;

        }
    }

    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void InstiateConstructor1()
        {
            var dummyObj = new DummyClass()
            {
                Name = "Darek"
            };

            var ctor = typeof(DummyClass).GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, new Type[] { }, null);
            var buildFactory = ExpressionConstructor.BuildFactory(ctor);
            var obj = buildFactory.Invoke(new object[] { });

        }

        [TestMethod]
        public void InstiateConstructor2()
        {
            var dummyObj = new DummyClass()
            {
                Name = "Darek"
            };

            var ctor = typeof(DummyClass).GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, new[] { typeof(string) }, null);
            var buildFactory = ExpressionConstructor.BuildFactory(ctor);
            var obj = buildFactory.Invoke(new object[] { "bbbb" }) as DummyClass;
            Assert.AreEqual(obj.Name, "bbbb");

        }

        [TestMethod]
        public void InstiateConstructor3()
        {
            var obj = typeof(DummyClass).CreateInstance<DummyClass>("bbbb");
            Assert.AreEqual(obj.Name, "bbbb");

        }

        [TestMethod]
        public void PropertySpecifierTest()
        {
            var dummyObj = new DummyClass()
            {
                Name = "Darek"
            };

            var propertySpecifier = dummyObj.GetPropertyInfo(t => t.Name);
        }

        [TestMethod]
        public void PropertySpecifierTest2()
        {
            var dummyObj = new DummyClass()
            {
                Name = "Darek"
            };

            var propertySpecifier = dummyObj.GetMemberName(t => t.Name);
        }

        [TestMethod]
        public void SquareExpression()
        {
            var f = ExpressionOperations.SquareExpression();
            var wynik = f(5);
        }

        [TestMethod]
        public void ObtainMethodTest1()
        {
            var dummyObj = new DummyClass()
            {
                Name = "Darek"
            };

            // var foo = new FancyObject<DummyClass, IList<string>>(dummyObj, c => c.GetCustomerList());

            //var x= foo.

        }

        [TestMethod]
        public void ExpressionParameter1()
        {
            var dummyObj = new DummyClass()
            {
                Name = "Darek"
            };

            var method = typeof(DummyClass).GetMethod("GetCustomerList", new[] { typeof(int) });
            var valueId = Expression.Parameter(typeof(int));

            var instance = Expression.Constant(dummyObj);

            var call = Expression.Call(instance, method, valueId);
            var func = Expression.Lambda(call, Expression.Parameter(typeof(DummyClass))).Compile();
            var result = func.DynamicInvoke(5);

            //Expression<Func<ICustomerInfo,IList<string>>> dane=t=>t.GetCustomerList(0);

            //Expression.Invoke(dane,Expression.Parameter(typeof(int)))
        }
    }
}
