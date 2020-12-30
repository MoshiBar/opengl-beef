using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.Linq;

namespace opengl_beef {
    class GlParser {
        public static Dictionary<String, GlEnum> Enums = new Dictionary<String, GlEnum>();
        public static Dictionary<String, GlFunction> Functions = new Dictionary<String, GlFunction>();

        public static List<GlVersion> Versions = new List<GlVersion>();

        public static void Parse(Stream xmlStream) {
            Console.WriteLine("Parsing OpenGL specification.");

            Enums.Clear();
            Functions.Clear();
            Versions.Clear();

            XmlDocument xml = new XmlDocument();
            xml.Load(xmlStream);

            ParseEnums(xml);
            ParseFunctions(xml);
            ParseVersions(xml);

            xmlStream.Dispose();
        }

        public static GlVersion GetGlVersion(CmdOptions options) {
            foreach (GlVersion glVersion in Versions) {
                if (glVersion.Version == options.Version) {
                    return glVersion;
                }
            }

            return null;
        }

        private static void ParseEnums(XmlDocument xml) {
            foreach (XmlNode enumsNode in xml.GetElementsByTagName("enums")) {
                foreach (XmlNode enumNode in enumsNode) {
                    if (enumNode.Name == "enum" && (enumNode.Attributes["api"] == null || enumNode.Attributes["api"].Value == "gl")) {
                        GlEnum glEnum = new GlEnum(enumNode);
                        Enums.Add(glEnum.Name, glEnum);
                    }
                }
            }
        }

        private static void ParseFunctions(XmlDocument xml) {
            foreach (XmlNode commandsNode in xml.GetElementsByTagName("commands")) {
                foreach (XmlNode commandNode in commandsNode) {
                    if (commandNode.Name == "command") {
	                    if (!commandNode.InnerText.Contains("struct "))//commands with structs arent supported yet
                        {
	                        GlFunction glFunction = new GlFunction(commandNode);
                            Functions.Add(glFunction.Name, glFunction);
                        }
                    }
                }
            }
        }

        private static void ParseVersions(XmlDocument xml) {
            foreach (XmlNode featureNode in xml.GetElementsByTagName("feature")) {
                if (featureNode.Attributes["api"].Value == "gl") {
                    Versions.Add(new GlVersion(featureNode));
                }
            }

            foreach(XmlNode featureNode in xml.GetElementsByTagName("extension"))
            {
	            if(featureNode.Attributes["supported"].Value.Split('|').Contains("gl"))
	            {
		            Versions.Add(new GlVersion(featureNode));
	            }
            }

            Versions.Sort();
        }
    }
}