using FrostySdk;
using FrostySdk.IO;
using FrostySdk.Managers;
using FrostySdk.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FrostySdk.Managers.Entries;

namespace LevelEditorPlugin.Resources
{
    public enum RasterTreeTypes
    {
        HeightfieldTreeType = 0x0,
        TerrainMaskTreeType = 0x1,
        TerrainColorTreeType = 0x2,
        TerrainMaterialTreeType = 0x3,
        DestructionDepthTreeType = 0x4,
        RasterTreeTypeCount = 0x5,
    };

    public struct QuadTreeNodeId
    {
        public ushort indexX;
        public ushort indexY;
        public byte level;

        public static int[] quadtreeNodeChildOffsetX = new int[4] { 0, 1, 1, 0 };
        public static int[] quadtreeNodeChildOffsetY = new int[4] { 0, 0, 1, 1 };

        public override string ToString()
        {
            return indexX + "x" + indexY + " : " + level;
        }
    }

    public struct AtlasTileId
    {
        public byte indexX;
        public byte indexY;
        public override string ToString()
        {
            return indexX + "x" + indexY;
        }
    }

    public class RasterTree
    {
        public virtual void loadAndInitialize(NativeReader reader) { }
        public virtual void loadData(NativeReader reader, RasterTreeNode node) { }
        public virtual ushort getNodeIndex(QuadTreeNodeId id) { return 0; }
        public virtual RasterTreeNode getNode(ushort index) { return null; }
        public virtual RasterTreeNode getRootNode() { return null; }
    }
    public class RasterTreeNode
    {
        public RasterTreeNode parent;
        public List<RasterTreeNode> children = new List<RasterTreeNode>();
        public QuadTreeNodeId id;
        public int firstChildIndex;
        public int index;

        public RasterTreeNode getChild(QuadTreeNodeId nodeId, List<RasterTreeNode> nodes)
        {
            if (firstChildIndex == -1)
                return null;
            int x = (nodeId.indexX >> (nodeId.level - id.level - 1)) - 2 * id.indexX;
            int y = (nodeId.indexY >> (nodeId.level - id.level - 1)) - 2 * id.indexY;

            if (x != 0)
                return nodes[((y != 0) ? 1 : 0) + 1 + firstChildIndex];
            else
                return nodes[((y != 0) ? 3 : 0) + firstChildIndex];
        }
    }
    public class HeightfieldTree : RasterTree
    {
        public uint nodeSamplesPerSide;
        public uint resourceAtlasSampleCountX;
        public uint resourceAtlasSampleCountY;
        public uint resourceBlurrinessFactor;
        public float worldSizeY;
        public uint minMaxStackDepth;
        public uint minMaxStackSize;
        public uint occluderGridStackDepth;
        public uint occluderGridStackSize;
        public uint unknown5;
        public uint nodeCount;
        public uint persistentNodeCount;

        public bool unknown1;
        public float unknown2;
        public uint unknown6;
        public uint unknown7;
        public float unknown8;
        public uint unknown9;
        public uint unknown10;
        public uint unknown11;

        public int DataSize => (int)((2 * nodeSamplesPerSide) * nodeSamplesPerSide) + ((int)(minMaxStackSize) * 2) + ((int)(occluderGridStackSize) * 2) + (int)(unknown5 * unknown5);

        public List<RasterTreeNode> nodes = new List<RasterTreeNode>();

