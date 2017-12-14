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

        #region Parse return status
        public static bool ParseInt(XmlNode node, out int value)
        {
            if (!int.TryParse(node.InnerText, out value))
            {
                Logger.Error(string.Format("{0} isn't an int.", GetNodePath(node)));
                return false;
            }
            return true;
        }

        public static bool ParseFloat(XmlNode node, out float value)
        {
            if (!float.TryParse(node.InnerText, out value))
            {
                Logger.Error(string.Format("{0} isn't a float.", GetNodePath(node)));
                return false;
            }
            return true;
        }

        public static bool ParseInt(XmlAttribute attr, out int value)
        {
            if (!int.TryParse(attr.InnerText, out value))
            {
                Logger.Error(string.Format("{0}.{1} isn't an int.", GetNodePath(attr.ParentNode), attr.Name));
                return false;
            }
            return true;
        }

        public static bool ParseFloat(XmlAttribute attr, out float value)
        {
            if (!float.TryParse(attr.InnerText, out value))
            {
                Logger.Error(string.Format("{0}.{1} isn't a float.", GetNodePath(attr.ParentNode), attr.Name));
                return false;
            }
            return true;
        }

        public static bool ParseAttrInt(XmlNode node, string attrName, out int value, bool optional = false)
        {
            XmlAttribute attr = GetAttr(node, attrName, optional);
            if (attr == null)
            {
                value = 0;
                return false;
            }
            if (!int.TryParse(attr.InnerText, out value))
            {
                Logger.Error(string.Format("{0}.{1} isn't an int.", GetNodePath(node), attrName));
                return false;
            }
            return true;
        }

        public static bool ParseAttrFloat(XmlNode node, string attrName, out float value, bool optional = false)
        {
            XmlAttribute attr = GetAttr(node, attrName, optional);
            if (attr == null)
            {
                value = 0;
                return false;
            }
            if (!float.TryParse(attr.InnerText, out value))
            {
                Logger.Error(string.Format("{0}.{1} isn't a float.", GetNodePath(node), attrName));
                return false;
            }
            return true;
        }

        public static bool ParseAttrString(XmlNode node, string attrName, out string value, bool optional = false)
        {
            XmlAttribute attr = GetAttr(node, attrName, optional);
            if (attr == null)
            {
                value = null;
                return false;
            }
            else
            {
                value = attr.InnerText;
                return true;
            }
        }

        public static bool ParseChildInt(XmlNode node, string childName, out int value, bool optional = false)
        {
            XmlNode childNode = GetChild(node, childName, optional);
            if (childNode == null)
            {
                value = 0;
                return false;
            }
            return ParseInt(childNode, out value);
        }

        public static bool ParseChildFloat(XmlNode node, string childName, out float value, bool optional = false)
        {
            XmlNode childNode = GetChild(node, childName, optional);
            if (childNode == null)
            {
                value = 0;
                return false;
            }
            return ParseFloat(childNode, out value);
        }

        public static bool ParseChildString(XmlNode node, string childName, out string value, bool optional = false)
        {
            XmlNode childNode = GetChild(node, childName, optional);
            if (childNode == null)
            {
                value = null;
                return false;
            }
            else
            {
                value = childNode.InnerText;
                return true;
            }
        }
        #endregion

        #region Parse return result
        public static int ParseInt(XmlNode node)
        {
            int value;
            ParseInt(node, out value);
            return value;
        }

        public static float ParseFloat(XmlNode node)
        {
            float value;
            ParseFloat(node, out value);
            return value;
        }

        public static int ParseInt(XmlAttribute attr)
        {
            int value;
            ParseInt(attr, out value);
            return value;
        }

        public static float ParseFloat(XmlAttribute attr)
        {
            float value;
            ParseFloat(attr, out value);
            return value;
        }

        public static int ParseAttrInt(XmlNode node, string attrName, bool optional = false)
        {
            int value;
            ParseAttrInt(node, attrName, out value, optional);
            return value;
        }

        public static float ParseAttrFloat(XmlNode node, string attrName, bool optional = false)
        {
            float value;
            ParseAttrFloat(node, attrName, out value, optional);
            return value;
        }

        public static string ParseAttrString(XmlNode node, string attrName, bool optional = false)
        {
            string value;
            ParseAttrString(node, attrName, out value, optional);
            return value;
        }

        public static int ParseChildInt(XmlNode node, string childName, bool optional = false)
        {
            int value;
            ParseChildInt(node, childName, out value, optional);
            return value;
        }

        public static float ParseChildFloat(XmlNode node, string childName, bool optional = false)
        {
            float value;
            ParseChildFloat(node, childName, out value, optional);
            return value;
        }

        public static string ParseChildString(XmlNode node, string childName, bool optional = false)
        {
            string value;
            ParseChildString(node, childName, out value, optional);
            return value;
        }
        #endregion
    }
}
