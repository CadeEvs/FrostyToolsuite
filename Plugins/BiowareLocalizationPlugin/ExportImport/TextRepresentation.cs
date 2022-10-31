using System;
using System.Xml.Serialization;

namespace BiowareLocalizationPlugin.ExportImport
{
    [Serializable()]
    [XmlRootAttribute("TextFile", Namespace = "", IsNullable = false)]
    public class TextFile
    {

        public string LanguageFormat { get; set; }

        [XmlArray("Texts")]
        [XmlArrayItem("TextRepresentation")]
        public TextRepresentation[] Texts { get; set; }
    }

    [Serializable()]
    public class TextRepresentation
    {
        public string TextId { get; set; }
        public string Text { get; set; }

        [XmlArray("Resources")]
        [XmlArrayItem("TextResource")]
        public string[] Resources { get; set; }
    }
}
