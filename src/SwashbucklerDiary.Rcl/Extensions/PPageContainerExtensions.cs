using Masa.Blazor.Presets;
using System.Reflection;

namespace SwashbucklerDiary.Rcl.Extensions
{
    public static class PPageContainerExtensions
    {
        // 缓存反射结果以提高性能
        private static readonly Type _patternPathType;
        private static readonly ConstructorInfo _patternPathCtor;
        private static readonly MethodInfo _pageTabsOnTabsUpdatedMethod;

        static PPageContainerExtensions()
        {
            // 获取包含这些类型的程序集
            var assembly = typeof(PPageContainer).Assembly;

            // 获取PatternPath类型
            _patternPathType = assembly.GetType("Masa.Blazor.Presets.PatternPath")
                ?? throw new Exception("Not find Masa.Blazor.Presets.PatternPath");

            // 获取PatternPath构造函数
            _patternPathCtor = _patternPathType.GetConstructor([typeof(string), typeof(bool), typeof(bool)])
                ?? throw new Exception("Not find Masa.Blazor.Presets.PatternPath Constructor");

            // 获取PageTabsOnTabsUpdated方法
            _pageTabsOnTabsUpdatedMethod = typeof(PPageContainer).GetMethod(
                "PageTabsOnTabsUpdated",
                BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                [typeof(object), _patternPathType.MakeArrayType()],
                null)
                ?? throw new Exception("Not find PPageContainer.PageTabsOnTabsUpdated");
        }

        public static void UpdatePatternPaths(this PPageContainer pPageContainer, IEnumerable<string> paths)
        {
            // 创建PatternPath数组
            var patternPaths = paths?
                .Where(path => !string.IsNullOrWhiteSpace(path))
                .Select(path => _patternPathCtor.Invoke(new object[] { path, true, false }))
                .ToArray();

            // 创建正确类型的数组
            var typedArray = Array.CreateInstance(_patternPathType, patternPaths?.Length ?? 0);
            if (patternPaths != null && patternPaths.Length > 0)
            {
                Array.Copy(patternPaths, typedArray, patternPaths.Length);
            }

            // 调用PageTabsOnTabsUpdated方法
            _pageTabsOnTabsUpdatedMethod.Invoke(pPageContainer, new object[] { null, typedArray });
        }
    }
}
