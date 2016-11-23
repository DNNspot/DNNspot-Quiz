using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace DNNspot.Quiz
{
    public static class Extensions
    {
        public static string GetAttributeValue(this XElement source, string name)
        {
            var attrib = source.Attribute(name);
            return attrib != null ? attrib.Value : string.Empty;
        }

        public static string GetElementValue(this XElement source, string name)
        {
            var v = source.Element(name);
            return v != null ? v.Value : string.Empty;
        }        
    }
}