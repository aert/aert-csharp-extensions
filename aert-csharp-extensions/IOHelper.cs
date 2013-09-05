using System;
using System.IO;

namespace aert_csharp_extensions
{
    /// <summary>
    /// Cette classe regroupe l'ensemble des fonctions utilitaires dédiées à la manipulation des chemins de fichiers.
    /// </summary>
    public static class IOHelper
    {
        /// <summary>
        /// Récupère tous les chemins existants vers les fichiers ayant pour nom <c>searchPattern</c>.
        /// </summary>
        /// <param name="basePath">Path du répertoire de recherche</param>
        /// <param name="searchPattern">Nom du fichier, sans son chemin complet</param>
        /// <returns></returns>
        public static string[] GetFilePaths(string basePath, string searchPattern)
        {
            string[] files = Directory.GetFiles(basePath, searchPattern, SearchOption.AllDirectories);
            return files;
        }

        /// <summary> 
        /// Déplace tous les fichiers respectant <c>searchPattern</c> vers <c>destPath</c>.
        /// </summary>
        /// <param name="basePath">Path du répertoire de recherche</param>
        /// <param name="searchPattern">Nom du fichier, sans son chemin complet</param>
        /// <param name="destPath">Dossier de destination</param>
        /// <returns>Le nouveau chemin des fichiers déplacés</returns>
        public static string[] MoveFiles(string basePath, string searchPattern, string destPath)
        {
            string[] files = Directory.GetFiles(basePath, searchPattern, SearchOption.TopDirectoryOnly);

            for (int i = 0; i < files.Length; i++)
            {
                string f = files[i];
                string newFile = Path.Combine(destPath, Path.GetFileName(f));

                File.Move(f, newFile);
                files[i] = newFile;
            }

            return files;
        }

        /// <summary>
        /// Supprimer récursivement le dossier spécifié, s'il existe
        /// </summary>
        /// <returns><c>true</c> si le dossier a été supprimé, <c>false</c> s'il n'existait pas</returns>
        public static bool DeleteDirIfExists(string dirPath)
        {
            if (Directory.Exists(dirPath))
            {
                //Console.WriteLine("DELETED: " + dirPath);
                Directory.Delete(dirPath, true);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Supprimer récursivement le dossier spécifié, s'il existe.
        /// <para>Aucune exception n'est levée au cas la suppression a échouée</para>
        /// </summary>
        /// <returns><c>true</c> si le dossier a été supprimé, <c>false</c> s'il n'existait pas</returns>
        public static bool DeleteDirIfExistsWithoutException(string dirPath)
        {
            try
            {
                return DeleteDirIfExists(dirPath);
            }
            catch (Exception)
            {
                return false;
            }
        }


        /// <summary>
        /// Supprime le fichier spécifié s'il existe
        /// </summary>
        /// <returns><c>true</c> si le fichier a été supprimé, <c>false</c> s'il n'existait pas</returns>
        public static bool DeleteFileIfExists(string filePath)
        {
            if (File.Exists(filePath))
            {
                //Console.WriteLine("DELETED: " + filePath);
                File.Delete(filePath);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Supprime le fichier spécifié s'il existe.
        /// <para>Aucune exception n'est levée au cas la suppression a échouée</para>
        /// </summary>
        /// <returns><c>true</c> si le fichier a été supprimé, <c>false</c> s'il n'existait pas</returns>
        public static bool DeleteFileIfExistsWithoutException(string filePath)
        {
            try
            {
                return DeleteFileIfExists(filePath);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Crée le dossier s'il n'existe pas
        /// </summary>
        /// <param name="dirPath"></param>
        /// <returns><c>true</c> si le dossier a été créé, <c>false</c> s'il existait déjà</returns>
        public static bool CreateDirectoryIfNotExists(string dirPath)
        {
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Copie le dossier source vers la destination.
        /// </summary>
        public static void CopyDirectory(string src, string dst)
        {
            if (dst[dst.Length - 1] != Path.DirectorySeparatorChar)
                dst += Path.DirectorySeparatorChar;
            if (!Directory.Exists(dst)) Directory.CreateDirectory(dst);

            string[] files = Directory.GetFileSystemEntries(src);

            foreach (string element in files)
            {
                // Sub directories
                if (Directory.Exists(element))
                    CopyDirectory(element, dst + Path.GetFileName(element));
                // Files in directory
                else
                    File.Copy(element, dst + Path.GetFileName(element), true);
            }
        }
    }
}
