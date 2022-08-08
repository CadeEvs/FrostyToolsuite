using Frosty.Core.Viewport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.Direct3D11;
using LevelEditorPlugin.Resources;
using SharpDX;
using Buffer = SharpDX.Direct3D11.Buffer;
using SharpDX.Direct3D;
using FrostySdk.IO;
using Frosty.Core;
using Frosty.Core.Screens;
using System.IO;

namespace LevelEditorPlugin.Render
{
    public class TerrainRenderable
    {
        public struct LayerData
        {
            public TerrainMaskTreeNode Node;
            public Vector4 Offsets;
        }

        public IEnumerable<TerrainChunkRenderable> TerrainChunks => terrainChunks;

        protected List<TerrainChunkRenderable> terrainChunks = new List<TerrainChunkRenderable>();

        public TerrainRenderable(RenderCreateState state, TerrainStreamingTree streamingData)
        {
            HeightfieldTree htree = streamingData.GetRasterTree(RasterTreeTypes.HeightfieldTreeType) as HeightfieldTree;
            TerrainMaskTree mtree = streamingData.GetRasterTree(RasterTreeTypes.TerrainMaskTreeType) as TerrainMaskTree;

            HeightfieldTreeNode hnode = htree.getRootNode() as HeightfieldTreeNode;
            TerrainMaskTreeNode mnode = mtree.getRootNode() as TerrainMaskTreeNode;

            //using (NativeWriter writer = new NativeWriter(new FileStream(@"Z:\Dump\Terrain\Output.txt", FileMode.Create)))
            //    PrintHeightfieldNodes(streamingData, htree, hnode, writer, 0);

            //using (NativeWriter writer = new NativeWriter(new FileStream(@"Z:\Dump\Terrain\Mask.txt", FileMode.Create)))
            //    PrintMaskNodes(streamingData, htree, mtree, mnode, writer, 0);

            Dictionary<Guid, NativeReader> chunkCache = new Dictionary<Guid, NativeReader>();
            IterateNodes(state, streamingData, htree, hnode, mtree, mnode, new List<LayerData>(), chunkCache);

            //iterateNodes(state, streamingData, streamingData.GetRootNode());

            foreach (NativeReader reader in chunkCache.Values)
                reader.Dispose();
        }

        public void Dispose()
        {
            foreach (TerrainChunkRenderable chunk in terrainChunks)
                chunk.Dispose();
        }

        //private void PrintMaskNodes(TerrainStreamingTree tree, HeightfieldTree htree, TerrainMaskTree mtree, TerrainMaskTreeNode mnode, NativeWriter writer, int tab, Dictionary<Guid, NativeReader> chunkCache)
        //{
        //    string basePath = @"Z:\Dump\Terrain\Masks\";
        //    writer.WriteLine("".PadLeft(tab) + $"{mnode.id}");

        //    if (mnode.subtiles.Count > 0)
        //    {
        //        if (mnode.data == null)
        //        {
        //            if (mnode.nonTrivialSubtileCount > 0)
        //            {
        //                ushort hidx = htree.getNodeIndex(mnode.id);
        //                HeightfieldTreeNode hhnode = (hidx != 0xFFFF) ? htree.getNode(hidx) as HeightfieldTreeNode : null;

        //                bool hiRes;
        //                NativeReader hreader = GetReaderForNode(tree, mnode, chunkCache, out hiRes);
        //                {
        //                    if (hiRes)
        //                    {
        //                        if (hhnode != null && hhnode.hasData && !hhnode.hasPersistentData)
        //                            hreader.Position = htree.DataSize * 4;

        //                        mtree.loadData(hreader, mnode.parent.children[3]);
        //                        mtree.loadData(hreader, mnode.parent.children[2]);
        //                        mtree.loadData(hreader, mnode.parent.children[1]);
        //                        mtree.loadData(hreader, mnode.parent.children[0]);
        //                    }
        //                    else
        //                    {
        //                        if (hhnode != null && hhnode.hasData && !hhnode.hasPersistentData)
        //                            hreader.Position = htree.DataSize;

        //                        mtree.loadData(hreader, mnode);
        //                    }
        //                }
        //            }

        //            List<Dictionary<AtlasTileId, byte[]>> layers = new List<Dictionary<AtlasTileId, byte[]>>();
        //            int z = 0;

        //            byte[] whiteTile = new byte[mtree.tileSamplesPerSide * mtree.tileSamplesPerSide];
        //            for (int i = 0; i < (mtree.tileSamplesPerSide * mtree.tileSamplesPerSide); i++)
        //                whiteTile[i] = 0xFF;

        //            for (int i = 0; i < mtree.maxLayerCount; i++)
        //                layers.Add(new Dictionary<AtlasTileId, byte[]>());

