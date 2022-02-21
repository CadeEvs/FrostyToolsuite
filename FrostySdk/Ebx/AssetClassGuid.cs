using System;

namespace FrostySdk.Ebx
{
    public struct AssetClassGuid
    {
        public Guid ExportedGuid => exportedGuid;
        public int InternalId => internalId;
        public bool IsExported => isExported;

        private Guid exportedGuid;
        private readonly int internalId;
        private readonly bool isExported;

        public AssetClassGuid(Guid inGuid, int inId)
        {
            exportedGuid = inGuid;
            internalId = inId;
            isExported = (inGuid != Guid.Empty);
        }

        public AssetClassGuid(int inId)
        {
            exportedGuid = Guid.Empty;
            internalId = inId;
            isExported = false;
        }

        public static bool operator ==(AssetClassGuid A, object B) => A.Equals(B);

        public static bool operator !=(AssetClassGuid A, object B) => !A.Equals(B);

        public override bool Equals(object obj)
        {
            switch (obj)
            {
                case null:
                    return false;
                case AssetClassGuid reference:
                    return (isExported == reference.isExported && exportedGuid == reference.exportedGuid && internalId == reference.internalId);
                case Guid guid:
                    return (isExported && guid == exportedGuid);
                case int id:
                    return (internalId == id);
                default:
                    return false;
            }
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = (int)2166136261;
                hash = (hash * 16777619) ^ exportedGuid.GetHashCode();
                hash = (hash * 16777619) ^ internalId.GetHashCode();
                hash = (hash * 16777619) ^ isExported.GetHashCode();
                return hash;
            }
        }

        public override string ToString() => isExported ? exportedGuid.ToString() : "00000000-0000-0000-0000-" + internalId.ToString("x12");
    }
}
