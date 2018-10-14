using System.IO;

namespace docker_netgen.Utils
{
    public class FileUtils
    {
        public string GetClosestMatchingFileInDirectory(string directoryPath, string target)
        {
            var currentBest = "";
            var files = Directory.GetFiles(directoryPath);
            foreach (var fileName in files)
            {
                if (fileName.Contains(target) && fileName.Length > currentBest.Length)
                    currentBest = fileName;
            }

            return currentBest;
        }
    }
}