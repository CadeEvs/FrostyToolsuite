using Frosty.Core;
using FrostySdk;
using FrostySdk.IO;
using FrostySdk.Managers;
using FrostySdk.Resources;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System;
using System.Collections;

namespace BiowareLocalizationPlugin.LocalizedResources
{

    public class LocalizedStringResource : Resource
    {

        /// <summary>
        /// Toggle to enable / disable further debug log messages -Remember to turn this off before release!
        /// </summary>
        private static readonly bool PrintVerificationTexts = false;

        /// <summary>
        /// Possible variants of where to position the reader in the case that the stated data offset does not match the data in the resource.
        /// None of these actually work, in the case of discrepancy you are screwed anyway :(
        /// </summary>
        private enum PositionOffsetErrorHandling
        {
            /// <summary>
            /// Use the current reader position going forward
            /// </summary>
            READER_POSITON,

            /// <summary>
            /// Set reader to the position stated in the header
            /// </summary>
            HEADER_DATAOFFSET,

            /// <summary>
            /// Set the reader to the position stated in the metadata
            /// </summary>
            METADATA_DATAOFFSET
        }

        /// <summary>
        /// How to handle incorrect metadata offsets in the resource header.
        /// </summary>
        private static readonly PositionOffsetErrorHandling ContinueAfterOffsetErrorVariant = PositionOffsetErrorHandling.HEADER_DATAOFFSET;

        /// <summary>
        /// The default texts
        /// </summary>
        private readonly List<LocalizedStringWithId> _localizedStrings = new List<LocalizedStringWithId>();

        /// <summary>
        /// List of supported characters, ordered by their position within the node list, i.e., their frequency within all texts
        /// </summary>
        private List<char> _supportedCharacters = new List<char>();

        /// <summary>
        /// Lists all the string ids that are stored at the key position
        /// </summary>
        private readonly Dictionary<int, List<uint>> _stringIdsAtPositionOffset = new Dictionary<int, List<uint>>();

        /// <summary>
        /// If any text is altered, the altered text entry will be kept in the modfiedResource.
        /// </summary>
        private ModifiedLocalizationResource _modifiedResource = null;

        /// <summary>
        /// The header information of the localization resource.
        /// </summary>
        private ResourceHeader _headerData;

        /// <summary>
        /// The default encoding's root node. Note that the rootNode itself is not part of the serialized data!
        /// </summary>
        private HuffmanNode _encodingRootNode;

        /// <summary>
        /// Byte array of currently unknown data packed between the header list of string positions, and the actual text entries.
        /// </summary>
        private List<byte[]> _unknownData;

        /// <summary>
        /// Ids and texts of Declinated adjectives for creafted items in DA:I
        /// This has internal access only for the test utils
        /// </summary>
        internal DragonAgeDeclinatedAdjectiveTuples DragonAgeDeclinatedCraftingNames { get; private set; }


        /// <summary>
        /// The (display) name of the resource this belongs to
        /// </summary>
        public string Name { get; private set; } = "not_yet_initialized";

        /// <summary>
        /// Event handler to be informed whenever the state of the modified resource changes drastically.
        /// </summary>
        public event EventHandler ResourceEventHandlers;

        // TODO this currently stores a lot of redundant information, clean up at a later stage!

        public LocalizedStringResource()
        {
            // nothing to do?!
        }

        public override void Read(NativeReader reader, AssetManager am, ResAssetEntry entry, ModifiedResource modifiedData)
        {

            // Profile Version MEA = 20170321
            // Profile Version DAI = 20141118

            Name = new StringBuilder(entry.Filename)
                .Append(" - ")
                .Append(entry.Name)
                .ToString();

            base.Read(reader, am, entry, modifiedData);

            if (ProfilesLibrary.DataVersion == (int)ProfileVersion.Anthem)
            {
                ReadAnthemStrings(reader, entry);
            }
            else
            {
                Read_MassEffect_DragonAge_Strings(reader);
            }

            _modifiedResource = modifiedData as ModifiedLocalizationResource;
            if(_modifiedResource != null)
            {
                _modifiedResource.InitResourceId(resRid);
            }

            // keep informed about changes...
            entry.AssetModified += (s, e) => OnModified( (ResAssetEntry)s );
        }

        private void OnModified(ResAssetEntry assetEntry)
        {
            // There is an unhandled edge case here:
            // When a resource is completely replaced by anoher one in a mod, then this method will not pick that up!

            ModifiedAssetEntry modifiedAsset = assetEntry.ModifiedEntry;
            ModifiedLocalizationResource newModifiedResource = modifiedAsset?.DataObject as ModifiedLocalizationResource;

            if(PrintVerificationTexts)
            {
                App.Logger.Log("Asset <{0}> entered onModified", assetEntry.DisplayName);
            }

            if(newModifiedResource != _modifiedResource)
            {
                _modifiedResource = newModifiedResource;
                ResourceEventHandlers?.Invoke(this, new EventArgs());
            }

            // revert the metadata just in case
            ReplaceMetaData(_headerData.DataOffset);
        }

