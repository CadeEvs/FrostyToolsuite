using System;
using System.Xml.Serialization;

namespace BiowareLocalizationPlugin.ExportImport
{
    [Serializable()]
    [XmlRoot("TextFile", Namespace = "", IsNullable = false)]
    public class TextFile
    {

        public string LanguageFormat { get; set; }

        [XmlArray("Texts")]
        [XmlArrayItem("TextRepresentation")]
        public TextRepresentation[] Texts { get; set; }

        //[CanBeNull]
        [XmlArray("DeclinatedAdjectives", IsNullable = true)]
        [XmlArrayItem("DeclinatedAdjective")]
        public DeclinatedAdjectiveRepresentation[] DeclinatedAdjectives { get; set; }
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

    [Serializable()]
    public class DeclinatedAdjectiveRepresentation
    {

        public string Resource { get; set; }

        public string AdjectiveId { get; set; }

        [XmlArray("Declinations")]
        [XmlArrayItem("Declination")]
        public string[] Declinations { get; set; }
    }
}
