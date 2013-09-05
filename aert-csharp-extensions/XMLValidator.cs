using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;

namespace aert_csharp_extensions
{
    /// <summary>
    /// Cette classe permet de gérer la validation XSD d'un fichier XML
    /// </summary>
    internal class XmlValidator
    {
        private bool _IsValid = true;
        private string _ValidationMessages = string.Empty;

        /// <summary>
        /// Constructeur privé
        /// </summary>
        private XmlValidator() { }

        /// <summary>
        /// Effectue la validation XSD d'un document XML
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="xsdFile"></param>
        /// <param name="validationMsgs">Messages issues de la validation</param>
        public static bool Validate(XmlDocument doc, string xsdFile, out string validationMsgs)
        {
            XmlValidator instance = new XmlValidator();
            return instance.ValidateThis(doc, xsdFile, out validationMsgs);
        }

        /// <summary>
        /// Effectue la validation XSD d'un document XML
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="xsdFile"></param>
        /// <param name="validationMsgs">Messages issues de la validation</param>
        public bool ValidateThis(XmlDocument doc, string xsdFile, out string validationMsgs)
        {

            // Set the validation settings.
            XmlReaderSettings settings = new XmlReaderSettings { ValidationType = ValidationType.Schema };

            XmlSchemaSet schemaSet = new XmlSchemaSet();
            schemaSet.Add(null, new XmlTextReader(xsdFile));

            settings.Schemas.Add(schemaSet);
            settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
            settings.ValidationEventHandler += ValidationCallBack;

            XmlReader reader = XmlReader.Create(new StringReader(doc.InnerXml), settings);

            // Parse the file. 
            while (reader.Read()) { }

            validationMsgs = _ValidationMessages;
            return _IsValid;
        }

        /// <summary>
        /// Affiche les erreurs et avertissements rencontrés lors de la validation.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void ValidationCallBack(object sender, ValidationEventArgs args)
        {
            _IsValid = false;

            if (args.Severity == XmlSeverityType.Warning)
                _ValidationMessages += "Avertissement : Aucun schéma correspondant trouvé. Validation non effectuée : " + args.Message + "\n";
            else
                _ValidationMessages += "Erreur : Validation échouée : " + args.Message + "\n";

            Console.Write(_ValidationMessages);
        }
    }
}