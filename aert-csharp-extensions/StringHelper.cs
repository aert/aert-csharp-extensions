using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace aert_csharp_extensions
{
    /// <summary>
    ///     Cette classe statique regroupe les méthodes d'extensions dédiées aux chaines de caractère.
    /// </summary>
    public static class StringHelper
    {
        private static long _lastTimeStamp = DateTime.UtcNow.Ticks;

        /// <summary>
        ///     Applique string.Format().
        /// </summary>
        public static string HelpFormat(this string format, params object[] data)
        {
            if (format == null)
                return null;

            return string.Format(format, data);
        }

        #region Null & Empty

        /// <summary>
        ///     Comparaison sans prise en compte de la casse et des espaces de fin.
        /// </summary>
        public static bool HelpIsNotNullAndEqualsApprox(this string s, string other)
        {
            return s != null && s.ToLower().Trim().Equals(other.ToLower().Trim());
        }

        /// <summary>
        ///     Indique si la chaine spécifiée est à <c>null</c> ou si elle contient uniquement des espaces.
        /// </summary>
        public static bool HelpIsNullOrEmptyApprox(this string s)
        {
            return s == null || string.IsNullOrEmpty(s.Trim());
        }

        /// <summary>
        ///     Trim even if null.
        /// </summary>
        public static string HelpTrimOrEmpty(this string s)
        {
            return s == null ? string.Empty : s.Trim();
        }

        /// <summary>
        ///     Indique si les 2 chaines sont identiques, sans prise en compte des espaces de début et de fin.
        ///     <para>
        ///         Si une des 2 chaine est nulle, retourne false
        ///     </para>
        /// </summary>
        public static bool HelpEqualsTrimmed(this string sOne, string sTwo)
        {
            if (sOne == null || sTwo == null)
                return false;
            return sOne.Trim().Equals(sTwo.Trim());
        }

        #endregion

        #region Pad

        /// <summary>
        ///     Ajoute un espace a chaque ligne de la chaine de caractères.
        /// </summary>
        public static string HelpMyPadRight(this string s)
        {
            return HelpMyPadRight(s, ' ', 4);
        }

        /// <summary>
        ///     Ajoute un espace a chaque ligne de la chaine de caractères.
        /// </summary>
        public static string HelpMyPadRight(this string s, char charEspace)
        {
            return HelpMyPadRight(s, charEspace, 4);
        }

        /// <summary>
        ///     Ajoute un espace a chaque ligne de la chaine de caractères.
        /// </summary>
        public static string HelpMyPadRight(this string s, char charEspace, int nbEspaces)
        {
            if (s == null)
                return null;

            string filler = new string(charEspace, nbEspaces);
            string[] lignes = s.Split('\n');

            for (int i = 0; i < lignes.Length; i++)
                lignes[i] = filler + lignes[i];

            return string.Join(Environment.NewLine, lignes);
        }

        #endregion

        #region DateTime

        /// <summary>
        ///     Converti la date au format SSAAMMJJ.
        /// </summary>
        /// <returns>Format : SSAAMMJJ</returns>
        public static string HelpToStringDate8(this DateTime date)
        {
            return string.Format("{0:yyyyMMdd}", date);
        }

        /// <summary>
        ///     Converti la date au format HHmmss.
        /// </summary>
        /// <returns>Format : HHmmss</returns>
        public static string HelpToStringTime6(this DateTime date)
        {
            return string.Format("{0:HHmmss}", date);
        }

        /// <summary>
        ///     Converti la date au format TimeStamp pour batch.
        /// </summary>
        /// <returns>Format : SSAAMMJJHHmmss</returns>
        public static string HelpToStringTimeStamp(this DateTime date)
        {
            return string.Format("{0:yyyyMMddHHmmss}", date);
        }

        /// <summary>
        ///     Génère un TimeStamp compatible PRIAM
        /// </summary>
        /// <returns></returns>
        public static string HelpGenerateTimeStamp26()
        {
            long orig, newval;
            do
            {
                orig = _lastTimeStamp;
                long now = DateTime.UtcNow.Ticks;
                newval = Math.Max(now, orig + 1);
            } while (Interlocked.CompareExchange(ref _lastTimeStamp, newval, orig) != orig);

            DateTime dt = new DateTime(newval, DateTimeKind.Utc);
            dt = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, dt.Millisecond, DateTimeKind.Utc);
            string first18 = dt.ToString("yyyyMMddHHmmssffff");
            string remainder = (newval - dt.Ticks).ToString(CultureInfo.InvariantCulture).HelpToFixedWidth(8, true);
            return first18 + remainder;
        }


        /// <summary>
        ///     Retourne une valeur date au format YYYYMMDD.
        ///     <para>
        ///         Seuls les 10 premiers car. sont pris en compte.
        ///     </para>
        ///     <para>
        ///         Prend en charge les formats suivants (separateurs="-/ ._"):
        ///         <list type="bullet">
        ///             <item>YYYYMMDD</item>
        ///             <item>YYYY_MM_DD</item>
        ///             <item>DD_MM_YYYY</item>
        ///         </list>
        ///     </para>
        /// </summary>
        /// <returns><c>null</c> on error</returns>
        public static DateTime? HelpConvertToDate_French(this string value)
        {
            if (value.HelpIsNullOrEmptyApprox())
                return null;

            const string seps = "-/ ._";

            // cleaning
            value = value.Replace(" ", "");
            if (value.Length > 10)
                value = value.Substring(0, 10);

            // validation
            if (!value.HelpIsDigitOrSeparator(seps))
                return null;
            if (value.Length < 10 && value.Length != 8)
                return null;
            if (value.Length == 8 && !value.HelpIsDigitOnly())
                return null;

            ////////////////
            // conversion
            int year, month, day;

            if (value.Length == 10)
            {
                string syear = value.Substring(0, 4);
                string smonth = value.Substring(5, 2);
                string sday = value.Substring(8, 2);

                // format UTC : YYYY_MM_DD
                if (syear.HelpIsDigitOnly() && smonth.HelpIsDigitOnly() && sday.HelpIsDigitOnly())
                {
                    if (char.IsDigit(value[4]) || char.IsDigit(value[7]))
                        return null;
                    year = int.Parse(syear);
                    month = int.Parse(smonth);
                    day = int.Parse(sday);
                    return new DateTime(year, month, day);
                }

                // format FR : DD_MM_YYYY
                sday = value.Substring(0, 2);
                smonth = value.Substring(3, 2);
                syear = value.Substring(6, 4);

                if (syear.HelpIsDigitOnly() && smonth.HelpIsDigitOnly() && sday.HelpIsDigitOnly())
                {
                    if (char.IsDigit(value[2]) || char.IsDigit(value[5]))
                        return null;
                    year = int.Parse(syear);
                    month = int.Parse(smonth);
                    day = int.Parse(sday);
                    return new DateTime(year, month, day);
                }
                return null;
            }

            //value.Length == 8
            year = int.Parse(value.Substring(0, 4));
            month = int.Parse(value.Substring(4, 2));
            day = int.Parse(value.Substring(6, 2));
            return new DateTime(year, month, day);
        }

        #endregion

        #region Tests AlphaNum

        /// <summary>
        ///     Indique si la chaine spécifiée uniquement composée de chiffres.
        /// </summary>
        public static bool HelpIsDigitOnly(this string s)
        {
            if (s.HelpIsNullOrEmptyApprox())
                return false;

            return s.ToArray().All(Char.IsDigit);
        }


        /// <summary>
        ///     Indique si la chaine spécifiée uniquement composée de chiffres ou du séparateur indiqué.
        /// </summary>
        public static bool HelpIsDigitOrSeparator(this string s, string separators)
        {
            if (s.HelpIsNullOrEmptyApprox())
                return false;
            if (separators.HelpIsNullOrEmptyApprox())
                return s.HelpIsDigitOnly();

            char[] seps = separators.ToCharArray();

            return s.ToArray().All(x => Char.IsDigit(x) || seps.Contains(x));
        }

        #endregion

        #region Fixed

        /// <summary>
        ///     Converti l'entrée en une chaine de taille fixe
        ///     <para>Si une exception n'est pas lancée en cas de taille supérieure, alors la chaine est concaténée</para>
        /// </summary>
        public static string HelpToFixedWidth(this string s, int width)
        {
            return HelpToFixedWidth(s, width, true);
        }

        /// <summary>
        ///     Converti l'entrée en une chaine de taille fixe
        ///     <para>Si une exception n'est pas lancée en cas de taille supérieure, alors la chaine est concaténée</para>
        /// </summary>
        public static string HelpToFixedWidth(this string s, int width, bool throwException)
        {
            if (s == null)
                throw new ArgumentNullException("s");

            s = s.TrimEnd();


            if (s.Length > width)
            {
                if (throwException)
                    throw new InvalidDataException(string.Format(
                        "La chaine spécifiée est supérieure à {0} caractères.", width));

                return s.Substring(0, width);
            }

            int n = width - s.Length;
            return s + new string(' ', n);
        }

        #endregion

        #region Path

        /// <summary>
        ///     Nettoie le nom du fichier.
        /// </summary>
        public static string HelpSanitizeFilename(this string filename, char replaceChar)
        {
            return Path.GetFileName(HelpSanitizePath(@"c:\" + filename, '-'));
        }

        /// <summary>
        ///     Nettoie le nom du fichier (seulement, pas le repertoire).
        /// </summary>
        private static string HelpSanitizePath(this string path, char replaceChar)
        {
            int filenamePos = path.LastIndexOf(Path.DirectorySeparatorChar) + 1;
            StringBuilder sb = new StringBuilder();
            sb.Append(path.Substring(0, filenamePos));
            for (int i = filenamePos; i < path.Length; i++)
            {
                char filenameChar = path[i];
                if (Path.GetInvalidFileNameChars().Any(c => filenameChar.Equals(c)))
                {
                    filenameChar = replaceChar;
                }

                sb.Append(filenameChar);
            }

            return sb.ToString();
        }

        #endregion

        #region Decimal

        public static decimal? HelpTryParseDecimal(this string str)
        {
            if (str.HelpIsNullOrEmptyApprox())
                return null;

            // on enleve les car. non ascii, nécessaire pour EPPLUS
            str = Regex.Replace(str, @"[^\u0000-\u007F]", string.Empty).Replace(",", ".").Replace(" ", string.Empty);

            decimal result;
            if (!decimal.TryParse(str, NumberStyles.Currency, CultureInfo.InvariantCulture, out result))
                return null;
            return result;
        }

        #endregion

        #region Cobol

        /// <summary>
        ///     Converti l'entrée en une chaine de taille fixe au format 0001
        /// </summary>
        public static string HelpToCobolLong(this string str, object width)
        {
            return HelpToCobolLong(str, (int) width);
        }

        /// <summary>
        /// Retourne la valeur sans les espaces insécables Excel.
        /// </summary>
        public static string HelpCleanExcelUnicode(this string str)
        {
            if (str.HelpIsNullOrEmptyApprox())
                return string.Empty;

            return Regex.Replace(str.Trim(), @"[^\u0000-\u007F]", " ");
        }

        /// <summary>
        ///     Convertit l'entrée en une chaine de taille fixe au format 0001
        /// </summary>
        public static string HelpToCobolLong(this string str, int width)
        {
            if (str == null)
                throw new ArgumentNullException("str");

            // on enleve les car. non ascii, nécessaire pour EPPLUS
            str = Regex.Replace(str, @"[^\u0000-\u007F]", string.Empty).Replace(",", ".").Replace(" ", string.Empty);

            if (str.HelpIsNullOrEmptyApprox())
                return string.Empty;

            string valeur = long.Parse(str).ToString(CultureInfo.InvariantCulture);

            if (valeur.Length > width)
                throw new InvalidDataException(
                    "La taille de la donnée est supérieure à celle de la cellule. (tailleVal=" + valeur.Length +
                    " - max=" + width + " - val=" + valeur + ")");

            // conversion en taille fixe
            if (valeur.Length < width)
            {
                int n = width - valeur.Length;
                valeur = new string('0', n) + valeur;
            }

            return valeur;
        }

        #endregion

        #region Stream

        /// <summary>
        /// Retourne un objet Stream à partir de la chaine spécifiée.
        /// </summary>
        public static Stream HelpToStream(this string str)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(str);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        #endregion
    }
}