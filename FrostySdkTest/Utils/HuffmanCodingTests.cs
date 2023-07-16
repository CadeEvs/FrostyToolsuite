using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Frosty.Sdk.IO;
using Frosty.Sdk.Utils;

namespace FrostySdkTest.Utils
{
    public class HuffmanEncodingTests
    {

        private static readonly object[] EncodingDecodingTestValues =
        {
            new object[] { false, 25 },
            new object[] { true, 22 }
        };

        /// <summary>
        /// Tests the encoding and decoding of some test strings. The argument source once encodes the strings with reusing existing entries, and once without, leading to different result byte lengths.
        /// </summary>
        /// <param name="compressResults"></param>
        /// <param name="encodedByteSize"></param>
        [TestCaseSource(nameof(EncodingDecodingTestValues))]
        public void TextEncodingDecoding(bool compressResults, int encodedByteSize)
        {
            string[] texts = { "These are ", "", "some ", "Test Texts", " for tests ", "some ", " these are" };

            HuffmanEncoder encoder = new();

            var encodingTree = encoder.BuildHuffmanEncodingTree(texts);

            int i = 0;
            var input = texts.Select(x => new Tuple<int, string>(i++, x)).ToList();

            var encodingResult = encoder.EncodeTexts(input, compressResults);
            Assert.Multiple(() =>
            {
                Assert.That(encodingResult.EncodedTexts, Has.Length.EqualTo(encodedByteSize));
                Assert.That(encodingResult.EncodedTextPositions, Has.Count.EqualTo(texts.Length));
            });
            HuffmanDecoder decoder = new();

            using (MemoryStream stream = new())
            {
                using (DataStream ds = new(stream))
                {
                    foreach (var val in encodingTree)
                    {
                        ds.WriteUInt32(val);
                    }

                    ds.Position = 0;

                    decoder.ReadHuffmanTable(new DataStream(stream), (uint)encodingTree.Count);

                }
            }

            using (MemoryStream stream = new())
            {
                using (DataStream ds = new(stream))
                {

                    var byteArray = encodingResult.EncodedTexts;

                    ds.Write(byteArray);
                    ds.Position = 0;

                    decoder.ReadOddSizedEncodedData(ds, (uint)byteArray.Length);
                }
            }

            List<string> decoded = new();

            foreach (var textId in encodingResult.EncodedTextPositions)
            {
                string decodedText = decoder.ReadHuffmanEncodedString(textId.Position);

                decoded.Add(decodedText);

                Assert.That(decodedText, Is.EqualTo(texts[textId.Identifier]));
            }

            Assert.Multiple(() =>
            {
                Assert.That(decoded, Has.Count.EqualTo(texts.Length));
                Assert.That(decoded.ToArray(), Is.EqualTo(texts));
            });
        }
    }
}