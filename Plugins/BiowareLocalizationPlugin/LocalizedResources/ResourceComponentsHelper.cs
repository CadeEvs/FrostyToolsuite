using Frosty.Core;
using Frosty.Core.Controls.Editors;
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
        public uint Unknown1 { get; set; }

        // Note: If nodeCount changes due to added Chars, then dataOffset changes!
        // Additional Note: This offset is also part of the metadata, the value in this header is not guaranteed to be correct!
        public uint DataOffset { get; set; }

        // Seems to be an (actually unused?) combination of language and the number of declination together with something else.
        // 0x {Language Number, e.g., 0 for En, 1 for french --> see TypeExplorer: LanguageFormat, this seems to match}
        // followed by  000
        // Last number is 0x4 ( unclear what this means ) + number of declinations to use in this resource.
        // Examples DAI basegame:
        //   En globalmaster:              Language 0 + 1 specified declination  =>     0x5
        //   French globaltranslated:      Language 1 + 8 specified declinations => 0x1000C
        //   French globaltesttranslated:  Language 1 + 9 specified declinations => 0x1000D
        //   Russian globaltranslated:     Language 6 + 6 specified declinations => 0x6000A
        public uint LanguageAndDeclinationsMarker { get; set; }

        // also no idea, can set these to zero and nothing bad happens. This might be a priority of sorts. Seems to be the same for similar resources accross languages.
        public uint Unknown2 { get; set; }

        // absolutely no clue whtat this is, every resource has a different value that doesnt seem to coincide with anything. Setting them to zero or maxvalue does not seem to change anything.
        public uint Unknown3 { get; set; }

        // // nodeCount is an even integer! The rootNode as would-be last node in the node list is *not* actually part of the list
        public uint NodeCount { get; set; }
        public uint NodeOffset { get; set; }
        public uint StringsCount { get; set; }
        public uint StringsOffset { get; set; }

        // If available, this points to the list of item names and the variations to use for them. If not, then the count is zero and the offset is the dataoffset.
        public DataCountAndOffsets ItemNameSetupCountsAndOffsets { get; set; }

        // If available, this points to the map of adjective variations to their relative list position. If not, then the count is zero and the offset is the dataoffset.
        public DataCountAndOffsets AdjectiveDeclinationsCountsAndOffsets { get; set; }

        // These are only available for very few resources, they contain the count and offset for the strings used when crafting items in DA:I
        // This starts at the 3rd of the DataCountAndOffsets, potentially this contains only zeros.
        public List<DataCountAndOffsets> DragonAgeDeclinatedCraftingNamePartsCountAndOffset { get; private set; } = new List<DataCountAndOffsets>();

        // This is *not* part of the actual header?
        // I could set this based on the AdjectiveDeclinationsCountsAndOffsets, but that sometimes includes variations that do not exist at all!
        public int MaxDeclinations { get; private set; } = 0;

        public void AddDragonAgeDeclinatedCraftingNamePart(DataCountAndOffsets coundAndOffset)
        {
            MaxDeclinations++;
            DragonAgeDeclinatedCraftingNamePartsCountAndOffset.Add(coundAndOffset);
        }

        public override string ToString()
        {

            StringBuilder sb = new StringBuilder();
            sb.Append("\n") // newline after resource name
                .AppendLine($"DataOffset is: <{DataOffset} | 0x{DataOffset:X}>")
                .AppendLine($"unknown1: <{Unknown1} | 0x{Unknown1:X}>")
                .AppendLine($"Language & Declinations Marker: <{LanguageAndDeclinationsMarker} | 0x{LanguageAndDeclinationsMarker:X}>")
                .AppendLine($"unknown2: <{Unknown2} | 0x{Unknown2:X}>")
                .AppendLine($"unknown3: <{Unknown3} | 0x{Unknown3:X}>")
                .AppendLine($"NodeCount: <{NodeCount}> starting at <{NodeOffset}>")
                .AppendLine($"StringCount: <{StringsCount}> starting at <{StringsOffset}>");

            if (ItemNameSetupCountsAndOffsets != null && ItemNameSetupCountsAndOffsets.Count > 0)
            {
                sb.AppendLine($"  Additional Item names to variation mapping for {ItemNameSetupCountsAndOffsets.Count} entries starts at {ItemNameSetupCountsAndOffsets.Offset}");
            }
            if (AdjectiveDeclinationsCountsAndOffsets != null && AdjectiveDeclinationsCountsAndOffsets.Count > 0)
            {
                sb.AppendLine($"  Additional mapping for {AdjectiveDeclinationsCountsAndOffsets.Count} declinations starts at {AdjectiveDeclinationsCountsAndOffsets.Offset}");
            }

            foreach (var craftingNamePartCounts in DragonAgeDeclinatedCraftingNamePartsCountAndOffset)
            {
                uint byte8Count = craftingNamePartCounts.Count;
                if (byte8Count > 0)
                {
                    uint totalsize = byte8Count * 8;
                    sb.AppendLine($"  Declinated crafting name parts of {byte8Count} entries, or {totalsize} bytes starts at <{craftingNamePartCounts.Offset}>");
                }
            }

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
                    // 0xFFFF FFF5 -> char U+000A, EOL
                    printLetter = "newLine";
                    break;
                case 4294967172:
                    // 0xFFFF FF84 -> char U+007B, {
                    printLetter = "left curly bracket";
                    break;
                case 4294967170:
                    // 0xFFFF FF82 -> char U+007D, }
                    printLetter = "right curly bracket";
                    break;
                default:
                    printLetter = Letter.ToString();
                    break;
            }

            return string.Format("[Value = <{0} | 0x{1}> | LetterValue = <0x{2}> Letter = <{3}>]", Value.ToString(), Value.ToString("X"), ((uint)Letter).ToString("X"), printLetter);
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

        private List<bool> NodeEncoding = null;

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

        /// <summary>
        /// Returns the encoding for this node, storing it for later requests.
        /// Kind of stole the idea from the LEX implementation:
        /// https://github.com/ME3Tweaks/LegendaryExplorer/blob/Beta/LegendaryExplorer/LegendaryExplorerCore/TLK/ME2ME3/HuffmanCompression.cs
        /// </summary>
        /// <returns></returns>
        public List<bool> GetNodeEncoding()
        {
            if (NodeEncoding == null)
            {
                NodeEncoding = new List<bool>();
                NodeEncoding.AddRange(ResourceUtils.GetCharEncoding(Parent));
                NodeEncoding.Add(ResourceUtils.GetBoolValueFromParent(this));
            }
            return NodeEncoding;
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
            return this.GetType().Name + " @ " + DefaultPosition;
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
            return Id.ToString("X8") + " @ " + DefaultPosition;
        }
    }

    // Only used when verification is enabled.
    public class DAILocalizedAdjective : LocalizedStringWithId
    {
        public readonly int Declination;

        public DAILocalizedAdjective(uint inId, int inDefaultPosition, int inDeclination) : base(inId, inDefaultPosition)
        {
            this.Declination = inDeclination;
        }

        public override string ToString()
        {
            return string.Format("adjective <{0}> of declination {1}", base.ToString(), Declination);
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
        public SortedDictionary<TextID, EncodedTextPosition> PrimaryTextIdsAndPositions { get; private set; }

        /// <summary>
        /// The ids and encoded texts with positions of all the declinated adjectives used in DAI crafting
        /// </summary>
        public List<SortedDictionary<TextID, EncodedTextPosition>> DeclinatedAdjectivesIdsAndPositions { get; private set; }

        /// <summary>
        /// The byte array of the encoded texts
        /// </summary>
        public byte[] TextBytes { get; private set; }

        public EncodedTextPositionGrouping(
            SortedDictionary<TextID, EncodedTextPosition> inPrimaryTextIdsAndPositions,
            List<SortedDictionary<TextID, EncodedTextPosition>> inDeclinatedAdjectiveIdsAndPositions,
            byte[] InTextBytes)
        {
            this.PrimaryTextIdsAndPositions = inPrimaryTextIdsAndPositions;
            this.DeclinatedAdjectivesIdsAndPositions = inDeclinatedAdjectiveIdsAndPositions;
            this.TextBytes = InTextBytes;
        }
    }

    /// <summary>
    /// Text id used for sorting when writing the texts.
    /// </summary>
    public class TextID : IComparable<TextID>
    {

        /// <summary>
        /// Mask to get only the first byte of the ID, which carries some meta information.
        /// For primary texts, an 8 at this position indicates that the text is a variation used with female protagonists.
        /// For the crafting item names there are more variants. At least 2,4,C and E. I'm fairly certain that 2 and 4 come before the noun, while C and E after. This is
        /// </summary>
        public static readonly uint VARIANT_MASK = 0xF0000000;

        // the inverse of the variant mask
        public static readonly uint NON_VARIANT_MASK = 0x0FFFFFFF;

        /// <summary>
        /// The actual id value.
        /// </summary>
        public uint Id { get; private set; }

        /// <summary>
        /// The id of the text without the highest 4 bit variant marker.
        /// </summary>
        public readonly uint m_nonVariantId;

        /// <summary>
        /// The variant of this text id.
        /// </summary>
        public readonly uint m_variantValue;

        public TextID(uint id)
        {
            Id = id;
            m_nonVariantId = id & NON_VARIANT_MASK;
            m_variantValue = id & VARIANT_MASK;
        }

        /// <summary>
        /// Indicates that the text this id belongs to is a ( female player character ) variant of another text.
        /// This id is the same as the id of the ( male character ) original text with added 0x80000000.
        /// For crafting adjective the variant value might be 0x20000000, 0x40000000, 0xC0000000, or 0xE0000000. Probably indicating wether the adjective comes before or after the noun.
        /// In the resources, these are ordered immediately after their non variant texts in the text position list.
        /// </summary>
        public bool IsVariant()
        {
            return m_variantValue > 0;
        }

        public int CompareTo(TextID other)
        {
            if (!IsVariant() && !other.IsVariant())
            {
                return Id.CompareTo(other.Id);
            }

            int nonVariantCompare = m_nonVariantId.CompareTo(other.m_nonVariantId);
            if (nonVariantCompare != 0)
            {
                return nonVariantCompare;
            }
            return m_variantValue.CompareTo(other.m_variantValue);
        }

        public override string ToString()
        {
            return Id.ToString("X8");
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var other = obj as TextID;
            return Id.Equals(other?.Id);
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
