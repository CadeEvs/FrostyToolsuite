using FrostySdk.IO;
using FrostySdk.Managers;
using FrostySdk.Resources;
using SharpDX;
using System.Collections.Generic;
using System.IO;

namespace SvgImagePlugin
{
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

    public class SvgImage : Resource
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

        public SvgImage()
        {
        }

        public SvgImage(float inWidth, float inHeight)
        {
            width = inWidth;
            height = inHeight;
        }

        public override void Read(NativeReader reader, AssetManager am, ResAssetEntry entry, ModifiedResource modifiedData)
        {
            base.Read(reader, am, entry, modifiedData);
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
}
