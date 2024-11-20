namespace FileExchange.Client.UI.Services.DirectoryAccess;

public class DirectoryService(ILogger<DirectoryService> logger)
{
  public string CurrentDirectory { get; set; } = Directory.GetCurrentDirectory();
  public DirectoryResult EnumerateDirectories()
  {
    return new DirectoryResult
    {
      Directory = CurrentDirectory,
      Directories = Directory.EnumerateDirectories(CurrentDirectory).ToList()
    };
  }

  public void MoveUp()
  {
    var parentDirectory = Directory.GetParent(CurrentDirectory);
    if(parentDirectory is null) return;
    CurrentDirectory = parentDirectory.FullName;
  }
  
  public void MoveDown(string directoryName)
  {
    if (Directory.Exists(directoryName) && directoryName != CurrentDirectory)
    {
      CurrentDirectory = directoryName;
    }
  }
}

public class DirectoryResult
{
  public string Directory { get; set; }
  public List<string> Directories { get; set; }
}