        //            foreach (TerrainMaskTreeNode.Subtile subtile in mnode.subtiles)
        //            {
        //                if ((subtile.flags & 2) == 0)
        //                {
        //                    layers[subtile.terrainLayerIndex].Add(subtile.atlasTileId, mnode.data[z++]);
        //                }
        //                else
        //                {
        //                    layers[subtile.terrainLayerIndex].Add(subtile.atlasTileId, whiteTile);
        //                }
        //            }

        //            int layerIdx = 0;
        //            foreach (Dictionary<AtlasTileId, byte[]> layer in layers)
        //            {
        //                if (layer.Count > 0)
        //                {
        //                    List<byte> pixels = new List<byte>();
        //                    byte[] layerData = new byte[(mtree.tileSamplesPerSide * mtree.tilesPerNodeSide) * (mtree.tileSamplesPerSide * mtree.tilesPerNodeSide)];

        //                    for (int row = 0; row < mtree.tilesPerNodeSide; row++)
        //                    {
        //                        for (int col = 0; col < mtree.tilesPerNodeSide; col++)
        //                        {
        //                            AtlasTileId tileId = new AtlasTileId() { indexX = (byte)(col + 1), indexY = (byte)(row + 1) };
        //                            int yOffset = (int)(row * mtree.tileSamplesPerSide);

        //                            if (layer.ContainsKey(tileId))
        //                            {
        //                                int xOffset = (int)(col * mtree.tileSamplesPerSide);
        //                                for (int y = 0; y < mtree.tileSamplesPerSide; y++)
        //                                {
        //                                    for (int x = 0; x < mtree.tileSamplesPerSide; x++)
        //                                    {
        //                                        layerData[((y + yOffset) * (mtree.tileSamplesPerSide * mtree.tilesPerNodeSide)) + (x + xOffset)] = layer[tileId][(y * mtree.tileSamplesPerSide) + x];
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }

        //                    foreach (var b in layerData)
        //                    {
        //                        pixels.Add(b);
        //                        pixels.Add(b);
        //                        pixels.Add(b);
        //                        pixels.Add((byte)0xFF);
        //                    }

        //                    byte[] tgaData = Frosty.Image.ImageCreator.CreateTGA((int)(mtree.tileSamplesPerSide * mtree.tilesPerNodeSide), (int)(mtree.tileSamplesPerSide * mtree.tilesPerNodeSide), pixels.ToArray());
        //                    using (NativeWriter tgaWriter = new NativeWriter(new FileStream(basePath + $"{mnode.id.level}x{mnode.id.indexX}x{mnode.id.indexY}.{layerIdx}.tga", FileMode.Create)))
        //                    {
        //                        tgaWriter.Write(tgaData);
        //                    }
        //                }

        //                layerIdx++;
        //            }
        //        }
        //    }

        //    for (int i = 0; i < mnode.children.Count; i++)
        //    {
        //        TerrainMaskTreeNode childNode = mnode.children[i] as TerrainMaskTreeNode;
        //        PrintMaskNodes(tree, htree, mtree, childNode, writer, tab + 4, chunkCache);
        //    }
        //}

        //private void PrintHeightfieldNodes(TerrainStreamingTree tree, HeightfieldTree htree, HeightfieldTreeNode hnode, NativeWriter writer, int tab, Dictionary<Guid, NativeReader> chunkCache)
        //{
        //    string basePath = @"Z:\Dump\Terrain\Textures\";
        //    writer.WriteLine("".PadLeft(tab) + $"{hnode.id}");

        //    if (hnode.hasData)
        //    {
        //        if (hnode.data == null)
        //        {
        //            bool hiRes;
        //            NativeReader hreader = GetReaderForNode(tree, hnode, chunkCache, out hiRes);
        //            {
        //                if (hiRes)
        //                {
        //                    htree.loadData(hreader, hnode.parent.children[3]);
        //                    htree.loadData(hreader, hnode.parent.children[2]);
        //                    htree.loadData(hreader, hnode.parent.children[1]);
        //                    htree.loadData(hreader, hnode.parent.children[0]);
        //                }
        //                else
        //                {
        //                    htree.loadData(hreader, hnode);
        //                }
        //            }
        //        }

        //        int z = 0;
        //        int dim = (int)htree.nodeSamplesPerSide;
        //        float spacing = ((hnode.size / (float)((dim - (htree.unknown11 * 2)) - 1)));
        //        float worldSizeY = htree.worldSizeY;

        //        BoundingBox tmpBb = new BoundingBox();
        //        tmpBb.Minimum.X = hnode.boundingBox.min.x;
        //        tmpBb.Minimum.Y = hnode.boundingBox.min.y;
        //        tmpBb.Minimum.Z = hnode.boundingBox.min.z;

