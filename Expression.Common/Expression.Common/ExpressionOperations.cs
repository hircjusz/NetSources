using System;
using System.Linq.Expressions;

namespace ExpressionLib.Common
{
    public class ExpressionOperations
    {

        public static Func<int, int> SquareExpression()
        {
            var xExpr = Expression.Parameter(typeof (int), "x");

            Expression<Func<int, int>> squarExpression = Expression.Lambda<Func<int, int>>(Expression.Multiply(xExpr, xExpr), xExpr);
            return squarExpression.Compile();
        }
    }
}
