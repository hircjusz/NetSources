using System;
using System.Reflection;

namespace ExpressionLib.Common
{
    public static class ExpressionConstructor
    {

        public static Func<object[], object> BuildFactory(ConstructorInfo ctor)
        {
            var parameterInfos = ctor.GetParameters();
            var parameterExpressions = new System.Linq.Expressions.Expression[parameterInfos.Length];
            var argument = System.Linq.Expressions.Expression.Parameter(typeof (object[]), "parameters");
            for (var i = 0; i < parameterExpressions.Length; i++)
            {
                parameterExpressions[i] = System.Linq.Expressions.Expression.Convert(
                    System.Linq.Expressions.Expression.ArrayIndex(argument, System.Linq.Expressions.Expression.Constant(i, typeof (int))),
                    parameterInfos[i].ParameterType.IsByRef
                        ? parameterInfos[i].ParameterType.GetElementType()
                        : parameterInfos[i].ParameterType);
            }
            return System.Linq.Expressions.Expression.Lambda<Func<object[], object>>(
                System.Linq.Expressions.Expression.New(ctor, parameterExpressions),
                new[] {argument}).Compile();
        }

        public static TBase Instantiate<TBase>(Type subtypeofTBase, object[] ctorArgs)
        {
            ctorArgs = ctorArgs ?? new object[0];
            var types = Array.ConvertAll(ctorArgs, a => a?.GetType() ?? typeof (object));
            var constructor = subtypeofTBase.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, types,
                null);
            if (constructor != null)
            {
                return (TBase) Instantiate(constructor, ctorArgs);
            }
            try
            {
                return (TBase) Activator.CreateInstance(subtypeofTBase, ctorArgs);
            }
            catch (MissingMethodException ex)
            {
                throw;
            }
        }

        public static object Instantiate(this ConstructorInfo ctor, object[] ctorArgs)
        {
            Func<object[], object> factory;
            factory =BuildFactory(ctor);
            return factory.Invoke(ctorArgs);
        }

        public static TBase CreateInstance<TBase>(this Type subtypeofTBase, params object[] ctorArgs)
        {
            return Instantiate<TBase>(subtypeofTBase, ctorArgs ?? new object[0]);
        }


    }





}
