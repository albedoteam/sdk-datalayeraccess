namespace AlbedoTeam.Sdk.DataLayerAccess.Migrations.Documents.Serializers
{
    using MongoDB.Bson.Serialization;
    using MongoDB.Bson.Serialization.Serializers;
    using Structs;

    internal class DocumentVersionSerializer : SerializerBase<DocumentVersion>
    {
        public override void Serialize(
            BsonSerializationContext context,
            BsonSerializationArgs args,
            DocumentVersion value)
        {
            var versionString = $"{value.Major}.{value.Minor}.{value.Revision}";
            context.Writer.WriteString(versionString);
        }

        public override DocumentVersion Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var versionString = context.Reader.ReadString();
            return new DocumentVersion(versionString);
        }
    }
}