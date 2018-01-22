namespace Swiss
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Linq.Expressions;

    /// <summary>
    /// See http://forums.asp.net/t/1321907.aspx
    /// 
    /// Usage: string pricePropertyName = ObjectUtils.GetMemberName&lt;IProduct&gt;(p =&gt; p.Price);
    /// 
    /// This work had no explicit license specified.
    /// </summary>
    public static class ObjectUtils
    {
        /// <summary>
        /// This does some magic, it returns the name of a property so it is no longer 
        /// a problematic string value problematic when refactoring.
        /// 
        /// Usage: string pricePropertyName = ObjectUtils.GetMemberName&lt;IProduct&gt;(p =&gt; p.Price);
        /// </summary>
        /// <typeparam name="T">The type to which the property belongs</typeparam>
        /// <param name="action">-</param>
        /// <returns>The property name</returns>
        public static string GetMemberName<T>(Expression<Func<T, object>> action)
        {
            var lambda = (LambdaExpression)action;

            if (lambda.Body is UnaryExpression)
            {
                var unary = (UnaryExpression)lambda.Body;
                var operand = unary.Operand;

                if (ExpressionType.MemberAccess == operand.NodeType)
                {
                    var memberExpr = (MemberExpression)operand;

                    return memberExpr.Member.Name;
                }
                else if (ExpressionType.Call == operand.NodeType)
                {
                    var methodExpr = (MethodCallExpression)operand;

                    return methodExpr.Method.Name;
                }
            }
            else
            {
                var memberExpr = (MemberExpression)lambda.Body;

                return memberExpr.Member.Name;
            }

            throw new InvalidOperationException();
        }
    }
}
