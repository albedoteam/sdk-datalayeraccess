namespace AlbedoTeam.Sdk.DataLayerAccess.Utils
{
    using System;
    using System.Linq.Expressions;

    internal static class ExpressionExtensions
    {
        public static Expression<Func<TDocument, bool>> AndAlso<TDocument>(
            this Expression<Func<TDocument, bool>> mainExpression,
            Expression<Func<TDocument, bool>> additionalExpression)
        {
            var parameter = Expression.Parameter(typeof(TDocument));

            var leftVisitor = new ReplaceExpressionVisitor(mainExpression.Parameters[0], parameter);
            var left = leftVisitor.Visit(mainExpression.Body);

            var rightVisitor = new ReplaceExpressionVisitor(additionalExpression.Parameters[0], parameter);
            var right = rightVisitor.Visit(additionalExpression.Body);

            var filter = Expression.Lambda<Func<TDocument, bool>>(
                Expression.AndAlso(left, right), parameter);

            return filter;
        }
    }
}