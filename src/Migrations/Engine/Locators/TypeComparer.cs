namespace AlbedoTeam.Sdk.DataLayerAccess.Migrations.Engine.Locators
{
    using System;
    using System.Collections.Generic;

    internal class TypeComparer : IEqualityComparer<Type>
    {
        public bool Equals(Type x, Type y)
        {
            return y is { } && x is { } && x.AssemblyQualifiedName == y.AssemblyQualifiedName;
        }

        public int GetHashCode(Type obj)
        {
            if (obj.AssemblyQualifiedName != null)
                return obj.AssemblyQualifiedName.GetHashCode();

            return -1;
        }
    }
}