        /// <summary>
        /// Fills the String list for Anthem.
        /// </summary>
        /// <param name="reader">the data reader.</param>
        /// <param name="entry">the res asset.</param>
        private void ReadAnthemStrings(NativeReader reader, ResAssetEntry entry)
        {
            _ = reader.ReadUInt();
            _ = reader.ReadUInt();
            _ = reader.ReadUInt();

            // initialize these, so there is no accidental crash in anthem
            _headerData = new ResourceHeader();
            _unknownData = new List<byte[]>();
            DragonAgeDeclinatedCraftingNames = new DragonAgeDeclinatedAdjectiveTuples(0);

            long numStrings = reader.ReadLong();
            reader.Position += 0x18;

            Dictionary<uint, List<uint>> hashToStringIdMapping = new Dictionary<uint, List<uint>>();

            for (int i = 0; i < numStrings; i++)
            {
                uint hash = reader.ReadUInt();
                uint stringId = reader.ReadUInt();
                reader.Position += 8;
                if (!hashToStringIdMapping.ContainsKey(hash))
                    hashToStringIdMapping.Add(hash, new List<uint>());
                hashToStringIdMapping[hash].Add(stringId);
            }

            reader.Position += 0x18;

            while (reader.Position < reader.Length)
            {
                uint hash = reader.ReadUInt();
                int stringLen = reader.ReadInt();
                string str = reader.ReadSizedString(stringLen);
                int stringPosition = (int)reader.Position; // anthem is not really supported anyways...

                if (hashToStringIdMapping.ContainsKey(hash))
                {
                    foreach (uint stringId in hashToStringIdMapping[hash])
                        _localizedStrings.Add(new LocalizedStringWithId(stringId, stringPosition, str ));
                }
                else
                {
                    App.Logger.Log("Cannot find {0} in {1}", hash.ToString("x8"), entry.Name);
                }
            }
        }

        /// <summary>
        /// Creates the localized string list from the huffman encoded Mass Effect and Dragon Age bundle entries.
        /// </summary>
        /// <param name="reader"></param>
        private void Read_MassEffect_DragonAge_Strings(NativeReader reader)
        {

            _headerData = ResourceUtils.ReadHeader(reader);

            if(PrintVerificationTexts)
            {
                App.Logger.Log("Read header data for <{0}>: {1}", Name, _headerData.ToString());
            }

            // position of huffman nodes is header.nodeOffset
            PositionSanityCheck(reader, _headerData.NodeOffset, "Header");
            _encodingRootNode = ResourceUtils.ReadNodes(reader, _headerData.NodeCount, out List<char> leafCharacters);
            _supportedCharacters = leafCharacters;

            // position of string id and position is right after huffman nodes: header.stringsOffset
            PositionSanityCheck(reader, _headerData.StringsOffset, "HuffmanCoding");
            ReadStringData(reader, _headerData.StringsCount);

            // position after string data is the start of header.unknownDataDef[0].offset
            PositionSanityCheck(reader, _headerData.FirstUnknownDataDefSegments[0].Offset, "StringData");
            _unknownData = new List<byte[]>();
            foreach (DataCountAndOffsets dataCountAndOffset in _headerData.FirstUnknownDataDefSegments)
            {
                _unknownData.Add(ResourceUtils.ReadUnkownSegment(reader, dataCountAndOffset));
            }

            DragonAgeDeclinatedCraftingNames = new DragonAgeDeclinatedAdjectiveTuples(_headerData.MaxDeclinations);

            for(int i = 0; i< _headerData.DragonAgeDeclinatedCraftingNamePartsCountAndOffset.Count; i++ )
            {
                DataCountAndOffsets dataCountAndOffset = _headerData.DragonAgeDeclinatedCraftingNamePartsCountAndOffset[i];

                List<LocalizedStringWithId> declinatedAdjectives = ReadDragonAgeDeclinatedItemNamePartIdsAndOffsets(reader, dataCountAndOffset);

                DragonAgeDeclinatedCraftingNames.AddAllAdjectiveForDeclination(declinatedAdjectives, i);
            }

            DataOffsetReaderPositionSanityCheck(reader);


            ReadStrings(reader, _encodingRootNode, GetAllLocalizedStrings());
        }

        /// <summary>
        /// Returns the list of all LocalizedString entries found in this resource.
        /// This list is being comprised of the main texts with unique ids,
        /// and the set of declinated adjective strings used in dragon age, which all share the same id for several declinations of a word.
        /// </summary>
        /// <returns></returns>
        private List<LocalizedString> GetAllLocalizedStrings()
        {
            List<LocalizedString> allLocalizedStrings = new List<LocalizedString>(_localizedStrings);

            allLocalizedStrings.AddRange(DragonAgeDeclinatedCraftingNames.GetAllDeclinatedAdjectiveTextLocations());

            return allLocalizedStrings;
        }

