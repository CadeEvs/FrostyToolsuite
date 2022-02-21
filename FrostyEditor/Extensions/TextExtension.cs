using System;
using System.IO;
using System.Windows;
using System.Windows.Markup;

namespace FrostyEditor.Extensions
{
    public class TextExtension : MarkupExtension
    {
        private readonly string textFile;
        public TextExtension(string inTextFile)
        {
            textFile = inTextFile;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            Uri uri = new Uri("pack://application:,,,/" + textFile);
            using (Stream stream = Application.GetResourceStream(uri).Stream)
            {
                using (TextReader reader = new StreamReader(stream))
                    return reader.ReadToEnd();
            }
        }
    }
}
