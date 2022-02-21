using Frosty.Core;
using FrostySdk.IO;
using FrostySdk.Managers;
using FrostySdk.Resources;
using SharpDX;
using System.Collections.Generic;
using System.IO;

namespace TestPlugin.Resources
{
    // Classes that derive from Resource are used to specify any custom handling of res related data. Refer to App.AssetManager.GetResAs<T>
    // to obtain the resource data. A ModifiedResource specialization can also be used to store the data in a more specialized way.
    public class SvgImageResource : Resource
    {
        public float Width => width;
        public float Height => height;
        public IEnumerable<SvgShape> Shapes
        {
            get
            {
                for (int i = 0; i < shapes.Count; i++)
                    yield return shapes[i];
            }
        }

        private float width;
        private float height;
        private List<SvgShape> shapes = new List<SvgShape>();

        public SvgImageResource()
        {
        }

        public SvgImageResource(float inWidth, float inHeight)
        {
            width = inWidth;
            height = inHeight;
        }

        public override void Read(NativeReader reader, AssetManager am = null, ResAssetEntry entry = null, ModifiedResource modifiedData = null)
        {
            width = reader.ReadFloat();
            height = reader.ReadFloat();

            int shapeCount = reader.ReadInt();
            for (int i = 0; i < shapeCount; i++)
                shapes.Add(new SvgShape(reader));
        }

        public void ClearShapes()
        {
            shapes.Clear();
        }

        public void AddShape(SvgShape shape)
        {
            shapes.Add(shape);
        }

        public override byte[] SaveBytes()
        {
            using (NativeWriter writer = new NativeWriter(new MemoryStream()))
            {
                writer.Write(width);
                writer.Write(height);
                writer.Write(shapes.Count);
                foreach (SvgShape shape in shapes)
                    shape.Write(writer);

                return writer.ToByteArray();
            }
        }
    }

    // Classes that derive from EbxAsset are used to specify any custom handling of ebx related data, through the use of a
    // specialized ModifiedResource object. Refer to App.AssetManager.GetEbxAs<T> to obtain the ebx data
    public class SvgImageAsset : EbxAsset
    {
        // returns the modified svg if one is available, otherwise it will return the original
        public SvgImageResource Resource => modified != null ? modified.Resource : resource;

        // stores the original svg
        private SvgImageResource resource;

        // stores the modified svg
        private ModifiedSvgImageAsset modified;

        public SvgImageAsset()
        {
        }

        // performs custom actions when initial loading of the ebx has completed, in this case
        // it then loads the SVG resource
        public override void OnLoadComplete()
        {
            dynamic svgData = RootObject;
            resource = App.AssetManager.GetResAs<SvgImageResource>(App.AssetManager.GetResEntry(svgData.Resource));
        }

        // invoked during asset load to give the asset a chance to perform any logic required
        // when loading the modified data
        public override void ApplyModifiedResource(ModifiedResource modifiedResource)
        {
            modified = modifiedResource as ModifiedSvgImageAsset;
        }

        // invoked during asset save to save the modified data
        public override ModifiedResource SaveModifiedResource()
        {
            return modified;
        }
    }

    // ModifiedResource and any class that derives from it are the classes used when it is required to store
    // data in a serializable fashion as opposed to a regular ebx/res byte array, it provides its own Read/Write
    // functions, and allows for the user to store any data they see fit, as long as they can apply it onto
    // an asset/resource.
    public class ModifiedSvgImageAsset : ModifiedResource
    {
        // This ModifiedResource is for the ebx data, however it stores the RES data for an SVG image.
        // In fact, the RES file is never modified, until the mod manager invokes the custom handler.
        // where a custom resource is created and added to the runtimeResource container.
        public SvgImageResource Resource => resource;
        private SvgImageResource resource;

        public ModifiedSvgImageAsset()
        {
        }

        // Creates a new instance providing a svg image resource
        public ModifiedSvgImageAsset(SvgImageResource inResource)
        {
            resource = inResource;
        }

        // This function is responsible for reading in the modified data from the project file
        public override void ReadInternal(NativeReader reader)
        {
            // reads in the svg image resource from the project file
            resource = new SvgImageResource();
            resource.Read(reader);
        }

        // This function is responsible for writing out the modified data to the project file
        public override void SaveInternal(NativeWriter writer)
        {
            // writes out the svg image resource
            writer.Write(resource.SaveBytes());
        }
    }

    #region -- SvgImage objects --

    static class ReaderExtensions
    {
        public static Vector2 ReadVec2(this NativeReader reader)
        {
            Vector2 vec = new Vector2
            {
                X = reader.ReadFloat(),
                Y = reader.ReadFloat()
            };
            return vec;
        }
    }

    public class SvgPath
    {
        public bool Closed { get => closed != 0; set => closed = (byte)((value) ? 1 : 0); }
        public Vector2 MinBoundingBox => minBoundingBox;
        public Vector2 MaxBoundingBox => maxBoundingBox;
        public List<Vector2> Points => points;

        private List<Vector2> points = new List<Vector2>();

        private byte closed;
        private Vector2 minBoundingBox;
        private Vector2 maxBoundingBox;

