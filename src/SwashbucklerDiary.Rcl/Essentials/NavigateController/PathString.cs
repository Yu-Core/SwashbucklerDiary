namespace SwashbucklerDiary.Rcl.Essentials
{
    public class PathString : IEquatable<PathString?>
    {
        private readonly string _originalPath;
        private readonly string _normalizedPath;

        public PathString(string? path)
        {
            if (path is null)
                throw new ArgumentNullException(nameof(path));

            _originalPath = path;
            _normalizedPath = NormalizePath(path);
        }

        public string Original => _originalPath;
        public string Normalized => _normalizedPath;

        private static string NormalizePath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return path;

            // 确保路径以/开头，统一为小写，并去除结尾的/
            return "/" + path.Trim('/').ToLowerInvariant();
        }

        public bool Equals(PathString? other)
        {
            if (other is null) return false;
            return _normalizedPath == other._normalizedPath;
        }

        public override bool Equals(object? obj) => Equals(obj as PathString);
        public override int GetHashCode() => _normalizedPath.GetHashCode();
        public override string ToString() => _originalPath;

        public static implicit operator PathString?(string? path) =>
            path is null ? null : new PathString(path);

        public static implicit operator string?(PathString? pathString) =>
            pathString?._originalPath;

        public static bool operator ==(PathString? left, PathString? right)
        {
            if (ReferenceEquals(left, right)) return true;
            if (left is null || right is null) return false;
            return left.Equals(right);
        }

        public static bool operator !=(PathString? left, PathString? right) => !(left == right);
    }
}
