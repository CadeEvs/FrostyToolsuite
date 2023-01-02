using Frosty.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace BiowareLocalizationPlugin.LocalizedResources
{
    class ResourceComponentsHelper
    {
        // not needed
    }

    /// <summary>
    /// A class containing header information from a resource.
    /// </summary>
    public class ResourceHeader
    {
        public const uint Magic = 0xd78b40eb;

        // no idea what this does, doesn't seem to affect anything
        public uint Unknown1;

        // Note: If nodeCount changes due to added Chars, then dataOffset changes!
        // Additional Note: This offset is also part of the metadata, the value in this header is not guaranteed to be correct!
        public uint DataOffset;

        // also no idea, can set these to zero and nothing bad happens
        public uint Unknown2;
        public uint Unknown3;
        public uint Unknown4;

        // // nodeCount is an even integer! The rootNode as would-be last node in the node list is *not* actually part of the list
        public uint NodeCount;
        public uint NodeOffset;
        public uint StringsCount;
        public uint StringsOffset;

        // Note: If one of nodeCount or stringsCount changes, then the offsets herein also change! This list has a length of 2
        public List<DataCountAndOffsets> FirstUnknownDataDefSegments = new List<DataCountAndOffsets>();

        // These are only available for very few resources, they contain the count and offset for the strings used when crafting items in DA:I
        // This starts at the 3rd of the DataCountAndOffsets, potentially this contains only zeros.
        public List<DataCountAndOffsets> DragonAgeDeclinatedCraftingNamePartsCountAndOffset { get; private set; } = new List<DataCountAndOffsets>();

        // this is *not* part of the actual header?
        public int MaxDeclinations { get; private set; } = 0;

        public void AddDragonAgeDeclinatedCraftingNamePart(DataCountAndOffsets coundAndOffset)
        {
            MaxDeclinations++;
            DragonAgeDeclinatedCraftingNamePartsCountAndOffset.Add(coundAndOffset);
        }

        public override string ToString()
        {
            string uk1AsHex = Unknown1.ToString("X");
            string uk2AsHex = Unknown2.ToString("X");
            string uk3AsHex = Unknown3.ToString("X");
            string uk4AsHex = Unknown4.ToString("X");

            StringBuilder sb = new StringBuilder();
            sb.Append($"unknown1: <{Unknown1} | 0x{uk1AsHex}>\n")
                .Append($"unknown2: <{Unknown2} | 0x{uk2AsHex}>\n")
                .Append($"unknown3: <{Unknown3} | 0x{uk3AsHex}>\n")
                .Append($"unknown4: <{Unknown4} | 0x{uk4AsHex}>\n")
                .Append($"NodeCount: <{NodeCount}> starting at <{NodeOffset}>\n")
                .Append($"StringCount: <{StringsCount}> starting at <{StringsOffset}>\n");

            foreach (var ukd in FirstUnknownDataDefSegments)
            {
                uint byte8Count = ukd.Count;
                if (byte8Count > 0)
                {
                    uint totalsize = byte8Count * 8;
                    sb.Append($"  Additional data of {byte8Count} 8Bytes, or {totalsize} bytes starts at <{ukd.Offset}>\n");
                }
            }

            foreach (var craftingNamePartCounts in DragonAgeDeclinatedCraftingNamePartsCountAndOffset)
            {
                uint byte8Count = craftingNamePartCounts.Count;
                if (byte8Count > 0)
                {
                    uint totalsize = byte8Count * 8;
                    sb.Append($"  Declinated crafting name parts of {byte8Count} 8Bytes, or {totalsize} bytes starts at <{craftingNamePartCounts.Offset}>\n");
                }
            }

            sb.Append($"DataOffset is: <{DataOffset}>\n");

            return sb.ToString();
        }
    }

    /// <summary>
    /// Another poco containing offsets to remember
    /// </summary>
    public class DataCountAndOffsets
    {
        public uint Count;
        public uint Offset;
    }

    /// <summary>
    /// A node in the huffman coding scheme
    /// </summary>
    public class HuffmanNode : IComparable<HuffmanNode>
    {
        public char Letter => (char)(~Value);

        public uint Value;
        public HuffmanNode Left { get; private set; }
        public HuffmanNode Right { get; private set; }

        public HuffmanNode Parent { get; private set; }

        public void SetLeftNode(HuffmanNode leftNode)
        {
            this.Left = leftNode;
            Left.Parent = this;
        }

        public void SetRightNode(HuffmanNode rightNode)
        {
            this.Right = rightNode;
            Right.Parent = this;
        }

        public override string ToString()
        {
            string printLetter;

            switch (Value)
            {
                case uint.MaxValue:
                    printLetter = "endDelimeter";
                    break;
                case 4294967285:
                    printLetter = "newLine";
                    break;
                default:
                    printLetter = Letter.ToString();
                    break;
            }

            return string.Format("[Value = <{0}> | Letter = <{1}>]", Value.ToString(), printLetter);
        }

        /// <summary>
        /// Returns the bit representation of this node, to be used in tests.
        /// </summary>
        /// <returns></returns>
        public string GetBitRepresentation()
        {
            if (Parent == null)
            {
                return "";
            }

            string bitVal;
            if (this == Parent.Left)
                bitVal = "0";
            else if (this == Parent.Right)
            {
                bitVal = "1";
            }
            else
            {
                bitVal = "ERROR!";
            }
            return Parent.GetBitRepresentation() + bitVal;
        }

        public int CompareTo(HuffmanNode other)
        {
            return Value.CompareTo(other.Value);
        }
    }

    /// <summary>
    /// Represents a huffman encoded text.
    /// </summary>
    public class EncodedText
    {
        public List<bool> Value { get; }

        private readonly int m_hashValue;

        public EncodedText(List<bool> encodedText)
        {
            this.Value = encodedText ?? throw new InvalidOperationException("\"encodedText\" must not be null!");
            m_hashValue = ComputeHash(encodedText);
        }

        public int GetLength()
        { return Value.Count; }


        public override int GetHashCode()
        {
            return m_hashValue;
        }

        public override bool Equals(object obj)
        {
            if (this.GetType() == obj.GetType())
            {
                EncodedText other = (EncodedText)obj;

                List<bool> otherValue = other.Value;
                if (Value.Count.Equals(otherValue.Count))
                {
                    for (int i = 0; i < Value.Count; i++)
                    {
                        if (Value[i] != otherValue[i])
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Compute the hashcode once per text, instead of all the time when requested
        /// </summary>
        /// <param name="encodedText"></param>
        /// <returns></returns>
        private static int ComputeHash(List<bool> encodedText)
        {
            int hash = 1;
            foreach (bool b in encodedText)
            {
                hash = 31 * hash + b.GetHashCode();
            }
            return hash;
        }
    }

    /// <summary>
    /// POCO to store an EncodedText and a position or offset value where this text can be found or is to be written.
    /// </summary>
    public class EncodedTextPosition : IComparable<EncodedTextPosition>
    {
        public EncodedText EncodedText { get; }
        public int Position { get; set; } = -1;

        public EncodedTextPosition(EncodedText inEncodedText)
        {
            EncodedText = inEncodedText;
        }

        public int GetLength()
        {
            return EncodedText.GetLength();
        }

        public int CompareTo(EncodedTextPosition other)
        {
            return Position.CompareTo(other.Position);
        }

        public override int GetHashCode()
        {
            return EncodedText.GetHashCode() * 31 + Position;
        }

        public override bool Equals(object obj)
        {
            if (GetType() == obj.GetType())
            {
                EncodedTextPosition other = (EncodedTextPosition)obj;

                return
                    Position.Equals(other.Position)
                    && EncodedText.Equals(other.EncodedText);
            }
            return false;
        }

    }

    public class HuffManConstructionNode : HuffmanNode, IComparable<HuffManConstructionNode>
    {
        public int Occurences { get; set; }

        public new HuffManConstructionNode Left { get; private set; }

        public new HuffManConstructionNode Right { get; private set; }

        public HuffManConstructionNode()
        {
            Occurences = 0;
        }

        public void SetLeftNode(HuffManConstructionNode leftNode)
        {
            base.SetLeftNode(leftNode);
            Left = leftNode;
            Occurences += leftNode.Occurences;
        }

        public void SetRightNode(HuffManConstructionNode rightNode)
        {
            base.SetRightNode(rightNode);
            Right = rightNode;
            Occurences += rightNode.Occurences;
        }

        public int CompareTo(HuffManConstructionNode other)
        {
            int cmp = Occurences.CompareTo(other.Occurences);
            if (cmp == 0)
            {
                cmp = GetDepth().CompareTo(other.GetDepth());
            }
            return cmp;
        }

        public int GetDepth()
        {
            int ld = Left != null ? Left.GetDepth() : 0;
            int rd = Right != null ? Right.GetDepth() : 0;

            return Math.Max(ld, rd);
        }
    }

    public class LocalizedString
    {
        public readonly int DefaultPosition;
        public string Value { get; set; }

        public LocalizedString(int inPosition)
        {
            DefaultPosition = inPosition;
        }

        public LocalizedString(int inPosition, string inText) : this(inPosition)
        {
            Value = inText;
        }

        public override string ToString()
        {
            if (Value != null)
            {
                return Value;
            }
            return this.GetType().Name + "@" + DefaultPosition;
        }
    }

    public class LocalizedStringWithId : LocalizedString
    {
        public readonly uint Id;

        public LocalizedStringWithId(uint inId, int inDefaultPosition) : base(inDefaultPosition)
        {
            this.Id = inId;
        }

        public LocalizedStringWithId(uint inId, int inDefaultPosition, string inText)
            : base(inDefaultPosition, inText)
        {
            this.Id = inId;
        }

        public override string ToString()
        {
            return Id.ToString("X8");
        }
    }

    /// <summary>
    /// This is the return object of the  ResourceUtils.GetEncodedTextsToWrite(...) method.
    /// It contains all the texts to write in the order to write them.
    /// It also contains the set of text ids and their positions for the stringData block to write
    /// And since DA:I is weird it also now contains all the sets of text ids (?) and their positions for each of the declinated adjectives used in crafting.
    /// </summary>
    public class EncodedTextPositionGrouping
    {

        /// <summary>
        /// The ids and encoded texts with positions of all the primarily used texts.
        /// </summary>
        public SortedDictionary<uint, EncodedTextPosition> PrimaryTextIdsAndPositions { get; private set; }

        /// <summary>
        /// The ids and encoded texts with positions of all the declinated adjectives used in DAI crafting
        /// </summary>
        public List<SortedDictionary<uint, EncodedTextPosition>> DeclinatedAdjectivesIdsAndPositions { get; private set; }

        /// <summary>
        /// Just all of the encoded texts with position again.
        /// </summary>
        public SortedSet<EncodedTextPosition> AllEncodedTextPositions { get; private set; }

        public EncodedTextPositionGrouping(
            SortedDictionary<uint, EncodedTextPosition> primaryTextIdsAndPositions,
            List<SortedDictionary<uint, EncodedTextPosition>> declinatedAdjectiveIdsAndPositions,
            SortedSet<EncodedTextPosition> allEncodedTextPositions)
        {
            this.PrimaryTextIdsAndPositions = primaryTextIdsAndPositions;
            this.DeclinatedAdjectivesIdsAndPositions = declinatedAdjectiveIdsAndPositions;
            this.AllEncodedTextPositions = allEncodedTextPositions;
        }
    }

    public class DragonAgeDeclinatedAdjectiveTuples
    {

        public int NumberOfDeclinations { get; private set; }

        public bool ContainsAdjectives => m_declinatedAdjectiveVariants.Count > 0;

        private readonly SortedDictionary<uint, LocalizedStringWithId[]> m_declinatedAdjectiveVariants;

        public DragonAgeDeclinatedAdjectiveTuples(int inNumberOfDeclinations)
        {
            this.NumberOfDeclinations = inNumberOfDeclinations;
            m_declinatedAdjectiveVariants = new SortedDictionary<uint, LocalizedStringWithId[]>();
        }

        public void AddDeclinatedAdjective(LocalizedStringWithId localizedText, int declination)
        {

            uint textId = localizedText.Id;
            if (declination >= NumberOfDeclinations)
            {
                App.Logger.LogError("Cannot Store given declinated adjective with ID <{0}> and declination <{1}> as there are only <{2}> declinations allowed!", textId, declination, NumberOfDeclinations);
                return;
            }

            bool entryExists = m_declinatedAdjectiveVariants.TryGetValue(textId, out LocalizedStringWithId[] declinatedAdjectivesArray);

            if (!entryExists)
            {
                declinatedAdjectivesArray = new LocalizedStringWithId[NumberOfDeclinations];
                m_declinatedAdjectiveVariants.Add(textId, declinatedAdjectivesArray);
            }

            declinatedAdjectivesArray[declination] = localizedText;
        }

        public void AddAllAdjectiveForDeclination(List<LocalizedStringWithId> articlesOfDeclination, int declination)
        {
            foreach (LocalizedStringWithId localizedText in articlesOfDeclination)
            {
                AddDeclinatedAdjective(localizedText, declination);
            }
        }

        public IEnumerable<uint> GetDeclinatedAdjectiveIds()
        {
            return m_declinatedAdjectiveVariants.Keys;
        }

        public IEnumerable<LocalizedString> GetDeclinatedAdjective(uint articleID)
        {

            bool entryExists = m_declinatedAdjectiveVariants.TryGetValue(articleID, out LocalizedStringWithId[] declinatedAdjectivesArray);
            if (entryExists)
            {
                return declinatedAdjectivesArray;
            }

            return new LocalizedString[0];
        }

        public IEnumerable<LocalizedString> GetAllDeclinatedAdjectiveTextLocations()
        {

            List<LocalizedString> allDeclinatedArticles = new List<LocalizedString>();
            foreach (var adjectiveIdArray in m_declinatedAdjectiveVariants.Values)
            {
                foreach (LocalizedString declination in adjectiveIdArray)
                {
                    if (declination != null)
                    {
                        allDeclinatedArticles.Add(declination);
                    }
                }
            }
            return allDeclinatedArticles;

        }

        /// <summary>
        /// Returns the declinated adjectives of the given declination number.
        /// </summary>
        /// <param name="declinationNumber">The declination, must be in the range [0-numberOfDeclinations[</param>
        /// <returns></returns>
        public IEnumerable<LocalizedStringWithId> GetAdjectivesOfDeclination(int declinationNumber)
        {

            if (declinationNumber < 0 || declinationNumber >= NumberOfDeclinations)
            {
                return new LocalizedStringWithId[0];
            }

            List<LocalizedStringWithId> adjectives = new List<LocalizedStringWithId>();
            foreach (var entry in m_declinatedAdjectiveVariants)
            {

                LocalizedStringWithId textEntry = entry.Value[declinationNumber];

                if (textEntry != null)
                {
                    adjectives.Add(textEntry);
                }
            }

            return adjectives;
        }

        /// <summary>
        /// Builds a string representing this instance and all included adjectives.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {

            StringBuilder sb = new StringBuilder();

            foreach (var entry in m_declinatedAdjectiveVariants)
            {
                sb.Append("[")
                    .Append(entry.Key.ToString("X8"))
                    .Append(":");

                bool firstEntry = true;
                foreach (var declination in entry.Value)
                {
                    if (firstEntry)
                    {
                        firstEntry = false;
                    }
                    else
                    {
                        sb.Append(" | ");
                    }
                    if (declination != null)
                    {
                        sb.Append(declination.Value);
                    }
                    else
                    {
                        sb.Append("<null>");
                    }
                }
                sb.Append("] ");
            }


            return sb.ToString();
        }
    }
}
