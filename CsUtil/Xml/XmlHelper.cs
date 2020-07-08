using System;
using System.Xml;

namespace CsUtil.Xml
{
    public class XmlHelper
    {
        public static XmlAttribute GetAttr(XmlNode node, string attrName, bool optional = false)
        {
            XmlAttribute attr = node.Attributes[attrName];
#if USE_LOGWRAPPER
            if (attr == null && !optional)
            {
                LogWrapper.LogError(GetNodePath(node), " don't contain an attr named ", attrName);
            }
#endif
            return attr;
        }

        public static XmlNode GetChild(XmlNode node, string childName, bool optional = false)
        {
            XmlNode childNode = node[childName];
#if USE_LOGWRAPPER
            if (childNode == null && !optional)
            {
                LogWrapper.LogError(GetNodePath(node), " don't contain an node named ", childName);
            }
#endif
            return childNode;
        }

        public static XmlNodeList GetChildNodeList(XmlNode node, string childName, string nodeName, bool optional = false)
        {
            XmlNode childNode = node[childName];
            if (childNode != null)
            {
                return childNode.SelectNodes(nodeName);
            }
#if USE_LOGWRAPPER
            else if (!optional)
            {
                LogWrapper.LogError(GetNodePath(node), " don't contain an node named ", childName);
            }
#endif
            return null;
        }

        public static string GetNodePath(XmlNode node)
        {
            if (node.ParentNode == null)
                return node.Name;
            else
                return GetNodePath(node.ParentNode) + "/" + node.Name;
        }

        #region Parse Node
        public static int ParseInt(XmlNode node, int default_value = 0)
        {
            int value;
            if (!int.TryParse(node.InnerText, out value))
            {
#if USE_LOGWRAPPER
                LogWrapper.LogError(GetNodePath(node), " isn't an int.");
#endif
                return default_value;
            }
            return value;
        }

        public static long ParseLong(XmlNode node, long default_value = 0)
        {
            long value;
            if (!long.TryParse(node.InnerText, out value))
            {
#if USE_LOGWRAPPER
                LogWrapper.LogError(GetNodePath(node), " isn't a long.");
#endif
                return default_value;
            }
            return value;
        }

        public static float ParseFloat(XmlNode node, float default_value = 0)
        {
            float value;
            if (!float.TryParse(node.InnerText, out value))
            {
#if USE_LOGWRAPPER
                LogWrapper.LogError(GetNodePath(node), " isn't a float.");
#endif
                return default_value;
            }
            return value;
        }

        public static bool ParseBool(XmlNode node, bool default_value = false)
        {
            bool value = false;
            if (!bool.TryParse(node.InnerText, out value))
            {
#if USE_LOGWRAPPER
                LogWrapper.LogError(GetNodePath(node), " isn't a bool.");
#endif
                return default_value;
            }
            return value;
        }
        #endregion

        #region Parse Attr
        public static int ParseAttrInt(XmlAttribute attr, int default_value = 0)
        {
            int value;
            if (!int.TryParse(attr.InnerText, out value))
            {
#if USE_LOGWRAPPER
                LogWrapper.LogError(GetNodePath(attr.OwnerElement) + "." + attr.Name, " isn't an int.");
#endif
                return default_value;
            }
            return value;
        }

        public static long ParseAttrLong(XmlAttribute attr, long default_value = 0)
        {
            long value;
            if (!long.TryParse(attr.InnerText, out value))
            {
#if USE_LOGWRAPPER
                LogWrapper.LogError(GetNodePath(attr.OwnerElement) + "." + attr.Name, " isn't a long.");
#endif
                return default_value;
            }
            return value;
        }

        public static float ParseAttrFloat(XmlAttribute attr, float default_value = 0)
        {
            float value;
            if (!float.TryParse(attr.InnerText, out value))
            {
#if USE_LOGWRAPPER
                LogWrapper.LogError(GetNodePath(attr.OwnerElement) + "." + attr.Name, " isn't a float.");
#endif
                return default_value;
            }
            return value;
        }

        public static bool ParseAttrBool(XmlAttribute attr, bool default_value = false)
        {
            bool value;
            if (!bool.TryParse(attr.InnerText, out value))
            {
#if USE_LOGWRAPPER
                LogWrapper.LogError(GetNodePath(attr.OwnerElement) + "." + attr.Name, " isn't a bool.");
#endif
                return default_value;
            }
            return value;
        }

        public static int ParseAttrInt(XmlNode node, string attrName, int default_value = 0)
        {
            XmlAttribute attr = GetAttr(node, attrName, true);
            if (attr == null)
            {
                return default_value;
            }
            return ParseAttrInt(attr, default_value);
        }

