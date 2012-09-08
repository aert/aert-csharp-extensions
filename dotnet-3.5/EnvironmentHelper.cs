using System;

namespace Aert.Helpers
{
    /// <summary>
    /// Permet de récupérer les paramètres de l'environnement d'exécution.
    /// Ex. : Site d'exécution, Utilisateur Anonyme du site, etc...
    /// </summary>
    public class EnvironmentHelper
    {
        /// <summary>
        /// Récupère le site d'exécution courant à partir de la variable d'environnement %PFTLSITE%
        /// </summary>
        /// <returns>
        /// Si : HI KI GI UI SI EI QI, alors env de prod
        /// Si : DA DB DI, alors env de recette
        /// Si : NI alors en de test
        /// </returns>
        public static string GetSite()
        {
            string r = Environment.GetEnvironmentVariable("PFTLSITE");
            return (r ?? "").Trim();
        }

        /// <summary>
        /// Récupère le chemin sauvé dans la variable d'environnement ENT{numEntree}.
        /// Exemple : ENT001, ENT002, ...
        /// </summary>
        /// <param name="numEntree">Numéro de l'entrée</param>
        /// <returns>La valeur contenue dans $ENT{XXX}</returns>
        public static string GetCheminEntree(int numEntree)
        {
            string nomEntree = string.Format("ENT{0}", numEntree.ToString("000"));

            string chemin = (Environment.GetEnvironmentVariable(nomEntree) ?? "").Trim();

            // tracing
            string logMsg = string.Format("{0}={1}", nomEntree, chemin);
            LogManager.Instance.AjouterPointTracingDiffere("ENV", "Environnement", LogManager.LOG_INFO, logMsg);

            return chemin;
        }

        /// <summary>
        /// Récupère le chemin sauvé dans la variable d'environnement SOR{numSortie}.
        /// Exemple : SOR001, SOR002, ...
        /// </summary>
        /// <param name="numSortie">Numéro de la sortie</param>
        /// <returns>La valeur contenue dans $SOR{XXX}</returns>
        public static string GetCheminSortie(int numSortie)
        {
            string nomSortie = string.Format("SOR{0}", numSortie.ToString("000"));

            string chemin = (Environment.GetEnvironmentVariable(nomSortie) ?? "").Trim();

            // tracing
            string logMsg = string.Format("{0}={1}", nomSortie, chemin);
            LogManager.Instance.AjouterPointTracingDiffere("ENV", "Environnement", LogManager.LOG_INFO, logMsg);

            return chemin;
        }

        /// <summary>
        /// Retourne l'identifiant de l'utilisateur à utiliser pour les appels vers la TSI Gespar.
        /// </summary>
        public static string GetAnonymousUser()
        {
            string env = GetSite();

            switch (env)
            {
                case "NI":
                    return "IWEBT77";       // Utilisateur Test
                case "DA":
                case "DB":
                case "DI":
                    return "IWEBDM";        // Utilisateur Recette
                case "HI":
                case "KI":
                case "GI":
                case "UI":
                case "SI":
                case "EI":
                case "QI":
                    return "IWEBDW";        // Utilisateur Production
                default:
                    return "IWEBDW";        // Utilisateur Production
            }
        }
    }
}
