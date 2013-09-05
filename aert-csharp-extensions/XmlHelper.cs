using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace aert_csharp_extensions
{
    public static class XmlHelper
    {
        #region Names

        public static bool HelpNameEquals(this XElement node, string otherString)
        {
            if (node == null)
                return false;
            return HelpGetName(node).HelpEqualsTrimmed(otherString);
        }

        /// <summary>
        /// Get's element's name.
        /// </summary>
        public static string HelpGetName(this XElement node)
        {
            if (node == null)
                return null;

            return node.Name.LocalName.Trim();
        }

        #endregion

        #region Values

        public static string HelpGetValue(this XElement node)
        {
            if (node == null)
                return null;
            return node.Value.HelpTrimOrEmpty();
        }

        #endregion

        #region Attributes

        public static bool HelpHasAttr(this XElement node, string attrName)
        {
            if (node == null)
                return false;

            return node.Attribute(attrName) != null;
        }

        public static string HelpGetAttr(this XElement node, string attrName)
        {
            if (node == null)
                return null;

            XAttribute attr = node.Attribute(attrName);
            return attr != null ? attr.Value.Trim() : null;
        }

        public static bool HelpAttrEquals(this XElement node, string attrName, string otherString)
        {
            if (node == null)
                return false;
            return HelpGetAttr(node, attrName).HelpEqualsTrimmed(otherString);
        }

        #endregion

        #region ToString

        /// <summary>
        /// Retourne le fil d'ariane du noeud.
        /// </summary>
        public static string HelpToStringNameHierarchy(this XElement node, bool includeLastChild = true, string separator = null)
        {
            if (node == null)
                return string.Empty;

            if (separator.HelpIsNullOrEmptyApprox())
                separator = "->";

            List<string> nameHierarchy = new List<string>();

            XElement currentNode = node;
            while (currentNode != null)
            {
                nameHierarchy.Add(HelpGetName(currentNode));
                currentNode = currentNode.Parent;
            }

            if (!includeLastChild)
                nameHierarchy.RemoveAt(0);

            nameHierarchy.Reverse();
            return string.Join(separator, nameHierarchy.ToArray());
        }

        /// <summary>
        /// Retourne le nom (non ambigue) du noeud (fil d'ariane).
        /// </summary>
        public static string HelpGetQualifiedName(this XElement node)
        {
            return node.HelpToStringNameHierarchy(true, ".");
        }

        /// <summary>
        /// Retourne le nom (non ambigue) du noeud (fil d'ariane) ainsi que la liste des attributs.
        /// </summary>
        public static string HelpToJsonString(this XElement node)
        {
            if (node == null)
                return "";

            string result = HelpGetQualifiedName(node);
            string attrs = string.Join(", ", node.Attributes().Select(x => x.Name + "=" + x.Value).ToArray());

            if (attrs.HelpIsNullOrEmptyApprox())
                return "{ " + result + " }";

            return "{ " + result + ": " + "{ " + attrs + " }" + " }";
        }

        #endregion

        /// <summary>
        /// Convertit la donnée XmlNode en XElement pour requêtage avec LINQ to XML.
        /// </summary>
        public static XElement HelperToXElement(this XmlNode node)
        {
            XDocument xDoc = new XDocument();
            using (XmlWriter xmlWriter = xDoc.CreateWriter())
                node.WriteTo(xmlWriter);
            return xDoc.Root;
        }

        /// <summary>
        /// Convertit la donnée XElement en XmlNode.
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
