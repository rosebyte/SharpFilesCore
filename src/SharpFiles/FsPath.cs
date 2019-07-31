namespace RoseByte.SharpFiles.Core
{
    public abstract class FsPath
    {
        public string Path { get; }

        protected FsPath(string value) => Path = value.TrimEnd('/', '\\');

        private long? _size;
        public long Size => _size ?? (_size = GetSize()).Value;

        public abstract bool IsFile { get; }
        public abstract bool IsFolder { get; }
        public abstract FsFolder Parent { get; }
        public abstract bool Exists { get; }
        public abstract void Remove();
        protected abstract long GetSize();
        public void RefreshSize() => _size = null;

        public static implicit operator string(FsPath input) => input.Path;
        public override string ToString() => Path;
        public override int GetHashCode() => Path != null ? Path.GetHashCode() : 0;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
            {
                return false;
            }

            return (obj is string str && str == Path) || Equals(obj as FsPath);
        }

        public bool Equals(FsPath other) => !ReferenceEquals(other, null) && string.Equals(Path, other.Path);

        public static bool operator ==(FsPath left, FsPath right)
        {
            if (ReferenceEquals(left, null))
            {
                return ReferenceEquals(right, null);
            }

            return left.Equals(right);
        }

        public static bool operator !=(FsPath left, FsPath right) => !(left == right);
    }
}