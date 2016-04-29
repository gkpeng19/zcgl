using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace NM.Util
{
    public static class XElementHelper
    {
        public static string GetString(this XElement source, string elementName, string defaultValue)
        {
            var e = source.Element(elementName);
            if (e != null)
            {
                return e.Value;
            }
            return defaultValue;
        }

        public static string GetString(this XElement source, string elementName)
        {

            return GetString(source, elementName, string.Empty);
        }

        public static int GetInt(this XElement source, string elementName)
        {
            return GetInt(source, elementName, 0);
        }

        public static int GetInt(this XElement source, string elementName, int defaultValue)
        {
            var e = source.Element(elementName);
            if (e != null)
            {
                int result = (int)e;
                //  Int32.TryParse(e.Value, out result);
                return result;
            }
            return defaultValue;
        }

        public static bool GetBoolean(this XElement source, string elementName)
        {
            return GetBoolean(source, elementName, false);
        }

        public static bool GetBoolean(this XElement source, string elementName, bool defaultValue)
        {
            var e = source.Element(elementName);
            if (e != null)
            {
                bool result = (bool)e;
                // Boolean.TryParse(e.Value, out result);
                return result;
            }
            return defaultValue;
        }

        public static string GetAttributeString(this XElement source, string attributeName)
        {
            return GetAttributeString(source, attributeName, string.Empty);
        }

        public static string GetAttributeString(this XElement source, string attributeName, string defaultValue)
        {
            var e = source.Attribute(attributeName);
            if (e != null)
            {
                return e.Value;
            }
            return defaultValue;
        }


        public static int GetAttributeInt(this XElement source, string attributeName)
        {
            return GetAttributeInt(source, attributeName, 0);
        }

        public static int GetAttributeInt(this XElement source, string attributeName, int defaultValue)
        {
            var e = source.Attribute(attributeName);
            if (e != null)
            {
                return (int)e;
            }
            return defaultValue;
        }

        public static bool GetAttributeBool(this XElement source, string attributeName)
        {
            return GetAttributeBool(source, attributeName, false);
        }

        public static bool GetAttributeBool(this XElement source, string attributeName, bool defaultValue)
        {
            var e = source.Attribute(attributeName);
            if (e != null)
            {
                return (bool)e;
            }
            return defaultValue;
        }
    }
}