        /// <summary>
        /// Returns a list of tuples with the id and bit offset for declinated adjectives used when crafting items in DA:I
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="countAndOffset"></param>
        /// <param name="craftingTextBlockId"></param>
        /// <returns></returns>
        private static List<LocalizedStringWithId> ReadDragonAgeDeclinatedItemNamePartIdsAndOffsets(NativeReader reader, DataCountAndOffsets countAndOffset)
        {

            List<LocalizedStringWithId > itemCraftingNameParts= new List<LocalizedStringWithId>();
            for (int i = 0; i < countAndOffset.Count; i++)
            {
                uint textId = reader.ReadUInt();
                int defaultPosition = reader.ReadInt();
                LocalizedStringWithId namePartInfo = new LocalizedStringWithId(textId, defaultPosition);
                itemCraftingNameParts.Add(namePartInfo);
            }

            if(PrintVerificationTexts && countAndOffset.Count > 0)
            {
                App.Logger.Log("... Read <{0}> declinated adjectives in a block", countAndOffset.Count);
            }

            return itemCraftingNameParts;
        }

        /// <summary>
        /// Sets the reader to the expected position, if the current position is somewhere else
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="expectedPosition"></param>
        /// <param name="positionName">the name of the position as used in the warning message</param>
        private void PositionSanityCheck(NativeReader reader, uint expectedPosition, string positionName)
        {
            if(reader.Position != expectedPosition)
            {
                App.Logger.LogWarning("Reader for resource <{0}> is at the wrong position after {1}!", Name, positionName);
            }
        }

        /// <summary>
        /// Reads the tuples of string id and string bit offset from the dataOffset.
        /// I.e., it fills the given int array with the offset for the string id, creates an empty _strings entry for the string id
        /// and also creates a dictionary with the information read from the list.
        /// 
        /// Note that several String Ids may point towards the same string position!
        /// Also note that the strings seem to be ordered by their ID, smallest to largest
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="stringsCount"></param>
        private void ReadStringData(NativeReader reader, uint stringsCount)
        {
            for (int i = 0; i < stringsCount; i++)
            {
                uint stringId = reader.ReadUInt();
                int positionOffset = reader.ReadInt();

                _localizedStrings.Add(new LocalizedStringWithId(stringId, positionOffset ));

                // memorize which ids are all stored at the same position
                List<uint> idList;
                if ( !_stringIdsAtPositionOffset.ContainsKey(positionOffset))
                {
                    idList = new List<uint>();
                    _stringIdsAtPositionOffset.Add(positionOffset, idList);
                }
                else
                {
                    idList = _stringIdsAtPositionOffset[positionOffset];
                }
                idList.Add(stringId);

            }
        }

        /// <summary>
        /// Checks that the current position matches the offset as given in the header or at least the metadata.
        /// If it does not match, the given reader's position is updated to the value in the metadata.
        /// </summary>
        /// <param name="reader"></param>
        private void DataOffsetReaderPositionSanityCheck(NativeReader reader)
        {

            uint currentPosition = (uint) reader.Position;
            uint dataOffsetFromHeader = _headerData.DataOffset;
            if (currentPosition != dataOffsetFromHeader)
            {

                uint dataOffsetFromMeta = BitConverter.ToUInt32(resMeta, 0);
                if(currentPosition == dataOffsetFromMeta)
                {
                    App.Logger.LogWarning("Header data for for resource <{0}> is incorrect. 8ByteBlockData is stated to end at <{1}>, instead current reader position is <{2}>, as stated in the metadata!", Name, _headerData.DataOffset, currentPosition);
                    return;
                }

                string expectedOffsetInsert = (dataOffsetFromHeader == dataOffsetFromMeta) ?
                    dataOffsetFromHeader.ToString() :
                    string.Format("{0} or {1}", dataOffsetFromHeader, dataOffsetFromMeta);

                uint newPosition;
                string continueWithOffsetText;
                switch (ContinueAfterOffsetErrorVariant)
                {
                    case PositionOffsetErrorHandling.READER_POSITON:
                        newPosition = currentPosition;
                        continueWithOffsetText = "Continuing with current position";
                        break;
                    case PositionOffsetErrorHandling.HEADER_DATAOFFSET:
                        newPosition = dataOffsetFromHeader;
                        continueWithOffsetText = string.Format("Continuing with stated position from header: <{0}>", newPosition);
                        break;
                    case PositionOffsetErrorHandling.METADATA_DATAOFFSET:
                        newPosition = dataOffsetFromMeta;
                        continueWithOffsetText = string.Format("Continuing with stated position from MetaData: <{0}>", newPosition);
                        break;
                    default:
                        throw new ArgumentException("Unknown PositionOffsetErrorHandling variant " + ContinueAfterOffsetErrorVariant);
                }

                App.Logger.LogWarning("Expected 8ByteBlockData DataSegment for <{0}> to end at <{1}>, instead current reader position is <{2}>! {3}",
                    Name, expectedOffsetInsert, currentPosition, continueWithOffsetText);

                reader.Position = newPosition;
            }
        }

