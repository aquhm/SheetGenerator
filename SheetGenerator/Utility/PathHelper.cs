namespace SheetGenerator.Util;

public static class PathHelper
{
    private static bool _developmentMode;
    public static readonly string ProjectRootPath;

    static PathHelper()
    {
        ProjectRootPath = GetProjectRoot();
    }

    public static void Initialize(bool developmentMode)
    {
        _developmentMode = developmentMode;
    }

    public static string GetProjectRoot()
    {
        if (_developmentMode)
        {
            var currentDir = new DirectoryInfo(Directory.GetCurrentDirectory());
            while (currentDir != null && !File.Exists(Path.Combine(currentDir.FullName, "SheetGenerator.csproj")))
            {
                currentDir = currentDir.Parent;
            }

            if (currentDir != null)
            {
                return currentDir.FullName;
            }
        }

        return Directory.GetCurrentDirectory();
    }
}
