using Frosty.Core;
using FrostySdk.IO;
using FrostySdk.Managers.Entries;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BiowareLocalizationPlugin.LocalizedResources
{
    /// <summary>
    /// This class includes some test functionality especially for veryfiying that a given resource can be written correctly enough to be read again by this plugin.
    /// The methods herein should *NEVER* be calles in production use! But I'm too lazy to write the same test methods each time again, so i just leave them all here...
    /// </summary>
    public class ResourceTestUtils
    {
        private ResourceTestUtils() { }

        /// <summary>
        /// Verifys that the Validity of the recreated flattened tree-nodes list. This is a debug method not intended to be called in production code
        /// </summary>
        /// <param name="original"></param>
        /// <param name="recreation"></param>
        public static void VerifySame(List<HuffmanNode> original, List<HuffmanNode> recreation)
        {

            bool same = true;

            int originalCount = original.Count;
            if (originalCount != recreation.Count)
            {
                App.Logger.Log("Original had {0} entries, recreation has {1} instead!", originalCount, recreation.Count);
                same = false;
                return;
            }

            for (int i = 0; i < originalCount; i++)
            {
                HuffmanNode node = original[i];
                HuffmanNode recreationNode = recreation[i];

                if (recreationNode.Value != node.Value)
                {
                    int recreationIndex = recreation.FindIndex((HuffmanNode toFind) => { return toFind.Value == node.Value; });
                    App.Logger.Log("Recreated Index of Node <{0}> is {1} instead of {2}!", node, recreationIndex, i);
                    same = false;

                    if (recreationIndex >= 0)
                    {
                        recreationNode = recreation[recreationIndex];
                    }
                    else
                    {
                        recreationNode = null;
                    }
                }
                if (recreationNode != null && (!node.GetBitRepresentation().Equals(recreationNode.GetBitRepresentation())))
                {
                    App.Logger.Log("Encoding of Node <{0}> is {1} instead of {2}!", node, recreationNode.GetBitRepresentation(), node.GetBitRepresentation());
                    same = false;
                }
            }

            if (same)
            {
                App.Logger.Log("The recreation matches the original!");
            }
        }

        /// <summary>
        /// Calculates dummy strings that roughly match the original character distribution for the character encoding.
        /// </summary>
        /// <param name="rootNode"></param>
        /// <returns>list of strings</returns>
        public static List<string> CalculateTestStrings(HuffmanNode rootNode)
        {
            List<List<char>> characterProbabilitySet = new List<List<char>>();

            List<HuffmanNode> levelList = new List<HuffmanNode>() {rootNode};

            while(levelList.Count > 0)
            {
                List<char> levelChars = new List<char>();
                foreach(var node in levelList)
                {
                    if(node.Left == null && node.Right == null)
                    {
                        levelChars.Add(node.Letter);
                    }
                    else if (! (node.Left != null && node.Right != null ))
                    {
                        App.Logger.Log("Uneven tree detected!");
                    }
                }

                characterProbabilitySet.Add(levelChars);
                levelList = ResourceUtils.GetNextLevel(levelList);
            }

            int multiplier = 1;
            StringBuilder charString = new StringBuilder();
            for(int level = characterProbabilitySet.Count - 1; level >=0; level--)
            {
                List<char> levelChars = characterProbabilitySet[level];

                //flip levelchars to recreate original encoding
                levelChars.Reverse();

                foreach(char c in levelChars)
                {
                    charString.Append( string.Join("", Enumerable.Repeat(c, multiplier)));
                }

                multiplier *= 2;
            }

            return new List<string>() { charString.ToString() };
        }

        public static void VerifyDefaultEncoding(HuffmanNode rootNode, string ResourceName)
        {
            App.Logger.Log("###################################################");
            App.Logger.Log("Comparing recreated encoding for resource <{0}>", ResourceName);

            var originalList = ResourceUtils.GetNodeList(rootNode);
            var originalCharDistribution = ResourceTestUtils.CalculateTestStrings(rootNode);

            HuffmanNode recalculatedRootNode = ResourceUtils.CalculateHuffmanEncoding(originalCharDistribution);
            var recalculatedList = ResourceUtils.GetNodeList(recalculatedRootNode);

            ResourceTestUtils.VerifySame(originalList, recalculatedList);
        }

        /// <summary>
        /// Verifies that the final text bit offsets in the encoded text-bit stream match.
        /// </summary>
        /// <param name="texts"></param>
        public static void VerifyTextPositions(SortedDictionary<uint, EncodedTextPosition> texts)
        {
            byte[] byteTexts = ResourceUtils.GetTextRepresentationToWrite( new SortedSet<EncodedTextPosition>( texts.Values));
            bool[] bitTexts = new bool[byteTexts.Length * 8];

            BitArray bitArray = new BitArray(byteTexts);
            bitArray.CopyTo(bitTexts, 0);

            int missMatches = 0;
            foreach (var entry in texts)
            {
                EncodedTextPosition text = entry.Value;

                bool[] textArray = new bool[text.GetLength()];
                Array.Copy(bitTexts, text.Position, textArray, 0, text.GetLength());

                if(!textArray.SequenceEqual(text.EncodedText.Value))
                {
                    //App.Logger.Log("Text with id <{0}> does not match in position!", entry.Key.ToString("X8"));
                    missMatches++;
                }
            }

            if(missMatches == texts.Count)
            {
                App.Logger.Log("None of the texts at the positions match!");
            }
            else if(missMatches == 0)
            {
                App.Logger.Log("All of the text positions fit");
            }
            else
            {
                App.Logger.Log("There are <{0}> texts out of <{1}> positioned incorrectly!", missMatches, texts.Count);
            }
        }

        /// <summary>
        /// Tests that, when a modified resource is written to a byte array that same byte array can be read again and produce the exact same texts.
        /// </summary>
        /// <param name="resource"></param>
        public static void ReadWriteTest(LocalizedStringResource resource)
        {
            App.Logger.Log("Test Rereading saved Data for <{0}>", resource.Name);

            SortedDictionary<uint, string> currentData = new SortedDictionary<uint, string>();
            foreach(var entry in resource.GetAllPrimaryTexts())
            {
                currentData[entry.Item1] = entry.Item2;
            }

            byte[] savedOutput = resource.SaveBytes();

            byte[] copyMetaData = new byte[resource.ResourceMeta.Length];
            resource.ResourceMeta.CopyTo(copyMetaData, 0);
            ResAssetEntry mockEntry = new ResAssetEntry() {ResRid=0, Name="dummy", ResMeta=copyMetaData };
            LocalizedStringResource recreation = new LocalizedStringResource();

            using(NativeReader reader = new NativeReader(new MemoryStream(savedOutput)))
            {
                recreation.Read(reader, null, mockEntry, null);
            }

            // // Note that recreated node list does still not match exactly! No matter wheter GetNodeList or GetNodeListToWrite is used <_<
            //var recreatedNodeList = ResourceUtils.GetNodeList(resource.GetRootNode());
            //var reRecreatedNodeList = ResourceUtils.GetNodeListToWrite(recreation.GetRootNode());
            //VerifySame(recreatedNodeList, reRecreatedNodeList);

            SortedDictionary<uint, string> recreatedData = new SortedDictionary<uint, string>();
            foreach (var entry in recreation.GetAllPrimaryTexts())
            {
                recreatedData[entry.Item1] = entry.Item2;
            }

            TestTextsFromReReadResource(currentData, recreatedData, "primary");

            int dragonAgeBlocksCount = resource.DragonAgeDeclinatedCraftingNames.NumberOfDeclinations;
            int recreatedDragonAgeBlocksCount = recreation.DragonAgeDeclinatedCraftingNames.NumberOfDeclinations;

            if(dragonAgeBlocksCount != recreatedDragonAgeBlocksCount)
            {
                App.Logger.Log("... Expected <{0}> blocks of declinated adjectives, got <{1}> instead!", dragonAgeBlocksCount, recreatedDragonAgeBlocksCount);
            }
            else
            {
                for(int i = 0; i< dragonAgeBlocksCount; i++)
                {
                    SortedDictionary<uint, string> originalBlockData = new SortedDictionary<uint, string>();
                    SortedDictionary<uint, string> recreatedBlockData = new SortedDictionary<uint, string>();

                    resource.DragonAgeDeclinatedCraftingNames.GetAdjectivesOfDeclination(i).ToList().ForEach(ls => originalBlockData[ls.Id] = ls.Value);
                    recreation.DragonAgeDeclinatedCraftingNames.GetAdjectivesOfDeclination(i).ToList().ForEach(ls => originalBlockData[ls.Id] = ls.Value);

                    TestTextsFromReReadResource(originalBlockData, recreatedBlockData, $"declinatedAdjectives[{i}]");
                }
            }
        }

        private static void TestTextsFromReReadResource(
            SortedDictionary<uint, string> originalData, SortedDictionary<uint, string> recreatedData,
            string blockNameForErrorMessages
            )
        {
            if (originalData.Count != recreatedData.Count)
            {
                App.Logger.Log("...Incorrect Number of {2} Texts! Expected {0}, got {1}", originalData.Count, recreatedData.Count, blockNameForErrorMessages);
            }
            else
            {
                int missMatching = 0;
                int missingIds = 0;
                foreach (uint key in originalData.Keys)
                {
                    string originalText = originalData[key];
                    
                    bool textExistsInRecreation = recreatedData.TryGetValue(key, out string recreationText);
                    if(!textExistsInRecreation)
                    {
                        missingIds++;
                    }

                    if (!originalText.Equals(recreationText))
                    {
                        missMatching++;
                    }
                }

                if (missMatching == originalData.Count)
                {
                    App.Logger.Log("...None of the {0} texts match! {1} Text(s) were missing!", blockNameForErrorMessages, missingIds);
                }
                else if (missMatching == 0)
                {
                    App.Logger.Log("...All of the {0} texts match", blockNameForErrorMessages);
                }
                else
                {
                    App.Logger.Log("...<{0}> texts missmatch and {3} missing out of all <{1}> {2} texts", missMatching, originalData.Count, blockNameForErrorMessages, missingIds);
                }

            }
        }
    }
}
