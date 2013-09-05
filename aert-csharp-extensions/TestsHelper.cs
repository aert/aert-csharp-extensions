using System.IO;
using System.Linq;
using System.Reflection;

namespace aert_csharp_extensions
{
    public static class TestsHelper
    {
        #region Path

        public static string Path(this string fileName)
        {
            string curDir = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return System.IO.Path.Combine(curDir, fileName);
        }

        /// <summary>
        /// Génére un chemin à partir d'une liste de chemin via <c>Path.Combine</c>.
        /// </summary>
        public static string PathCombine(this string[] paths)
        {
            return paths.Aggregate("", System.IO.Path.Combine);
        }

        #endregion

        #region Dir

        public static void CreateDir(string dirPath)
        {
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
        }

        /// <summary>
        /// Supprime le dossier et les sous-dossiers.
        /// </summary>
        public static void DeleteDir(string dirPath)
        {
            if (Directory.Exists(dirPath))
                Directory.Delete(dirPath, true);
        }

        /// <summary>
        /// Supprime la liste de dossiers et leurs sous-dossiers.
        /// </summary>
        public static void DeleteDirs(string[] dirs)
        {
            foreach (string d in dirs)
            {
                DeleteDir(d);
            }
        }

        public static void DeleteAssetsDirs()
        {
            string[] dirs = GetSubDirs(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "__assets_*");
            DeleteDirs(dirs);
        }

        public static bool YesDirExists(this string dirPath)
        {
            return Directory.Exists(dirPath);
        }

        public static string[] GetSubDirs(this string dirPath, string searchPattern)
        {
            string[] dirs = Directory.GetDirectories(dirPath, searchPattern);
            return dirs;
        }

        public static string GetFirstSubDirName(this string dirPath)
        {
            string[] dirs = Directory.GetDirectories(dirPath);
            return (dirs.Length > 0) ? System.IO.Path.GetFileName(dirs[0]) : "";
        }

        #endregion

        #region Files

        public static bool YesFiles(string dirPath, string searchPattern)
        {
            string[] fichiersDat = Directory.GetFiles(dirPath, searchPattern);
            return fichiersDat.Length > 0;
        }

        public static bool YesFilesRecursive(string dirPath, string searchPattern)
        {
            string[] fichiersDat = Directory.GetFiles(dirPath, searchPattern, SearchOption.AllDirectories);
            return fichiersDat.Length > 0;
        }

        public static bool YesFileExistsAndHasLines(string filePath, int minNumLines, out int numLines)
        {
            if (!File.Exists(filePath))
            {
                numLines = 0;
                return false;
            }
            numLines = File.ReadAllLines(filePath).Length;
            return numLines >= minNumLines;
        }

        public static bool YesFileExistsAndContains(string filePath, string search)
        {
            if (!File.Exists(filePath))
                return false;
            
            string content = File.ReadAllText(filePath);
            return content.Contains(search);
        }

        public static bool YesFileExistsAndHasSize(string filePath, int minSize, out long size)
        {
            if (!File.Exists(filePath))
            {
                size = 0;
                return false;
            }
            size = new FileInfo(filePath).Length;
            return size > minSize;
        }

        #endregion
    }
}