        /// <summary>
        /// Reads the actual texts as sequence of huffman encoded characters.
        /// The texts are read at the positions given in the list of LocalizedString, updating each of the entries afterwards.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="rootNode"></param>
        /// <param name="allLocalizedStrings"></param>
        private void ReadStrings(NativeReader reader, HuffmanNode rootNode, List<LocalizedString> allLocalizedStrings)
        {
            byte[] values = reader.ReadBytes((int)(reader.Length - reader.Position));
            int textLengthInBytes = values.Length;

            using (BitReader bitReader = new BitReader(new MemoryStream(values)))
            {

                foreach(LocalizedString stringDefinition in allLocalizedStrings)
                {
                    int bitOffset = stringDefinition.DefaultPosition;
                    string textName = stringDefinition.ToString();

                    bool sanitiyCheckSuccess = CheckPositionExists(textLengthInBytes, bitOffset, stringDefinition.ToString());
                    if(sanitiyCheckSuccess)
                    {
                        bitReader.SetPosition(bitOffset);
                        stringDefinition.Value = ReadSingleText(bitReader, rootNode);

                    } else
                    {
                        // mark that the text could not be read - a generic warning might be bad, because it conflates all the different texts!
                        string dummy = string.Format("Text <{0}> could not be read!", stringDefinition.ToString());
                        stringDefinition.Value = dummy;
                    }
                }
            }
        }

        /// <summary>
        /// Reads a single text from the given bit reader's current position
        /// </summary>
        /// <param name="bitReader"></param>
        /// <param name="rootNode"></param>
        /// <returns>text</returns>
        private static string ReadSingleText(BitReader bitReader, HuffmanNode rootNode)
        {
            StringBuilder sb = new StringBuilder();
            while (true)
            {
                HuffmanNode n = rootNode;
                while (n.Left != null && n.Right != null && !bitReader.EndOfStream)
                {
                    bool b = bitReader.GetBit();
                    if (b) n = n.Right;
                    else n = n.Left;
                }

                if (n.Letter == 0x00 || bitReader.EndOfStream)
                {
                    return sb.ToString();
                }
                // else
                sb.Append(n.Letter);
            }
        }

        /// <summary>
        /// Sanity Check for Dragon age position offsets.
        /// There can be instance where the position appears as negative (int), though i don't yet know if the uint position is also outside the array
        /// </summary>
        /// <param name="textLengthInBytes"></param>
        /// <param name="bitPosition"></param>
        /// <param name="textName"></param>
        /// <returns>true if the position is ok</returns>
        private bool CheckPositionExists(int textLengthInBytes, int bitPosition, string textName)
        {

            int bytePosition = (bitPosition >> 5) * 4;
            if( (bytePosition >= 0)
                && (bytePosition < textLengthInBytes))
            {
                return true;
            }

            App.Logger.LogError(
                "Could not read text <{0}> in resource <{1}>! The stated position <Bit: {2} or Byte: {3}> is outside the data array of byte length <{4}>!",
                textName, Name, bitPosition, bytePosition, textLengthInBytes);

            return false;
        }

        /// <summary>
        /// Returns all the characters supported by this resource.
        /// </summary>
        /// <returns>List of chars.</returns>
        public IEnumerable<char> GetSupportedCharacters()
        {
            return _supportedCharacters;
        }

        /// <summary>
        /// Iterates over all (primary) texts in this resource and returns them in the tuple as follows:
        /// Tuple#0: text id (uint)
        /// Tuple#1: text (string)
        /// Tuple#2: whether the text is modified or default (bool). True if modified.
        /// The same text can be encountered twice in this enumeration, first in its vanilla state, and then the modified version.
        /// Note: This method does *NOT* return the texts referenced in the 8ByteBlockData between the StringData and the Strings parts of the resource.
        /// </summary>
        /// <returns>An enumerable (stream) of the primary text entries</returns>
        public IEnumerable<Tuple<uint, string, bool>> GetAllPrimaryTexts()
        {
            for (int i = 0; i < _localizedStrings.Count; i++)
            {
                yield return new Tuple<uint, string, bool>(_localizedStrings[i].Id, _localizedStrings[i].Value, false);
            }

            if(_modifiedResource != null)
            {
                foreach ( KeyValuePair<uint, string> modifiedEntry in _modifiedResource.AlteredTexts)
                {
                    yield return new Tuple<uint, string, bool>(modifiedEntry.Key, modifiedEntry.Value, true);
                }
            }
        }

        /// <summary>
        /// Returns the list of all other vanilla string ids (as hex representation) that share the same text and position.
        /// To Reiterate: This only lists vanilla unmodified strings and positions!
        /// </summary>
        /// <param name="textId"></param>
        /// <returns>Enumeration of hexadecimal string ids</returns>
        public IEnumerable<string> GetAllTextIdsAtPositionOf(uint textId)
        {

            LocalizedStringWithId textEntry = null;
            foreach (LocalizedStringWithId searchTextEntry in _localizedStrings)
            {
                if(searchTextEntry.Id == textId)
                {
                    textEntry = searchTextEntry;
                    break;
                }
            }

            if(textEntry != null && (textEntry.DefaultPosition >=0))
            {
                bool valuePresent = _stringIdsAtPositionOffset.TryGetValue(textEntry.DefaultPosition, out List<uint> allStringIds);
                if(valuePresent)
                {
                    return GetAllStringsIdsAsHex(allStringIds);
                }
            }
            return Enumerable.Empty<string>();
        }

        private static IEnumerable<string> GetAllStringsIdsAsHex(List<uint> allStringIds)
        {
            foreach (uint stringId in allStringIds)
            {
                yield return stringId.ToString("X8");
            }
        }