        public override void loadAndInitialize(NativeReader reader)
        {
            nodeSamplesPerSide = reader.ReadUInt();
            resourceAtlasSampleCountX = reader.ReadUInt();
            resourceAtlasSampleCountY = reader.ReadUInt();
            resourceBlurrinessFactor = (uint)(1 << reader.ReadInt());
            worldSizeY = reader.ReadFloat();

            float physicsMetersPerSample = reader.ReadFloat();
            int physicsCropWidth = reader.ReadInt();

            unknown1 = reader.ReadBoolean();
            unknown2 = reader.ReadFloat();
            minMaxStackDepth = reader.ReadUInt();
            occluderGridStackDepth = reader.ReadUInt();
            unknown5 = reader.ReadUInt();
            unknown6 = reader.ReadUInt();
            unknown7 = reader.ReadUInt();
            unknown8 = reader.ReadFloat();

            uint tmp = (uint)(1 << (int)(minMaxStackDepth - 1));
            int cnt = 0;

            minMaxStackSize = 0;

            do
            {
                uint b = (uint)((long)tmp >> cnt++);
                minMaxStackSize += 2 * b * b;
            }
            while (cnt < minMaxStackDepth);

            tmp = (uint)(1 << (int)(occluderGridStackDepth - 1));
            cnt = 0;

            occluderGridStackSize = 0;

            do
            {
                uint b = (uint)((int)tmp >> cnt++);
                occluderGridStackSize += (b + 1) * (b + 1);
            }
            while (cnt < occluderGridStackDepth);

            nodeCount = reader.ReadUInt();
            persistentNodeCount = reader.ReadUInt();

            unknown9 = reader.ReadUInt();
            unknown10 = reader.ReadUInt();
            unknown11 = reader.ReadUInt();

            for (int i = 0; i < nodeCount; i++)
                nodes.Add(new HeightfieldTreeNode());

            QuadTreeNodeId rootNodeId = new QuadTreeNodeId();
            int id = 0;
            int index = 1;

            loadNodes(reader, id, ref index, rootNodeId);
        }

        public override ushort getNodeIndex(QuadTreeNodeId id)
        {
            RasterTreeNode node = nodes[0];
            while (node != null)
            {
                if (node.id.level == id.level)
                    break;
                node = node.getChild(id, nodes);
            }
            return (ushort)((node != null) ? node.index : 0xFFFF);
        }

        public override RasterTreeNode getNode(ushort index)
        {
            return nodes[(int)index];
        }

        private void loadNodes(NativeReader reader, int id, ref int index, QuadTreeNodeId nodeId)
        {
            HeightfieldTreeNode node = nodes[id] as HeightfieldTreeNode;

            node.offset = reader.Position;
            node.id = nodeId;
            node.index = id;
            node.firstChildIndex = -1;
            node.boundingBox.min.x = reader.ReadFloat();
            node.boundingBox.min.y = reader.ReadFloat();
            node.boundingBox.min.z = reader.ReadFloat();
            node.boundingBox.max.x = reader.ReadFloat();
            node.boundingBox.max.y = reader.ReadFloat();
            node.boundingBox.max.z = reader.ReadFloat();

            if (ProfilesLibrary.DataVersion == 20170321 || ProfilesLibrary.DataVersion == 20171110)
                node.unknown1 = reader.ReadFloat();

            bool nodeDisabled = reader.ReadBoolean();
            if (nodeDisabled)
                return;

            bool nodeHasBeenPruned = reader.ReadBoolean();
            bool nodeHasData = reader.ReadBoolean();
            bool nodeUnknown = reader.ReadBoolean();
            bool nodePersistent = reader.ReadBoolean();

            if (nodeHasBeenPruned)
                node.unknownBytes1 = reader.ReadBytes((int)(minMaxStackSize * 2));

            node.hasPersistentData = nodePersistent;
            node.hasData = nodeHasData;
            node.hasUnknown = nodeUnknown;
            if (nodeHasData && nodePersistent)
                loadData(reader, node);

            bool nodeHasChildren = reader.ReadBoolean();
            if (nodeHasChildren)
            {
                node.firstChildIndex = index;
                index += 4;

                QuadTreeNodeId childNodeId = new QuadTreeNodeId();
                childNodeId.level = (byte)(nodeId.level + 1);

                for (int i = 0; i < 4; i++)
                {
                    childNodeId.indexX = (ushort)(QuadTreeNodeId.quadtreeNodeChildOffsetX[i] + 2 * nodeId.indexX);
                    childNodeId.indexY = (ushort)(QuadTreeNodeId.quadtreeNodeChildOffsetY[i] + 2 * nodeId.indexY);

                    node.children.Add(nodes[node.firstChildIndex + i]);
                    nodes[node.firstChildIndex + i].parent = node;

                    loadNodes(reader, node.firstChildIndex + i, ref index, childNodeId);
                }
            }
        }