        public SvgPath()
        {
            minBoundingBox = new Vector2();
            maxBoundingBox = new Vector2();

            minBoundingBox.X = float.MaxValue;
            minBoundingBox.Y = float.MaxValue;
            maxBoundingBox.X = float.MinValue;
            maxBoundingBox.Y = float.MinValue;
        }

        internal SvgPath(NativeReader reader)
        {
            int pointCount = reader.ReadInt();
            for (int i = 0; i < pointCount; i++)
                points.Add(reader.ReadVec2());

            closed = reader.ReadByte();
            minBoundingBox = reader.ReadVec2();
            maxBoundingBox = reader.ReadVec2();
        }

        public void AddPoint(Vector2 point)
        {
            points.Add(point);
            if (point.X < minBoundingBox.X)
                minBoundingBox.X = point.X;
            if (point.Y < minBoundingBox.Y)
                minBoundingBox.Y = point.Y;
            if (point.X > maxBoundingBox.X)
                maxBoundingBox.X = point.X;
            if (point.Y > maxBoundingBox.Y)
                maxBoundingBox.Y = point.Y;
        }

        internal void Write(NativeWriter writer)
        {
            writer.Write(points.Count);
            foreach (Vector2 pnt in points)
            {
                writer.Write(pnt.X);
                writer.Write(pnt.Y);
            }
            writer.Write(closed);
            writer.Write(minBoundingBox.X);
            writer.Write(minBoundingBox.Y);
            writer.Write(maxBoundingBox.X);
            writer.Write(maxBoundingBox.Y);
        }
    }

    public class SvgShape
    {
        public bool Stroke { get => stroke; set => stroke = value; }
        public uint StrokeColor { get => strokeColor; set => strokeColor = value; }
        public bool Fill { get => fill; set => fill = value; }
        public uint FillColor { get => fillColor; set => fillColor = value; }
        public float Opacity { get => opacity; set => opacity = value; }
        public float Thickness { get => thickness; set => thickness = value; }
        public byte Visible { get => visible; set => visible = value; }

        public IEnumerable<SvgPath> Paths
        {
            get
            {
                for (int i = 0; i < paths.Count; i++)
                    yield return paths[i];
            }
        }

        private bool stroke;
        private uint strokeColor;
        private bool fill;
        private uint fillColor;

        private float opacity;
        private float thickness;

        private byte[] unknownBytes;

        private byte visible;
        private Vector2 minBoundingBox;
        private Vector2 maxBoundingBox;

        private List<SvgPath> paths = new List<SvgPath>();

        public SvgShape()
        {
            opacity = 1.0f;
            thickness = 1.0f;
            unknownBytes = new byte[0x28];
            visible = 1;

            minBoundingBox = new Vector2();
            maxBoundingBox = new Vector2();

            minBoundingBox.X = float.MaxValue;
            minBoundingBox.Y = float.MaxValue;
            maxBoundingBox.X = float.MinValue;
            maxBoundingBox.Y = float.MinValue;
        }

        internal SvgShape(NativeReader reader)
        {
            fill = reader.ReadBoolean();
            if (fill)
                fillColor = reader.ReadUInt();
            stroke = reader.ReadBoolean();
            if (stroke)
                strokeColor = reader.ReadUInt();

            opacity = reader.ReadFloat();
            thickness = reader.ReadFloat();

            unknownBytes = reader.ReadBytes(0x28);

            visible = reader.ReadByte();
            minBoundingBox = reader.ReadVec2();
            maxBoundingBox = reader.ReadVec2();

            int pathCount = reader.ReadInt();
            for (int i = 0; i < pathCount; i++)
                paths.Add(new SvgPath(reader));
        }

        public void AddPaths(IEnumerable<SvgPath> inPaths)
        {
            paths.AddRange(inPaths);
            foreach (SvgPath path in inPaths)
            {
                if (path.MinBoundingBox.X < minBoundingBox.X)
                    minBoundingBox.X = path.MinBoundingBox.X;
                if (path.MinBoundingBox.Y < minBoundingBox.Y)
                    minBoundingBox.Y = path.MinBoundingBox.Y;
                if (path.MaxBoundingBox.X > maxBoundingBox.X)
                    maxBoundingBox.X = path.MaxBoundingBox.X;
                if (path.MaxBoundingBox.Y > maxBoundingBox.Y)
                    maxBoundingBox.Y = path.MaxBoundingBox.Y;
            }
        }

        internal void Write(NativeWriter writer)
        {
            writer.Write(fill);
            if (fill)
                writer.Write(fillColor);
            writer.Write(stroke);
            if (stroke)
                writer.Write(strokeColor);
            writer.Write(opacity);
            writer.Write(thickness);
            writer.Write(unknownBytes);
            writer.Write(visible);

            writer.Write(minBoundingBox.X);
            writer.Write(minBoundingBox.Y);
            writer.Write(maxBoundingBox.X);
            writer.Write(maxBoundingBox.Y);

            writer.Write(paths.Count);
            foreach (SvgPath path in paths)
                path.Write(writer);
        }
    }

    #endregion
}
