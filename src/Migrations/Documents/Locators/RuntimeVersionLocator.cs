namespace AlbedoTeam.Sdk.DataLayerAccess.Migrations.Documents.Locators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Attributes;
    using Structs;

    internal class RuntimeVersionLocator : AbstractLocator<DocumentVersion, Type>, IRuntimeVersionLocator
    {
        public override DocumentVersion? GetLocateOrNull(Type identifier)
        {
            if (!LocatesDictionary.ContainsKey(identifier))
                return null;

            LocatesDictionary.TryGetValue(identifier, out var value);
            return value;
        }

        public override void Locate()
        {
            var types =
                from a in AppDomain.CurrentDomain.GetAssemblies()
                from t in a.GetTypes()
                let attributes = t.GetCustomAttributes(typeof(CollectionAttribute), true)
                where attributes != null && attributes.Length > 0
                select new { Type = t, Attributes = attributes.Cast<CollectionAttribute>() };

            var versions = new Dictionary<Type, DocumentVersion>();

            foreach (var type in types)
            {
                var version = type.Attributes.First().RuntimeVersion;
                versions.Add(type.Type, version);
            }

            LocatesDictionary = versions;
        }
    }
}