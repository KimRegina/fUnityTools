using System.IO;

public static class FileUtils
{
    /// <summary>
    /// 是否为git仓库
    /// </summary>
    /// <param name="folderPath">文件夹路径</param>
    /// <returns></returns>
    public static bool IsGitRepo(string folderPath)
    {
        DirectoryInfo info = new DirectoryInfo($"{folderPath}/.git");
        return info.Exists;
    }
}