        public override void loadData(NativeReader reader, RasterTreeNode node)
        {
            HeightfieldTreeNode hnode = node as HeightfieldTreeNode;
            if (hnode.data != null)
                return;

            hnode.data = reader.ReadBytes((int)((2 * nodeSamplesPerSide) * nodeSamplesPerSide));
            hnode.unknownBytes1 = reader.ReadBytes((int)(minMaxStackSize) * 2);
            hnode.unknownBytes2 = reader.ReadBytes((int)(occluderGridStackSize) * 2);
            hnode.unknownBytes3 = reader.ReadBytes((int)(unknown5 * unknown5));
        }

        public override RasterTreeNode getRootNode()
        {
            return nodes[0];
        }
    }
    public class HeightfieldTreeNode : RasterTreeNode
    {
        public AxisAlignedBox boundingBox;
        public byte[] data;
        public bool hasData;
        public bool hasPersistentData;
        public bool hasUnknown;
        public float unknown1;
        public long offset;

        public byte[] unknownBytes1;
        public byte[] unknownBytes2;
        public byte[] unknownBytes3;

        public uint size { get { return (uint)(boundingBox.max.x - boundingBox.min.x); } }
    }
    public class TerrainMaskTree : RasterTree
    {
        public uint tileSamplesPerSide;
        public uint tilesPerNodeSide;
        public uint resourceAtlasSampleCountX;
        public uint resourceAtlasSampleCountY;
        public uint blurrinessFactor;
        public AxisAlignedBox2 treeCoverage;
        public uint nodeCount;
        public uint persistentNonTrivialSubtileCount;
        public uint atlasTileCountX;
        public uint atlasTileCountY;
        public uint unknown1;

        public uint maxLayerCount;

        public List<RasterTreeNode> nodes = new List<RasterTreeNode>();
        public List<TerrainMaskTreeNode.Subtile> subtiles = new List<TerrainMaskTreeNode.Subtile>();

        public override void loadAndInitialize(NativeReader reader)
        {
            tileSamplesPerSide = reader.ReadUInt();
            tilesPerNodeSide = reader.ReadUInt();
            reader.ReadUInt();
            resourceAtlasSampleCountX = reader.ReadUInt();
            resourceAtlasSampleCountY = reader.ReadUInt();

            int blurriness = reader.ReadInt();
            blurrinessFactor = (uint)(1 << blurriness);
            treeCoverage = reader.ReadAxisAlignedBox2();

            nodeCount = reader.ReadUInt();
            uint subtileCount = reader.ReadUInt();
            persistentNonTrivialSubtileCount = reader.ReadUInt();

            if (ProfilesLibrary.DataVersion == 20170321 || ProfilesLibrary.DataVersion == 20161021 || ProfilesLibrary.DataVersion == 20150223 || ProfilesLibrary.DataVersion == 20151117 || ProfilesLibrary.DataVersion == 20171117)
                unknown1 = reader.ReadUInt();

            for (int i = 0; i < nodeCount; i++)
                nodes.Add(new TerrainMaskTreeNode());

            atlasTileCountX = resourceAtlasSampleCountX / tileSamplesPerSide;
            atlasTileCountY = resourceAtlasSampleCountY / tileSamplesPerSide;

            QuadTreeNodeId rootNodeId = new QuadTreeNodeId();
            int id = 0;
            int index = 1;

            loadNodes(reader, id, ref index, rootNodeId, treeCoverage);

            foreach (TerrainMaskTreeNode.Subtile subTile in subtiles)
            {
                if (subTile.terrainLayerIndex + 1 > maxLayerCount)
                    maxLayerCount = (uint)(subTile.terrainLayerIndex + 1);
            }
        }

