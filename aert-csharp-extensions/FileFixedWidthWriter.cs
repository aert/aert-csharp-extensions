using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;

namespace aert_csharp_extensions
{
    /// <summary>
    /// Cette classe permet de gérer les opérations d'écriture vers un fichier texte à taille fixe.
    /// 
    /// <para>
    /// # Les entêtes de colonnes et leur taille sont d'abord spécifiés dans le dictionnaire <c>FormatFichier</c>
    /// # Le traitement s'appuie ensuite sur ces informations pour générer le fichier texte à partir du contenu du tableau fourni en entrée.
    /// </para>
    /// </summary>
    class FileFixedWidthWriter
    {
        #region Attributs

        public OrderedDictionary FormatFichier { get; private set; }
        public int TotalWitdh { get; private set; }

        #endregion

        #region Constructeurs

        /// <summary>
        /// Constructeur.
        /// </summary>
        public FileFixedWidthWriter(OrderedDictionary formatFichier)
        {
            if (formatFichier == null)
                throw new ArgumentNullException("formatFichier");

            FormatFichier = formatFichier;

            // Calcul de la largeur max du fichier à partir du format
            TotalWitdh = 0;
            foreach (int taille in FormatFichier.Values)
                TotalWitdh += taille;
        }

        #endregion

        #region Méthodes publiques

        /// <summary>
        /// Sauve le tableau vers un fichier texte de taille fixe.
        /// Si une valeur est supérieure à la taille de la cellule -> Exception.
        /// </summary>
        /// <exception cref="InvalidDataException"></exception>
        public void Write(TextWriter tw, string[][] data)
        {
            if (tw == null)
                throw new ArgumentNullException("tw");
            if (data == null)
                throw new ArgumentNullException("data");
            if (data.Length == 0)
                return;

            // vérification du nombre de colonnes du tableau

            if (data.Any(d => d.Length != FormatFichier.Count))
                throw new InvalidDataException("Le nombre de colonnes du tableau ne correspond pas au format spécifié.");

            // traitements

            foreach (string[] d in data)
                ProcessLine(tw, d);

        }

        #endregion

        #region Méthodes privées

        /// <summary>
        /// Insère une ligne du tableau vers le fichier de sortie
        /// </summary>
        /// <exception cref="InvalidDataException"></exception>
        private void ProcessLine(TextWriter tw, string[] data)
        {
            StringBuilder sb = new StringBuilder();
            object[] values = new object[FormatFichier.Values.Count];
            FormatFichier.Values.CopyTo(values, 0);

            for (int i = 0; i < FormatFichier.Count; i++)
            {
                string valeur = data[i] ?? "";
                int taille = (int)values[i];

                if (valeur.Length > taille)
                    throw new InvalidDataException("La taille de la donnée est supérieure à celle de la cellule. (tailleVal=" + valeur.Length + " - max=" + taille + " - val=" + valeur + ")");

                // conversion en taille fixe
                if (valeur.Length < taille)
                {
                    int n = taille - valeur.Length;
                    valeur += new string(' ', n);
                }

                sb.Append(valeur);
            }

            tw.WriteLine(sb);
        }

        #endregion
    }
}
