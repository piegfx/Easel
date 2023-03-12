namespace Easel.Editor;

public class EaselProject
{
    public string Name;
    
    public string ProjectFileName;

    public static EaselProject Create(string path, string name)
    {
        string fullPath = Path.Combine(path, name);
        Directory.CreateDirectory(fullPath);
        
        Directory.CreateDirectory(Path.Combine(fullPath, "Content"));
    }
}