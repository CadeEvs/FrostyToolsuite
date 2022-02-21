namespace FrostySdk.Ebx
{
    public struct ResourceRef
    {
        public static ResourceRef Zero = new ResourceRef(0);
        private readonly ulong resourceId;

        public ResourceRef(ulong value)
        {
            resourceId = value;
        }

        public static implicit operator ulong(ResourceRef value) => value.resourceId;

        public static implicit operator ResourceRef(ulong value) => new ResourceRef(value);

        public override bool Equals(object obj)
        {
            if (obj is ResourceRef a)
            {
                return resourceId == a.resourceId;
            }
            
            if (obj is ulong b)
            {
                return resourceId == b;
            }

            return false;
        }

        public static bool operator ==(ResourceRef a, ResourceRef b) => a.Equals(b);

        public static bool operator !=(ResourceRef a, ResourceRef b) => !a.Equals(b);

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = (int)2166136261;
                hash = (hash * 16777619) ^ resourceId.GetHashCode();
                return hash;
            }
        }
        public override string ToString() => resourceId.ToString("X16");
    }
}
