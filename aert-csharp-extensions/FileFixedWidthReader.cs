using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;

namespace aert_csharp_extensions
{
    /// <summary>
    /// Cette classe permet de gérer les opérations les opérations de lecture à partir d'un fichier texte à taille fixe.
    /// 
    /// <para>
    /// # Les entêtes de colonnes et leur taille sont d'abord spécifiés dans le dictionnaire <c>FormatFichier</c>
    /// # Le traitement s'appuie ensuite sur ces informations pour générer en sortie le tableau associé au contenu du fichier.
    /// </para>
    /// </summary>
    class FileFixedWidthReader
    {
        #region Attributs

        public OrderedDictionary FormatFichier { get; private set; }
        public int TotalWitdh { get; private set; }

        // privés
        private int _LineNum;

        #endregion

        #region Constructeurs

        /// <summary>
        /// Constructeur.
        /// </summary>
        public FileFixedWidthReader(OrderedDictionary formatFichier)
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
        /// Récupère le contenu du fichier texte fixe vers un tableau de string.
        /// </summary>
        /// <exception cref="InvalidDataException"></exception>
        public string[][] Parse(TextReader tr)
        {
            if (tr == null)
                throw new ArgumentNullException("tr");

            // parsing
            _LineNum = 0;

            List<string[]> result = new List<string[]>();
            string[] r;

            while ((r = ProcessLine(tr)) != null)
                result.Add(r);

            return result.ToArray();
        }

        #endregion

        #region Méthodes privées

        /// <summary>
        /// Lit la ligne suivante du reader et renvoie les valeures parsées.
        /// </summary>
        /// <returns>NULL en fin de fichier</returns>
        /// <exception cref="InvalidDataException"></exception>
        private string[] ProcessLine(TextReader tr)
        {
            string line = tr.ReadLine();

            if (line == null)
                return null;

            _LineNum++;

            // On vérifie que la ligne est au bon format.
            if (line.Length != TotalWitdh)
                throw new InvalidDataException(string.Format(
                    "Ligne {0} : la taille du fichier ({1}) ne correspond pas celle du format géré ({2})."
                    , _LineNum, line.Length, TotalWitdh));

            // parsing
            string[] result = new string[FormatFichier.Count];

            int numCol = 0;
            int lastPosition = 0;
            foreach (int tailleCol in FormatFichier.Values)
            {
                numCol++;
                result[numCol - 1] = line.Substring(lastPosition, tailleCol).Trim();
                lastPosition += tailleCol;
            }

            return result;
        }

        #endregion
    }
}