        // This is only here for test purposes!
        public HuffmanNode GetRootNode()
        {
            return _encodingRootNode;
        }

        public override byte[] SaveBytes()
        {

            // remove these logs
            if (PrintVerificationTexts) { App.Logger.Log("Writing Texts for <{0}>", Name); }

            /*Plan of Action:
             * -recalculate Huffman encoding
             *  -- getHuffman codes for each character
             * -recompute list of string ids and string positions with altered texts
             *  -- need to already encoded strings for the position calculation!
             * -recompute other offsets based on the last step info
             * -replace the meta data with the new dataOffset (!)
             * -write header with altered offsets
             * -write stuff and things between header and strings
             * -write strings with the new huffman encoding
             */

            List<SortedDictionary<uint, string>> allTexts =GetAllSortedTextsToWrite();
            HuffmanNode newRootNode = GetEncodingRootNode(allTexts);

            uint nodeOffset = _headerData.NodeOffset;

            // flatten the tree, we need to list representation again...
            List<HuffmanNode> nodeList = ResourceUtils.GetNodeListToWrite(newRootNode);
            uint newNodeCount = (uint)nodeList.Count;

            uint encodingNodesSize = newNodeCount * 4;
            uint newStringsOffset = nodeOffset + encodingNodesSize;

            Dictionary<char, List<bool>> encoding = ResourceUtils.GetCharEncoding(nodeList);
            EncodedTextPositionGrouping encodedTextsGrouping = ResourceUtils.GetEncodedTextsToWrite(allTexts, encoding);

            uint newStringsCount = (uint)encodedTextsGrouping.PrimaryTextIdsAndPositions.Count;

            uint blockOffset = newStringsOffset + (newStringsCount*8);
            uint lastBlockSize = 0;
            List<DataCountAndOffsets> recalculatedAdditionalOffsets = new List<DataCountAndOffsets>();

            // one for the money: No idea what this is
            foreach (DataCountAndOffsets unknownDef in _headerData.FirstUnknownDataDefSegments)
            {
                uint byteBlockCount8 = unknownDef.Count;
                blockOffset += lastBlockSize;
                recalculatedAdditionalOffsets.Add(new DataCountAndOffsets()
                {
                    Count = byteBlockCount8,
                    Offset = blockOffset

                });

                lastBlockSize = byteBlockCount8 * 8;
            }

            // two for the show: These are the ids and positions of the declinated adjectives used in DA:I crafting
            foreach(var declinatedAdjectivesBlock in encodedTextsGrouping.DeclinatedAdjectivesIdsAndPositions)
            {
                uint byteBlockCount8 = (uint) declinatedAdjectivesBlock.Count;
                blockOffset += lastBlockSize;
                recalculatedAdditionalOffsets.Add(new DataCountAndOffsets()
                {
                    Count = byteBlockCount8,
                    Offset = blockOffset

                });
                lastBlockSize = byteBlockCount8 * 8;
            }

            uint newDataOffset = blockOffset + lastBlockSize;
            // replace the metadata which is where the game actually reads the dataoffset from.
            ReplaceMetaData(newDataOffset);

            using (NativeWriter writer = new NativeWriter(new MemoryStream()))
            {

                // Then write the type dependent header data
                writer.Write(ResourceHeader.Magic);

                writer.Write(_headerData.Unknown1);

                writer.Write(newDataOffset);

                writer.Write(_headerData.Unknown2);
                writer.Write(_headerData.Unknown3);
                writer.Write(_headerData.Unknown4);

                writer.Write(newNodeCount);
                writer.Write(nodeOffset);
                writer.Write(newStringsCount);
                writer.Write(newStringsOffset);

                foreach( DataCountAndOffsets uds in recalculatedAdditionalOffsets)
                {
                    writer.Write(uds.Count);
                    writer.Write(uds.Offset);
                }

                if (PrintVerificationTexts)
                    { App.Logger.Log(".. Writer Position before <{0}> nodes is <{1}>, expected <{2}> ", nodeList.Count, writer.Position, nodeOffset); }

                // Write huffman nodes
                foreach (HuffmanNode node in nodeList)
                {
                    writer.Write(node.Value);
                }

                long actualStringsOffset = writer.Position;
                if (PrintVerificationTexts)
                    { App.Logger.Log(".. Writer Position before textlocations is <{0}>, expected <{1}> ", writer.Position, newStringsOffset); }

                //Write string id positions
                foreach (KeyValuePair<uint, EncodedTextPosition> entry in encodedTextsGrouping.PrimaryTextIdsAndPositions)
                {
                    writer.Write(entry.Key);
                    writer.Write(entry.Value.Position);
                }

                if (PrintVerificationTexts)
                {
                    App.Logger.Log(".. Writer Position after <{0}> textlocations is <{1}>, expected <{2}>. Length of last part was <{3}>",
                        encodedTextsGrouping.PrimaryTextIdsAndPositions.Count, writer.Position, recalculatedAdditionalOffsets[0].Offset, writer.Position - actualStringsOffset);
                }

                //Write unknownDataSegments
                foreach (byte[] someData in _unknownData)
                {
                    writer.Write(someData);
                }

                // write the ids and positions of the declinated adjectives.
                foreach( var declinationBlock in encodedTextsGrouping.DeclinatedAdjectivesIdsAndPositions)
                {
                    foreach (KeyValuePair<uint, EncodedTextPosition> entry in declinationBlock)
                    {
                        writer.Write(entry.Key);
                        writer.Write(entry.Value.Position);
                    }
                }

                if (PrintVerificationTexts)
                    { App.Logger.Log(".. Writer Position before texts is <{0}>, expected <{1}>", writer.Position, newDataOffset); }

                // Write encoded texts
                byte[] bitTexts = ResourceUtils.GetTextRepresentationToWrite(encodedTextsGrouping.AllEncodedTextPositions);
                writer.Write(bitTexts);

                if (PrintVerificationTexts)
                {
                    App.Logger.Log(".. Writer Position after encoded texts is <{0}>. EncodedTexts size was <{1}> byte", writer.Position, bitTexts.Length);
                }

                return writer.ToByteArray();
            }
        }

