using System.Collections.Specialized;
using System.IO;
using System.Text;

namespace aert_csharp_extensions
{
    /// <summary>
    /// Cette classe utilitaire permet de gérer les opérations de lecture/écriture vers des fichiers texte à taille fixe.
    /// </summary>
    public class FileFixedWidthHelper
    {
        /// <summary>
        /// Récupère le contenu du fichier texte de taille fixe vers un tableau de string.
        /// </summary>
        /// <param name="chemin">Chemin du fichier d'entrée</param>
        /// <param name="formatFichier">Format du fichier (nom et taille des colonnes) : <c>OrderedDictionary[string, int]</c></param>
        public static string[][] ParseFile(string chemin, OrderedDictionary formatFichier)
        {
            FileFixedWidthReader reader = new FileFixedWidthReader(formatFichier);

            string[][] result;

            using (FileStream fs = new FileStream(chemin, FileMode.Open))
            {
                using (StreamReader sr = new StreamReader(fs, Encoding.GetEncoding(1252)))
                    result = reader.Parse(sr);
            }

            return result;
        }

        /// <summary>
        /// Sauve le tableau vers un fichier texte de taille fixe.
        /// Si une valeur est supérieure à la taille de la cellule -> Exception.
        /// </summary>
        /// <param name="chemin">Chemin du fichier de sortie</param>
        /// <param name="formatFichier">Format du fichier (nom et taille des colonnes) : <c>OrderedDictionary[string, int]</c></param>
        /// <param name="data">Tableau à sauver</param>
        public static void WriteFile(string chemin, OrderedDictionary formatFichier, string[][] data)
        {
            FileFixedWidthWriter writer = new FileFixedWidthWriter(formatFichier);

            using (FileStream fs = new FileStream(chemin, FileMode.Create))
            {
                using (StreamWriter sw = new StreamWriter(fs, Encoding.GetEncoding(1252)))
                    writer.Write(sw, data);
            }
        }

        /// <summary>
        /// Sauve le tableau vers un fichier texte de taille fixe.
        /// Si une valeur est supérieure à la taille de la cellule -> Exception.
        /// </summary>
        public static void WriteStream(TextWriter tw, OrderedDictionary formatFichier, string[][] data)
        {
            FileFixedWidthWriter writer = new FileFixedWidthWriter(formatFichier);
            writer.Write(tw, data);
        }

        /// <summary>
        /// Sauve le tableau vers un fichier texte de taille fixe.
        /// Si une valeur est supérieure à la taille de la cellule -> Exception.
        /// </summary>
        public static string ToFixedString(OrderedDictionary formatFichier, string[][] data)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);

            WriteStream(sw, formatFichier, data);

            return sb.ToString();
        }
    }
}
