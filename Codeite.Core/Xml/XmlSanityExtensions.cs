using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Xml.Linq;

public static class XmlSanityExtensions
{
    public static string XmlEscape(this string str)
    {
        return SecurityElement.Escape(str);
    }

    public static string AttributeValue(this XElement element, XName attributeName)
    {
        var val = element.AttributeValueOrNull(attributeName);
        if (val == null)
            throw new ArgumentException(String.Format("Element {0} had no attribute {1}", element.Name.LocalName, attributeName));
        return val;
    }

    public static IEnumerable<string> AttributeValues(this IEnumerable<XElement> elements, XName attributeName)
    {
        return elements.Select(e => e.AttributeValue(attributeName));
    }

    public static string AttributeValueOrNull(this XElement element, XName attributeName)
    {
        var attribute = element.Attribute(attributeName);
        return attribute == null ? null : attribute.Value;
    }

    public static bool HasAnyElements(this XContainer container, XName elementName)
    {
        return container.Elements(elementName).Any();
    }

    public static XElement EnsureRoot(this XDocument document)
    {
        var root = document.Root;
        if (root == null)
            throw new MissingMemberException("There was no root element");
        return root;
    }

    public static string ElementValue(this XElement element, XName name)
    {
        var child = element.Element(name);
        if (child == null)
            throw new MissingMemberException("There was no child element named " + name);
        return child.Value;
    }
}