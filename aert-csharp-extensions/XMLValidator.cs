using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace aert_csharp_extensions
{
    /// <summary>
    /// Cette classe permet de gérer la validation XSD d'un fichier XML
    /// </summary>
    public class XmlValidator
    {
        /// <summary>
        /// Constructeur privé
        /// </summary>
        private XmlValidator() { }

        /// <summary>
        /// Effectue la validation XSD d'un document XML.
        /// </summary>
        /// <param name="xmlFilePath">Chemin du fichier XML</param>
        /// <param name="xsdContent">Contenu du fichier XSD</param>
        /// <param name="validationMsg">Sortie d'erreurs</param>
        public static bool Validate(string xmlFilePath, string xsdContent, out string validationMsg)
        {
            XmlValidator instance = new XmlValidator();
            return instance.ValidateThis(xmlFilePath, xsdContent, out validationMsg);
        }

        /// <summary>
        /// Effectue la validation XSD d'un document XML
        /// </summary>
        public static bool Validate(XmlDocument doc, string xsdFile, out string validationMsgs)
        {
            XmlValidator instance = new XmlValidator();
            return instance.ValidateThis(doc, xsdFile, out validationMsgs);
        }


        /// <summary>
        /// Effectue la validation XSD d'un document XML.
        /// </summary>
        /// <param name="xmlFilePath">Chemin du fichier XML</param>
        /// <param name="xsdContent">Contenu du fichier XSD</param>
        /// <param name="validationMsgs">Sortie d'erreurs</param>
        public bool ValidateThis(string xmlFilePath, string xsdContent, out string validationMsgs)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlFilePath);

            XmlTextReader xsdReader = new XmlTextReader(xsdContent.HelpToStream());
            return ValidateThis(doc, xsdReader, out validationMsgs);
        }


        /// <summary>
        /// Effectue la validation XSD d'un document XML
        /// </summary>
        public bool ValidateThis(XmlDocument doc, string xsdFile, out string validationMsgs)
        {
            XmlTextReader xsdReader = new XmlTextReader(xsdFile);
            return ValidateThis(doc, xsdReader, out validationMsgs);
        }

        /// <summary>
        /// Effectue la validation XSD d'un document XML
        /// </summary>
        public bool ValidateThis(XmlDocument doc, XmlTextReader xsdReader, out string validationMsgs)
        {
            XmlSchemaSet schemas = new XmlSchemaSet();
            schemas.Add("", xsdReader);

            XDocument xdoc = doc.HelpToXDocument();
            string msg = "";
            xdoc.Validate(schemas, (o, e) =>
            {
                msg += e.Message +"\n";
            });

            validationMsgs = msg;

            return msg.HelpIsNullOrEmptyApprox();
        }
    }
}