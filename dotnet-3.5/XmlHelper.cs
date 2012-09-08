using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Aert.Helpers
{
    /// <summary>
    /// Extensions facilitant l'accès aux données XML.
    /// </summary>
    public static class XmlHelper
    {
        /// <summary>
        /// Converti la donnée XmlNode en XElement pour requêtage avec LINQ to XML.
        /// </summary>
        public static XElement HelperToXElement(this XmlNode node)
        {
            XDocument xDoc = new XDocument();
            using (XmlWriter xmlWriter = xDoc.CreateWriter())
                node.WriteTo(xmlWriter);
            return xDoc.Root;
        }

        /// <summary>
        /// Converti la donnée XElement en XmlNode.
        /// </summary>
        public static XmlNode HelperToXmlNode(this XElement element)
        {
            using (XmlReader xmlReader = element.CreateReader())
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(xmlReader);
                return xmlDoc;
            }
        }
    }
}
