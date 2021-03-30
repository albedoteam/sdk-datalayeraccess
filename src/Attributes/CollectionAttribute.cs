namespace AlbedoTeam.Sdk.DataLayerAccess.Attributes
{
    using System;
    using Migrations.Documents.Structs;

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class CollectionAttribute : Attribute
    {
        /// <summary>
        ///     Defines the collection and the versioning
        /// </summary>
        /// <param name="name">The name of the collection</param>
        /// <param name="startupVersion">The main version of documents, to be migrated on app startup</param>
        /// <param name="runtimeVersion">The secondary version of documents, to be serialized at runtime</param>
        public CollectionAttribute(
            string name,
            string startupVersion = "0.0.0",
            string runtimeVersion = null)
        {
            Name = name;
            StartupVersion = startupVersion;

            RuntimeVersion = string.IsNullOrWhiteSpace(runtimeVersion)
                ? startupVersion
                : runtimeVersion;

            CollectionInformation = new CollectionLocationInformation(name);
        }

        public string Name { get; }
        public DocumentVersion StartupVersion { get; }
        public DocumentVersion RuntimeVersion { get; }
        public CollectionLocationInformation CollectionInformation { get; }
    }
}