        private void loadNodes(NativeReader reader, int id, ref int index, QuadTreeNodeId nodeId, AxisAlignedBox2 nodeCoverage)
        {
            TerrainMaskTreeNode node = nodes[id] as TerrainMaskTreeNode;
            node.id = nodeId;
            node.index = id;
            node.firstChildIndex = -1;
            node.nodeCoverage = nodeCoverage;

            node.nodeHasBeenPruned = reader.ReadBoolean();
            ushort subtileCount = reader.ReadUShort();

            node.nonTrivialSubtileCount = reader.ReadUShort();
            node.nonTrivialPersistentSubtileCountRecursive = reader.ReadUShort();

            if (!node.nodeHasBeenPruned && subtileCount > 0)
            {
                float nodeWidth = nodeCoverage.max.x - nodeCoverage.min.x;
                float nodeHeight = nodeCoverage.max.y - nodeCoverage.min.y;

                for (int i = 0; i < subtileCount; i++)
                {
                    TerrainMaskTreeNode.Subtile subtile = new TerrainMaskTreeNode.Subtile();
                    subtile.terrainLayerIndex = reader.ReadByte();
                    subtile.worldCoverage = reader.ReadAxisAlignedBox2();
                    bool isLeaf = reader.ReadBoolean();
                    bool unk = reader.ReadBoolean();
                    subtile.flags = (byte)(((isLeaf) ? 1 : 0) | ((unk) ? 2 : 0));
                    ushort highResolutionCoverageGridStackSize = 11;
                    if (ProfilesLibrary.DataVersion == 20131115 || ProfilesLibrary.DataVersion == 20141118)
                        highResolutionCoverageGridStackSize = reader.ReadUShort();
                    else if (ProfilesLibrary.DataVersion == 20151117)
                        highResolutionCoverageGridStackSize = 0xAB;
                    subtile.highResolutionCoverageGridStack = reader.ReadBytes((int)highResolutionCoverageGridStackSize);

                    float subTileWidth = subtile.worldCoverage.max.x - subtile.worldCoverage.min.x;
                    float subTileHeight = subtile.worldCoverage.max.y - subtile.worldCoverage.min.y;

                    subtile.atlasTileId.indexX = (byte)((tilesPerNodeSide * ((subtile.worldCoverage.min.x - nodeCoverage.min.x) / (float)nodeWidth)) + 1);
                    subtile.atlasTileId.indexY = (byte)((tilesPerNodeSide * ((subtile.worldCoverage.min.y - nodeCoverage.min.y) / (float)nodeHeight)) + 1);

                    node.subtiles.Add(subtile);
                    subtiles.Add(subtile);

                    if (!node.layerIndices.Contains(subtile.terrainLayerIndex))
                        node.layerIndices.Add(subtile.terrainLayerIndex);
                }

                node.nodePersistent = reader.ReadBoolean();
                if (node.nodePersistent)
                {
                    loadData(reader, node);
                    if (ProfilesLibrary.DataVersion == 20151117 || ProfilesLibrary.DataVersion == 20171117)
                        reader.ReadUInt();
                }

                bool nodeHasChildren = reader.ReadBoolean();
                if (nodeHasChildren)
                {
                    node.firstChildIndex = index;
                    index += 4;

                    QuadTreeNodeId childNodeId = new QuadTreeNodeId();
                    childNodeId.level = (byte)(nodeId.level + 1);

                    for (int i = 0; i < 4; i++)
                    {
                        childNodeId.indexX = (ushort)(QuadTreeNodeId.quadtreeNodeChildOffsetX[i] + 2 * nodeId.indexX);
                        childNodeId.indexY = (ushort)(QuadTreeNodeId.quadtreeNodeChildOffsetY[i] + 2 * nodeId.indexY);

                        AxisAlignedBox2 subCoverage = nodeCoverage;
                        float width = (subCoverage.max.x - subCoverage.min.x) * 0.5f;
                        float height = (subCoverage.max.y - subCoverage.min.y) * 0.5f;

                        subCoverage.min.x = subCoverage.min.x + (width * QuadTreeNodeId.quadtreeNodeChildOffsetX[i]);
                        subCoverage.max.x = subCoverage.max.x - (width * (1.0f - QuadTreeNodeId.quadtreeNodeChildOffsetX[i]));

                        subCoverage.min.y = subCoverage.min.y + (height * QuadTreeNodeId.quadtreeNodeChildOffsetY[i]);
                        subCoverage.max.y = subCoverage.max.y - (height * (1.0f - QuadTreeNodeId.quadtreeNodeChildOffsetY[i]));

                        node.children.Add(nodes[node.firstChildIndex + i]);
                        nodes[node.firstChildIndex + i].parent = node;

                        loadNodes(reader, node.firstChildIndex + i, ref index, childNodeId, subCoverage);
                    }
                }
            }
        }

