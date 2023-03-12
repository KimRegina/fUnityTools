using System.IO;

public static class FileUtils
{
    public static bool IsGitRepo(string folderPath)
    {
        DirectoryInfo info = new DirectoryInfo($"{folderPath}/.git");
        return info.Exists;
    }
}