        public static long ParseAttrLong(XmlNode node, string attrName, long default_value = 0)
        {
            XmlAttribute attr = GetAttr(node, attrName, true);
            if (attr == null)
            {
                return default_value;
            }
            return ParseAttrLong(attr, default_value);
        }

        public static float ParseAttrFloat(XmlNode node, string attrName, float default_value = 0)
        {
            XmlAttribute attr = GetAttr(node, attrName, true);
            if (attr == null)
            {
                return default_value;
            }
            return ParseAttrFloat(attr, default_value);
        }

        public static bool ParseAttrBool(XmlNode node, string attrName, bool default_value = false)
        {
            XmlAttribute attr = GetAttr(node, attrName, true);
            if (attr == null)
            {
                return default_value;
            }
            return ParseAttrBool(attr, default_value);
        }

        public static string ParseAttrString(XmlNode node, string attrName, string default_value = null)
        {
            XmlAttribute attr = GetAttr(node, attrName, true);
            if (attr == null)
            {
                return default_value;
            }
            return attr.InnerText;
        }
        #endregion

        #region Parse Child Node
        public static int ParseChildInt(XmlNode node, string childName, int default_value = 0)
        {
            XmlNode childNode = GetChild(node, childName, true);
            if (childNode == null)
            {
                return default_value;
            }
            return ParseInt(childNode, default_value);
        }

        public static long ParseChildLong(XmlNode node, string childName, long default_value = 0)
        {
            XmlNode childNode = GetChild(node, childName, true);
            if (childNode == null)
            {
                return default_value;
            }
            return ParseLong(childNode, default_value);
        }

        public static float ParseChildFloat(XmlNode node, string childName, float default_value = 0)
        {
            XmlNode childNode = GetChild(node, childName, true);
            if (childNode == null)
            {
                return default_value;
            }
            return ParseFloat(childNode, default_value);
        }

        public static bool ParseChildBool(XmlNode node, string childName, bool default_value = false)
        {
            XmlNode childNode = GetChild(node, childName, true);
            if (childNode == null)
            {
                return default_value;
            }
            return ParseBool(childNode, default_value);
        }

        public static string ParseChildString(XmlNode node, string childName, string default_value = null)
        {
            XmlNode childNode = GetChild(node, childName, true);
            if (childNode == null)
            {
                return default_value;
            }
            return childNode.InnerText;
        }
        #endregion

        #region Try Parse
        public static bool TryParseAttrInt(XmlNode node, string attrName, out int value, bool optional = false)
        {
            XmlAttribute attr = GetAttr(node, attrName, optional);
            if (attr == null)
            {
                value = 0;
                return false;
            }
            if (!int.TryParse(attr.InnerText, out value))
            {
#if USE_LOGWRAPPER
                LogWrapper.LogError("{0}.{1} isn't an int.", GetNodePath(node), attrName);
#endif
                return false;
            }
            return true;
        }

        public static bool TryParseAttrLong(XmlNode node, string attrName, out long value, bool optional = false)
        {
            XmlAttribute attr = GetAttr(node, attrName, optional);
            if (attr == null)
            {
                value = 0;
                return false;
            }
            if (!long.TryParse(attr.InnerText, out value))
            {
#if USE_LOGWRAPPER
                LogWrapper.LogError("{0}.{1} isn't a long.", GetNodePath(node), attrName);
#endif
                return false;
            }
            return true;
        }

        public static bool TryParseAttrFloat(XmlNode node, string attrName, out float value, bool optional = false)
        {
            XmlAttribute attr = GetAttr(node, attrName, optional);
            if (attr == null)
            {
                value = 0;
                return false;
            }
            if (!float.TryParse(attr.InnerText, out value))
            {
#if USE_LOGWRAPPER
                LogWrapper.LogError("{0}.{1} isn't a float.", GetNodePath(node), attrName);
#endif
                return false;
            }
            return true;
        }

        public static bool TryParseAttrFloat(XmlNode node, string attrName, out bool value, bool optional = false)
        {
            XmlAttribute attr = GetAttr(node, attrName, optional);
            if (attr == null)
            {
                value = false;
                return false;
            }
            if (!bool.TryParse(attr.InnerText, out value))
            {
#if USE_LOGWRAPPER
                LogWrapper.LogError("{0}.{1} isn't a bool.", GetNodePath(node), attrName);
#endif
                return false;
            }
            return true;
        }
        #endregion
    }
}
