using System;
using System.Linq.Expressions;
using System.Reflection;

namespace ExpressionLib.Common
{
    public static class ExpressionMethod
    {

        public static string ExtractMethodName<THandler>(Expression<Action<THandler>> methodHandler)
        {
            var expression = methodHandler.Body as MethodCallExpression;
            if (expression != null)
            {
                return expression.Method.Name;
            }
            throw new ArgumentException(
                "Couldn't extract method to handle the event from given expression. Expression should point to method that ought to handle subscribed event, something like: 's => s.HandleClick(null, null)'.");
        }

        private static TExpression EnsureIs<TExpression>(System.Linq.Expressions.Expression expression) where TExpression : System.Linq.Expressions.Expression
        {
            var casted = expression as TExpression;
            if (casted == null)
            {
                throw new Exception(
                    "Unexpected shape of expression. Expected direct call to method, something like 'x => x.Foo'");
            }

            return casted;
        }


        public  static string ObtainMethodName<TService>(this TService ctor,Expression<Func<TService, Action>> methodToUse)
        {
            var call = EnsureIs<UnaryExpression>(methodToUse.Body);
            var createDelegate = EnsureIs<MethodCallExpression>(call.Operand);
            var method = EnsureIs<ConstantExpression>(
                // Silverlight 5 and .NET 4.5 do this differently than older versions
#if SL5 || DOTNET45
				createDelegate.Object
#else
                createDelegate.Arguments[2]
#endif
                );

            return ((MethodInfo)method.Value).Name;
        }



    }
}
