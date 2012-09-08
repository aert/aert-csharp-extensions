using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Aert.Helpers
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
    }
}