        public override void loadData(NativeReader reader, RasterTreeNode node)
        {
            TerrainMaskTreeNode mnode = node as TerrainMaskTreeNode;
            if (mnode.data != null)
                return;

            mnode.data = new byte[mnode.nonTrivialSubtileCount][];
            int z = 0;

            for (int i = 0; i < mnode.subtiles.Count; i++)
            {
                TerrainMaskTreeNode.Subtile subtile = mnode.subtiles[i];
                if ((subtile.flags & 2) == 0)
                    mnode.data[z++] = reader.ReadBytes((int)(tileSamplesPerSide * tileSamplesPerSide));
            }
        }

        public override ushort getNodeIndex(QuadTreeNodeId id)
        {
            RasterTreeNode node = nodes[0];
            while (node != null)
            {
                if (node.id.level == id.level)
                    break;
                node = node.getChild(id, nodes);
            }
            return (ushort)((node != null) ? node.index : 0xFFFF);
        }

        public override RasterTreeNode getNode(ushort index)
        {
            return nodes[(int)index];
        }

        public override RasterTreeNode getRootNode()
        {
            return nodes[0];
        }
    }
    public class TerrainMaskTreeNode : RasterTreeNode
    {
        public class Subtile
        {
            public byte terrainLayerIndex;
            public AxisAlignedBox2 worldCoverage;
            public byte flags;
            public byte[] highResolutionCoverageGridStack;
            public AtlasTileId atlasTileId;
        }

        public AxisAlignedBox2 nodeCoverage;
        public ushort nonTrivialSubtileCount;
        public ushort nonTrivialPersistentSubtileCountRecursive;

        public bool nodeHasBeenPruned;
        public bool nodePersistent;

        public List<Subtile> subtiles = new List<Subtile>();
        public List<int> layerIndices = new List<int>();
        public byte[][] data;
    }
    public class TerrainColorTree : RasterTree
    {
        public override void loadAndInitialize(NativeReader reader)
        {
        }
    }
    public class TerrainMaterialTree : RasterTree
    {
        public uint nodeSamplesPerSide;
        public uint blurrinessFactor;
        public AxisAlignedBox treeCoverage;
        public uint nodeCount;
        public uint persistentNodeCount;
        public uint levelMax;
        public uint unknown1;

        public List<RasterTreeNode> nodes = new List<RasterTreeNode>();
        public List<uint> materialPairIndices = new List<uint>();

        public override void loadAndInitialize(NativeReader reader)
        {
            nodeSamplesPerSide = reader.ReadUInt();

            int blurriness = reader.ReadInt();
            blurrinessFactor = (uint)(1 << blurriness);

            treeCoverage.min.x = reader.ReadFloat();
            treeCoverage.min.y = reader.ReadFloat();
            treeCoverage.max.x = reader.ReadFloat();
            treeCoverage.max.y = reader.ReadFloat();

            nodeCount = reader.ReadUInt();
            persistentNodeCount = reader.ReadUInt();
            levelMax = reader.ReadUInt();

            for (int i = 0; i < nodeCount; i++)
                nodes.Add(new TerrainMaterialTreeNode());

            QuadTreeNodeId rootNodeId = new QuadTreeNodeId();
            int id = 0;
            int index = 1;

            loadNodes(reader, id, ref index, rootNodeId);

            uint materialPairCount = reader.ReadUInt();
            for (int i = 0; i < materialPairCount; i++)
                materialPairIndices.Add(reader.ReadUInt());
            unknown1 = reader.ReadUInt();
        }

