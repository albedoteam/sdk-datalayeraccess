using System.Linq.Expressions;

namespace AlbedoTeam.Sdk.DataLayerAccess.Utils
{
    internal class ReplaceExpressionVisitor : ExpressionVisitor
    {
        private readonly Expression _newValue;
        private readonly Expression _oldValue;

        public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
        {
            _oldValue = oldValue;
            _newValue = newValue;
        }

        public override Expression Visit(Expression node)
        {
            return node == _oldValue ? _newValue : base.Visit(node);
        }
    }
}