        //        tmpBb.Maximum.X = hnode.boundingBox.max.x;
        //        tmpBb.Maximum.Y = hnode.boundingBox.max.y;
        //        tmpBb.Maximum.Z = hnode.boundingBox.max.z;

        //        float minX = (tmpBb.Minimum.X - (spacing * 2));
        //        float minY = (tmpBb.Minimum.Z - (spacing * 2));

        //        List<byte> pixels = new List<byte>();
        //        for (int y = 0; y < dim; y++)
        //        {
        //            for (int x = 0; x < dim; x++)
        //            {
        //                float newX = minX + (x * spacing);
        //                float newY = minY + (y * spacing);

        //                float height = (BitConverter.ToUInt16(hnode.data, z * 2) / (float)ushort.MaxValue);
        //                byte channelColor = (byte)(height * 255.0f);

        //                pixels.Add(channelColor);
        //                pixels.Add(channelColor);
        //                pixels.Add(channelColor);
        //                pixels.Add((byte)0xFF);

        //                z++;
        //            }
        //        }

        //        byte[] tgaData = Frosty.Image.ImageCreator.CreateTGA(dim, dim, pixels.ToArray());
        //        using (NativeWriter tgaWriter = new NativeWriter(new FileStream(basePath + $"{hnode.id.level}x{hnode.id.indexX}x{hnode.id.indexY}.tga", FileMode.Create)))
        //        {
        //            tgaWriter.Write(tgaData);
        //        }
        //    }

        //    for (int i = 0; i < hnode.children.Count; i++)
        //    {
        //        HeightfieldTreeNode childNode = hnode.children[i] as HeightfieldTreeNode;
        //        PrintHeightfieldNodes(tree, htree, childNode, writer, tab + 4, chunkCache);
        //    }
        //}

        private void IterateNodes(RenderCreateState state, TerrainStreamingTree tree, HeightfieldTree htree, HeightfieldTreeNode hnode, TerrainMaskTree mtree, TerrainMaskTreeNode mnode, List<LayerData> layerDatas, Dictionary<Guid, NativeReader> chunkCache)
        {
            if (mnode != null)
            {
                foreach (int tileIndex in mnode.layerIndices)
                {
                    while (layerDatas.Count <= tileIndex)
                        layerDatas.Add(new LayerData());
                    layerDatas[tileIndex] = new LayerData() { Node = mnode, Offsets = new Vector4(0, 0, 1, 1) };
                }
            }

            //for (int i = 0; i < layerDatas.Count; i++)
            //{
            //    if (hnode.id.level == layerDatas[i].Node.id.level)
            //    {
            //        var layer = layerDatas[i];
            //        layer.Offsets = new Vector4(0, 0, 1, 1);
            //        layerDatas[i] = layer;
            //    }
            //}

            if (hnode.children.Count != 0 /*|| mnode.children.Count != 0*/)
            {
                Vector4[] offsets = new Vector4[4]
                {
                    new Vector4(0, 0, 0.5f, 0.5f),
                    new Vector4(1, 0, 0.5f, 0.5f),
                    new Vector4(1, 1, 0.5f, 0.5f),
                    new Vector4(0, 1, 0.5f, 0.5f)
                };

                List<LayerData> tmpLayers = new List<LayerData>();
                tmpLayers.AddRange(layerDatas);

                for (int i = 0; i < 4; i++)
                {
                    HeightfieldTreeNode childHNode = (hnode.children.Count != 0) ? hnode.children[i] as HeightfieldTreeNode : hnode;
                    TerrainMaskTreeNode childMNode = (mnode != null && mnode.children.Count != 0) ? mnode.children[i] as TerrainMaskTreeNode : null;

                    layerDatas.Clear();
                    for (int j = 0; j < tmpLayers.Count; j++)
                    {
                        if (tmpLayers[j].Node != null)
                        {
                            LayerData layer = tmpLayers[j];
                            layer.Offsets = new Vector4(
                                tmpLayers[j].Offsets.X + ((tmpLayers[j].Offsets.Z * offsets[i].Z) * offsets[i].X),
                                tmpLayers[j].Offsets.Y + ((tmpLayers[j].Offsets.W * offsets[i].W) * offsets[i].Y),
                                tmpLayers[j].Offsets.Z * offsets[i].Z,
                                tmpLayers[j].Offsets.W * offsets[i].W
                                );
                            layerDatas.Add(layer);
                        }
                    }

                    IterateNodes(state, tree, htree, childHNode, mtree, childMNode, layerDatas, chunkCache);
                }
                return;
            }

            if (hnode.hasData)
            {
                bool hiRes = false;
                if (hnode.data == null)
                {
                    NativeReader hreader = GetReaderForNode(tree, hnode, chunkCache, out hiRes);
                    {
                        if (hiRes)
                        {
                            htree.loadData(hreader, hnode.parent.children[3]);
                            htree.loadData(hreader, hnode.parent.children[2]);
                            htree.loadData(hreader, hnode.parent.children[1]);
                            htree.loadData(hreader, hnode.parent.children[0]);
                        }
                        else
                        {
                            htree.loadData(hreader, hnode);
                        }
                    }
                }

                List<LayerData> temp = new List<LayerData>();
                foreach (LayerData layer in layerDatas)
                {
                    if (layer.Node == null)
                        continue;
                    if (layer.Node.data == null && layer.Node.nonTrivialSubtileCount != 0)
                    {
                        ushort hidx = htree.getNodeIndex(layer.Node.id);
                        HeightfieldTreeNode hhnode = (hidx != 0xFFFF) ? htree.getNode(hidx) as HeightfieldTreeNode : null;

                        NativeReader mreader = GetReaderForNode(tree, layer.Node, chunkCache, out hiRes);
                        {
                            if (hiRes)
                            {
                                if (hhnode != null && hhnode.hasData && !hhnode.hasPersistentData)
                                    mreader.Position = htree.DataSize * 4;

                                mtree.loadData(mreader, layer.Node.parent.children[3]);
                                mtree.loadData(mreader, layer.Node.parent.children[2]);
                                mtree.loadData(mreader, layer.Node.parent.children[1]);
                                mtree.loadData(mreader, layer.Node.parent.children[0]);
                            }
                            else
                            {
                                if (hhnode != null && hhnode.hasData && !hhnode.hasPersistentData)
                                    mreader.Position = htree.DataSize;

                                mtree.loadData(mreader, layer.Node);
                            }
                        }
                    }
                    temp.Add(layer);
                }

                TerrainChunkRenderable chunk = new TerrainChunkRenderable(state, htree, hnode, mtree, temp);
                if (chunk.IsValid)
                    terrainChunks.Add(chunk);
            }
        }