        private void loadNodes(NativeReader reader, int id, ref int index, QuadTreeNodeId nodeId)
        {
            TerrainMaterialTreeNode node = nodes[id] as TerrainMaterialTreeNode;
            node.id = nodeId;
            node.index = id;
            node.firstChildIndex = -1;

            bool nodeHasData = reader.ReadBoolean();
            bool nodePersistent = reader.ReadBoolean();

            if (nodeHasData && nodePersistent)
            {
                int size = reader.ReadInt();
                node.rleLineOffsets = new ushort[nodeSamplesPerSide];
                node.rleData = reader.ReadBytes(size);

                int idx = 0;
                ushort lineOffset = 0;

                do
                {
                    node.rleLineOffsets[idx] = lineOffset;
                    ushort lineSize = reader.ReadUShort();
                    lineOffset += lineSize;
                    idx++;
                }
                while (idx < nodeSamplesPerSide);
            }

            bool nodeHasChildren = reader.ReadBoolean();
            if (nodeHasChildren)
            {
                node.firstChildIndex = index;
                index += 4;

                QuadTreeNodeId childNodeId = new QuadTreeNodeId();
                childNodeId.level = (byte)(nodeId.level + 1);

                for (int i = 0; i < 4; i++)
                {
                    childNodeId.indexX = (ushort)(QuadTreeNodeId.quadtreeNodeChildOffsetX[i] + 2 * nodeId.indexX);
                    childNodeId.indexY = (ushort)(QuadTreeNodeId.quadtreeNodeChildOffsetY[i] + 2 * nodeId.indexY);

                    node.children.Add(nodes[node.firstChildIndex + i]);
                    loadNodes(reader, node.firstChildIndex + i, ref index, childNodeId);
                }
            }
        }

        public override RasterTreeNode getNode(ushort index)
        {
            return nodes[(int)index];
        }

        public override ushort getNodeIndex(QuadTreeNodeId id)
        {
            RasterTreeNode node = nodes[0];
            while (node != null)
            {
                if (node.id.level == id.level)
                    break;
                node = node.getChild(id, nodes);
            }
            return (ushort)((node != null) ? node.index : 0xFFFF);
        }

        public override void loadData(NativeReader reader, RasterTreeNode node)
        {
            TerrainMaterialTreeNode mnode = node as TerrainMaterialTreeNode;
            if (mnode.rleData != null)
                return;

            int size = reader.ReadInt();
            mnode.rleLineOffsets = new ushort[nodeSamplesPerSide];
            mnode.rleData = reader.ReadBytes(size);

            int idx = 0;
            ushort lineOffset = 0;

            do
            {
                mnode.rleLineOffsets[idx] = lineOffset;
                ushort lineSize = reader.ReadUShort();
                lineOffset += lineSize;
                idx++;
            }
            while (idx < nodeSamplesPerSide);
        }
    }
    public class TerrainMaterialTreeNode : RasterTreeNode
    {
        public byte[] rleData;
        public ushort[] rleLineOffsets;
    }
    public class DestructionDepthTree : RasterTree
    {
        public override void loadAndInitialize(NativeReader reader)
        {
        }
    }

    public class TerrainStreamingTreeNode
    {
        public uint lod0Size;
        public Guid lod0Id;
        public uint lod1Size;
        public Guid lod1Guid;
        public List<TerrainStreamingTreeNode> children = new List<TerrainStreamingTreeNode>();
        public QuadTreeNodeId id;
    }