        /// <summary>
        /// Returns the sorted dictionary of texts by their id, as they should be written into the resource.
        /// Each of the dictionarys sorted by id is in turn found in a list based on where the text ids originate from,
        /// i.e., the primary text id definition, or one of the blocks for declinated crafting adjective name-parts.
        /// </summary>
        /// <returns>the texts sorted by their id</returns>
        private List<SortedDictionary<uint, string>> GetAllSortedTextsToWrite()
        {

            // contains the texts string sorted by their id, with position in the list by their id encountere in the resource block
            List<SortedDictionary<uint, string>> allTextsToWrite = new List<SortedDictionary<uint, string>>();

            SortedDictionary<uint, string> primaryTextsToWrite = new SortedDictionary<uint, string>();
            allTextsToWrite.Add(primaryTextsToWrite);

            foreach (var entry in GetAllPrimaryTexts())
            {
                primaryTextsToWrite[entry.Item1] = entry.Item2;
            }

            if(PrintVerificationTexts)
            {
                App.Logger.Log("..Preparing to write resource <{0}>. Added <{1}> primary texts.", Name, primaryTextsToWrite.Count);
            }

            for(int declination = 0; declination < DragonAgeDeclinatedCraftingNames.NumberOfDeclinations; declination++)
            {
                SortedDictionary<uint, string> declinatedTextsToWrite = new SortedDictionary<uint, string>();
                allTextsToWrite.Add(declinatedTextsToWrite);

                foreach (LocalizedStringWithId declinatedString in DragonAgeDeclinatedCraftingNames.GetAdjectivesOfDeclination(declination))
                {
                    declinatedTextsToWrite[declinatedString.Id] = declinatedString.Value;
                }
            }

            if (PrintVerificationTexts)
            {
                PrintDeclinatedAdjectivesWritingVerifications(allTextsToWrite);
            }


            return allTextsToWrite;
        }

        /// <summary>
        /// Prints the verification for writing declinated adjectives.
        /// </summary>
        /// <param name="allTextsToWrite"></param>
        private static void PrintDeclinatedAdjectivesWritingVerifications(List<SortedDictionary<uint, string>> allTextsToWrite)
        {
            if (allTextsToWrite.Count > 1)
            {
                int min = int.MaxValue;
                int max = 0;

                for (int groupId = 1; groupId < allTextsToWrite.Count; groupId++)
                {
                    var group = allTextsToWrite[groupId];
                    int groupSize = group.Count;
                    min = min > groupSize ? groupSize : min;
                    max = max < groupSize ? groupSize : max;
                }

                string groupSizeText;
                if (min == max)
                {
                    groupSizeText = $"of {max} number of text ids";
                }
                else
                {
                    groupSizeText = $"of beween {min} and {max} number of texts";
                }

                App.Logger.Log("... Added <{0}> groups of declinated crafting adjectives {1}.", allTextsToWrite.Count - 1, groupSizeText);
            }
        }

        /// <summary>
        /// Returns the encoding originally used, if all characters are included. Otherwise recalculates a new encoding. 
        /// </summary>
        /// <param name="allSortedTexts">The enumeration of all texts and their id</param>
        /// <returns>The root huffman node for the encoding</returns>
        private HuffmanNode GetEncodingRootNode(List<SortedDictionary<uint, string>> allSortedTexts)
        {

            if(_modifiedResource == null)
            {
                return _encodingRootNode;
            }

            // compare added texts chars to allowed chars, if new ones, recalculate encoding
            IEnumerable<string> alteredTexts = _modifiedResource.AlteredTexts.Values;
            bool includesOnlySupported = ResourceUtils.IncludesOnlySupportedCharacters(alteredTexts, _supportedCharacters);

            if (includesOnlySupported)
            {
                return _encodingRootNode;
            }

            if (PrintVerificationTexts)
            {
                App.Logger.Log("Recalculating encoding for resource: <{0}>", Name);
            }

            var allTexts = new HashSet<string>();
            foreach(var entry in allSortedTexts)
            {
                allTexts.UnionWith(entry.Values);
            }

            return ResourceUtils.CalculateHuffmanEncoding(allTexts);
        }

