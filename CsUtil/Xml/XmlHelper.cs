using System;
using System.Collections.Generic;
using System.Xml;
using CsUtil.Util;

namespace CsUtil.Xml
{
    public class XmlHelper
    {
        public static XmlAttribute GetAttr(XmlNode node, string attrName, bool optional = false)
        {
            XmlAttribute attr = node.Attributes[attrName];
            if (attr == null && !optional)
            {
                Logger.Error(string.Format("{0} don't contain an attr named {1}.", GetNodePath(node), attrName));
            }
            return attr;
        }

        public static XmlNode GetChild(XmlNode node, string childName, bool optional = false)
        {
            XmlNode childNode = node[childName];
            if (childNode == null && !optional)
            {
                Logger.Error(string.Format("{0} don't contain an node named {1}.", GetNodePath(node), childName));
            }
            return childNode;
        }

        public static string GetNodePath(XmlNode node)
        {
            if (node.ParentNode == null)
                return node.Name;
            else
                return GetNodePath(node.ParentNode) + "/" + node.Name;
        }

        public static int ParseInt(XmlNode node)
        {
            int value;
            if (!int.TryParse(node.InnerText, out value))
            {
                Logger.Error(string.Format("{0} isn't an int.", GetNodePath(node)));
                return 0;
            }
            return value;
        }

        public static float ParseFloat(XmlNode node)
        {
            float value;
            if (!float.TryParse(node.InnerText, out value))
            {
                Logger.Error(string.Format("{0} isn't a float.", GetNodePath(node)));
                return 0;
            }
            return value;
        }

        public static int ParseInt(XmlAttribute attr)
        {
            int value;
            if (!int.TryParse(attr.InnerText, out value))
            {
                Logger.Error(string.Format("{0}.{1} isn't an int.", GetNodePath(attr.ParentNode), attr.Name));
                return 0;
            }
            return value;
        }

        public static float ParseFloat(XmlAttribute attr)
        {
            float value;
            if (!float.TryParse(attr.InnerText, out value))
            {
                Logger.Error(string.Format("{0}.{1} isn't a float.", GetNodePath(attr.ParentNode), attr.Name));
                return 0;
            }
            return value;
        }

        public static int ParseAttrInt(XmlNode node, string attrName, bool optional = false)
        {
            XmlAttribute attr = GetAttr(node, attrName, optional);
            if (attr == null)
            {
                return 0;
            }
            int value;
            if (!int.TryParse(attr.InnerText, out value))
            {
                Logger.Error(string.Format("{0}.{1} isn't an int.", GetNodePath(node), attrName));
                return 0;
            }
            return value;
        }

        public static float ParseAttrFloat(XmlNode node, string attrName, bool optional = false)
        {
            XmlAttribute attr = GetAttr(node, attrName, optional);
            if (attr == null)
            {
                return 0;
            }
            float value;
            if (!float.TryParse(attr.InnerText, out value))
            {
                Logger.Error(string.Format("{0}.{1} isn't a float.", GetNodePath(node), attrName));
                return 0;
            }
            return value;
        }

        public static string ParseAttrString(XmlNode node, string attrName, bool optional = false)
        {
            XmlAttribute attr = GetAttr(node, attrName, optional);
            if (attr == null)
            {
                return null;
            }
            return attr.InnerText;
        }

        public static int ParseChildInt(XmlNode node, string childName, bool optional = false)
        {
            XmlNode childNode = GetChild(node, childName, optional);
            if (childNode == null)
            {
                return 0;
            }
            return ParseInt(childNode);
        }

        public static float ParseChildFloat(XmlNode node, string childName, bool optional = false)
        {
            XmlNode childNode = GetChild(node, childName, optional);
            if (childNode == null)
            {
                return 0;
            }
            return ParseFloat(childNode);
        }

        public static string ParseChildString(XmlNode node, string childName, bool optional = false)
        {
            XmlNode childNode = GetChild(node, childName, optional);
            if (childNode == null)
            {
                return null;
            }
            return childNode.InnerText;
        }
    }
}