    public class TerrainStreamingTree : Resource
    {
        List<Type> rasterTreeTypeList = new List<Type>()
        {
            typeof(HeightfieldTree),
            typeof(TerrainMaskTree),
            typeof(TerrainColorTree),
            typeof(TerrainMaterialTree),
            typeof(DestructionDepthTree)
        };

        List<RasterTree> rasterTrees = new List<RasterTree>();
        List<TerrainStreamingTreeNode> streamingNodes = new List<TerrainStreamingTreeNode>();

        public override void Read(NativeReader reader, AssetManager am, ResAssetEntry entry, ModifiedResource modifiedData)
        {
            uint unblurredSamplesPerNodeSidePot = reader.ReadUInt();
            bool trackTextureDetailFalloff = reader.ReadBoolean();
            float invisibleDetailReductionFactor = reader.ReadFloat();
            float occludedDetailReductionFactor = reader.ReadFloat();
            uint resourceBlurriness = reader.ReadUInt();
            uint unknown = reader.ReadUInt();
            uint nodeCount = reader.ReadUInt();
            bool freeStreamingEnabled = reader.ReadBoolean();

            for (int i = 0; i < (int)RasterTreeTypes.RasterTreeTypeCount; i++)
                rasterTrees.Add(null);

            byte rasterTreeType = reader.ReadByte();
            while (rasterTreeType != 0xFF)
            {
                uint rasterTreeLoadSize = reader.ReadUInt();
                long startPos = reader.Position;

                RasterTree rt = (RasterTree)Activator.CreateInstance(rasterTreeTypeList[rasterTreeType]);
                rasterTrees[rasterTreeType] = rt;

                rt.loadAndInitialize(reader);

                reader.Position = startPos + rasterTreeLoadSize;
                rasterTreeType = reader.ReadByte();
            }

            for (int i = 0; i < nodeCount; i++)
                streamingNodes.Add(new TerrainStreamingTreeNode());

            int id = 0;
            int index = 1;
            QuadTreeNodeId rootNodeId = new QuadTreeNodeId();

            loadNodes(reader, id, ref index, rootNodeId);
        }

        public TerrainStreamingTreeNode GetRootNode()
        {
            return streamingNodes[0];
        }

        public TerrainStreamingTreeNode GetNode(QuadTreeNodeId id)
        {
            return streamingNodes.Find(n => n.id.indexX == id.indexX && n.id.indexY == id.indexY && n.id.level == id.level);
        }

        public RasterTree GetRasterTree(RasterTreeTypes treeType)
        {
            return rasterTrees[(int)treeType];
        }

        private void loadNodes(NativeReader reader, int id, ref int index, QuadTreeNodeId nodeId)
        {
            TerrainStreamingTreeNode node = streamingNodes[id];

            node.id = nodeId;
            node.lod0Size = reader.ReadUInt();
            node.lod0Id = reader.ReadGuid();

            bool lod1Enabled = reader.ReadBoolean();
            if (lod1Enabled)
            {
                node.lod1Size = reader.ReadUInt();
                node.lod1Guid = reader.ReadGuid();
            }

            bool persistentDedicatedServer = reader.ReadBoolean();
            bool hasChildren = reader.ReadBoolean();

            if (hasChildren)
            {
                int firstChildIndex = index;
                index += 4;

                QuadTreeNodeId childNodeId = new QuadTreeNodeId();
                childNodeId.level = (byte)(nodeId.level + 1);

                for (int i = 0; i < 4; i++)
                {
                    childNodeId.indexX = (ushort)(QuadTreeNodeId.quadtreeNodeChildOffsetX[i] + 2 * nodeId.indexX);
                    childNodeId.indexY = (ushort)(QuadTreeNodeId.quadtreeNodeChildOffsetY[i] + 2 * nodeId.indexY);

                    node.children.Add(streamingNodes[firstChildIndex + i]);
                    loadNodes(reader, firstChildIndex + i, ref index, childNodeId);
                }
            }
        }
    }
}
