using System;
using System.Collections.Specialized;

namespace aert_csharp_extensions
{
    public static class IterableHelper
    {
        /// <summary>
        /// Préfixe le dictionnaire ordonné <c>additional</c> au dictionnaire <c>dest</c>.
        /// </summary>
        public static void HelpPrepend(this OrderedDictionary dest, OrderedDictionary additional)
        {
            if (additional == null || additional.Count == 0)
                return;
            if (dest == null)
                throw new ArgumentNullException("dest");

            // récupération des clefs par index
            object[] keys = new object[additional.Keys.Count];
            additional.Keys.CopyTo(keys, 0);

            // insertion
            for (int i = 0; i < additional.Count; i++)
                dest.Insert(i, keys[i], additional[i]);
        }
    }
}