        /// <summary>
        /// Replaces the current metadata with newly computed ones with the correct data offset
        /// </summary>
        /// <param name="newDataOffset"></param>
        private void ReplaceMetaData(uint newDataOffset)
        {
            using (NativeWriter metaDataWriter = new NativeWriter(new MemoryStream(16) ) )
            {
                metaDataWriter.Write(newDataOffset);
                metaDataWriter.Write(0);
                metaDataWriter.Write(0);
                metaDataWriter.Write(0);

                resMeta = metaDataWriter.ToByteArray();
            }
        }

        public override ModifiedResource SaveModifiedResource()
        {
            return _modifiedResource;
        }

        /// <summary>
        /// Adds or edits the text of the given id.
        /// </summary>
        /// <param name="textId"></param>
        /// <param name="text"></param>
        public void SetText(uint textId, string text)
        {

            // Try to revert if text equals original
            // -> drawback is long iteration over all texts or another huge instance of textid to text dictionary :(

            // have to try anyway as long as no dedicated remove is present..
            foreach(var entry in _localizedStrings)
            {
                if(textId == entry.Id)
                {
                    // found the right one
                    // neither the entryValue nor the given text can be null
                    if (entry.Value.Equals(text))
                    {
                        // It is the original text, remove instead
                        RemoveText(textId);
                        return;
                    }
                    break;
                }
            }

            SetText0(textId, text);
        }

        private void SetText0(uint textId, string text)
        {
            if (_modifiedResource == null)
            {
                _modifiedResource = new ModifiedLocalizationResource();
                _modifiedResource.InitResourceId(resRid);

                App.AssetManager.ModifyRes(resRid, this);
            }
            _modifiedResource.SetText(textId, text);
        }

        public void RemoveText(uint textId)
        {
            if(_modifiedResource != null)
            {
                _modifiedResource.RemoveText(textId);

                if(_modifiedResource.AlteredTexts.Count == 0)
                {
                    // remove this resource, it isn't needed anymore
                    // This is also done via the listener, but whatever
                    _modifiedResource = null;

                    AssetManager assetManager = App.AssetManager;
                    ResAssetEntry entry = assetManager.GetResEntry(resRid);
                    App.AssetManager.RevertAsset(entry);
                }
            }
        }

        public string GetDefaultText(uint textId)
        {
            // FIXME hopefully this isn't used often...
            foreach (var entry in _localizedStrings)
            {
                if (textId == entry.Id)
                {
                    return entry.Value;
                }
            }
            return null;
        }

        public IEnumerable<uint> GetAllModifiedTextsIds()
        {
            if(_modifiedResource == null)
            {
                return new List<uint>();
            }

            return new List<uint>(_modifiedResource.AlteredTexts.Keys);
        }
    }


    /// <summary>
    /// This modified resource is used to store the altered texts only in the project file and mods.
    /// </summary>
    public class ModifiedLocalizationResource : ModifiedResource
    {

        /// <summary>
        /// Version number that is incremented with changes to how modfiles are persisted.
        /// This should allow to detect outdated mods and maybe even read them correctly if mod writing is ever changed.
        /// Versions:
        /// 1: Includes writing the primary texts as number of texts + textid text tuples
        /// 2: Adds another number altered adjectives, and then for each adjective the id and number of declinations, followed by the declinated adjectives themselves
        /// </summary>
        private static readonly uint MOD_PERSISTENCE_VERSION= 2;

        // Just to make sure we write / overwrite and merge the correct asset!
        private ulong _resRid = 0x0;

        /// <summary>
        /// The dictionary of altered or new texts in this modified resource.
        /// </summary>
        public Dictionary<uint, string> AlteredTexts { get; } = new Dictionary<uint, string>();

        /// <summary>
        /// The dictionary of altered declinated adjectives used for crafting in dragon age.
        /// </summary>
        public Dictionary<uint, List<string>> AlteredDeclinatedCraftingAdjectives { get; } = new Dictionary<uint, List<string>>();

        /// <summary>
        /// Sets a modified text into the dictionary.
        /// </summary>
        /// <param name="textId">The uint id of the string</param>
        /// <param name="text">The new string</param>
        public void SetText(uint textId, string text)
        {
            AlteredTexts[textId] = text;
        }

        /// <summary>
        /// Verbose remove method accessor.
        /// </summary>
        /// <param name="textId"></param>
        public void RemoveText(uint textId)
        {
            AlteredTexts.Remove(textId);
        }

        /// <summary>
        /// Sets or replaces the set of declinated crafting adjectives
        /// </summary>
        /// <param name="adjectiveId"></param>
        /// <param name="adjectives"></param>
        public void SetDeclinatedCraftingAdjective(uint adjectiveId, List<string> adjectives)
        {
            AlteredDeclinatedCraftingAdjectives[adjectiveId] = adjectives;
        }

        /// <summary>
        /// Removes the altered adjectives
        /// </summary>
        /// <param name="adjectiveId"></param>
        public void RemoveDeclinatedCraftingAdjective(uint adjectiveId)
        {
            AlteredDeclinatedCraftingAdjectives.Remove(adjectiveId);
        }

