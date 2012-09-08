using System;
using System.IO;

namespace Aert.Helpers
{
    /// <summary>
    /// Cette classe statique regroupe les méthodes d'extensions dédiées aux chaines de caractère.
    /// </summary>
    public static class StringHelper
    {
        /// <summary>
        /// Comparaison sans prise en compte de la casse et des espaces de fin.
        /// </summary>
        public static bool IsNotNullAndEqualsApprox(this string s, string other)
        {
            return s != null && s.ToLower().Trim().Equals(other.ToLower().Trim());
        }

        /// <summary>
        /// Indique si la chaine spécifiée est à <c>null</c> ou si elle contient uniquement des espaces.
        /// </summary>
        public static bool IsNullOrEmptyApprox(this string s)
        {
            return s == null || string.IsNullOrEmpty(s.Trim());
        }

        /// <summary>
        /// Indique si la chaine spécifiée est une chaîne numérique (sans espance, ni point, ni virgule)...
        /// </summary>
        public static bool IsDigitOnly(this string s)
        {
            if (IsNullOrEmptyApprox(s)) return false;

            for (int i = 0; i < s.Length; i++)
            {
                if (!char.IsDigit(s, i))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Nettoie le nom du fichier.
        /// </summary>
        public static string SanitizeFilename(this string filename, char replaceChar)
        {
            return Path.GetFileName(SanitizePath(@"c:\" + filename, '-'));
        }

        /// <summary>
        /// Ajoute un espace a chaque ligne de la chaine de caractères.
        /// </summary>
        public static string MyPadRight(this string s)
        {
            return MyPadRight(s, ' ', 4);
        }

        /// <summary>
        /// Ajoute un espace a chaque ligne de la chaine de caractères.
        /// </summary>
        public static string MyPadRight(this string s, char charEspace)
        {
            return MyPadRight(s, charEspace, 4);
        }

        /// <summary>
        /// Ajoute un espace a chaque ligne de la chaine de caractères.
        /// </summary>
        public static string MyPadRight(this string s, char charEspace, int nbEspaces)
        {
            if (s == null)
                return s;

            string filler = new string(charEspace, nbEspaces);
            string[] lignes = s.Split('\n');

            for (int i = 0; i < lignes.Length; i++)
                lignes[i] = filler + lignes[i];

            return string.Join(Environment.NewLine, lignes);
        }

        /// <summary>
        /// Converti la date au format TimeStamp pour batch.
        /// </summary>
        /// <returns>Format : SSAAMMJJHHmmss</returns>
        public static string ToStringTimeStamp(this DateTime date)
        {
            return string.Format("{0:yyyyMMddHHmmss}", date);
        }

        /// <summary>
        /// Converti l'entrée en une chaine de taille fixe
        /// <para>Si une exception n'est pas lancée en cas de taille supérieure, alors la chaine est concaténée</para>
        /// </summary>
        public static string ToFixedWidth(this string s, int width)
        {
            return ToFixedWidth(s, width, true);
        }

        /// <summary>
        /// Converti l'entrée en une chaine de taille fixe
        /// <para>Si une exception n'est pas lancée en cas de taille supérieure, alors la chaine est concaténée</para>
        /// </summary>
        public static string ToFixedWidth(this string s, int width, bool throwException)
        {
            if (s == null)
                throw new ArgumentNullException("s");

            s = s.TrimEnd();


            if (s.Length > width)
            {
                if (throwException)
                    throw new InvalidDataException(string.Format("La chaine spécifiée est supérieure à {0} caractères.", width));

                return s.Substring(0, width);
            }

            int n = width - s.Length;
            return s + new string(' ', n);
        }

        /// <summary>
        /// Nettoie le nom du fichier (seulement, pas le repertoire).
        /// </summary>
        private static string SanitizePath(this string path, char replaceChar)
        {
            int filenamePos = path.LastIndexOf(Path.DirectorySeparatorChar) + 1;
            var sb = new System.Text.StringBuilder();
            sb.Append(path.Substring(0, filenamePos));
            for (int i = filenamePos; i < path.Length; i++)
            {
                char filenameChar = path[i];
                foreach (char c in Path.GetInvalidFileNameChars())
                    if (filenameChar.Equals(c))
                    {
                        filenameChar = replaceChar;
                        break;
                    }

                sb.Append(filenameChar);
            }

            return sb.ToString();
        }
    }
}
