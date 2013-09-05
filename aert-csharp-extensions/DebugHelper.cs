using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Xml.Linq;

namespace aert_csharp_extensions
{

    /// <summary>
    /// Classe utilitaire permettant de faciliter l'affichage des informations de déboguage.
    /// Le format YAML est préféré pour la conversion object->string .
    /// </summary>
    public class DebugHelper
    {
        #region Affichage

        /// <summary>
        /// Affiche l'objet <c>data</c> au format YAML.
        /// </summary>
        public static void Print(string titre, IEnumerable<Dictionary<string, string>> data)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0} : ", titre);
            if (data == null)
            {
                sb.Append("(null)");
                Debug.WriteLine(sb);
                return;
            }

            sb.Append("\n");

            int row = 0;
            foreach (Dictionary<string, string> dic in data)
            {
                row++;
                sb.Append(" - ");

                int col = 0;
                foreach (KeyValuePair<string, string> pair in dic)
                {
                    col++;
                    if (col > 1)
                        sb.Append(", ");
                    sb.AppendFormat("{0}: \"{1}\"", pair.Key, pair.Value);
                }
                sb.Append("\n");
            }

            sb.AppendFormat("# Count {0}", row);
            Debug.WriteLine(sb);
        }

        /// <summary>
        /// Affiche l'objet <c>data</c> au format YAML.
        /// </summary>
        public static void Print(string titre, IEnumerable<XElement> data)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0} : ", titre);
            if (data == null)
            {
                sb.Append("(null)");
                Debug.WriteLine(sb);
                return;
            }

            sb.Append("\n");
            int row = 0;
            foreach (XElement elem in data)
            {
                row++;
                sb.Append(" - ");
                sb.Append(elem.ToString().Replace(Environment.NewLine, ""));
                sb.Append("\n");
            }

            sb.AppendFormat("# Count = {0}", row);
            Debug.WriteLine(sb);
        }


        /// <summary>
        /// Affiche l'objet <c>data</c> au format YAML.
        /// </summary>
        public static void Print(string titre, Dictionary<string, string> data)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0} : ", titre);
            if (data == null)
            {
                sb.Append("(null)");
                Debug.WriteLine(sb);
                return;
            }

            sb.Append("{");
            int col = 0;
            foreach (KeyValuePair<string, string> pair in data)
            {
                col++;
                if (col > 1) sb.Append(", ");
                sb.AppendFormat("{0}: \"{1}\"", pair.Key, pair.Value);
            }
            sb.Append("}\n");
            sb.AppendFormat("# Count = {0}", col);
            Debug.WriteLine(sb);
        }

        /// <summary>
        /// Affiche l'objet <c>data</c> au format YAML.
        /// </summary>
        public static void Print(string titre, IEnumerable<string> data)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0} : ", titre);
            if (data == null)
            {
                sb.Append("(null)");
                Debug.WriteLine(sb);
                return;
            }

            sb.Append("\n");
            int nb = 0;
            foreach (string val in data)
            {
                nb++;
                sb.AppendFormat(" - {0}\n", val);
            }
            sb.AppendFormat("# Count = {0}", nb);
            Debug.WriteLine(sb);
        }

        /// <summary>
        /// Affiche l'objet <c>data</c> au format YAML.
        /// </summary>
        public static void Print(string titre, string data)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0} : ", titre);
            if (data == null)
            {
                sb.Append("(null)");
                Debug.WriteLine(sb);
                return;
            }

            sb.AppendFormat("{0}", data);
            Debug.WriteLine(sb);
        }

        #endregion

        #region Utilitaires

        /// <summary>
        /// Affiche la progression si un écart significatif est détecté.
        /// </summary>
        public static bool PrintProgress(string title, int val, int max, int lastVal, int stepRatio)
        {
            StringBuilder sb = new StringBuilder();
            int percent;
            if (GetPercent(val, max, lastVal, stepRatio, out percent))
            {
                sb.AppendFormat(title, percent);
                Debug.WriteLine(sb);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Calcul le pourcentage effectué et indique s'il y'a lieu d'afficher la progression.
        /// </summary>
        public static bool GetPercent(int val, int max, int lastVal, int stepRatio, out int percent)
        {
            percent = val * 100 / max;
            int lastvalRatio = lastVal * 100 / max;

            return (val == lastVal) || (percent >= lastvalRatio + stepRatio);
        }

        #endregion
    }
}