        /// <summary>
        /// Initializes the resource id, this is used to make sure we modify and overwrite the correct resource.
        /// </summary>
        /// <param name="otherResRid"></param>
        public void InitResourceId(ulong otherResRid)
        {
            if(_resRid != 0x0 && _resRid != otherResRid)
            {
                string errorMsg = string.Format(
                        "Trying to initialize modified resource for resRid <{0}> with contents of resource resRid <{1}> - This may indicate a mod made for a different game version!",
                        _resRid.ToString("X"), otherResRid.ToString("X"));
                App.Logger.LogWarning(errorMsg);
            }
            _resRid = otherResRid;
        }

        /// <summary>
        /// Merges this resource with the given other resource by talking all of the other resources texts, overwriting already present texts for the same id if they exist.
        /// This method alters the state of this resource.
        /// </summary>
        /// <param name="higherPriorityModifiedResource">The other, higher priority resource, to merge into this one.</param>
        public void Merge(ModifiedLocalizationResource higherPriorityModifiedResource)
        {

            if(_resRid != higherPriorityModifiedResource._resRid)
            {
                String errorMsg =     string.Format(
                        "Trying to merge resource with resRid <{0}> into resource for resRid <{1}> - This may indicate a mod made for a different game version!",
                        higherPriorityModifiedResource._resRid.ToString("X"), _resRid.ToString("X"));
                App.Logger.LogWarning(errorMsg);
            }

            foreach(KeyValuePair<uint, string> textEntry in higherPriorityModifiedResource.AlteredTexts)
            {
                SetText(textEntry.Key, textEntry.Value);
            }

            foreach (KeyValuePair<uint, List<string>> adjectivesEntry in higherPriorityModifiedResource.AlteredDeclinatedCraftingAdjectives)
            {
                SetDeclinatedCraftingAdjective(adjectivesEntry.Key, adjectivesEntry.Value);
            }
        }

        /// <summary>
        /// This function is responsible for reading in the modified data from the project file.
        /// </summary>
        /// <param name="reader"></param>
        public override void ReadInternal(NativeReader reader)
        {

            uint modPersistenceVersion = reader.ReadUInt();
            InitResourceId(reader.ReadULong());

            if(MOD_PERSISTENCE_VERSION < modPersistenceVersion)
            {
                ResAssetEntry asset = App.AssetManager.GetResEntry(_resRid);
                string assetName = asset != null ? asset.Path : "<unknown>";

                App.Logger.LogError("TextMod for localization resource <{0}> was written with a newer version of the Bioware Localization Plugin and cannot be read!", assetName);
                return;
            }

            byte[] entryBytes = reader.ReadBytes((int) (reader.Length - reader.Position));
            using (BinaryReader textReader = new BinaryReader(new MemoryStream(entryBytes), Encoding.UTF8))
            {
                ReadPrimaryVersion1Texts(textReader);

                if(modPersistenceVersion>=2)
                {
                    ReadDeclinatedAdjectivesVersion2Texts(textReader);
                }
            }
        }

        private void ReadPrimaryVersion1Texts(BinaryReader textReader)
        {
            int numberOfEntries = textReader.ReadInt32();
            for (int i = 0; i < numberOfEntries; i++)
            {
                uint textId = textReader.ReadUInt32();
                string text = textReader.ReadString();

                SetText(textId, text);
            }
        }

        private void ReadDeclinatedAdjectivesVersion2Texts(BinaryReader textReader)
        {
            int numberOfAdjectives = textReader.ReadInt32();
            for (int i = 0; i < numberOfAdjectives; i++)
            {
                uint adjectiveId = textReader.ReadUInt32();
                int numberOfDeclinations = textReader.ReadInt32();

                List<string> adjectives = new List<string>(numberOfDeclinations);

                for(int j = 0; i< numberOfDeclinations; j++)
                {
                    adjectives.Add(textReader.ReadString());
                }

                SetDeclinatedCraftingAdjective(adjectiveId, adjectives);
            }
        }

        /// <summary>
        /// This function is responsible for writing out the modified data to the project file.
        /// <para>I.e., the written data is this:
        /// [uint: resRid][int: numberOfEntries] {numberOfEntries * [[uint: stringId]['nullTerminatedString': String]]}
        /// </para>
        /// </summary>
        /// <param name="writer"></param>
        /// <exception cref="InvalidOperationException">If called without having initialized the resource</exception>
        public override void SaveInternal(NativeWriter writer)
        {

            // assert this is for a valid resource!
            if(_resRid == 0x0)
            {
                throw new InvalidOperationException("Modified resource not bound to any resource!");
            }

            writer.Write(MOD_PERSISTENCE_VERSION);

            writer.Write(_resRid);
            writer.Write(AlteredTexts.Count);

            // use a binary writer from here to write all the texts in utf-8
            writer.Write(ResourceUtils.ConvertTextEntriesToBytes(AlteredTexts));

            writer.Write(AlteredDeclinatedCraftingAdjectives.Count);

            // kind of stupid to do the whole writer creation again here, but its only for one resource or so..
            writer.Write(ResourceUtils.ConvertAdjectivesToBytes(AlteredDeclinatedCraftingAdjectives));
        }

        public ulong GetResRid()
        {
            return _resRid;
        }
    }
}
