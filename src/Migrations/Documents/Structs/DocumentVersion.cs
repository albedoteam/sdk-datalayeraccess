namespace AlbedoTeam.Sdk.DataLayerAccess.Migrations.Documents.Structs
{
    using System;
    using Exceptions;

    public struct DocumentVersion : IComparable<DocumentVersion>
    {
        private const char VersionSplitChar = '.';
        private const int MaxLength = 3;
        public readonly int Major;
        public readonly int Minor;
        public readonly int Revision;

        public DocumentVersion(string version)
        {
            var versionParts = version.Split(VersionSplitChar);

            if (versionParts.Length != MaxLength) throw new VersionStringToLongException(version);

            ParseVersionPart(versionParts[0], out Major);
            ParseVersionPart(versionParts[1], out Minor);
            ParseVersionPart(versionParts[2], out Revision);
        }

        private DocumentVersion(int major, int minor, int revision)
        {
            Major = major;
            Minor = minor;
            Revision = revision;
        }

        public static DocumentVersion Default()
        {
            return default;
        }

        public static DocumentVersion Empty()
        {
            return new DocumentVersion(-1, 0, 0);
        }

        public static implicit operator DocumentVersion(string version)
        {
            return new DocumentVersion(version);
        }

        public static implicit operator string(DocumentVersion documentVersion)
        {
            return documentVersion.ToString();
        }

        public override string ToString()
        {
            return $"{Major}.{Minor}.{Revision}";
        }

        #region compare

        public int CompareTo(DocumentVersion other)
        {
            if (Equals(other)) return 0;

            return this > other ? 1 : -1;
        }

        public static bool operator ==(DocumentVersion a, DocumentVersion b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(DocumentVersion a, DocumentVersion b)
        {
            return !(a == b);
        }

        public static bool operator >(DocumentVersion a, DocumentVersion b)
        {
            return a.Major > b.Major
                   || a.Major == b.Major && a.Minor > b.Minor
                   || a.Major == b.Major && a.Minor == b.Minor && a.Revision > b.Revision;
        }

        public static bool operator <(DocumentVersion a, DocumentVersion b)
        {
            return a != b && !(a > b);
        }

        public static bool operator <=(DocumentVersion a, DocumentVersion b)
        {
            return a == b || a < b;
        }

        public static bool operator >=(DocumentVersion a, DocumentVersion b)
        {
            return a == b || a > b;
        }

        private bool Equals(DocumentVersion other)
        {
            return other.Major == Major && other.Minor == Minor && other.Revision == Revision;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is DocumentVersion version && Equals(version);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var result = Major;
                result = (result * 397) ^ Minor;
                result = (result * 397) ^ Revision;
                return result;
            }
        }

        #endregion

        #region parse operations

        private static void ParseVersionPart(string value, out int target)
        {
            var revisionString = value;
            if (!int.TryParse(revisionString, out target))
                throw new InvalidVersionValueException(revisionString);
        }

        #endregion
    }
}