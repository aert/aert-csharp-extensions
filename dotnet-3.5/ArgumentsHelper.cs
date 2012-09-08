using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;

namespace Aert.Helpers
{
    /// <summary>
    /// Classe utilitaire permettant de faciliter le traitement des paramètres fournis en ligne de commande.
    /// <para>
    /// Valid parameters forms:
    /// {-,/,--}param{ ,=,:}((",')value(",'))
    /// </para>
    /// </summary>
    /// <example>
    /// Examples: 
    /// -param1 value1 --param2 /param3:"Test-:-work" 
    ///  /param4=happy -param5 '--=nice=--'
    /// </example>
    public class ArgumentsHelper
    {
        // Variables
        private readonly StringDictionary _parameters;

        // Constructor
        public ArgumentsHelper(IEnumerable<string> args)
        {
            _parameters = new StringDictionary();
            Regex spliter = new Regex(@"^-{1,2}|^/|=|:",
                RegexOptions.IgnoreCase | RegexOptions.Compiled);

            Regex remover = new Regex(@"^['""]?(.*?)['""]?$",
                RegexOptions.IgnoreCase | RegexOptions.Compiled);

            string parameter = null;

            // Valid parameters forms:
            // {-,/,--}param{ ,=,:}((",')value(",'))
            // Examples: 
            // -param1 value1 --param2 /param3:"Test-:-work" 
            //   /param4=happy -param5 '--=nice=--'
            foreach (string txt in args)
            {
                // Look for new parameters (-,/ or --) and a
                // possible enclosed value (=,:)
                string[] parts = spliter.Split(txt, 3);

                switch (parts.Length)
                {
                    // Found a value (for the last parameter 
                    // found (space separator))
                    case 1:
                        if (parameter != null)
                        {
                            if (!_parameters.ContainsKey(parameter))
                            {
                                parts[0] =
                                    remover.Replace(parts[0], "$1");

                                _parameters.Add(parameter, parts[0]);
                            }
                            parameter = null;
                        }
                        // else Error: no parameter waiting for a value (skipped)
                        break;

                    // Found just a parameter
                    case 2:
                        // The last parameter is still waiting. 
                        // With no value, set it to true.
                        if (parameter != null)
                        {
                            if (!_parameters.ContainsKey(parameter))
                                _parameters.Add(parameter, "true");
                        }
                        parameter = parts[1].ToLower();
                        break;

                    // Parameter with enclosed value
                    case 3:
                        // The last parameter is still waiting. 
                        // With no value, set it to true.
                        if (parameter != null)
                        {
                            if (!_parameters.ContainsKey(parameter))
                                _parameters.Add(parameter, "true");
                        }

                        parameter = parts[1];

                        // Remove possible enclosing characters (",')
                        if (!_parameters.ContainsKey(parameter))
                        {
                            parts[2] = remover.Replace(parts[2], "$1");
                            _parameters.Add(parameter, parts[2]);
                        }

                        parameter = null;
                        break;
                }
            }
            // In case a parameter is still waiting
            if (parameter != null)
            {
                if (!_parameters.ContainsKey(parameter))
                    _parameters.Add(parameter, "true");
            }
        }

        // Retrieve a parameter value if it exists 
        // (overriding C# indexer property)
        public string this[string param]
        {
            get
            {
                return (_parameters[param.ToLower()]);
            }
        }

        /// <summary>
        /// Retourne la liste des arguments spécifiés en ligne de commandes non contenus dans <c>knownParams</c>.
        /// </summary>
        public string[] GetUnknownParams(string[] knownParams)
        {
            string[] allParams = _parameters.Keys.Cast<string>().ToArray();

            if (knownParams == null || knownParams.Length == 0)
                return allParams;

            knownParams = knownParams.Select(x => x.ToLower()).ToArray();
            return allParams.Where(p => !knownParams.Contains(p)).ToArray();
        }
    }
}
