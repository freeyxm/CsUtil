using System;
using System.Collections.Generic;
using System.Text;

public class XmlDataWriter
{
    protected StringBuilder m_builder = new StringBuilder();
    protected string m_indent = "";
    protected string m_line_break = "\n";

    public XmlDataWriter(string indent, string line_break)
    {
        m_indent = indent;
        m_line_break = line_break;
    }

    public virtual void Clear()
    {
        m_builder.Length = 0;
    }

    public override string ToString()
    {
        return m_builder.ToString();
    }

    public XmlDataWriter AppendNodeHead(string name, int indent_count = 0)
    {
        AppendString(m_indent, indent_count);
        m_builder.Append("<").Append(name);
        return this;
    }

    public XmlDataWriter AppendNodeHeadEnd()
    {
        m_builder.Append(">").Append(m_line_break);
        return this;
    }

    public XmlDataWriter AppendNodeTail(string name, int indent_count = 0)
    {
        AppendString(m_indent, indent_count);
        m_builder.Append("</").Append(name).Append(">").Append(m_line_break);
        return this;
    }

    public XmlDataWriter AppendNodeTail()
    {
        m_builder.Append("/>").Append(m_line_break);
        return this;
    }

    public XmlDataWriter AppendAttr(string name, int value)
    {
        m_builder.Append(" ").Append(name).Append("=\"").Append(value).Append("\"");
        return this;
    }

    public XmlDataWriter AppendAttrCond(string name, int value, int default_value)
    {
        if (value != default_value)
        {
            AppendAttr(name, value);
        }
        return this;
    }

    public XmlDataWriter AppendAttr(string name, float value)
    {
        m_builder.Append(" ").Append(name).Append("=\"").Append(value).Append("\"");
        return this;
    }

    public XmlDataWriter AppendAttr(string name, bool value)
    {
        m_builder.Append(" ").Append(name).Append("=\"").Append(value ? "true" : "false").Append("\"");
        return this;
    }

    public XmlDataWriter AppendAttrCond(string name, bool value, bool default_value)
    {
        if (value != default_value)
        {
            AppendAttr(name, value);
        }
        return this;
    }

    public XmlDataWriter AppendAttr(string name, string value)
    {
        m_builder.Append(" ").Append(name).Append("=\"").Append(value).Append("\"");
        return this;
    }

    public XmlDataWriter AppendAttrCond(string name, string value, string default_value)
    {
        if (!(string.IsNullOrEmpty(value) && string.IsNullOrEmpty(default_value))
            && value != default_value)
        {
            AppendAttr(name, value);
        }
        return this;
    }

    public XmlDataWriter AppendAttr(string name, List<int> value, string split)
    {
        m_builder.Append(" ").Append(name).Append("=\"");
        for (int i = 0; i < value.Count; ++i)
        {
            if (i > 0)
            {
                m_builder.Append(split);
            }
            m_builder.Append(value[i]);
        }
        m_builder.Append("\"");
        return this;
    }

    public XmlDataWriter AppendString(string str, int count)
    {
        for (int i = 0; i < count; ++i)
        {
            m_builder.Append(str);
        }
        return this;
    }

    public XmlDataWriter AppendNewLine()
    {
        m_builder.Append(m_line_break);
        return this;
    }
}