        private NativeReader GetReaderForNode(TerrainStreamingTree tree, RasterTreeNode node, Dictionary<Guid, NativeReader> chunkCache, out bool hiRes)
        {
            TerrainStreamingTreeNode snode = tree.GetNode(node.id);
            hiRes = false;

            if (snode != null)
            {
                if (!chunkCache.ContainsKey(snode.lod0Id))
                    chunkCache.Add(snode.lod0Id, new NativeReader(App.AssetManager.GetChunk(App.AssetManager.GetChunkEntry(snode.lod0Id))));

                return chunkCache[snode.lod0Id];
            }
                
            snode = tree.GetNode(node.parent.id);
            hiRes = true;

            if (!chunkCache.ContainsKey(snode.lod1Guid))
                chunkCache.Add(snode.lod1Guid, new NativeReader(App.AssetManager.GetChunk(App.AssetManager.GetChunkEntry(snode.lod1Guid))));

            return chunkCache[snode.lod1Guid];
        }
    }

    public struct TerrainVertex
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 TexCoords;
        public Vector3 Color;
    }

    public class BindableTextureWithData : BindableTexture
    {
        public BindableTextureWithData(Device device, Texture2DDescription description, bool srv, bool rtv, ShaderResourceViewDescription? srvDesc = null, RenderTargetViewDescription? rtvDesc = null, DataBox? data = null)
        {
            description.BindFlags |= (srv ? BindFlags.ShaderResource : BindFlags.None);
            description.BindFlags |= (rtv ? BindFlags.RenderTarget : BindFlags.None);
            description.BindFlags |= GetAdditionalFlags();
            Texture = new Texture2D(device, description, data.HasValue ? new[] { data.Value } : null);
            if (srv)
            {
                SRV = ((srvDesc != null) ? new ShaderResourceView(device, Texture, srvDesc.Value) : new ShaderResourceView(device, Texture));
            }
            if (rtv)
            {
                RTV = ((rtvDesc != null) ? new RenderTargetView(device, Texture, rtvDesc.Value) : new RenderTargetView(device, Texture));
            }
        }
    }

    public class TerrainChunkRenderable
    {
        public OrientedBoundingBox Bounds { get; private set; }
        public int Level { get; private set; }
        public bool IsValid { get; private set; }
        public int LayerCount { get; private set; }
        public List<Vector4> TexOffsets { get; private set; } = new List<Vector4>();

        private Buffer vertexBuffer;
        private Buffer indexBuffer;

        private int indexCount;
        private PrimitiveTopology primitiveType;

        private List<BindableTextureWithData> textures = new List<BindableTextureWithData>();
        private static Random r = new Random();

        public TerrainChunkRenderable(RenderCreateState state, HeightfieldTree htree, HeightfieldTreeNode hnode, TerrainMaskTree mtree/*, TerrainMaskTreeNode mnode*/, List<TerrainRenderable.LayerData> layerDatas)
        {
            Level = hnode.id.level;

            List<TerrainVertex> vertices = new List<TerrainVertex>();
            List<uint> indices = new List<uint>();

            float spacing = 0;
            float scale = 1.0f;
            int dim = 0;

            BoundingBox tmpBb = new BoundingBox();

            uint blurriness = htree.resourceBlurrinessFactor;
            float worldSizeY = htree.worldSizeY;
            int level = hnode.id.level;
            dim = (int)htree.nodeSamplesPerSide;
            spacing = ((hnode.size / (float)((dim - (htree.unknown11 * 2)) - 1))) * scale;
            tmpBb.Minimum.X = hnode.boundingBox.min.x * scale;
            tmpBb.Minimum.Y = hnode.boundingBox.min.y * scale;
            tmpBb.Minimum.Z = hnode.boundingBox.min.z * scale;

            tmpBb.Maximum.X = hnode.boundingBox.max.x * scale;
            tmpBb.Maximum.Y = hnode.boundingBox.max.y * scale;
            tmpBb.Maximum.Z = hnode.boundingBox.max.z * scale;

            Bounds = new OrientedBoundingBox(tmpBb);

            float minX = (tmpBb.Minimum.X - (spacing * 2));
            float minY = (tmpBb.Minimum.Z - (spacing * 2));

            Vector3 color = new Vector3(r.NextFloat(0, 1), r.NextFloat(0, 1), r.NextFloat(0, 1));

            float texspacing = 1.0f / (float)(dim - 1);
            float tmp = 1 * ((dim - 1) / (float)(mtree.tileSamplesPerSide * mtree.tilesPerNodeSide));
            int voffset = 0;

            for (int ny = 0; ny < 4; ny++)
            {
                for (int nx = 0; nx < 4; nx++)
                {
                    for (int y = 0; y < (dim / 4) + 1; y++)
                    {
                        for (int x = 0; x < (dim / 4) + 1; x++)
                        {
                            float newX = minX + (x * spacing) + (nx * ((dim / 4) * spacing/* + 1.0f*/));
                            float newY = minY + (y * spacing) + (ny * ((dim / 4) * spacing /*+ 1.0f*/));
                            float height = worldSizeY * (BitConverter.ToUInt16(hnode.data, (((ny * ((dim / 4)) + y) * dim) + (nx * ((dim / 4)) + x)) * 2) / (float)ushort.MaxValue);

                            //float div1 = 1 / 34.0f;
                            //float div2 = 1 / 33.5f;

                            float tx = (((x * 2) * (1 / 66.0f)) * 0.25f) + (nx * 0.25f);
                            float ty = (((y * 2) * (1 / 66.0f)) * 0.25f) + (ny * 0.25f);

                            //float tx = ((((x * 2) * (1 / 70.25f)) + (1 / 66.0f) + (1 / 66.0f)) * 0.25f) + (nx * 0.25f);
                            //float ty = ((((y * 2) * (1 / 70.25f)) + (1 / 66.0f) + (1 / 66.0f)) * 0.25f) + (ny * 0.25f);

                            //float tx = ((x * (1 / 34.0f) + div2) * 0.25f) + (nx * 0.25f);
                            //float ty = ((y * (1 / 34.0f) + div2) * 0.25f) + (ny * 0.25f);

                            //float tx = (x * ((1 / 33.0f) * 0.25f) + (nx * 0.25f)) + (tmp / (float)(dim - 1));
                            //float ty = (y * ((1 / 33.0f) * 0.25f) + (ny * 0.25f)) + (tmp / (float)(dim - 1));

                            vertices.Add(new TerrainVertex()
                            {
                                Position = new Vector3(newX, height, newY),
                                TexCoords = new Vector2(tx, ty),
                                Color = color
                            });
                        }
                    }

                    for (int y = 0; y < (dim / 4); y++)
                    {
                        for (int x = 0; x < (dim / 4); x++)
                        {
                            indices.Add((uint)(((y + 0) * ((dim / 4) + 1)) + (x + 0) + voffset));
                            indices.Add((uint)(((y + 1) * ((dim / 4) + 1)) + (x + 1) + voffset));
                            indices.Add((uint)(((y + 0) * ((dim / 4) + 1)) + (x + 1) + voffset));

                            indices.Add((uint)(((y + 0) * ((dim / 4) + 1)) + (x + 0) + voffset));
                            indices.Add((uint)(((y + 1) * ((dim / 4) + 1)) + (x + 0) + voffset));
                            indices.Add((uint)(((y + 1) * ((dim / 4) + 1)) + (x + 1) + voffset));
                        }
                    }

                    voffset = vertices.Count;
                }
            }
            //int z = 0;
            //for (int y = 0; y < dim; y++)
            //{
            //    for (int x = 0; x < dim; x++)
            //    {
            //        float newX = minX + (x * spacing);
            //        float newY = minY + (y * spacing);

            //        float y0 = 0;
            //        float y1 = worldSizeY;
            //        float height = y0 + (y1 - y0) * (BitConverter.ToUInt16(hnode.data, z * 2) / 65535.0f);

            //        //Vector3 color = new Vector3(
            //        //    (x >= htree.unknown11 && y >= htree.unknown11) ? 1.0f : 0.0f,
            //        //    0.0f, 0.0f);

            //        //float texspacing = 1.0f / (float)((dim - (htree.unknown11 * 2)));
            //        //float tx = (((x * texspacing) - (texspacing * 2)) * offset.Z) + offset.X;
            //        //float ty = (((y * texspacing) - (texspacing * 2)) * offset.W) + offset.Y;

            //        //float texspacing = 1.0f / (float)(dim);
            //        //float tx = (x * texspacing) + (tmp / (float)dim);
            //        //float ty = (y * texspacing) + (tmp / (float)dim);

            //        //float tx = (((x * texspacing)) * offset.Z) + offset.X;
            //        //float ty = (((y * texspacing)) * offset.W) + offset.Y;

            //        //if (height <= 0.0f)
            //        //{
            //        //    IsValid = false;
            //        //    return;
            //        //}

            //        vertices.Add(new TerrainVertex()
            //        {
            //            Position = new Vector3(newX, height, newY),
            //            TexCoords = new Vector2(0, 0),
            //            Color = color
            //        });

            //        z++;
            //    }
            //}

            //float texspacing = 1.0f / (float)(dim - 1);

            //for (int ny = 0; ny < 4; ny++)
            //{
            //    for (int nx = 0; nx < 4; nx++)
            //    {
            //        for (int y = 0; y < (dim / 4) + 1; y++)
            //        {
            //            for (int x = 0; x < (dim / 4) + 1; x++)
            //            {
            //                int vid = ((y + (ny * (dim / 4))) * dim) + (x + (nx * (dim / 4)));
            //                var vertex = vertices[vid];

            //                vertex.TexCoords.X = (x * ((1 / 32.0f) * 0.25f) + (nx * 0.25f)) + ((nx == 0) ? (tmp / (float)(dim - 1)) : 0);
            //                vertex.TexCoords.Y = (y * ((1 / 32.0f) * 0.25f) + (ny * 0.25f)) + ((ny == 0) ? (tmp / (float)(dim - 1)) : 0);

            //                vertices[vid] = vertex;
            //            }
            //        }
            //    }
            //}

            //for (int y = 0; y < (dim / 4) + 1; y++)
            //{
            //    for (int x = 0; x < (dim / 4) + 1; x++)
            //    {
            //        int vid = (y * dim) + (x + (dim / 4));
            //        var vertex = vertices[vid];

            //        vertex.TexCoords.X = (x * texspacing + 0.25f) + (tmp / (float)dim);
            //        vertex.TexCoords.Y = (y * texspacing) + (tmp / (float)dim);

            //        vertices[vid] = vertex;
            //    }
            //}

            //for (int y = 0; y < (dim / 4) + 1; y++)
            //{
            //    for (int x = 0; x < (dim / 4) + 1; x++)
            //    {
            //        int vid = ((y + (dim / 4)) * dim) + (x + (dim / 4));
            //        var vertex = vertices[vid];

            //        vertex.TexCoords.X = (x * texspacing + 0.5f) + (tmp / (float)dim);
            //        vertex.TexCoords.Y = (y * texspacing + 0.5f) + (tmp / (float)dim);

            //        vertices[vid] = vertex;
            //    }
            //}

            //for (int y = 0; y < (dim / 2) + 1; y++)
            //{
            //    for (int x = 0; x < (dim / 2) + 1; x++)
            //    {
            //        int vid = ((y + (dim / 2)) * dim) + x;
            //        var vertex = vertices[vid];

            //        vertex.TexCoords.X = (((x * texspacing2) + texspacing * mtree.blurrinessFactor * 2) * offset.Z) + offset.X;
            //        vertex.TexCoords.Y = (((y * texspacing + 0.5f) + texspacing * mtree.blurrinessFactor * 2) * offset.W) + offset.Y;

            //        vertices[vid] = vertex;
            //    }
            //}

            //for (int y = 0; y < dim - 1; y++)
            //{
            //    for (int x = 0; x < dim - 1; x++)
            //    {
            //        indices.Add((uint)(((y + 0) * dim) + (x + 0)));
            //        indices.Add((uint)(((y + 1) * dim) + (x + 1)));
            //        indices.Add((uint)(((y + 0) * dim) + (x + 1)));

            //        indices.Add((uint)(((y + 0) * dim) + (x + 0)));
            //        indices.Add((uint)(((y + 1) * dim) + (x + 0)));
            //        indices.Add((uint)(((y + 1) * dim) + (x + 1)));
            //    }
            //}

            calculateNormals(indices, ref vertices);
            vertexBuffer = Buffer.Create<TerrainVertex>(state.Device, BindFlags.VertexBuffer, vertices.ToArray());
            indexBuffer = Buffer.Create<uint>(state.Device, BindFlags.IndexBuffer, indices.ToArray());

            indexCount = (dim - 1) * (dim - 1) * 6;
            primitiveType = PrimitiveTopology.TriangleList;

            IsValid = true;
            //if (mnode == null)
            //    return;

            List<Dictionary<AtlasTileId, byte[]>> layers = new List<Dictionary<AtlasTileId, byte[]>>();
            int z = 0;

            byte[] whiteTile = new byte[mtree.tileSamplesPerSide * mtree.tileSamplesPerSide];
            for (int i = 0; i < (mtree.tileSamplesPerSide * mtree.tileSamplesPerSide); i++)
                whiteTile[i] = 0xFF;

            for (int i = 0; i < mtree.maxLayerCount; i++)
                layers.Add(new Dictionary<AtlasTileId, byte[]>());

            for (int i = 0; i < layerDatas.Count; i++)
            {
                z = 0;
                TerrainRenderable.LayerData layer = layerDatas[i];
                for (int j = 0; j < layer.Node.subtiles.Count; j++)
                {
                    TerrainMaskTreeNode.Subtile subtile = layer.Node.subtiles[j];
                    if (subtile.terrainLayerIndex == i)
                    {
                        if ((subtile.flags & 2) == 0)
                        {
                            layers[subtile.terrainLayerIndex].Add(subtile.atlasTileId, layer.Node.data[z]);
                        }
                        else
                        {
                            layers[subtile.terrainLayerIndex].Add(subtile.atlasTileId, whiteTile);
                        }
                    }

                    if ((subtile.flags & 2) == 0)
                        z++;
                }

                TexOffsets.Add(layer.Offsets);
            }

            //System.Diagnostics.Debug.Print(TexOffsets[1].ToString());

            //foreach (TerrainMaskTreeNode.Subtile subtile in mnode.subtiles)
            //{
            //    if ((subtile.flags & 2) == 0)
            //    {
            //        layers[subtile.terrainLayerIndex].Add(subtile.atlasTileId, mnode.data[z++]);
            //    }
            //    else
            //    {
            //        layers[subtile.terrainLayerIndex].Add(subtile.atlasTileId, whiteTile);
            //    }
            //}

            LayerCount = layers.Count;

            int layerId = 0;
            List<byte[]> totalLayerData = new List<byte[]>();
            for (int i = 0; i < layers.Count; i++)
                totalLayerData.Add(new byte[(mtree.tileSamplesPerSide * mtree.tilesPerNodeSide) * (mtree.tileSamplesPerSide * mtree.tilesPerNodeSide)]);

            foreach (Dictionary<AtlasTileId, byte[]> layer in layers)
            {
                for (int row = 0; row < mtree.tilesPerNodeSide; row++)
                {
                    for (int col = 0; col < mtree.tilesPerNodeSide; col++)
                    {
                        AtlasTileId tileId = new AtlasTileId() { indexX = (byte)(col + 1), indexY = (byte)(row + 1) };
                        int yOffset = (int)(row * mtree.tileSamplesPerSide);

                        if (layer.ContainsKey(tileId))
                        {
                            int xOffset = (int)(col * mtree.tileSamplesPerSide);
                            for (int y = 0; y < mtree.tileSamplesPerSide; y++)
                            {
                                Array.Copy(
                                    layer[tileId], (y * mtree.tileSamplesPerSide),
                                    totalLayerData[layerId], ((y + yOffset) * (mtree.tileSamplesPerSide * mtree.tilesPerNodeSide)) + xOffset,
                                    mtree.tileSamplesPerSide
                                    );

                                //for (int x = 0; x < mtree.tileSamplesPerSide; x++)
                                //    totalLayerData[layerId][((y + yOffset) * (mtree.tileSamplesPerSide * mtree.tilesPerNodeSide)) + (x + xOffset)] = layer[tileId][(y * mtree.tileSamplesPerSide) + x];
                            }
                        }
                    }
                }

                layerId++;
            }

            if (totalLayerData[1].All(b => b == 0x00))
                Console.WriteLine("TEST");

            Texture2DDescription texDesc = new Texture2DDescription()
            {
                ArraySize = 1,
                BindFlags = BindFlags.ShaderResource,
                CpuAccessFlags = CpuAccessFlags.None,
                Format = SharpDX.DXGI.Format.R8G8B8A8_UNorm,
                Height = (int)(mtree.tileSamplesPerSide * mtree.tilesPerNodeSide),
                MipLevels = 1,
                OptionFlags = ResourceOptionFlags.None,
                SampleDescription = new SharpDX.DXGI.SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
                Width = (int)(mtree.tileSamplesPerSide * mtree.tilesPerNodeSide)
            };

            for (int x = 0; x < ((layers.Count - 1) / 4) + 1; x++)
            {
                DataStream ds = new DataStream((int)(mtree.tileSamplesPerSide * mtree.tilesPerNodeSide) * (int)(mtree.tileSamplesPerSide * mtree.tilesPerNodeSide) * 4, false, true);
                for (int i = 0; i < ((mtree.tileSamplesPerSide * mtree.tilesPerNodeSide) * (mtree.tileSamplesPerSide * mtree.tilesPerNodeSide)); i++)
                {
                    if ((x * 4) + 0 < totalLayerData.Count) { ds.WriteByte(totalLayerData[(x * 4) + 0][i]); } else { ds.Write((byte)0x00); }
                    if ((x * 4) + 1 < totalLayerData.Count) { ds.WriteByte(totalLayerData[(x * 4) + 1][i]); } else { ds.Write((byte)0x00); }
                    if ((x * 4) + 2 < totalLayerData.Count) { ds.WriteByte(totalLayerData[(x * 4) + 2][i]); } else { ds.Write((byte)0x00); }
                    if ((x * 4) + 3 < totalLayerData.Count) { ds.WriteByte(totalLayerData[(x * 4) + 3][i]); } else { ds.Write((byte)0x00); }
                }

                DataBox box = new DataBox(ds.DataPointer, (int)(mtree.tileSamplesPerSide * mtree.tilesPerNodeSide) * 4, 0);
                textures.Add(new BindableTextureWithData(state.Device, texDesc, true, false, data: box));
                ds.Dispose();
            }
        }

        public void Dispose()
        {
            foreach (BindableTextureWithData texture in textures)
            {
                if (texture != null)
                    texture.Dispose();
            }
            vertexBuffer.Dispose();
            indexBuffer.Dispose();
        }

        public void Render(DeviceContext context, MeshRenderPath renderPath)
        {
            if (renderPath == MeshRenderPath.Deferred)
            {
                // override the permutation textures with the layer textures
                for (int i = 0; i < textures.Count; i++)
                    context.PixelShader.SetShaderResource(i + 1, textures[i].SRV);

                context.InputAssembler.PrimitiveTopology = primitiveType;
                context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(vertexBuffer, Utilities.SizeOf<TerrainVertex>(), 0));
                context.InputAssembler.SetIndexBuffer(indexBuffer, SharpDX.DXGI.Format.R32_UInt, 0);
                context.DrawIndexed(indexCount, 0, 0);
            }
        }

        protected void calculateNormals(List<uint> indices, ref List<TerrainVertex> vertices)
        {
            for (int i = 0; i < indices.Count; i += 3)
            {
                TerrainVertex v1 = vertices[(int)indices[i + 0]];
                TerrainVertex v2 = vertices[(int)indices[i + 1]];
                TerrainVertex v3 = vertices[(int)indices[i + 2]];

                Vector3 pos1 = v1.Position;
                Vector3 pos2 = v2.Position;
                Vector3 pos3 = v3.Position;

                Vector3 edge1 = (pos2 - pos1);
                Vector3 edge2 = (pos3 - pos1);

                Vector3 tangentX = edge1;
                Vector3 tangentZ = Vector3.Cross(edge2, edge1);

                tangentX.Normalize();
                tangentZ.Normalize();

                Vector3 tangentY = Vector3.Cross(tangentX, tangentZ);
                tangentY.Normalize();

                v1.Normal = tangentZ;
                v2.Normal = tangentZ;
                v3.Normal = tangentZ;

                vertices[(int)indices[i + 0]] = v1;
                vertices[(int)indices[i + 1]] = v2;
                vertices[(int)indices[i + 2]] = v3;
            }
        }
    }
}
