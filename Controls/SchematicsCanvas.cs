using LevelEditorPlugin.Editors;
using LevelEditorPlugin.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using LevelEditorPlugin.Managers;
using FrostySdk.IO;
using LevelEditorPlugin.Data;
using LevelEditorPlugin.Library.Reflection;
using System.Diagnostics;
using System.Collections.Specialized;
using System.Windows.Controls.Primitives;
using Frosty.Core.Managers;
using FrostySdk.Ebx;
using Entity = LevelEditorPlugin.Entities.Entity;

namespace LevelEditorPlugin.Controls
{
    public abstract class BaseVisual
    {
        public virtual object Data { get => null; }

        public Guid UniqueId;
        public Rect Rect;
        public bool IsSelected;

        public BaseVisual(double x, double y)
        {
            Rect.X = x;
            Rect.Y = y;
        }

        public virtual bool OnMouseOver(Point mousePos)
        {
            return false;
        }

        public virtual bool OnMouseDown(Point mousePos, MouseButton mouseButton)
        {
            return false;
        }

        public virtual bool OnMouseUp(Point mousePos, MouseButton mouseButton)
        {
            return false;
        }

        public virtual bool OnMouseLeave()
        {
            return false;
        }

        public virtual bool HitTest(Point mousePos)
        {
            return true;
        }

        public virtual void Move(Point newPos)
        {
            Rect.Location = newPos;
        }

        public abstract void Update();
        public abstract void Render(SchematicsCanvas.DrawingContextState state);
        public virtual void RenderDebug(SchematicsCanvas.DrawingContextState state)
        {
            // do nothing
        }
    }

    public abstract class BaseNodeVisual : BaseVisual
    {
        public class Port
        {
            public BaseNodeVisual Owner;
            public Rect Rect;
            public string Name;
            public int NameHash;
            public Type DataType;
            public bool IsConnected;
            public bool ShowWhileCollapsed;
            public bool IsDynamicallyGenerated;
            public int ConnectionCount;
            public bool IsHighlighted;

            public int PortType;
            public int PortDirection;

            public InterfaceShortcutNodeVisual ShortcutNode;
            public List<InterfaceShortcutNodeVisual> ShortcutChildren = new List<InterfaceShortcutNodeVisual>();

            public Port(BaseNodeVisual owner)
            {
                Owner = owner;
            }

            public Port(BaseNodeVisual owner, Type dataType)
            {
                Owner = owner;
                DataType = dataType;
            }
        }

        public string Title;
        public double GlyphWidth;
        public int ConnectionCount;
        public Port HightlightedPort;

        public BaseNodeVisual(double x, double y)
            : base(x, y)
        {
        }

        public abstract void ApplyLayout(SchematicsLayout layout, SchematicsCanvas canvas);
        public abstract SchematicsLayout.Node GenerateLayout();
        public abstract bool Matches(FrostySdk.Ebx.PointerRef pr, int nameHash, int direction);
        public abstract bool IsValid();
        public abstract Port Connect(string name, int nameHash, int portType, int direction);

        protected int HashString(string value)
        {
            if (value.StartsWith("0x"))
                return int.Parse(value.Substring(2), System.Globalization.NumberStyles.HexNumber);
            return Frosty.Hash.Fnv1.HashString(value);
        }
    }

    public class WirePointVisual : BaseVisual
    {
        public WireVisual Wire;

        public WirePointVisual(WireVisual wire, double x, double y)
            : base(x - 5, y - 5)
        {
            Wire = wire;
            Rect.Width = 10;
            Rect.Height = 10;
        }

        public override void Update()
        {
        }

        public override void Render(SchematicsCanvas.DrawingContextState state)
        {
            Pen wirePen = null;
            switch (Wire.WireType)
            {
                case 0: wirePen = state.WireLinkPen; break;
                case 1: wirePen = state.WireEventPen; break;
                case 2: wirePen = state.WirePropertyPen; break;
            }

            state.DrawingContext.DrawRectangle((IsSelected) ? state.NodeSelectedBrush : wirePen.Brush, null, new Rect(state.WorldMatrix.Transform(new Point(Rect.X, Rect.Y)), new Size(Rect.Width * state.Scale, Rect.Height * state.Scale)));
        }
    }

    public class CommentNodeVisual : BaseVisual
    {
        public override object Data => CommentData;

        public CommentNodeData CommentData;

        private Brush colorBrush;
        private Pen colorPen;
        private List<BaseVisual> nodes;

        public CommentNodeVisual(CommentNodeData commentData, IEnumerable<BaseVisual> selectedVisuals, double x, double y) 
            : base(x, y)
        {
            CommentData = commentData;

            nodes = new List<BaseVisual>();
            nodes.AddRange(selectedVisuals);

            colorBrush = new SolidColorBrush(Color.FromScRgb(1.0f, commentData.Color.x, commentData.Color.y, commentData.Color.z));
            colorPen = new Pen(colorBrush, 5.0);

            UpdatePositionAndSize();
        }

        public SchematicsLayout.Comment GenerateLayout()
        {
            SchematicsLayout.Comment commentLayout = new SchematicsLayout.Comment();
            commentLayout.UniqueId = UniqueId;
            commentLayout.Position = Rect.Location;
            commentLayout.Text = CommentData.CommentText;
            commentLayout.Color = Color.FromArgb(255, (byte)(CommentData.Color.x * 255.0f), (byte)(CommentData.Color.y * 255.0f), (byte)(CommentData.Color.z * 255.0f));
            commentLayout.Children = new List<Guid>();

            foreach (BaseVisual node in nodes)
            {
                commentLayout.Children.Add(node.UniqueId);
            }

            return commentLayout;
        }

        public void AddNode(BaseVisual node)
        {
            nodes.Add(node);
            UpdatePositionAndSize();
        }

        public override bool HitTest(Point mousePos)
        {
            Rect invalidZoneRect = new Rect(Rect.X + 5, Rect.Y + 20, Rect.Width - 10, Rect.Height - 25);
            return !invalidZoneRect.Contains(mousePos);
        }

        public override void Move(Point newPos)
        {
            Point offset = new Point(newPos.X - Rect.X, newPos.Y - Rect.Y);
            foreach (BaseVisual node in nodes)
            {
                if (node.IsSelected)
                    continue;

                Point newNodePos = new Point(node.Rect.X + offset.X, node.Rect.Y + offset.Y);
                node.Move(newNodePos);
            }

            base.Move(newPos);
        }

        public override void Update()
        {
            UpdatePositionAndSize();
        }

        public override void Render(SchematicsCanvas.DrawingContextState state)
        {
            colorPen.Thickness = 5.0 * state.Scale;
            colorPen.Brush = (IsSelected) ? Brushes.PaleGoldenrod : colorBrush;

            Rect outlineRect = new Rect(Rect.X + 2.5, Rect.Y + 2.5, Rect.Width - 5, Rect.Height - 5);
            Rect titleRect = new Rect(Rect.X, Rect.Y, Rect.Width, 20);

            state.DrawingContext.DrawRectangle(null, colorPen, state.TransformRect(outlineRect));
            state.DrawingContext.DrawRectangle(colorPen.Brush, null, state.TransformRect(titleRect));

            if (state.InvScale < 2.5)
            {
                Rect innerOutlineRect = new Rect(Rect.X + 5, Rect.Y + 20, Rect.Width - 10, Rect.Height - 25);
                Point titlePos = new Point(Rect.X + 5, Rect.Y + 3.5);

                state.DrawingContext.DrawRectangle(null, state.BlackPen, state.TransformRect(Rect));
                state.DrawingContext.DrawRectangle(null, state.BlackPen, state.TransformRect(innerOutlineRect));

                if (!string.IsNullOrEmpty(CommentData.CommentText))
                {
                    double luminance = (CommentData.Color.x * 0.3) + (CommentData.Color.y * 0.59) + (CommentData.Color.z * 0.11);
                    // comment title
                    GlyphRun titleGlyphRun = state.ConvertTextLinesToGlyphRun(state.TransformPoint(titlePos), true, CommentData.CommentText);
                    state.DrawingContext.DrawGlyphRun((luminance > 0.2) ? Brushes.Black : Brushes.White, titleGlyphRun);
                }
            }
        }

        private void UpdatePositionAndSize()
        {
            Rect rect = nodes[0].Rect;
            for (int i = 1; i < nodes.Count; i++)
            {
                rect.Union(nodes[i].Rect);
            }

            Rect.X = rect.X - 10;
            Rect.Y = rect.Y - 30;
            Rect.Width = rect.Width + 20;
            Rect.Height = rect.Height + 40;

            Color tmpColor = Color.FromScRgb(1.0f, CommentData.Color.x, CommentData.Color.y, CommentData.Color.z);
            if (tmpColor != (colorPen.Brush as SolidColorBrush).Color)
            {
                colorBrush = new SolidColorBrush(tmpColor);
                colorPen.Brush = colorBrush;
            }
        }
    }

    public class InterfaceNodeVisual : BaseNodeVisual
    {
        public override object Data => InterfaceData;

        public Guid BlueprintGuid;
        public InterfaceDescriptor InterfaceDescriptor;

        public object InterfaceData;

        public Port Self;
        public int Direction;
        public int FieldType;

        protected Geometry icon;
        protected Guid interfaceDescriptorGuid;

        protected InterfaceNodeVisual(Guid inGuid, InterfaceDescriptor interfaceDescriptor, int direction, double x, double y)
            : base(x, y)
        {
            BlueprintGuid = inGuid;
            InterfaceDescriptor = interfaceDescriptor;
            Direction = direction;

            if (interfaceDescriptor != null)
            {
                interfaceDescriptorGuid = Guid.Parse(interfaceDescriptor.Data.__InstanceGuid.ToString());
            }
        }

        public InterfaceNodeVisual(Guid inGuid, InterfaceDescriptor interfaceDescriptor, int direction, FrostySdk.Ebx.DataField dataField, double x, double y)
            : this(inGuid, interfaceDescriptor, direction, x, y)
        {
            InterfaceData = dataField;
            Self = new Port(this) { Name = dataField.Name, NameHash = HashString(dataField.Name), PortType = 2, PortDirection = direction };
            FieldType = 2;

            icon = PathGeometry.Parse("M 3.8490068 15.694406 C 2.8951868 14.441416 2.2924017 13.060398 2.0727974 11.625 1.9971471 11.130527 1.9971479 10.019909 2.0727989 9.578125 2.2366781 8.6211106 2.5655703 7.787545 3.09127 6.9968456 3.7245472 6.0443403 4.6392063 5.1577127 5.8785009 4.2950346 L 6.296875 4.0038029 6.4263153 4.1300984 6.5557556 4.256394 5.8607512 4.9485095 C 5.4784987 5.329173 5.0927762 5.7390625 5.0035897 5.859375 4.6537348 6.3313293 4.1174954 7.556974 3.8414373 8.515625 3.3593056 10.189895 3.2152789 12.272408 3.4880726 13.625 c 0.1093501 0.542191 0.4163267 1.311544 0.751816 1.884224 0.078488 0.133979 0.1374815 0.247827 0.1310961 0.252994 -0.077923 0.06307 -0.269156 0.175282 -0.2987095 0.175282 -0.021019 0 -0.1214898 -0.109393 -0.2232684 -0.243094 z m 9.1295782 0.0881 -0.13079 -0.143781 0.281414 -0.248356 c 0.52209 -0.46076 1.126417 -1.10588 1.347339 -1.438287 0.526179 -0.791711 1.060017 -2.162665 1.318105 -3.385037 C 15.97901 9.6938868 16.08161 8.5090203 16.052117 7.59375 16.010754 6.3101451 15.776332 5.4738386 15.170797 4.4496193 15.062068 4.2657113 15.037512 4.2016895 15.068541 4.1830106 c 0.02246 -0.01352 0.09935 -0.060281 0.170872 -0.1039133 l 0.130039 -0.079331 0.306494 0.4135732 c 0.920087 1.241538 1.370257 2.2592643 1.627951 3.6804109 0.07439 0.410254 0.08238 0.5299972 0.08445 1.265625 0.0025 0.8892256 -0.0283 1.1595086 -0.203255 1.7840256 -0.333161 1.189222 -1.274647 2.539818 -2.522638 3.618817 -0.322968 0.279235 -1.300759 1.023396 -1.465041 1.114989 -0.08724 0.04864 -0.08922 0.04778 -0.218824 -0.0947 z m -6.37843 -2.154203 c -0.2095397 -0.07238 -0.3713253 -0.20243 -0.49078 -0.394522 -0.083989 -0.13506 -0.09375 -0.177041 -0.09375 -0.403207 0 -0.224626 0.01069 -0.271395 0.096993 -0.424326 0.056007 -0.09925 0.1528828 -0.211491 0.2292544 -0.265625 0.1224219 -0.08678 0.1524031 -0.09375 0.4030069 -0.09375 0.2499418 0 0.2941353 0.0102 0.5751453 0.132812 0.340645 0.148627 0.4815575 0.163972 0.6349884 0.06915 C 8.0693478 12.17817 8.3529775 11.773999 8.7436706 11.125 l 0.2539656 -0.421875 -0.044221 -0.1875 C 8.9290944 10.4125 8.8234781 9.8798662 8.7187128 9.3319947 8.5148491 8.2658856 8.4502425 8.023869 8.3176388 7.8295672 8.1702413 7.6135881 8.0726519 7.5779369 7.6328125 7.5793875 L 7.25 7.58065 V 7.4011159 C 7.25 7.2227528 7.2506625 7.2214753 7.3515625 7.2052284 7.4074219 7.196234 7.7976562 7.1391969 8.21875 7.078479 8.6398438 7.0177613 9.1353194 6.9422651 9.3198069 6.9107098 c 0.3918512 -0.067024 0.3635384 -0.080856 0.53455 0.2611652 0.1518451 0.3036887 0.1923231 0.4390262 0.3230561 1.0801225 0.06286 0.3082762 0.122491 0.5602022 0.132505 0.5598353 0.01001 -3.656e-4 0.06014 -0.060132 0.111388 -0.1328125 0.2259 -0.3203597 0.742643 -0.9356991 1.002862 -1.1942119 0.422719 -0.4199454 0.732047 -0.5849188 1.107082 -0.5904379 0.55192 -0.00812 0.874373 0.290122 0.874402 0.8087545 10e-6 0.1855006 -0.01412 0.2433884 -0.09296 0.380849 -0.108776 0.1896629 -0.246424 0.3034885 -0.451511 0.3733688 -0.210805 0.071828 -0.382389 0.041679 -0.715086 -0.1256481 -0.157698 -0.079313 -0.325847 -0.1442032 -0.373665 -0.1442 -0.126825 7.8e-6 -0.304349 0.082558 -0.423435 0.1969015 -0.113955 0.1094175 -0.502736 0.6262094 -0.737927 0.980901 l -0.148978 0.2246725 0.223472 1.0812373 c 0.286237 1.384915 0.364046 1.639198 0.556021 1.817115 0.06059 0.05616 0.115659 0.07418 0.226639 0.07418 0.200408 0 0.415428 -0.157932 0.718326 -0.527609 0.128385 -0.15669 0.244702 -0.284891 0.258483 -0.284891 0.01378 0 0.08163 0.03744 0.150781 0.0832 0.08525 0.05641 0.119781 0.09869 0.107263 0.131312 -0.05797 0.151068 -0.427324 0.603932 -0.780977 0.957554 -0.589513 0.589461 -0.907641 0.756169 -1.388196 0.727452 C 10.25685 13.63296 10.09123 13.55999 9.9025788 13.371334 9.7181834 13.186933 9.4385697 12.528427 9.2672128 11.875 9.2198862 11.694531 9.1709294 11.528306 9.15842 11.50561 9.14359 11.4787 9.0558919 11.59832 8.9063891 11.84936 c -0.759046 1.274578 -1.1482932 1.684429 -1.7126147 1.803267 -0.2323338 0.04893 -0.4016397 0.04199 -0.5936194 -0.02432 z");
        }

        public InterfaceNodeVisual(Guid inGuid, InterfaceDescriptor interfaceDescriptor, int direction, FrostySdk.Ebx.DynamicEvent eventField, double x, double y)
            : this(inGuid, interfaceDescriptor, direction, x, y)
        {
            InterfaceData = eventField;
            Self = new Port(this) { Name = eventField.Name, NameHash = HashString(eventField.Name), PortType = 1, PortDirection = direction };
            FieldType = 1;

            icon = PathGeometry.Parse("m 9.84375 15.314723 c -0.2619556 -0.0539 -0.5079903 -0.252899 -0.6126919 -0.495574 -0.075513 -0.175023 -0.067182 -0.490302 0.017701 -0.669795 0.080295 -0.169792 0.2598549 -0.344046 0.4302324 -0.417519 0.1528766 -0.06593 0.4882155 -0.06633 0.6401575 -7.78e-4 0.651081 0.280907 0.675467 1.197174 0.04023 1.511397 -0.148233 0.07332 -0.3637059 0.103524 -0.5156247 0.07227 z M 7.0375991 13.356686 c -0.2794922 -0.139277 -0.4116916 -0.417483 -0.347441 -0.731169 0.032194 -0.157177 0.057878 -0.194456 0.2910147 -0.422392 0.5688438 -0.556153 1.3586303 -0.957366 2.2230859 -1.12933 0.3872316 -0.07703 1.2042513 -0.07703 1.5914823 0 0.864251 0.171924 1.668517 0.579946 2.224155 1.128363 0.230869 0.22787 0.257537 0.266833 0.289763 0.423359 0.06455 0.313516 -0.06763 0.591825 -0.347258 0.731169 -0.292131 0.145575 -0.52124 0.09647 -0.831053 -0.178109 -0.409637 -0.363056 -0.861441 -0.615031 -1.34123 -0.748014 C 10.583657 12.373338 10.46418 12.363143 10 12.363143 c -0.46418 0 -0.5836569 0.01019 -0.7901181 0.06742 -0.4797885 0.132983 -0.9315928 0.384958 -1.34123 0.748014 -0.3098125 0.274582 -0.5389216 0.323684 -0.8310528 0.178109 z M 5.046875 11.198074 C 4.7013613 11.015924 4.5704177 10.595902 4.7588861 10.274305 4.8437936 10.129421 5.3160384 9.6956156 5.6806797 9.4275422 7.6286394 7.9954609 10.124813 7.6195047 12.40625 8.4145841 c 0.905546 0.3155822 1.733966 0.8093522 2.479744 1.4780225 0.343903 0.3083464 0.436982 0.4569174 0.436761 0.6971514 C 15.322403 10.974415 15.050826 11.25 14.672116 11.25 14.432944 11.25 14.33627 11.2011 14.046875 10.933739 13.213771 10.164065 12.284999 9.6857234 11.156918 9.4453363 10.796304 9.3684916 10.706822 9.3618528 10.015625 9.3606644 9.3511344 9.3595206 9.2246519 9.3676641 8.90625 9.4320778 7.7655162 9.6628519 6.7953356 10.154793 5.9628256 10.924573 5.8199903 11.056645 5.6578734 11.183896 5.6025662 11.207352 5.4675206 11.264626 5.1634859 11.25955 5.046875 11.198074 Z M 3.203125 8.9706978 C 2.810182 8.8753772 2.5925106 8.5072275 2.7050437 8.1282878 2.7624783 7.9348844 2.8946108 7.7945684 3.4463292 7.3410916 4.9509989 6.10435 6.7462719 5.3390506 8.765625 5.0735512 c 0.5105341 -0.067124 1.838572 -0.075753 2.34375 -0.015229 1.35034 0.1617805 2.502359 0.5111043 3.652449 1.1075224 0.73048 0.378815 1.390542 0.8215813 1.973087 1.3235372 0.42063 0.3624397 0.507362 0.4615025 0.560386 0.6400544 C 17.421926 8.555839 17.111436 8.9783265 16.672182 8.9773193 16.453824 8.9768188 16.332609 8.9160113 16.0625 8.6714719 15.443532 8.1110994 14.74232 7.6366684 13.984375 7.2654397 13.371117 6.9650759 12.920177 6.7944247 12.336073 6.6416637 11.506909 6.4248122 10.863934 6.34375 9.97306 6.34375 7.7099891 6.34375 5.5588106 7.1817469 3.8920522 8.7126219 3.6419956 8.9422925 3.4234767 9.0241509 3.203125 8.9706978 Z");
        }

        public InterfaceNodeVisual(Guid inGuid, InterfaceDescriptor interfaceDescriptor, int direction, FrostySdk.Ebx.DynamicLink linkField, double x, double y)
            : this(inGuid, interfaceDescriptor, direction, x, y)
        {
            InterfaceData = linkField;
            Self = new Port(this) { Name = linkField.Name, NameHash = HashString(linkField.Name), PortType = 0, PortDirection = direction };
            FieldType = 0;

            icon = PathGeometry.Parse("M 6.4147411 16.562423 C 5.4937239 16.416143 4.7061036 15.962351 4.1625968 15.264837 3.8272056 14.83441 3.6109265 14.380603 3.4843925 13.841797 3.4366961 13.638696 3.426488 13.507328 3.4259609 13.089844 c -5.8e-4 -0.459348 0.00611 -0.532678 0.072736 -0.797537 C 3.6249393 11.790468 3.8074701 11.398406 4.107722 10.984167 4.178494 10.886527 4.8902887 10.153321 5.6894883 9.3548196 6.9718375 8.0735899 7.1749105 7.8816852 7.4177421 7.7216118 8.0418197 7.3102237 8.6429614 7.1288138 9.3815114 7.1289935 c 0.9432486 2.187e-4 1.7496176 0.3227172 2.4156406 0.9660754 0.217432 0.210032 0.581754 0.6782765 0.581754 0.7476977 0 0.033825 -1.1792 1.2009284 -1.249179 1.2363644 -0.08232 0.04168 -0.07766 0.04521 -0.131608 -0.099639 C 10.914213 9.7542013 10.772814 9.5489236 10.567516 9.3543583 10.22682 9.0314743 9.8471129 8.8789063 9.3842242 8.8789063 c -0.3031369 0 -0.5028103 0.048299 -0.8060992 0.194988 C 8.3472981 9.1855361 8.3358271 9.1961834 6.9066303 10.62538 5.4806881 12.051323 5.4665248 12.066574 5.35438 12.296875 c -0.3447858 0.708054 -0.2212486 1.477998 0.3247494 2.023996 0.5464266 0.546426 1.3203545 0.670225 2.0233642 0.32366 0.2117295 -0.104377 0.279177 -0.160834 0.7792969 -0.652317 l 0.5475064 -0.53805 0.2324219 0.07512 c 0.5327579 0.172194 1.1302372 0.243686 1.6542972 0.197946 0.180468 -0.01575 0.331069 -0.02628 0.334668 -0.02341 0.0036 0.0029 -0.470131 0.478385 -1.052734 1.056687 -1.20567 1.19677 -1.3965526 1.348827 -1.986672 1.582576 -0.4087738 0.161917 -0.7208157 0.222391 -1.2054186 0.233609 -0.240625 0.0056 -0.5066283 -8.51e-4 -0.5911183 -0.01427 z M 9.9978598 12.828662 C 9.3193151 12.705787 8.7042445 12.389265 8.2028476 11.904931 7.9854161 11.694899 7.6210937 11.226655 7.6210937 11.157233 c 0 -0.03383 1.1791998 -1.2009278 1.249179 -1.2363639 0.082319 -0.041685 0.077661 -0.045212 0.131608 0.099639 0.083906 0.225291 0.2253048 0.430568 0.4306033 0.625134 0.340696 0.322884 0.720403 0.475452 1.183292 0.475452 0.303137 0 0.50281 -0.0483 0.806099 -0.194988 0.2309 -0.111677 0.241841 -0.121839 1.670536 -1.5514862 C 14.520323 7.9457559 14.531334 7.9338855 14.644535 7.7013652 14.99014 6.9914744 14.867601 6.22586 14.320871 5.6791294 13.774444 5.1327028 13.000516 5.0089041 12.297506 5.355469 12.085777 5.4598459 12.018329 5.5163034 11.518209 6.0077857 L 10.970703 6.5458361 10.738281 6.4707146 C 10.205523 6.2985211 9.6080438 6.2270293 9.0839844 6.2727688 8.9035156 6.2885199 8.7529147 6.2990543 8.7493157 6.2961783 8.7457168 6.2933023 9.2194472 5.8177929 9.8020501 5.2394906 10.849581 4.1996923 11.014905 4.0555375 11.421875 3.8270785 11.64342 3.7027107 11.968492 3.5802209 12.295224 3.4979934 c 0.261537 -0.06582 0.33645 -0.072611 0.79462 -0.072032 0.417484 5.272e-4 0.548852 0.010735 0.751953 0.058432 0.538806 0.126534 0.992613 0.3428131 1.42304 0.6782043 0.62717 0.4886944 1.058501 1.1768833 1.25077 1.9956063 0.0477 0.2031006 0.05791 0.3344688 0.05843 0.7519531 5.8e-4 0.4593477 -0.0061 0.5326779 -0.07274 0.7975371 -0.126242 0.5018388 -0.308773 0.8939011 -0.609025 1.3081398 -0.07077 0.09764 -0.782566 0.8308458 -1.581766 1.629347 -1.282349 1.28123 -1.485422 1.473135 -1.728254 1.633208 -0.442539 0.291719 -0.873275 0.465095 -1.371205 0.551927 -0.290033 0.05058 -0.929575 0.04971 -1.2131932 -0.0017 z");
        }

        public override void ApplyLayout(SchematicsLayout layout, SchematicsCanvas canvas)
        {
            SchematicsLayout.Node node = layout.Nodes.FirstOrDefault(n => (n.InterfaceData.HasValue && n.InterfaceData.Value.NameHash == Self.NameHash && n.InterfaceData.Value.FieldType == FieldType && n.InterfaceData.Value.Direction == Direction));
            if (node.FileGuid == BlueprintGuid)
            {
                UniqueId = (node.UniqueId != Guid.Empty) ? node.UniqueId : UniqueId;

                Rect.X = node.Position.X;
                Rect.Y = node.Position.Y;

                if (node.ShortcutData.HasValue)
                {
                    foreach (SchematicsLayout.Shortcut shortcut in node.ShortcutData.Value.Shortcuts)
                    {
                        Port port = Self;
                        canvas.GenerateShortcuts(this, Self, shortcut);
                    }
                }
            }
        }

        public override SchematicsLayout.Node GenerateLayout()
        {
            Guid instanceGuid = (InterfaceDescriptor.Data.__InstanceGuid.IsExported) ? InterfaceDescriptor.Data.__InstanceGuid.ExportedGuid : Guid.Parse(InterfaceDescriptor.Data.__InstanceGuid.ToString());

            SchematicsLayout.Node node = new SchematicsLayout.Node();
            node.UniqueId = UniqueId;
            node.FileGuid = BlueprintGuid;
            node.InstanceGuid = instanceGuid;
            node.InterfaceData = new SchematicsLayout.InterfaceData() { NameHash = Self.NameHash, FieldType = FieldType, Direction = Direction };
            node.IsCollapsed = false;
            node.Position = Rect.Location;

            if (Self.ShortcutNode != null)
            {
                SchematicsLayout.ShortcutData shortcutData = new SchematicsLayout.ShortcutData();
                shortcutData.Shortcuts = new List<SchematicsLayout.Shortcut>();

                SchematicsLayout.Shortcut shortcut = new SchematicsLayout.Shortcut();
                shortcut.UniqueId = Self.ShortcutNode.UniqueId;
                shortcut.DisplayName = Self.ShortcutNode.ShortcutData.DisplayName;
                shortcut.Position = Self.ShortcutNode.Rect.Location;
                shortcut.ChildPositions = new List<Point>();
                shortcut.ChildGuids = new List<Guid>();

                foreach (InterfaceShortcutNodeVisual child in Self.ShortcutChildren)
                {
                    shortcut.ChildPositions.Add(child.Rect.Location);
                    shortcut.ChildGuids.Add(child.UniqueId);
                }

                shortcutData.Shortcuts.Add(shortcut);
                node.ShortcutData = shortcutData;
            }

            return node;
        }

        public override bool Matches(FrostySdk.Ebx.PointerRef pr, int nameHash, int direction)
        {
            return pr.External.FileGuid == BlueprintGuid && pr.External.ClassGuid == interfaceDescriptorGuid && nameHash == Self.NameHash && direction == Direction;
        }

        public override bool IsValid()
        {
            return ConnectionCount > 0;
        }

        public override Port Connect(string name, int nameHash, int portType, int direction)
        {
            Self.IsConnected = true;
            Self.ConnectionCount++;

            ConnectionCount++;
            return Self;
        }

        public override bool OnMouseOver(Point mousePos)
        {
            Rect portRect = new Rect(Rect.X + Self.Rect.X, Rect.Y + Self.Rect.Y, Self.Rect.Width, Self.Rect.Height);
            bool changedHighlight = false;

            if (portRect.Contains(mousePos))
            {
                Mouse.OverrideCursor = Cursors.Arrow;
                HightlightedPort = Self;
                Self.IsHighlighted = true;
                changedHighlight = true;
            }
            else if (Self.IsHighlighted)
            {
                HightlightedPort = null;
                Self.IsHighlighted = false;
                changedHighlight = true;
            }

            return changedHighlight;
        }

        public override bool OnMouseLeave()
        {
            if (HightlightedPort != null)
            {
                HightlightedPort.IsHighlighted = false;
                HightlightedPort = null;
                return true;
            }
            return false;
        }

        public override void Update()
        {
            Rect.Width = 20 + (Self.Name.Length * GlyphWidth) + 30;
            Rect.Width = Rect.Width + (10 - (Rect.Width % 10));
            Rect.Height = 20;

            if (Direction == 1)
            {
                Self.Rect = new Rect(4, 4, 12, 12);
            }
            else
            {
                Self.Rect = new Rect(Rect.Width - 16, 4, 12, 12);
            }
        }

        public override void Render(SchematicsCanvas.DrawingContextState state)
        {
            Point nodePosition = state.WorldMatrix.Transform(Rect.Location);
            Brush nodeBackgroundBrush = (IsSelected) ? state.NodeSelectedBrush : state.NodeBackgroundBrush;

            // draw background
            state.DrawingContext.DrawRectangle(nodeBackgroundBrush, state.BlackPen, new Rect(nodePosition.X, nodePosition.Y, Rect.Width * state.Scale, Rect.Height * state.Scale));

            if (state.InvScale >= 5)
                return;

            Brush connectorBrush = null;
            switch (FieldType)
            {
                case 0: connectorBrush = state.SchematicLinkBrush; break;
                case 1: connectorBrush = state.SchematicEventBrush; break;
                case 2: connectorBrush = state.SchematicPropertyBrush; break;
            }

            // draw connector
            state.DrawingContext.DrawRectangle((Self.IsHighlighted) ? state.NodeSelectedBrush : connectorBrush, state.BlackPen, new Rect(nodePosition.X + (Self.Rect.X * state.Scale), nodePosition.Y + (Self.Rect.Y * state.Scale), 12 * state.Scale, 12 * state.Scale));
            if (!Self.IsConnected)
                state.DrawingContext.DrawRectangle(nodeBackgroundBrush, state.BlackPen, new Rect(nodePosition.X + ((Self.Rect.X + 2) * state.Scale), nodePosition.Y + ((Self.Rect.Y + 2) * state.Scale), 8 * state.Scale, 8 * state.Scale));

            // draw symbol
            {
                double xOffset = (Direction == 0) ? 0 : (Rect.Width - 20) * state.Scale;
                state.DrawingContext.DrawRectangle(connectorBrush, state.BlackPen, new Rect(nodePosition.X + xOffset, nodePosition.Y, 20 * state.Scale, 20 * state.Scale));

                if (icon != null)
                {
                    xOffset = (Direction == 0) ? 0 : (Rect.Width - 20);

                    TransformGroup group = new TransformGroup();
                    group.Children.Add(new ScaleTransform(state.Scale, state.Scale));
                    group.Children.Add(new TranslateTransform(nodePosition.X + xOffset * state.Scale, nodePosition.Y));

                    state.DrawingContext.PushTransform(group);
                    state.DrawingContext.DrawGeometry(Brushes.Black, null, icon);
                    state.DrawingContext.Pop();
                }
            }

            if (state.InvScale < 2.5)
            {
                if (!string.IsNullOrEmpty(Self.Name))
                {
                    // node title
                    double xOffset = (Direction == 1) ? (20 * state.Scale) : ((Rect.Width - 20) * state.Scale) - Self.Name.Length * state.LargeFont.AdvanceWidth;
                    GlyphRun titleGlyphRun = state.ConvertTextLinesToGlyphRun(new Point(nodePosition.X + xOffset, nodePosition.Y + (4 * state.Scale)), true, Self.Name);
                    state.DrawingContext.DrawGlyphRun(Brushes.Black, titleGlyphRun);
                }
            }
        }
    }

    public class InterfaceShortcutNodeVisual : InterfaceNodeVisual
    {
        public override object Data => ShortcutData;

        public BaseNodeVisual OwnerNode;
        public Port OwnerPort;

        public IShortcutNodeData ShortcutData;

        public InterfaceShortcutNodeVisual(BaseNodeVisual node, Port port, IShortcutNodeData inData, int direction, double x, double y)
            : base(Guid.Empty, null, direction, x, y)
        {
            ShortcutData = inData;

            OwnerNode = node;
            OwnerPort = port;
            Rect.Size = node.Rect.Size;
            FieldType = port.PortType;
            Self = new Port(this) { Name = port.Name, NameHash = port.NameHash };
            GlyphWidth = node.GlyphWidth;

            icon = Geometry.Parse("M 8.888587 17.982632 C 8.0968795 17.955544 7.2488877 17.851374 6.5626389 17.696906 5.4000834 17.435226 4.5358167 17.0054 4.111736 16.477999 3.994131 16.331741 3.8612254 16.07883 3.8176485 15.918367 c -0.014632 -0.05388 -0.030536 -0.189796 -0.035343 -0.30204 -0.010268 -0.239767 0.021885 -0.409145 0.1166632 -0.614558 0.3388827 -0.734465 1.4424479 -1.361551 2.9108137 -1.654029 0.1748641 -0.03483 0.4133152 -0.07564 0.5298913 -0.09068 0.1165761 -0.01504 0.239213 -0.0322 0.2725265 -0.03812 l 0.06057 -0.01077 -1.2804389 -2 C 5.0099783 9.048977 4.9527379 8.9517267 4.7871107 8.4809246 4.646399 8.0809451 4.5650893 7.7172318 4.5254051 7.3102638 4.4988012 7.0374385 4.5154273 6.4394949 4.5570707 6.1714286 4.6861372 5.3406039 5.0123269 4.5894207 5.5389725 3.9102041 5.680467 3.7277182 6.0293892 3.36546 6.2145958 3.2087582 7.2282394 2.351122 8.5129689 1.929703 9.8470992 2.0172189 11.380677 2.117818 12.800316 2.9236621 13.656509 4.1795918 14.21005 4.991569 14.489173 5.8946364 14.489111 6.8733693 14.489062 7.6314134 14.327698 8.3202909 13.99383 8.9877551 13.919515 9.1363249 13.463632 9.8902867 12.769721 11.012245 12.16159 11.99551 11.183455 13.578776 10.596089 14.530612 10.008723 15.482449 9.5195287 16.2706 9.5089917 16.282058 9.493955 16.29841 9.3795546 16.12678 8.9772536 15.484314 L 8.4646739 14.665737 8.326087 14.676657 c -0.9899466 0.078 -2.1502448 0.37234 -2.7068462 0.686655 -0.1005755 0.05679 -0.2768495 0.181239 -0.2768495 0.195446 0 0.01702 0.1999966 0.157469 0.3097826 0.217545 0.5912757 0.323552 1.6397459 0.569981 2.8369565 0.66679 0.4173097 0.03374 1.5137986 0.03877 1.9076086 0.0087 1.389354 -0.105943 2.526354 -0.386235 3.089906 -0.76172 0.192881 -0.128513 0.191071 -0.123087 0.06989 -0.209582 -0.343693 -0.245309 -1.131971 -0.523651 -1.907611 -0.673579 l -0.141841 -0.02742 0.05349 -0.08364 c 0.02942 -0.046 0.224741 -0.351806 0.43405 -0.679561 0.209309 -0.327756 0.382157 -0.598168 0.384106 -0.600917 0.0059 -0.0084 0.458092 0.108237 0.691931 0.178451 0.12106 0.03635 0.322826 0.10675 0.44837 0.156444 0.937499 0.371096 1.486182 0.85133 1.657905 1.451079 0.03105 0.10844 0.03689 0.167309 0.03631 0.365963 -6.15e-4 0.210595 -0.0056 0.252959 -0.04554 0.383673 -0.154562 0.506425 -0.556884 0.90621 -1.257373 1.249447 -1.159089 0.567949 -2.982414 0.851942 -5.021739 0.782165 z M 9.8660693 8.373544 C 10.602557 8.2571513 11.246517 7.7302759 11.509928 7.0285714 11.602003 6.7832922 11.626179 6.634566 11.625373 6.3183673 11.624672 6.0434354 11.621371 6.0120831 11.574148 5.8320408 11.516907 5.6138046 11.404852 5.3656194 11.277041 5.1739899 11.165992 5.0074934 10.881334 4.7242078 10.714674 4.614336 9.9196521 4.0902103 8.9126518 4.1253055 8.1700154 4.7030206 7.7776821 5.0082263 7.5098685 5.4384856 7.4033639 5.9346939 c -0.041319 0.1925089 -0.041319 0.5911645 0 0.7836734 0.137521 0.6407156 0.5400483 1.1616998 1.1265274 1.4580454 0.4119564 0.20816 0.8538602 0.2733556 1.336178 0.1971313 z");

            Update();
        }

        public override SchematicsLayout.Node GenerateLayout()
        {
            return new SchematicsLayout.Node();
        }

        public override void Update()
        {
            if (ShortcutData.DisplayName != Self.Name)
            {
                Self.Name = ShortcutData.DisplayName;
                Update();

                foreach (InterfaceShortcutNodeVisual child in OwnerPort.ShortcutChildren)
                {
                    child.ShortcutData.DisplayName = Self.Name;
                    child.Update();
                }
            }

            base.Update();
        }

        public override bool OnMouseOver(Point mousePos)
        {
            Rect portRect = new Rect(Rect.X + Self.Rect.X, Rect.Y + Self.Rect.Y, Self.Rect.Width, Self.Rect.Height);
            bool changedHighlight = false;

            if (portRect.Contains(mousePos))
            {
                Mouse.OverrideCursor = Cursors.Arrow;
                HightlightedPort = Self;
                Self.IsHighlighted = true;
                changedHighlight = true;
            }
            else if (Self.IsHighlighted)
            {
                HightlightedPort = null;
                Self.IsHighlighted = false;
                changedHighlight = true;
            }

            return changedHighlight;
        }

        public override bool OnMouseLeave()
        {
            if (HightlightedPort != null)
            {
                HightlightedPort.IsHighlighted = false;
                HightlightedPort = null;
                return true;
            }
            return false;
        }

        public override void Render(SchematicsCanvas.DrawingContextState state)
        {
            base.Render(state);

            if (IsSelected)
            {
                if (this == OwnerPort.ShortcutNode)
                {
                    double targetX = (Direction == 0) ? Rect.X : Rect.X + Rect.Width;
                    for (int i = 0; i < OwnerPort.ShortcutChildren.Count; i++)
                    {
                        InterfaceShortcutNodeVisual childNode = OwnerPort.ShortcutChildren[i];
                        double sourceX = (childNode.Direction == 0) ? childNode.Rect.X : childNode.Rect.X + childNode.Rect.Width;

                        state.DrawingContext.DrawLine(state.ShortcutWirePen,
                            state.WorldMatrix.Transform(new Point(sourceX, childNode.Rect.Y + childNode.Self.Rect.Y + 6)),
                            state.WorldMatrix.Transform(new Point(targetX, Rect.Y + Self.Rect.Y + 6)));

                        if (state.InvScale < 2.5)
                        {
                            state.DrawingContext.DrawRectangle(state.ShortcutWirePen.Brush, null, new Rect(
                                state.WorldMatrix.Transform(new Point(sourceX - 5, childNode.Rect.Y + childNode.Self.Rect.Y + 1)), new Size(10 * state.Scale, 10 * state.Scale)
                                ));
                            state.DrawingContext.DrawRectangle(state.ShortcutWirePen.Brush, null, new Rect(
                                state.WorldMatrix.Transform(new Point(targetX - 5, Rect.Y + Self.Rect.Y + 1)), new Size(10 * state.Scale, 10 * state.Scale)
                                ));
                        }
                    }
                }
                else
                {
                    double sourceX = (OwnerPort.ShortcutNode.Direction == 0) ? OwnerPort.ShortcutNode.Rect.X : OwnerPort.ShortcutNode.Rect.X + OwnerPort.ShortcutNode.Rect.Width;
                    double targetX = (Direction == 0) ? Rect.X : Rect.X + Rect.Width;

                    state.DrawingContext.DrawLine(state.ShortcutWirePen,
                        state.WorldMatrix.Transform(new Point(sourceX, OwnerPort.ShortcutNode.Rect.Y + OwnerPort.ShortcutNode.Self.Rect.Y + 6)),
                        state.WorldMatrix.Transform(new Point(targetX, Rect.Y + Self.Rect.Y + 6))
                        );

                    if (state.InvScale < 2.5)
                    {
                        state.DrawingContext.DrawRectangle(state.ShortcutWirePen.Brush, null, new Rect(
                            state.WorldMatrix.Transform(new Point(sourceX - 5, OwnerPort.ShortcutNode.Rect.Y + OwnerPort.ShortcutNode.Self.Rect.Y + 1)), new Size(10 * state.Scale, 10 * state.Scale)
                            ));
                        state.DrawingContext.DrawRectangle(state.ShortcutWirePen.Brush, null, new Rect(
                            state.WorldMatrix.Transform(new Point(targetX - 5, Rect.Y + Self.Rect.Y + 1)), new Size(10 * state.Scale, 10 * state.Scale)
                            ));
                    }
                }
            }
        }
    }

    public class NodeVisual : BaseNodeVisual
    {
        public override object Data => Entity;

        public bool IsCollapsed;
        public bool CollapseHover;
        public int Realm;

        public List<Port> InputLinks = new List<Port>();
        public List<Port> OutputLinks = new List<Port>();
        public List<Port> InputEvents = new List<Port>();
        public List<Port> OutputEvents = new List<Port>();
        public List<Port> InputProperties = new List<Port>();
        public List<Port> OutputProperties = new List<Port>();
        public Port Self;

        public IEnumerable<Port> AllPorts => InputLinks.Concat(OutputLinks.Concat(InputEvents.Concat(OutputEvents.Concat(InputProperties.Concat(OutputProperties.Concat(new[] { Self }))))));

        public ILogicEntity Entity;

        public ContextMenu NodeContextMenu;
        public ContextMenu PortContextMenu;

        public NodeVisual(ILogicEntity entity, double x, double y)
            : base(x, y)
        {
            Entity = entity;
            Title = Entity.DisplayName;
            IsCollapsed = true;
            IsSelected = false;
            CollapseHover = false;

            // main connector
            Self = new Port(this) { Name = "", NameHash = 0, Rect = new Rect(4, 4, 12, 12), PortType = 0, PortDirection = 1, DataType = Entity.GetType() };
        }

        public override bool OnMouseOver(Point mousePos)
        {
            Rect collapseButtonRect = new Rect(Rect.X + (Rect.Width - 16), Rect.Y + 4, 12, 12);
            bool oldCollapseHover = CollapseHover;

            CollapseHover = false;
            if (collapseButtonRect.Contains(mousePos))
            {
                Mouse.OverrideCursor = Cursors.Arrow;
                CollapseHover = true;
            }

            bool changedHighlight = false;
            foreach (Port port in AllPorts)
            {
                Rect portRect = new Rect(Rect.X + port.Rect.X, Rect.Y + port.Rect.Y, port.Rect.Width, port.Rect.Height);
                if (portRect.Contains(mousePos))
                {
                    Mouse.OverrideCursor = Cursors.Arrow;
                    HightlightedPort = port;
                    port.IsHighlighted = true;
                    changedHighlight = true;
                }
                else if (port.IsHighlighted)
                {
                    if (HightlightedPort == port)
                    {
                        HightlightedPort = null;
                    }
                    port.IsHighlighted = false;
                    changedHighlight = true;
                }
            }

            return oldCollapseHover != CollapseHover || changedHighlight;
        }

        public override bool OnMouseDown(Point mousePos, MouseButton mouseButton)
        {
            return false;
        }

        public override bool OnMouseUp(Point mousePos, MouseButton mouseButton)
        {
            if (mouseButton == MouseButton.Left)
            {
                if (CollapseHover)
                {
                    IsCollapsed = !IsCollapsed;
                    Update();
                    return true;
                }
            }
            else if (mouseButton == MouseButton.Right)
            {
                foreach (Port port in AllPorts)
                {
                    if (!port.IsConnected || port.ShortcutNode != null)
                        continue;

                    Rect portRect = new Rect(Rect.X + port.Rect.X, Rect.Y + port.Rect.Y, 12, 12);
                    if (portRect.Contains(mousePos))
                    {
                        PortContextMenu.DataContext = port;
                        PortContextMenu.IsOpen = true;
                        return true;
                    }
                }

                if (NodeContextMenu != null && Rect.Contains(mousePos))
                {
                    NodeContextMenu.DataContext = this;
                    NodeContextMenu.IsOpen = true;
                    return true;
                }
            }

            return false;
        }

        public override bool OnMouseLeave()
        {
            bool invalidate = false;
            if (CollapseHover)
            {
                CollapseHover = false;
                invalidate = true;
            }
            if (HightlightedPort != null)
            {
                HightlightedPort.IsHighlighted = false;
                HightlightedPort = null;
                invalidate = true;
            }
            
            return invalidate;
        }

        public override void ApplyLayout(SchematicsLayout layout, SchematicsCanvas canvas)
        {
            SchematicsLayout.Node node = layout.Nodes.FirstOrDefault(n => n.FileGuid == Entity.FileGuid && n.InstanceGuid == Entity.InstanceGuid);
            if (node.FileGuid == Entity.FileGuid)
            {
                UniqueId = (node.UniqueId != Guid.Empty) ? node.UniqueId : UniqueId;
                IsCollapsed = node.IsCollapsed;
                Rect.X = node.Position.X;
                Rect.Y = node.Position.Y;

                if (node.ShortcutData.HasValue)
                {
                    foreach (SchematicsLayout.Shortcut shortcut in node.ShortcutData.Value.Shortcuts)
                    {
                        Port port = Self;
                        if (shortcut.ExtraData.HasValue)
                        {
                            port = AllPorts.FirstOrDefault(p => p.NameHash == shortcut.ExtraData.Value.NameHash && p.PortDirection == shortcut.ExtraData.Value.Direction && p.PortType == shortcut.ExtraData.Value.FieldType);
                        }
                        canvas.GenerateShortcuts(this, port, shortcut);
                    }
                }
            }
        }

        public override SchematicsLayout.Node GenerateLayout()
        {
            SchematicsLayout.Node node = new SchematicsLayout.Node();
            node.UniqueId = UniqueId;
            node.FileGuid = Entity.FileGuid;
            node.InstanceGuid = Entity.InstanceGuid;
            node.IsCollapsed = IsCollapsed;
            node.Position = Rect.Location;

            List<SchematicsLayout.Shortcut> shortcuts = new List<SchematicsLayout.Shortcut>();
            foreach (Port port in AllPorts)
            {
                if (port.ShortcutNode != null)
                {
                    SchematicsLayout.Shortcut shortcut = new SchematicsLayout.Shortcut();
                    shortcut.UniqueId = port.ShortcutNode.UniqueId;
                    shortcut.DisplayName = port.ShortcutNode.ShortcutData.DisplayName;
                    shortcut.Position = port.ShortcutNode.Rect.Location;
                    shortcut.ChildPositions = new List<Point>();
                    shortcut.ChildGuids = new List<Guid>();

                    if (port != Self)
                    {
                        SchematicsLayout.InterfaceData extraData = new SchematicsLayout.InterfaceData();
                        extraData.NameHash = port.NameHash;
                        extraData.Direction = port.PortDirection;
                        extraData.FieldType = port.PortType;
                        shortcut.ExtraData = extraData;
                    }

                    foreach (InterfaceShortcutNodeVisual child in port.ShortcutChildren)
                    {
                        shortcut.ChildPositions.Add(child.Rect.Location);
                        shortcut.ChildGuids.Add(child.UniqueId);
                    }

                    shortcuts.Add(shortcut);
                }
            }

            if (shortcuts.Count > 0)
            {
                SchematicsLayout.ShortcutData shortcutData = new SchematicsLayout.ShortcutData();
                shortcutData.Shortcuts = shortcuts;
                node.ShortcutData = shortcutData;
            }

            return node;
        }

        public override bool Matches(FrostySdk.Ebx.PointerRef pr, int nameHash, int direction)
        {
            Guid entityFileGuid = Entity.FileGuid;
            return pr.External.FileGuid == entityFileGuid && pr.External.ClassGuid == Entity.InstanceGuid;
        }

        public override bool IsValid()
        {
            if (ConnectionCount != 0)
                return true;

            if (Entity is ISpatialEntity && !(Entity is INotRealSpatialEntity))
                return false;

            if (Entity is IHideInSchematicsView)
                return false;

            return true;
        }

        public override Port Connect(string name, int nameHash, int portType, int direction)
        {
            List<Port> inputList = null;
            List<Port> outputList = null;

            if (nameHash == 0)
            {
                Self.IsConnected = true;
                Self.ConnectionCount++;
                ConnectionCount++;

                return Self;
            }

            switch (portType)
            {
                case 0: inputList = InputLinks; outputList = OutputLinks; break;
                case 1: inputList = InputEvents; outputList = OutputEvents; break;
                case 2: inputList = InputProperties; outputList = OutputProperties; break;
            }

            Port port = null;
            switch (direction)
            {
                case 0: port = inputList.FirstOrDefault(p => p.NameHash == nameHash); break;
                case 1: port = outputList.FirstOrDefault(p => p.NameHash == nameHash); break;
            };

            // if port does not exist, create a dynamic one
            if (port == null)
            {
                // check to see if hash is found in strings.txt
                string foundName = FrostySdk.Utils.GetString(nameHash);
                if (foundName != name)
                {
                    name = foundName;
                    nameHash = HashString(name);
                }
                
                port = new Port(this)
                {
                    Name = name,
                    NameHash = nameHash,
                    IsDynamicallyGenerated = true,
                    PortType = portType,
                    PortDirection = 1- direction
                };
                
                if (direction == 0)
                {
                    inputList.Add(port);
                }
                else
                {
                    outputList.Add(port);
                }
            }

            port.IsConnected = true;
            port.ConnectionCount++;

            ConnectionCount++;
            return port;
        }

        public override void Update()
        {
            int connectorIdx = 0;
            int offset = 0;
            int linkCount = 0;
            int eventCount = 0;
            int propertyCount = 0;

            UpdatePorts();

            Realm = (int)Entity.Realm;

            Rect.Width = CalculateNodeWidth(out linkCount, out eventCount, out propertyCount);
            Rect.Height = 20 + 4;

            // header rows
            Rect.Height += Entity.HeaderRows.Count() * 10.0;
            double headerHeight = Rect.Height - 4;

            // input links
            foreach (Port port in InputLinks)
            {
                if (!IsCollapsed || port.IsConnected)
                {
                    port.Rect = new Rect(4, ((headerHeight + 4 + (15 * connectorIdx))), 12, 12);
                    connectorIdx++;
                }
            }

            offset += (linkCount != 0 && eventCount != 0) ? 5 : 0;
            connectorIdx = linkCount;

            // input events
            foreach (Port port in InputEvents)
            {
                if ((!IsCollapsed || port.IsConnected) || port.ShowWhileCollapsed)
                {
                    port.Rect = new Rect(4, ((headerHeight + offset + 4 + (15 * connectorIdx))), 12, 12);
                    connectorIdx++;
                }
            }

            offset += (eventCount != 0 || linkCount != 0) ? 5 : 0;
            connectorIdx = eventCount + linkCount;

            // input properties
            foreach (Port port in InputProperties)
            {
                if ((!IsCollapsed || port.IsConnected) || port.ShowWhileCollapsed)
                {
                    port.Rect = new Rect(4, ((headerHeight + 4 + offset + (15 * connectorIdx))), 12, 12);
                    connectorIdx++;
                }
            }



            connectorIdx = 0;
            offset = 0;

            // output links
            foreach (Port port in OutputLinks)
            {
                if ((!IsCollapsed || port.IsConnected) || port.ShowWhileCollapsed)
                {
                    port.Rect = new Rect(Rect.Width - 4 - 12, ((headerHeight + 4 + (15 * connectorIdx))), 12, 12);
                    connectorIdx++;
                }
            }

            offset += (linkCount != 0 && eventCount != 0) ? 5 : 0;
            connectorIdx = linkCount;

            // output events
            foreach (Port port in OutputEvents)
            {
                if ((!IsCollapsed || port.IsConnected) || port.ShowWhileCollapsed)
                {
                    port.Rect = new Rect(Rect.Width - 4 - 12, ((headerHeight + offset + 4 + (15 * connectorIdx))), 12, 12);
                    connectorIdx++;
                }
            }

            offset += (eventCount != 0 || linkCount != 0) ? 5 : 0;
            connectorIdx = eventCount + linkCount;

            // output properties
            foreach (Port port in OutputProperties)
            {
                if ((!IsCollapsed || port.IsConnected) || port.ShowWhileCollapsed)
                {
                    port.Rect = new Rect(Rect.Width - 4 - 12, ((headerHeight + 4 + offset + (15 * connectorIdx))), 12, 12);
                    connectorIdx++;
                }
            }

            Rect.Height += eventCount * 15 + propertyCount * 15 + linkCount * 15;

            if (linkCount > 0 && eventCount > 0)
                Rect.Height += 5;
            if ((eventCount > 0 || linkCount > 0) && propertyCount > 0)
                Rect.Height += 5;

            Rect.Height += 1;
        }

        public override void Render(SchematicsCanvas.DrawingContextState state)
        {
            DrawHeader(state);
            DrawBody(state);
        }

        public override void RenderDebug(SchematicsCanvas.DrawingContextState state)
        {
            IEnumerable<string> debugRows = Entity.DebugRows;
            if (debugRows.Count() == 0)
                return;

            double debugHeight = (debugRows.Count() * 10.0);

            Point nodePosition = state.WorldMatrix.Transform(new Point(Rect.Location.X, Rect.Location.Y - debugHeight - 9));
            Size nodeSize = new Size(Rect.Size.Width * state.Scale, (debugHeight + 5) * state.Scale);
            Point p = new Point(5, debugHeight + 5);

            if (state.InvScale < 1.5)
            {

                CombinedGeometry cg = new CombinedGeometry(
                    new RectangleGeometry(new Rect(nodePosition, nodeSize)),
                    new PathGeometry(new[]
                    {
                        new PathFigure(new Point(nodePosition.X + (p.X * state.Scale), nodePosition.Y + p.Y * state.Scale), new []
                        {
                            new PolyLineSegment(new []
                            {
                                new Point(nodePosition.X + (p.X + 8) * state.Scale, nodePosition.Y + p.Y * state.Scale),
                                new Point(nodePosition.X + (p.X + 4) * state.Scale, nodePosition.Y + (p.Y + 4) * state.Scale)
                            }, true)

                        }, true)
                    }));

                state.DrawingContext.DrawGeometry(Brushes.White, state.BlackPen, cg);

                // header rows
                double offsetY = 5.0;
                foreach (string value in Entity.DebugRows)
                {
                    string rowValue = value;
                    if (rowValue.Length > 44)
                    {
                        rowValue = rowValue.Remove(44) + "...";
                    }

                    GlyphRun headerRowGlyphRun = state.ConvertTextLinesToGlyphRun(new Point(nodePosition.X + (5 * state.Scale), nodePosition.Y + ((offsetY - 0.5) * state.Scale)), false, rowValue);
                    state.DrawingContext.DrawGlyphRun(Brushes.Black, headerRowGlyphRun);
                    offsetY += 10.0;
                }
            }
        }

        private void UpdatePorts()
        {
            List<Port> oldInputLinks = new List<Port>();
            List<Port> oldOutputLinks = new List<Port>();
            List<Port> oldInputEvents = new List<Port>();
            List<Port> oldOutputEvents = new List<Port>();
            List<Port> oldInputProps = new List<Port>();
            List<Port> oldOutputProps = new List<Port>();

            oldInputLinks.AddRange(InputLinks);
            oldOutputLinks.AddRange(OutputLinks);
            oldInputEvents.AddRange(InputEvents);
            oldOutputEvents.AddRange(OutputEvents);
            oldInputProps.AddRange(InputProperties);
            oldOutputProps.AddRange(OutputProperties);

            // input links
            foreach (ConnectionDesc linkDesc in Entity.Links.Where(e => e.Direction == Direction.Out))
            {
                int hash = HashString(linkDesc.Name);
                Port port = InputLinks.Find(p => p.NameHash == hash);

                if (port == null)
                {
                    InputLinks.Add(new Port(this, linkDesc.DataType) { Name = ReflectionUtils.AddStringToList(linkDesc.Name), NameHash = hash, PortType = 0, PortDirection = 1 });
                }
                else
                {
                    port.DataType = linkDesc.DataType;
                    oldInputLinks.Remove(port);
                }
            }

            // input events
            foreach (ConnectionDesc eventDesc in Entity.Events.Where(e => e.Direction == Direction.In))
            {
                int hash = HashString(eventDesc.Name);
                Port port = InputEvents.Find(p => p.NameHash == hash);

                if (port == null)
                {
                    InputEvents.Add(new Port(this) { Name = ReflectionUtils.AddStringToList(eventDesc.Name), NameHash = hash, PortType = 1, PortDirection = 1 });
                }
                else
                {
                    oldInputEvents.Remove(port);
                }
            }

            // input properties
            foreach (ConnectionDesc propDesc in Entity.Properties.Where(p => p.Direction == Direction.In))
            {
                int hash = HashString(propDesc.Name);
                Port port = InputProperties.Find(p => p.NameHash == hash);

                if (port == null)
                {
                    InputProperties.Add(new Port(this, propDesc.DataType) { Name = ReflectionUtils.AddStringToList(propDesc.Name), NameHash = hash, PortType = 2, PortDirection = 1 });
                }
                else
                {
                    port.DataType = propDesc.DataType;
                    oldInputProps.Remove(port);
                }
            }

            // output links
            foreach (ConnectionDesc linkDesc in Entity.Links.Where(e => e.Direction == Direction.In))
            {
                int hash = HashString(linkDesc.Name);
                Port port = OutputLinks.Find(p => p.NameHash == hash);

                if (port == null)
                {
                    OutputLinks.Add(new Port(this, linkDesc.DataType) { Name = ReflectionUtils.AddStringToList(linkDesc.Name), NameHash = hash, PortType = 0, PortDirection = 0 });
                }
                else
                {
                    port.DataType = linkDesc.DataType;
                    oldOutputLinks.Remove(port);
                }
            }

            // output events
            foreach (ConnectionDesc eventDesc in Entity.Events.Where(e => e.Direction == Direction.Out))
            {
                int hash = HashString(eventDesc.Name);
                Port port = OutputEvents.Find(p => p.NameHash == hash);

                if (port == null)
                {
                    OutputEvents.Add(new Port(this) { Name = ReflectionUtils.AddStringToList(eventDesc.Name), NameHash = hash, PortType = 1, PortDirection = 0 });
                }
                else
                {
                    oldOutputEvents.Remove(port);
                }
            }

            // output properties
            foreach (ConnectionDesc propDesc in Entity.Properties.Where(p => p.Direction == Direction.Out))
            {
                int hash = HashString(propDesc.Name);
                Port port = OutputProperties.Find(p => p.NameHash == hash);

                if (port == null)
                {
                    OutputProperties.Add(new Port(this, propDesc.DataType) { Name = ReflectionUtils.AddStringToList(propDesc.Name), NameHash = hash, PortType = 2, PortDirection = 0 });
                }
                else
                {
                    port.DataType = propDesc.DataType;
                    oldOutputProps.Remove(port);
                }
            }

            foreach (Port port in oldInputLinks) { if (!port.IsDynamicallyGenerated) { InputLinks.Remove(port); } }
            foreach (Port port in oldOutputLinks) { if (!port.IsDynamicallyGenerated) { OutputLinks.Remove(port); } }
            foreach (Port port in oldInputEvents) { if (!port.IsDynamicallyGenerated) { InputEvents.Remove(port); } }
            foreach (Port port in oldOutputEvents) { if (!port.IsDynamicallyGenerated) { OutputEvents.Remove(port); } }
            foreach (Port port in oldInputProps) { if (!port.IsDynamicallyGenerated) { InputProperties.Remove(port); } }
            foreach (Port port in oldOutputProps) { if (!port.IsDynamicallyGenerated) { OutputProperties.Remove(port); } }
        }

        private void DrawHeader(SchematicsCanvas.DrawingContextState state)
        {
            double headerHeight = (Entity.HeaderRows.Count() * 10.0) + 20;
            Point nodePosition = state.WorldMatrix.Transform(Rect.Location);
            Size nodeSize = Rect.Size;

            // title background
            state.DrawingContext.DrawRectangle(state.NodeTitleBackgroundBrush, state.BlackPen, new Rect(nodePosition.X, nodePosition.Y, Rect.Width * state.Scale, headerHeight * state.Scale));

            if (state.InvScale >= 5)
                return;

            // self connector
            Brush brush = (Self.IsHighlighted) ? state.NodeSelectedBrush : state.SchematicLinkBrush;
            state.DrawingContext.DrawRectangle(brush, state.BlackPen, new Rect(nodePosition.X + (4 * state.Scale), nodePosition.Y + (4 * state.Scale), 12 * state.Scale, 12 * state.Scale));
            if (!Self.IsConnected)
                state.DrawingContext.DrawRectangle(state.NodeTitleBackgroundBrush, state.BlackPen, new Rect(nodePosition.X + (6 * state.Scale), nodePosition.Y + (6 * state.Scale), 8 * state.Scale, 8 * state.Scale));
            if (Self.IsHighlighted)
                DrawTooltip(state, Self);

            // @todo: cache the shapes

            // collapse button
            state.DrawingContext.DrawRectangle((CollapseHover) ? state.NodeSelectedBrush : state.NodeBackgroundBrush, state.BlackPen, new Rect(nodePosition.X + ((nodeSize.Width - 16) * state.Scale), nodePosition.Y + (4 * state.Scale), 12 * state.Scale, 12 * state.Scale));
            if (IsCollapsed)
            {
                Point p = new Point(nodeSize.Width - 16 + 2, 6);
                state.DrawingContext.DrawGeometry(state.NodeTitleBackgroundBrush, state.BlackPen, new PathGeometry(new[]
                {
                    new PathFigure(new Point(nodePosition.X + (p.X * state.Scale), nodePosition.Y + (p.Y + 2) * state.Scale), new []
                    {
                        new PolyLineSegment(new []
                        {
                            new Point(nodePosition.X + (p.X + 8) * state.Scale, nodePosition.Y + (p.Y + 2) * state.Scale),
                            new Point(nodePosition.X + (p.X + 4) * state.Scale, nodePosition.Y + (p.Y + 6) * state.Scale)
                        }, true)

                    }, true)
                }));
            }
            else
            {
                Point p = new Point(nodeSize.Width - 16 + 2, 6);
                state.DrawingContext.DrawGeometry(state.NodeTitleBackgroundBrush, state.BlackPen, new PathGeometry(new[]
                {
                    new PathFigure(new Point(nodePosition.X + (p.X * state.Scale), nodePosition.Y + (p.Y + 6) * state.Scale), new []
                    {
                        new PolyLineSegment(new []
                        {
                            new Point(nodePosition.X + (p.X + 8) * state.Scale, nodePosition.Y + (p.Y + 6) * state.Scale),
                            new Point(nodePosition.X + (p.X + 4) * state.Scale, nodePosition.Y + (p.Y + 2) * state.Scale)
                        }, true)

                    }, true)
                }));
            }

            // client/server
            if (state.InvScale < 2)
            {
                if (Realm == 0 || Realm == 2)
                {
                    Point p = new Point(nodeSize.Width - 30 + 1, 6);
                    state.DrawingContext.DrawGeometry(state.NodeSelectedBrush, null, new PathGeometry(new[]
                    {
                        new PathFigure(new Point(nodePosition.X + (p.X + 0) * state.Scale, nodePosition.Y + (p.Y + 0) * state.Scale), new []
                        {
                            new PolyLineSegment(new []
                            {
                                new Point(nodePosition.X + (p.X + 4) * state.Scale, nodePosition.Y + (p.Y + 0) * state.Scale),
                                new Point(nodePosition.X + (p.X + 3.75) * state.Scale, nodePosition.Y + (p.Y + 1) * state.Scale),
                                new Point(nodePosition.X + (p.X + 1) * state.Scale, nodePosition.Y + (p.Y + 1) * state.Scale),
                                new Point(nodePosition.X + (p.X + 1) * state.Scale, nodePosition.Y + (p.Y + 7) * state.Scale),
                                new Point(nodePosition.X + (p.X + 3.75) * state.Scale, nodePosition.Y + (p.Y + 7) * state.Scale),
                                new Point(nodePosition.X + (p.X + 4) * state.Scale, nodePosition.Y + (p.Y + 8) * state.Scale),
                                new Point(nodePosition.X + (p.X + 0) * state.Scale, nodePosition.Y + (p.Y + 8) * state.Scale)
                            }, true)

                        }, true)
                    }));
                }
            }
            if (state.InvScale < 2)
            {
                if (Realm == 1 || Realm == 2)
                {
                    Point p = new Point(nodeSize.Width - 30 + 2, 6);
                    state.DrawingContext.DrawGeometry(state.NodeSelectedBrush, null, new PathGeometry(new[]
                    {
                        new PathFigure(new Point(nodePosition.X + (p.X + 4) * state.Scale, nodePosition.Y + (p.Y + 0) * state.Scale), new []
                        {
                            new PolyLineSegment(new []
                            {
                                new Point(nodePosition.X + (p.X + 8) * state.Scale, nodePosition.Y + (p.Y + 0) * state.Scale),
                                new Point(nodePosition.X + (p.X + 7.75) * state.Scale, nodePosition.Y + (p.Y + 1) * state.Scale),
                                new Point(nodePosition.X + (p.X + 5) * state.Scale, nodePosition.Y + (p.Y + 1) * state.Scale),
                                new Point(nodePosition.X + (p.X + 5) * state.Scale, nodePosition.Y + (p.Y + 3.5) * state.Scale),
                                new Point(nodePosition.X + (p.X + 8) * state.Scale, nodePosition.Y + (p.Y + 3.5) * state.Scale),
                                new Point(nodePosition.X + (p.X + 8) * state.Scale, nodePosition.Y + (p.Y + 8) * state.Scale),
                                new Point(nodePosition.X + (p.X + 4) * state.Scale, nodePosition.Y + (p.Y + 8) * state.Scale),
                                new Point(nodePosition.X + (p.X + 4.5) * state.Scale, nodePosition.Y + (p.Y + 7) * state.Scale),
                                new Point(nodePosition.X + (p.X + 7) * state.Scale, nodePosition.Y + (p.Y + 7) * state.Scale),
                                new Point(nodePosition.X + (p.X + 7) * state.Scale, nodePosition.Y + (p.Y + 4.5) * state.Scale),
                                new Point(nodePosition.X + (p.X + 4) * state.Scale, nodePosition.Y + (p.Y + 4.5) * state.Scale)
                            }, true)

                        }, true)
                    }));
                }
            }

            if (state.InvScale < 2.5)
            {
                // node title
                GlyphRun titleGlyphRun = state.ConvertTextLinesToGlyphRun(
                    new Point(
                        nodePosition.X + (20 * state.Scale),
                        nodePosition.Y + (3.5 * state.Scale)), 
                    true, 
                    (Entity as Entity).HasFlags(EntityFlags.HasLogic) ? Title : Title + "*");
                state.DrawingContext.DrawGlyphRun(Brushes.White, titleGlyphRun);

                if (state.InvScale < 1.5)
                {
                    // header rows
                    double offsetY = 20.0;
                    foreach (string value in Entity.HeaderRows)
                    {
                        string rowValue = value;
                        if (rowValue.Length > 44)
                        {
                            rowValue = rowValue.Remove(44) + "...";
                        }

                        if (value != "")
                        {
                            GlyphRun headerRowGlyphRun = state.ConvertTextLinesToGlyphRun(new Point(nodePosition.X + (5 * state.Scale), nodePosition.Y + ((offsetY - 0.5) * state.Scale)), false, rowValue);
                            state.DrawingContext.DrawGlyphRun(Brushes.LightGray, headerRowGlyphRun);
                        }

                        offsetY += 10.0;
                    }
                }
            }
        }

        private void DrawBody(SchematicsCanvas.DrawingContextState state)
        {
            double headerHeight = (Entity.HeaderRows.Count() * 10.0) + 20;
            Point nodePosition = state.WorldMatrix.Transform(Rect.Location);
            Brush nodeBackgroundBrush = (IsSelected) ? state.NodeSelectedBrush : state.NodeBackgroundBrush;

            // node body background
            state.DrawingContext.DrawRectangle(nodeBackgroundBrush, state.BlackPen, new Rect(nodePosition.X, nodePosition.Y + headerHeight * state.Scale, Rect.Width * state.Scale, (Rect.Height - headerHeight) * state.Scale));

            if (state.InvScale >= 5)
                return;

            // draw input links
            foreach (Port port in InputLinks)
            {
                if ((IsCollapsed && !port.IsConnected) && !port.ShowWhileCollapsed)
                    continue;

                Brush brush = (port.IsHighlighted) ? state.NodeSelectedBrush : state.SchematicLinkBrush;
                state.DrawingContext.DrawRectangle(brush, state.BlackPen, new Rect(nodePosition.X + port.Rect.X * state.Scale, nodePosition.Y + port.Rect.Y * state.Scale, 12 * state.Scale, 12 * state.Scale));
                
                if (!port.IsConnected)
                    state.DrawingContext.DrawRectangle(nodeBackgroundBrush, state.BlackPen, new Rect(nodePosition.X + (port.Rect.X + 2) * state.Scale, nodePosition.Y + (port.Rect.Y + 2) * state.Scale, 8 * state.Scale, 8 * state.Scale));

                if (state.InvScale < 2.5)
                {
                    GlyphRun varGlypRun = state.ConvertTextLinesToGlyphRun(
                        new Point(
                            nodePosition.X + (port.Rect.X + 14) * state.Scale, 
                            nodePosition.Y + port.Rect.Y * state.Scale), 
                        true, 
                        port.IsDynamicallyGenerated ? port.Name + "*" : port.Name);
                    state.DrawingContext.DrawGlyphRun(Brushes.Black, varGlypRun);

                    if (port.IsHighlighted)
                    {
                        DrawTooltip(state, port);
                    }
                }
            }
            // draw input events
            foreach (Port port in InputEvents)
            {
                if ((IsCollapsed && !port.IsConnected) && !port.ShowWhileCollapsed)
                    continue;

                Brush brush = (port.IsHighlighted) ? state.NodeSelectedBrush : state.SchematicEventBrush;
                state.DrawingContext.DrawRectangle(brush, state.BlackPen, new Rect(nodePosition.X + port.Rect.X * state.Scale, nodePosition.Y + port.Rect.Y * state.Scale, 12 * state.Scale, 12 * state.Scale));
                
                if (!port.IsConnected)
                    state.DrawingContext.DrawRectangle(nodeBackgroundBrush, state.BlackPen, new Rect(nodePosition.X + (port.Rect.X + 2) * state.Scale, nodePosition.Y + (port.Rect.Y + 2) * state.Scale, 8 * state.Scale, 8 * state.Scale));

                if (state.InvScale < 2.5)
                {
                    GlyphRun varGlypRun = state.ConvertTextLinesToGlyphRun(
                        new Point(
                            nodePosition.X + (port.Rect.X + 14) * state.Scale, 
                            nodePosition.Y + port.Rect.Y * state.Scale),
                        true,
                        port.IsDynamicallyGenerated ? port.Name + "*" : port.Name);
                    state.DrawingContext.DrawGlyphRun(Brushes.Black, varGlypRun);
                }
            }
            // draw input properties
            foreach (Port port in InputProperties)
            {
                if ((IsCollapsed && !port.IsConnected) && !port.ShowWhileCollapsed)
                    continue;

                Brush brush = (port.IsHighlighted) ? state.NodeSelectedBrush : state.SchematicPropertyBrush;
                state.DrawingContext.DrawRectangle(brush, state.BlackPen, new Rect(nodePosition.X + port.Rect.X * state.Scale, nodePosition.Y + port.Rect.Y * state.Scale, 12 * state.Scale, 12 * state.Scale));
                
                if (!port.IsConnected)
                    state.DrawingContext.DrawRectangle(nodeBackgroundBrush, state.BlackPen, new Rect(nodePosition.X + (port.Rect.X + 2) * state.Scale, nodePosition.Y + (port.Rect.Y + 2) * state.Scale, 8 * state.Scale, 8 * state.Scale));

                if (state.InvScale < 2.5)
                {
                    GlyphRun varGlypRun = state.ConvertTextLinesToGlyphRun(
                        new Point(
                            nodePosition.X + (port.Rect.X + 14) * state.Scale, 
                            nodePosition.Y + port.Rect.Y * state.Scale), 
                        true, 
                        port.IsDynamicallyGenerated ? port.Name + "*" : port.Name);
                    state.DrawingContext.DrawGlyphRun(Brushes.Black, varGlypRun);

                    if (port.IsHighlighted)
                    {
                        DrawTooltip(state, port);
                    }
                }
            }

            // draw output links
            foreach (Port port in OutputLinks)
            {
                if ((IsCollapsed && !port.IsConnected) && !port.ShowWhileCollapsed)
                    continue;

                Brush brush = (port.IsHighlighted) ? state.NodeSelectedBrush : state.SchematicLinkBrush;
                state.DrawingContext.DrawRectangle(brush, state.BlackPen, new Rect(nodePosition.X + port.Rect.X * state.Scale, nodePosition.Y + port.Rect.Y * state.Scale, 12 * state.Scale, 12 * state.Scale));
                
                if (!port.IsConnected)
                    state.DrawingContext.DrawRectangle(nodeBackgroundBrush, state.BlackPen, new Rect(nodePosition.X + (port.Rect.X + 2) * state.Scale, nodePosition.Y + (port.Rect.Y + 2) * state.Scale, 8 * state.Scale, 8 * state.Scale));

                if (state.InvScale < 2.5)
                {
                    string portName = port.IsDynamicallyGenerated ? port.Name + "*" : port.Name;
                    GlyphRun varGlypRun = state.ConvertTextLinesToGlyphRun(
                        new Point(
                            nodePosition.X + ((port.Rect.X - 4) * state.Scale) - portName.Length * state.LargeFont.AdvanceWidth, 
                            nodePosition.Y + port.Rect.Y * state.Scale),
                        true,
                        portName);
                    state.DrawingContext.DrawGlyphRun(Brushes.Black, varGlypRun);

                    if (port.IsHighlighted)
                    {
                        DrawTooltip(state, port);
                    }
                }

            }
            // draw output events
            foreach (Port port in OutputEvents)
            {
                if ((IsCollapsed && !port.IsConnected) && !port.ShowWhileCollapsed)
                    continue;

                Brush brush = (port.IsHighlighted) ? state.NodeSelectedBrush : state.SchematicEventBrush;
                state.DrawingContext.DrawRectangle(brush, state.BlackPen, new Rect(nodePosition.X + port.Rect.X * state.Scale, nodePosition.Y + port.Rect.Y * state.Scale, 12 * state.Scale, 12 * state.Scale));
                
                if (!port.IsConnected)
                    state.DrawingContext.DrawRectangle(nodeBackgroundBrush, state.BlackPen, new Rect(nodePosition.X + (port.Rect.X + 2) * state.Scale, nodePosition.Y + (port.Rect.Y + 2) * state.Scale, 8 * state.Scale, 8 * state.Scale));

                if (state.InvScale < 2.5)
                {
                    string portName = port.IsDynamicallyGenerated ? port.Name + "*" : port.Name;
                    GlyphRun varGlypRun = state.ConvertTextLinesToGlyphRun(
                        new Point(
                            nodePosition.X + ((port.Rect.X - 4) * state.Scale) - portName.Length * state.LargeFont.AdvanceWidth, 
                            nodePosition.Y + port.Rect.Y * state.Scale), 
                        true, 
                        portName);
                    state.DrawingContext.DrawGlyphRun(Brushes.Black, varGlypRun);
                }
            }
            // draw output properties
            foreach (Port port in OutputProperties)
            {
                if ((IsCollapsed && !port.IsConnected) && !port.ShowWhileCollapsed)
                    continue;

                Brush brush = (port.IsHighlighted) ? state.NodeSelectedBrush : state.SchematicPropertyBrush;
                state.DrawingContext.DrawRectangle(brush, state.BlackPen, new Rect(nodePosition.X + port.Rect.X * state.Scale, nodePosition.Y + port.Rect.Y * state.Scale, 12 * state.Scale, 12 * state.Scale));
                
                if (!port.IsConnected)
                    state.DrawingContext.DrawRectangle(nodeBackgroundBrush, state.BlackPen, new Rect(nodePosition.X + (port.Rect.X + 2) * state.Scale, nodePosition.Y + (port.Rect.Y + 2) * state.Scale, 8 * state.Scale, 8 * state.Scale));

                if (state.InvScale < 2.5)
                {
                    string portName = port.IsDynamicallyGenerated ? port.Name + "*" : port.Name;
                    GlyphRun varGlypRun = state.ConvertTextLinesToGlyphRun(
                        new Point(
                            nodePosition.X + ((port.Rect.X - 4) * state.Scale) - portName.Length * state.LargeFont.AdvanceWidth, 
                            nodePosition.Y + (port.Rect.Y) * state.Scale), 
                        true, 
                        portName);
                    state.DrawingContext.DrawGlyphRun(Brushes.Black, varGlypRun);

                    if (port.IsHighlighted)
                    {
                        DrawTooltip(state, port);
                    }
                }
            }
        }

        private void DrawTooltip(SchematicsCanvas.DrawingContextState state, Port port)
        {
            if (port.ShowWhileCollapsed)
                return;

            state.DrawingContext.PushOpacity(0.5);
            string dataTypeValue = (port.DataType != null) ? ConvertTypeName(port.DataType) : "Any";

            Rect rect = new Rect(
                port.Owner.Rect.X + port.Rect.X,
                port.Owner.Rect.Y + port.Rect.Y,
                5 + dataTypeValue.Length * GlyphWidth + 5,
                port.Rect.Height
                );

            if (port.PortDirection == 1) rect.X -= (5 + dataTypeValue.Length * GlyphWidth + 5) + 4;
            else rect.X += port.Rect.Width + 4;

            state.DrawingContext.DrawRectangle(state.NodeTitleBackgroundBrush, state.BlackPen, state.TransformRect(rect));

            rect.X += 5;
            rect.Y += 0;

            GlyphRun varGlypRun = state.ConvertTextLinesToGlyphRun(state.TransformPoint(rect.Location), true, dataTypeValue);
            state.DrawingContext.DrawGlyphRun(Brushes.White, varGlypRun);
            state.DrawingContext.Pop();
        }

        private string ConvertTypeName(Type inType)
        {
            switch(inType.Name)
            {
                case "Single": return "Float32";
                case "Int": return "Int32";
            }
            if (inType.Name.Equals("List`1"))
            {
                return $"{inType.GetGenericArguments()[0].Name}[]";
            }
            return inType.Name;
        }

        private double CalculateNodeWidth(out int linkCount, out int eventCount, out int propertyCount)
        {
            double nodeWidth = 20 + (Entity.DisplayName.Length * GlyphWidth) + 60;
            nodeWidth = nodeWidth - (nodeWidth % 10);

            double longestInput = 0.0;
            double longestOutput = 0.0;
            double longestHeaderRow = 0.0;

            int tmpEventCount = eventCount = 0;
            int tmpPropertyCount = propertyCount = 0;
            int tmpLinkCount = linkCount = 0;

            foreach (Port port in InputLinks)
            {
                if ((!IsCollapsed || port.IsConnected) || port.ShowWhileCollapsed)
                {
                    double strLength = port.Name.Length * GlyphWidth;
                    longestInput = (strLength > longestInput) ? strLength : longestInput; linkCount++;
                }
            }
            foreach (Port port in OutputLinks)
            {
                if ((!IsCollapsed || port.IsConnected) || port.ShowWhileCollapsed)
                {
                    double strLength = port.Name.Length * GlyphWidth;
                    longestOutput = (strLength > longestOutput) ? strLength : longestOutput; tmpLinkCount++;
                }
            }

            foreach (Port port in InputEvents)
            {
                if ((!IsCollapsed || port.IsConnected) || port.ShowWhileCollapsed)
                {
                    double strLength = port.Name.Length * GlyphWidth;
                    longestInput = (strLength > longestInput) ? strLength : longestInput; eventCount++;
                }
            }
            foreach (Port port in OutputEvents)
            {
                if ((!IsCollapsed || port.IsConnected) || port.ShowWhileCollapsed)
                {
                    double strLength = port.Name.Length * GlyphWidth;
                    longestOutput = (strLength > longestOutput) ? strLength : longestOutput; tmpEventCount++;
                }
            }

            foreach (Port port in InputProperties)
            {
                if ((!IsCollapsed || port.IsConnected) || port.ShowWhileCollapsed)
                {
                    double strLength = port.Name.Length * GlyphWidth;
                    longestInput = (strLength > longestInput) ? strLength : longestInput; propertyCount++;
                }
            }
            foreach (Port port in OutputProperties)
            {
                if ((!IsCollapsed || port.IsConnected) || port.ShowWhileCollapsed)
                {
                    double strLength = port.Name.Length * GlyphWidth;
                    longestOutput = (strLength > longestOutput) ? strLength : longestOutput; tmpPropertyCount++;
                }
            }
            foreach (string row in Entity.HeaderRows)
            {
                int strLen = (row.Length > 44) ? strLen = 47 : row.Length;
                double rowLength = strLen * (GlyphWidth * 0.6);
                longestHeaderRow = (rowLength > longestHeaderRow) ? rowLength : longestHeaderRow;
            }

            double textLength = longestInput + 20 + longestOutput + 20;
            textLength = (longestHeaderRow > textLength) ? longestHeaderRow : textLength;

            double newNodeWidth = 20 + textLength + 20;
            newNodeWidth = newNodeWidth - (newNodeWidth % 10);

            eventCount = (tmpEventCount > eventCount) ? tmpEventCount : eventCount;
            propertyCount = (tmpPropertyCount > propertyCount) ? tmpPropertyCount : propertyCount;
            linkCount = (tmpLinkCount > linkCount) ? tmpLinkCount : linkCount;

            return (nodeWidth > newNodeWidth) ? nodeWidth : newNodeWidth;
        }
    }

    public class WireVisual
    {
        public object Data;
        public BaseNodeVisual Source;
        public BaseNodeVisual.Port SourcePort;
        public BaseNodeVisual Target;
        public BaseNodeVisual.Port TargetPort;
        public int WireType;
        public int ConnectOrder;
        public List<WirePointVisual> WirePoints = new List<WirePointVisual>();

        private GeometryGroup geometry;
        private PathGeometry hitTestGeometry;
        private Stopwatch glowTimer;

        private Point mousePosition;

        private static Pen hitTestPen;

        public WireVisual(BaseNodeVisual source, string sourceName, int sourceHash, BaseNodeVisual target, string targetName, int targetHash, int portType)
        {
            Source = source;
            Target = target;
            WireType = portType;

            if (Source != null)
            {
                SourcePort = Source.Connect(sourceName, sourceHash, portType, 1);
            }
            if (Target != null)
            {
                TargetPort = Target.Connect(targetName, targetHash, portType, 0);
            }

            if (Source != null && Target != null)
            {
                ConnectOrder = (WireType == 0) ? TargetPort.ConnectionCount - 1 : SourcePort.ConnectionCount - 1;

                geometry = new GeometryGroup();
                geometry.Children.Add(new LineGeometry(Source.Rect.Location, Target.Rect.Location));

                if (hitTestPen == null)
                {
                    hitTestPen = new Pen(Brushes.Transparent, 4.0);
                    hitTestPen.Freeze();
                }

                glowTimer = new Stopwatch();
            }
        }

        public SchematicsLayout.Wire GenerateLayout()
        {
            SchematicsLayout.Wire wireLayout = new SchematicsLayout.Wire();

            SchematicsLayout.Node sourceNode = Source.GenerateLayout();
            SchematicsLayout.Node targetNode = Target.GenerateLayout();

            wireLayout.WirePoints = new List<SchematicsLayout.WirePoint>();

            foreach (WirePointVisual wirePoint in WirePoints)
            {
                SchematicsLayout.WirePoint wirePointLayout = new SchematicsLayout.WirePoint();
                wirePointLayout.UniqueId = wirePoint.UniqueId;
                wirePointLayout.Position = wirePoint.Rect.Location;
                wireLayout.WirePoints.Add(wirePointLayout);
            }

            return wireLayout;
        }

        public void ApplyLayout(SchematicsLayout.Wire wireLayout, List<BaseVisual> visuals)
        {
            foreach (SchematicsLayout.WirePoint wirePointLayout in wireLayout.WirePoints)
            {
                WirePointVisual wp = new WirePointVisual(this, wirePointLayout.Position.X + 5, wirePointLayout.Position.Y + 5) { UniqueId = (wirePointLayout.UniqueId != Guid.Empty) ? wirePointLayout.UniqueId : Guid.NewGuid() };
                visuals.Add(wp);

                AddWirePoint(wp, force: true);
            }
        }

        public bool OnMouseMove(Point mousePos)
        {
            mousePosition = mousePos;
            return false;
        }

        public bool OnMouseDown(Point mousePos, MouseButton mousebutton)
        {
            return false;
        }

        public bool OnMouseUp(Point mousePos, MouseButton mouseButton)
        {
            if (hitTestGeometry != null)
            {
                if (hitTestGeometry.FillContains(mousePos) && Keyboard.IsKeyDown(Key.LeftCtrl))
                {
                    return true;
                }
            }
            if (Source == null || Target == null)
            {
                if (hoveredNode != null)
                {
                    if (hoveredNode is NodeVisual)
                    {
                        foreach (BaseNodeVisual.Port port in (hoveredNode as NodeVisual).AllPorts)
                        {
                            port.ShowWhileCollapsed = false;
                        }
                        hoveredNode.Update();
                    }
                    hoveredNode = null;
                }
            }
            return false;
        }

        private BaseNodeVisual hoveredNode;
        private BaseNodeVisual.Port hoveredPort;
        public BaseNodeVisual.Port HoveredPort => hoveredPort;

        public void UpdateEditing(Point mousePos, IEnumerable<BaseVisual> visibleNodes)
        {
            mousePosition = mousePos;

            BaseNodeVisual.Port wirePort = (Source != null) ? SourcePort : TargetPort;
            BaseNodeVisual oldHoveredNode = hoveredNode;

            hoveredNode = null;
            hoveredPort = null;

            foreach (BaseVisual visual in visibleNodes)
            {
                if (visual is NodeVisual)
                {
                    NodeVisual node = visual as NodeVisual;
                    if (node.Rect.Contains(mousePos))
                    {
                        hoveredNode = node;
                        foreach (BaseNodeVisual.Port port in node.AllPorts.Where(p => p.PortType == WireType && p.PortDirection != wirePort.PortDirection))
                        {
                            if (port.DataType == wirePort.DataType || wirePort.DataType == typeof(Any) || port.DataType == typeof(Any))
                            {
                                port.IsHighlighted = true;
                            }

                            Rect portRect = new Rect(node.Rect.X + port.Rect.X, node.Rect.Y + port.Rect.Y, port.Rect.Width + port.Name.Length * node.GlyphWidth, port.Rect.Height);
                            if (port.PortDirection == 0) portRect.X -= port.Name.Length * node.GlyphWidth;

                            if (portRect.Contains(mousePos))
                            {
                                hoveredPort = port;
                                mousePosition = node.Rect.Location;
                                mousePosition.X += port.Rect.Location.X + ((port.PortDirection == 0) ? 12 : 0);
                                mousePosition.Y += port.Rect.Location.Y + 6;
                            }
                            port.ShowWhileCollapsed = true;
                        }
                        hoveredNode.Update();
                        break;
                    }
                }
                else if (visual is InterfaceNodeVisual)
                {
                    InterfaceNodeVisual node = visual as InterfaceNodeVisual;
                    if (node.Rect.Contains(mousePos))
                    {
                        if (node.Self.PortType == WireType && node.Direction != wirePort.PortDirection)
                        {
                            hoveredNode = node;
                            Rect portRect = new Rect(node.Rect.X + node.Self.Rect.X, node.Rect.Y + node.Self.Rect.Y, node.Self.Rect.Width + node.Self.Name.Length * node.GlyphWidth, node.Self.Rect.Height);
                            if (node.Self.PortDirection == 0) portRect.X -= node.Self.Name.Length * node.GlyphWidth;

                            if (portRect.Contains(mousePos))
                            {
                                hoveredPort = node.Self;
                                mousePosition = node.Rect.Location;
                                mousePosition.X += node.Self.Rect.Location.X + ((node.Self.PortDirection == 0) ? 12 : 0);
                                mousePosition.Y += node.Self.Rect.Location.Y + 6;
                            }

                            hoveredNode.Update();
                            break;
                        }
                    }
                }
            }

            if (oldHoveredNode != hoveredNode && oldHoveredNode != null)
            {
                if (oldHoveredNode is NodeVisual)
                {
                    foreach (BaseNodeVisual.Port port in (oldHoveredNode as NodeVisual).AllPorts)
                    {
                        port.IsHighlighted = false;
                        port.ShowWhileCollapsed = false;
                    }
                    oldHoveredNode.Update();
                }
            }
        }

        public void Disconnect()
        {
            if (Source != null)
            {
                SourcePort.ConnectionCount--;
                if (SourcePort.ConnectionCount == 0)
                {
                    SourcePort.IsConnected = false;
                }
            }
            if (Target != null)
            {
                TargetPort.ConnectionCount--;
                if (TargetPort.ConnectionCount == 0)
                {
                    TargetPort.IsConnected = false;
                }
            }
        }

        public void AddWirePoint(WirePointVisual wirePoint, bool force = false)
        {
            if (!force)
            {
                AddWirePointInternal(wirePoint);
                return;
            }

            wirePoint.IsSelected = false;
            WirePoints.Add(wirePoint);
            geometry.Children.Add(new LineGeometry(Source.Rect.Location, Target.Rect.Location));
        }

        private void AddWirePointInternal(WirePointVisual wirePoint)
        {
            int index = 0;
            foreach (Geometry geom in geometry.Children)
            {
                if (geom.GetWidenedPathGeometry(hitTestPen).FillContains(new Point(wirePoint.Rect.X + 5, wirePoint.Rect.Y + 5)))
                {
                    WirePoints.Insert(index, wirePoint);
                    geometry.Children.Add(new LineGeometry(Source.Rect.Location, Target.Rect.Location));

                    break;
                }
                index++;
            }

            index++;
        }

        public void RemoveWirePoint(WirePointVisual wirePoint)
        {
            int index = WirePoints.IndexOf(wirePoint);

            WirePoints.RemoveAt(index);
            geometry.Children.RemoveAt(index + 1);
        }

        public void Render(SchematicsCanvas.DrawingContextState state)
        {
            BaseNodeVisual.Port portToCheck = (WireType == 0) ? TargetPort : SourcePort;
            bool drawConnectOrder = (state.ConnectorOrdersVisible && portToCheck.ConnectionCount > 1 && state.InvScale < 2.5);

            Point a = (Source != null) ? Source.Rect.Location : mousePosition;
            Point b = (Target != null) ? Target.Rect.Location : mousePosition;

            Pen wirePen = null;
            switch (WireType)
            {
                case 0: wirePen = state.WireLinkPen; break;
                case 1: wirePen = state.WireEventPen; break;
                case 2: wirePen = state.WirePropertyPen; break;
            }

            if (Source != null)
            {
                a.X += SourcePort.Rect.Location.X + 12;
                a.Y += SourcePort.Rect.Location.Y + 6;
            }
            if (Target != null)
            {
                b.X += TargetPort.Rect.Location.X;
                b.Y += TargetPort.Rect.Location.Y + 6;
            }

            Point ab = new Point(a.X + 9, a.Y);
            Point ba = new Point(b.X - 9, b.Y);

            state.DrawingContext.DrawLine(wirePen, state.WorldMatrix.Transform(a), state.WorldMatrix.Transform(ab));

            if (WirePoints.Count > 0)
            {
                Point startPoint = ab;
                for (int i = 0; i < WirePoints.Count; i++)
                {
                    Point endPoint = new Point(WirePoints[i].Rect.Location.X + 5, WirePoints[i].Rect.Location.Y + 5);

                    (geometry.Children[i] as LineGeometry).StartPoint = startPoint;
                    (geometry.Children[i] as LineGeometry).EndPoint = endPoint;

                    state.DrawingContext.DrawLine(wirePen, state.WorldMatrix.Transform(startPoint), state.WorldMatrix.Transform(endPoint));
                    startPoint = endPoint;
                }

                (geometry.Children[WirePoints.Count] as LineGeometry).StartPoint = startPoint;
                (geometry.Children[WirePoints.Count] as LineGeometry).EndPoint = ba;

                state.DrawingContext.DrawLine(wirePen, state.WorldMatrix.Transform(startPoint), state.WorldMatrix.Transform(ba));
            }
            else
            {
                if (Source != null && Target != null)
                {
                    (geometry.Children[0] as LineGeometry).StartPoint = ab;
                    (geometry.Children[0] as LineGeometry).EndPoint = ba;
                }

                state.DrawingContext.DrawLine(wirePen, state.WorldMatrix.Transform(ab), state.WorldMatrix.Transform(ba));
            }

            if (drawConnectOrder)
            {
                int geomIndex = (geometry.Children.Count / 2);
                Geometry geomToUse = geometry.Children[geomIndex];

                Rect box = new Rect(state.WorldMatrix.Transform(new Point(geomToUse.Bounds.X + (geomToUse.Bounds.Width / 2) - 6, geomToUse.Bounds.Y + (geomToUse.Bounds.Height / 2) - 5)), new Size(12 * state.Scale, 10 * state.Scale));
                state.DrawingContext.DrawRectangle(wirePen.Brush, state.BlackPen, box);

                string connectorString = (ConnectOrder + 1).ToString();
                Point textCenter = state.WorldMatrix.Transform(new Point(geomToUse.Bounds.X + (geomToUse.Bounds.Width / 2) - (state.SmallFont.OriginalAdvanceWidth * connectorString.Length) * 0.5, geomToUse.Bounds.Y + (geomToUse.Bounds.Height / 2) - state.SmallFont.OriginalAdvanceHeight * 0.5));
                GlyphRun text = state.ConvertTextLinesToGlyphRun(textCenter, false, connectorString);
                state.DrawingContext.DrawGlyphRun(Brushes.Black, text);
            }

            state.DrawingContext.DrawLine(wirePen, state.WorldMatrix.Transform(ba), state.WorldMatrix.Transform(b));

            if (Source != null && Target != null)
            {
                hitTestGeometry = geometry.GetWidenedPathGeometry(state.WireHitTestPen);
            }
        }

        public void RenderDebug(SchematicsCanvas.DrawingContextState state)
        {
            if (WireType == 2)
            {
                IProperty propA = (Source is NodeVisual)
                    ? (Source as NodeVisual).Entity.GetProperty(SourcePort.NameHash)
                    : (Source as InterfaceNodeVisual).InterfaceDescriptor.GetProperty(SourcePort.NameHash);
                //var propB = (Target as NodeVisual)?.Entity.GetProperty(TargetPort.NameHash);

                if (propA != null)
                {
                    if (propA.SetOnFrame >= state.LastFrameCount)
                    {
                        glowTimer.Restart();
                    }
                }
            }
            else if (WireType == 1)
            {
                IEvent eventA = (Source is NodeVisual)
                    ? (Source as NodeVisual).Entity.GetEvent(SourcePort.NameHash)
                    : (Source as InterfaceNodeVisual).InterfaceDescriptor.GetEvent(SourcePort.NameHash);
                //var eventB = (Target as NodeVisual)?.Entity.GetEvent(TargetPort.NameHash);

                if (eventA != null)
                {
                    if (eventA.SetOnFrame >= state.LastFrameCount)
                    {
                        glowTimer.Restart();
                    }
                }
            }

            if (glowTimer.IsRunning)
            {
                if (glowTimer.Elapsed.TotalSeconds > 1)
                {
                    glowTimer.Stop();
                    return;
                }

                Point a = Source.Rect.Location;
                Point b = Target.Rect.Location;

                a.X += SourcePort.Rect.Location.X + 12;
                a.Y += SourcePort.Rect.Location.Y + 6;

                b.X += TargetPort.Rect.Location.X;
                b.Y += TargetPort.Rect.Location.Y + 6;

                Point ab = new Point(a.X + 9, a.Y);
                Point ba = new Point(b.X - 9, b.Y);

                state.DrawingContext.PushOpacity((1 - glowTimer.Elapsed.TotalSeconds) * 0.5);
                state.DrawingContext.DrawLine(state.GlowPen, state.WorldMatrix.Transform(a), state.WorldMatrix.Transform(ab));

                if (WirePoints.Count > 0)
                {
                    Point startPoint = ab;
                    for (int i = 0; i < WirePoints.Count; i++)
                    {
                        Point endPoint = new Point(WirePoints[i].Rect.Location.X + 5, WirePoints[i].Rect.Location.Y + 5);

                        (geometry.Children[i] as LineGeometry).StartPoint = startPoint;
                        (geometry.Children[i] as LineGeometry).EndPoint = endPoint;

                        state.DrawingContext.DrawLine(state.GlowPen, state.WorldMatrix.Transform(startPoint), state.WorldMatrix.Transform(endPoint));
                        startPoint = endPoint;
                    }

                    (geometry.Children[WirePoints.Count] as LineGeometry).StartPoint = startPoint;
                    (geometry.Children[WirePoints.Count] as LineGeometry).EndPoint = ba;

                    state.DrawingContext.DrawLine(state.GlowPen, state.WorldMatrix.Transform(startPoint), state.WorldMatrix.Transform(ba));
                }
                else
                {
                    (geometry.Children[0] as LineGeometry).StartPoint = ab;
                    (geometry.Children[0] as LineGeometry).EndPoint = ba;

                    state.DrawingContext.DrawLine(state.GlowPen, state.WorldMatrix.Transform(ab), state.WorldMatrix.Transform(ba));
                }

                state.DrawingContext.DrawLine(state.GlowPen, state.WorldMatrix.Transform(ba), state.WorldMatrix.Transform(b));
                state.DrawingContext.Pop();
            }
        }
    }

    public class SchematicsData
    {
        public ObservableCollection<Entity> Entities;
        public ObservableCollection<object> LinkConnections;
        public ObservableCollection<object> EventConnections;
        public ObservableCollection<object> PropertyConnections;
        public object InterfaceDescriptor;
        public Guid BlueprintGuid;
        public EntityWorld World;
    }

    public struct FontData
    {
        public GlyphTypeface GlyphTypeface;
        public double OriginalRenderingEmSize, RenderingEmSize, AdvanceWidth, AdvanceHeight;
        public double OriginalAdvanceWidth, OriginalAdvanceHeight;
        public Point BaselineOrigin;

        public static FontData MakeFont(Typeface typeface, double size)
        {
            FontData fontData = new FontData();
            typeface.TryGetGlyphTypeface(out fontData.GlyphTypeface);

            fontData.OriginalRenderingEmSize = size;
            fontData.RenderingEmSize = fontData.OriginalRenderingEmSize;
            fontData.AdvanceWidth = fontData.GlyphTypeface.AdvanceWidths[0] * fontData.RenderingEmSize;
            fontData.AdvanceHeight = fontData.GlyphTypeface.Height * fontData.RenderingEmSize;
            fontData.OriginalAdvanceHeight = fontData.AdvanceHeight;
            fontData.OriginalAdvanceWidth = fontData.AdvanceWidth;
            fontData.BaselineOrigin = new Point(0, fontData.GlyphTypeface.Baseline * fontData.RenderingEmSize);

            return fontData;
        }

        public void Update(double newScale)
        {
            RenderingEmSize = OriginalRenderingEmSize * newScale;
            AdvanceWidth = GlyphTypeface.AdvanceWidths[0] * RenderingEmSize;
            AdvanceHeight = GlyphTypeface.Height * RenderingEmSize;
            BaselineOrigin = new Point(0, GlyphTypeface.Baseline * RenderingEmSize);
        }
    }

    public class WireAddedEventArgs : EventArgs
    {
        public ILogicEntity SourceEntity { get; private set; }
        public ILogicEntity TargetEntity { get; private set; }
        public int SourceId { get; private set; }
        public int TargetId { get; private set; }
        public int ConnectionType { get; private set; }

        public WireAddedEventArgs(ILogicEntity inSource, ILogicEntity inTarget, int inSourceId, int inTargetId, int inConnectionType)
        {
            SourceEntity = inSource;
            TargetEntity = inTarget;
            SourceId = inSourceId;
            TargetId = inTargetId;
            ConnectionType = inConnectionType;
        }
    }

    public class NodeModifiedEventArgs : EventArgs
    {
    }

    public class SchematicsCanvas : GridCanvas
    {
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(SchematicsData), typeof(SchematicsCanvas), new FrameworkPropertyMetadata(null, OnEntitiesChanged));
        public static readonly DependencyProperty ConnectorOrdersVisibleProperty = DependencyProperty.Register("ConnectorOrdersVisible", typeof(bool), typeof(SchematicsCanvas), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty SuppressLayoutSaveProperty = DependencyProperty.Register("SuppressLayoutSave", typeof(bool), typeof(SchematicsCanvas), new FrameworkPropertyMetadata(false));

        public static readonly DependencyProperty SchematicLinkBrushProperty = DependencyProperty.Register("SchematicLinkBrush", typeof(Brush), typeof(SchematicsCanvas), new FrameworkPropertyMetadata(null));
        public static readonly DependencyProperty SchematicEventBrushProperty = DependencyProperty.Register("SchematicEventBrush", typeof(Brush), typeof(SchematicsCanvas), new FrameworkPropertyMetadata(null));
        public static readonly DependencyProperty SchematicPropertyBrushProperty = DependencyProperty.Register("SchematicPropertyBrush", typeof(Brush), typeof(SchematicsCanvas), new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty SelectedNodeChangedCommandProperty = DependencyProperty.Register("SelectedNodeChangedCommand", typeof(ICommand), typeof(SchematicsCanvas), new FrameworkPropertyMetadata(null));
        public static readonly DependencyProperty WireAddedCommandProperty = DependencyProperty.Register("WireAddedCommand", typeof(ICommand), typeof(SchematicsCanvas), new FrameworkPropertyMetadata(null));
        public static readonly DependencyProperty NodeModifiedCommandProperty = DependencyProperty.Register("NodeModifiedCommand", typeof(ICommand), typeof(SchematicsCanvas), new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty ModifiedDataProperty = DependencyProperty.Register("ModifiedData", typeof(object), typeof(SchematicsCanvas), new FrameworkPropertyMetadata(null, OnDataModified));
        public static readonly DependencyProperty UpdateDebugProperty = DependencyProperty.Register("UpdateDebug", typeof(object), typeof(SchematicsCanvas), new FrameworkPropertyMetadata(null, OnDebugUpdateModified));

        public object ModifiedData
        {
            get => GetValue(ModifiedDataProperty);
            set => SetValue(ModifiedDataProperty, value);
        }
        public SchematicsData ItemsSource
        {
            get => (SchematicsData)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }
        public bool ConnectorOrdersVisible
        {
            get => (bool)GetValue(ConnectorOrdersVisibleProperty);
            set => SetValue(ConnectorOrdersVisibleProperty, value);
        }
        public bool SuppressLayoutSave
        {
            get => (bool)GetValue(SuppressLayoutSaveProperty);
            set => SetValue(SuppressLayoutSaveProperty, value);
        }
        public Brush SchematicLinkBrush
        {
            get => (Brush)GetValue(SchematicLinkBrushProperty);
            set => SetValue(SchematicLinkBrushProperty, value);
        }
        public Brush SchematicEventBrush
        {
            get => (Brush)GetValue(SchematicEventBrushProperty);
            set => SetValue(SchematicEventBrushProperty, value);
        }
        public Brush SchematicPropertyBrush
        {
            get => (Brush)GetValue(SchematicPropertyBrushProperty);
            set => SetValue(SchematicPropertyBrushProperty, value);
        }
        public ICommand SelectedNodeChangedCommand
        {
            get => (ICommand)GetValue(SelectedNodeChangedCommandProperty);
            set => SetValue(SelectedNodeChangedCommandProperty, value);
        }
        public ICommand WireAddedCommand
        {
            get => (ICommand)GetValue(WireAddedCommandProperty);
            set => SetValue(WireAddedCommandProperty, value);
        }
        public ICommand NodeModifiedCommand
        {
            get => (ICommand)GetValue(NodeModifiedCommandProperty);
            set => SetValue(NodeModifiedCommandProperty, value);
        }
        public object UpdateDebug
        {
            get => GetValue(UpdateDebugProperty);
            set => SetValue(UpdateDebugProperty, value);
        }

        public double OriginalAdvanceWidth;

        private FontData largeFont;
        private FontData smallFont;

        private Pen wireLinkPen;
        private Pen wireEventPen;
        private Pen wirePropertyPen;
        private Pen wireHitTestPen;
        private Pen shortcutWirePen;
        private Pen marqueePen;
        private Pen glowPen;

        private List<BaseVisual> nodeVisuals = new List<BaseVisual>();
        private List<BaseVisual> visibleNodeVisuals = new List<BaseVisual>();
        private List<WireVisual> wireVisuals = new List<WireVisual>();

        private List<BaseVisual> selectedNodes = new List<BaseVisual>();
        private List<Point> selectedOffsets = new List<Point>();

        private BaseVisual hoveredNode;

        private bool isMarqueeSelecting;
        private Point marqueeSelectionStart;
        private Point marqueeSelectionCurrent;

        private FormattedText simulationText;

        private DrawingVisual debugLayerVisual;
        private DrawingVisual debugWireVisual;

        private long lastFrameCount = -1;
        private WireVisual editingWire = null;

        public class DrawingContextState
        {
            public DrawingContext DrawingContext { get; private set; }

            // Options
            public bool ConnectorOrdersVisible { get; private set; }

            // Font
            public FontData LargeFont { get; private set; }
            public FontData SmallFont { get; private set; }

            // Matrices
            public MatrixTransform WorldMatrix { get; private set; }
            public MatrixTransform ViewMatrix { get; private set; }
            public double Scale { get; private set; }
            public double InvScale { get; private set; }

            // Various Pens cached
            public Pen BlackPen { get; private set; }
            public Pen GridMinorPen { get; private set; }
            public Pen GridMajorPen { get; private set; }
            public Pen WireLinkPen { get; private set; }
            public Pen WireEventPen { get; private set; }
            public Pen WirePropertyPen { get; private set; }
            public Pen WireHitTestPen { get; private set; }
            public Pen ShortcutWirePen { get; private set; }
            public Pen GlowPen { get; private set; }

            // Various Brushes
            public Brush NodeTitleBackgroundBrush { get; private set; }
            public Brush NodeBackgroundBrush { get; private set; }
            public Brush NodeSelectedBrush { get; private set; }
            public Brush SchematicLinkBrush { get; private set; }
            public Brush SchematicEventBrush { get; private set; }
            public Brush SchematicPropertyBrush { get; private set; }

            public long LastFrameCount { get; private set; }

            public static DrawingContextState CreateFromValues(DrawingContext inContext, MatrixTransform worldMatrix, double inScale)
            {
                DrawingContextState state = new DrawingContextState();
                state.DrawingContext = inContext;
                state.WorldMatrix = worldMatrix;
                state.ViewMatrix = new MatrixTransform();
                state.Scale = inScale;
                state.InvScale = 1.0 / inScale;

                state.BlackPen = new Pen(Brushes.Black, 1.0);
                state.NodeTitleBackgroundBrush = new SolidColorBrush(Color.FromRgb(63, 63, 63));
                state.NodeBackgroundBrush = new SolidColorBrush(Color.FromRgb(194, 194, 194));
                state.SchematicLinkBrush = new SolidColorBrush(Color.FromArgb(0xff, 0x6f, 0xa9, 0xce));
                state.SchematicEventBrush = new SolidColorBrush(Color.FromArgb(0xff, 0xf8, 0xf8, 0xf8));
                state.SchematicPropertyBrush = new SolidColorBrush(Color.FromArgb(0xff, 0x5f, 0xd9, 0x5f));

                state.LargeFont = FontData.MakeFont(new Typeface("Consolas"), 10);
                state.SmallFont = FontData.MakeFont(new Typeface("Consolas"), 7);

                return state;
            }

            private DrawingContextState()
            {
            }

            public DrawingContextState(SchematicsCanvas owner, DrawingContext currentContext)
            {
                DrawingContext = currentContext;

                WorldMatrix = owner.GetWorldMatrix();
                ViewMatrix = owner.GetViewMatrix();
                Scale = owner.scale;
                InvScale = 1.0 / owner.scale;

                ConnectorOrdersVisible = owner.ConnectorOrdersVisible;

                BlackPen = owner.blackPen;
                GridMinorPen = owner.gridMinorPen;
                GridMajorPen = owner.gridMajorPen;
                WireLinkPen = owner.wireLinkPen;
                WireEventPen = owner.wireEventPen;
                WirePropertyPen = owner.wirePropertyPen;
                WireHitTestPen = owner.wireHitTestPen;
                ShortcutWirePen = owner.shortcutWirePen;
                GlowPen = owner.glowPen;

                NodeTitleBackgroundBrush = new SolidColorBrush(Color.FromRgb(63, 63, 63));
                NodeBackgroundBrush = new SolidColorBrush(Color.FromRgb(194, 194, 194));
                NodeSelectedBrush = Brushes.PaleGoldenrod;
                SchematicLinkBrush = owner.SchematicLinkBrush;
                SchematicEventBrush = owner.SchematicEventBrush;
                SchematicPropertyBrush = owner.SchematicPropertyBrush;

                LastFrameCount = owner.lastFrameCount;

                LargeFont = owner.largeFont;
                SmallFont = owner.smallFont;
            }

            public Rect TransformRect(Rect rect)
            {
                return new Rect(WorldMatrix.Transform(rect.Location), new Size(rect.Width * Scale, rect.Height * Scale));
            }

            public Point TransformPoint(Point p)
            {
                return WorldMatrix.Transform(p);
            }

            public GlyphRun ConvertTextLinesToGlyphRun(Point position, bool large, string line)
            {
                FontData fontData = (large) ? LargeFont : SmallFont;

                List<ushort> glyphIndices = new List<ushort>();
                List<double> advanceWidths = new List<double>();
                List<Point> glyphOffsets = new List<Point>();

                double y = -position.Y;
                double x = position.X;

                for (int j = 0; j < line.Length; ++j)
                {
                    ushort glyphIndex = fontData.GlyphTypeface.CharacterToGlyphMap[line[j]];
                    glyphIndices.Add(glyphIndex);
                    advanceWidths.Add(0);
                    glyphOffsets.Add(new Point(x, y));

                    x += fontData.AdvanceWidth;
                }

                return new GlyphRun(
                    fontData.GlyphTypeface,
                    0,
                    false,
                    fontData.RenderingEmSize,
                    96.0f,
                    glyphIndices,
                    fontData.BaselineOrigin,
                    advanceWidths,
                    glyphOffsets,
                    null,
                    null,
                    null,
                    null,
                    null);
            }
        }

        public SchematicsCanvas()
        {
            largeFont = FontData.MakeFont(new Typeface("Consolas"), 10);
            smallFont = FontData.MakeFont(new Typeface("Consolas"), 7);
            OriginalAdvanceWidth = largeFont.AdvanceWidth;

            viewport = new TranslateTransform();

            Focusable = true;
            IsEnabled = true;

            simulationText = new FormattedText("SIMULATING", System.Globalization.CultureInfo.InvariantCulture, FlowDirection.LeftToRight, new Typeface("Consolas"), 48.0, Brushes.White, 96.0);

            debugLayerVisual = new DrawingVisual();
            debugWireVisual = new DrawingVisual();
        }

        // @todo: organise this
        public void GenerateShortcuts(BaseNodeVisual node, BaseNodeVisual.Port port, SchematicsLayout.Shortcut? layoutData = null)
        {
            UndoContainer undoContainer = new UndoContainer("Make Shortcut");
            InterfaceShortcutNodeVisual shortcutNode = new InterfaceShortcutNodeVisual(node, port, new ShortcutParentNodeData() { DisplayName = port.Name }, 1 - port.PortDirection, 0, 0) { UniqueId = Guid.NewGuid() };

            double newX = node.Rect.X + ((port.PortDirection == 0) ? node.Rect.Width + 10 : -shortcutNode.Rect.Width - 10);
            double newY = node.Rect.Y + port.Rect.Y - 4;

            shortcutNode.Rect.Location = new Point(newX, newY);

            if (layoutData != null)
            {
                shortcutNode.UniqueId = (layoutData.Value.UniqueId != Guid.Empty) ? layoutData.Value.UniqueId : shortcutNode.UniqueId;
                shortcutNode.Rect.Location = layoutData.Value.Position;
                shortcutNode.ShortcutData.DisplayName = (layoutData.Value.DisplayName != null) ? layoutData.Value.DisplayName : shortcutNode.ShortcutData.DisplayName;
            }

            InterfaceShortcutNodeVisual parentShortcut = shortcutNode;
            undoContainer.Add(new GenericUndoUnit("",
                (o) => 
                { 
                    AddNode(parentShortcut); 
                    port.ShortcutNode = parentShortcut;
                    parentShortcut.Update();
                }, 
                (o) => 
                { 
                    nodeVisuals.Remove(parentShortcut); 
                    port.ShortcutNode = null; 
                }));
            
            int index = 0;
            int childIndex = 0;

            for (int i = wireVisuals.Count - 1; i >= 0; i--)
            {
                WireVisual wire = wireVisuals[i];
                if ((port.PortDirection == 0 && wire.SourcePort == port) || (port.PortDirection == 1 && wire.TargetPort == port))
                {
                    BaseNodeVisual targetNode = (port.PortDirection == 0) ? wire.Target : wire.Source;
                    BaseNodeVisual.Port targetPort = (port.PortDirection == 0) ? wire.TargetPort : wire.SourcePort;

                    shortcutNode = new InterfaceShortcutNodeVisual(node, port, new ShortcutChildNodeData() { DisplayName = port.Name }, port.PortDirection, 0, 0) { UniqueId = Guid.NewGuid() };

                    newY = targetNode.Rect.Y + targetPort.Rect.Y - 4;
                    newX = (targetNode.Rect.X + targetPort.Rect.X) + ((port.PortDirection == 0) ? -shortcutNode.Rect.Width - 10 : 30);

                    shortcutNode.Rect.Location = new Point(newX, newY);

                    if (layoutData != null)
                    {
                        if (index < layoutData.Value.ChildPositions.Count)
                        {
                            if (layoutData.Value.ChildGuids != null)
                            {
                                shortcutNode.UniqueId = (layoutData.Value.ChildGuids[index] != Guid.Empty) ? layoutData.Value.ChildGuids[index] : shortcutNode.UniqueId;
                            }
                            shortcutNode.Rect.Location = layoutData.Value.ChildPositions[index++];
                            shortcutNode.ShortcutData.DisplayName = (layoutData.Value.DisplayName != null) ? layoutData.Value.DisplayName : shortcutNode.ShortcutData.DisplayName;
                        }
                    }

                    InterfaceShortcutNodeVisual childToAdd = shortcutNode;
                    undoContainer.Add(new GenericUndoUnit("",
                        (o) => 
                        { 
                            AddNode(childToAdd);
                            port.ShortcutChildren.Add(childToAdd);
                            childToAdd.Update();
                        }, 
                        (o) => 
                        { 
                            nodeVisuals.Remove(childToAdd);
                            port.ShortcutChildren.Remove(childToAdd); 
                        }));

                    int indexToDelete = i;
                    WireVisual wireToDelete = wireVisuals[indexToDelete];

                    foreach (WirePointVisual wirePoint in wire.WirePoints)
                    {
                        undoContainer.Add(new GenericUndoUnit("",
                            (o) =>
                            {
                                nodeVisuals.Remove(wirePoint);
                            }, (o) =>
                            {
                                nodeVisuals.Add(wirePoint);
                                wireToDelete.WirePoints.Add(wirePoint);
                            }));
                    }

                    undoContainer.Add(new GenericUndoUnit("",
                        (o) => 
                        {
                            wireToDelete.WirePoints.Clear();
                            wireVisuals.Remove(wireToDelete);
                        }, 
                        (o) =>
                        {
                            wireVisuals.Insert(indexToDelete, wireToDelete);
                        }));

                    int childIndexToUse = childIndex;
                    WireVisual wireToModify = wire;

                    undoContainer.Add(new GenericUndoUnit("",
                        (o) =>
                        {
                            BaseNodeVisual sourceNode = (port.PortDirection == 0) ? port.ShortcutChildren[childIndexToUse] : wireToModify.Source;
                            targetNode = (port.PortDirection == 0) ? wireToModify.Target : port.ShortcutChildren[childIndexToUse];

                            WireVisual newWire = new WireVisual(sourceNode, wireToModify.SourcePort.Name, wireToModify.SourcePort.NameHash, targetNode, wireToModify.TargetPort.Name, wireToModify.TargetPort.NameHash, wireToModify.WireType);
                            wireVisuals.Add(newWire);
                        }, 
                        (o) =>
                        {
                            wireVisuals.RemoveAt(wireVisuals.Count - 1);
                        }));

                    childIndex++;
                }
            }

            {
                undoContainer.Add(new GenericUndoUnit("",
                    (o) =>
                    {
                        BaseNodeVisual sourceNode = (port.PortDirection == 0) ? node : port.ShortcutNode;
                        BaseNodeVisual.Port sourcePort = (port.PortDirection == 0) ? port : port.ShortcutNode.Self;

                        BaseNodeVisual targetNode = (port.PortDirection == 0) ? port.ShortcutNode : node;
                        BaseNodeVisual.Port targetPort = (port.PortDirection == 0) ? port.ShortcutNode.Self : port;

                        WireVisual newWire = new WireVisual(sourceNode, sourcePort.Name, sourcePort.NameHash, targetNode, targetPort.Name, targetPort.NameHash, port.PortType);
                        wireVisuals.Add(newWire);
                    }, 
                    (o) =>
                    {
                        wireVisuals.RemoveAt(wireVisuals.Count - 1);
                    }));
            }

            undoContainer.Add(new GenericUndoUnit("", o => InvalidateVisual(), o => ClearSelection()));

            UndoManager.Instance.CommitUndo(undoContainer);
            if (layoutData.HasValue)
            {
                UndoManager.Instance.Clear();
            }
        }

        public void UpdateDebugLayer()
        {
            lastFrameCount = ItemsSource.World.SimulationFrameCount;
            InvalidateVisual();
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            wireLinkPen = new Pen(SchematicLinkBrush, 2.0); wireLinkPen.Freeze();
            wireEventPen = new Pen(SchematicEventBrush, 2.0); wireEventPen.Freeze();
            wirePropertyPen = new Pen(SchematicPropertyBrush, 2.0); wirePropertyPen.Freeze();
            wireHitTestPen = new Pen(Brushes.Transparent, 4.0); wireHitTestPen.Freeze();
            shortcutWirePen = new Pen(Brushes.WhiteSmoke, 2.0) { DashStyle = DashStyles.Dash }; shortcutWirePen.Freeze();
            marqueePen = new Pen(Brushes.SlateGray, 2.0) { DashStyle = DashStyles.Dash }; marqueePen.Freeze();
            glowPen = new Pen(Brushes.Yellow, 5.0); glowPen.Freeze();

            Unloaded += (o, ev) =>
            {
                if (SuppressLayoutSave)
                    return;

                SchematicsLayout layout = new SchematicsLayout();

                layout.FileGuid = ItemsSource.BlueprintGuid;
                layout.CanvasPosition = new Point(offset.X, offset.Y);
                layout.CanvasScale = scale;
                layout.Nodes = new List<SchematicsLayout.Node>();
                layout.Wires = new List<SchematicsLayout.Wire>();
                layout.Comments = new List<SchematicsLayout.Comment>();

                foreach (BaseVisual node in nodeVisuals)
                {
                    if (node is BaseNodeVisual && !(node is InterfaceShortcutNodeVisual))
                    {
                        layout.Nodes.Add((node as BaseNodeVisual).GenerateLayout());
                    }
                    else if (node is CommentNodeVisual)
                    {
                        layout.Comments.Add((node as CommentNodeVisual).GenerateLayout());
                    }
                }

                for (int i = 0; i < wireVisuals.Count; i++)
                {
                    if (wireVisuals[i].WirePoints.Count > 0)
                    {
                        SchematicsLayout.Wire wireLayout = wireVisuals[i].GenerateLayout();
                        wireLayout.Index = i;

                        layout.Wires.Add(wireLayout);
                    }
                }

                SchematicsLayoutManager.Instance.AddLayout(ItemsSource.BlueprintGuid, layout);
            };

            Focus();
        }

        protected void Update()
        {
            selectedNodes.Clear();
            selectedOffsets.Clear();

            hoveredNode = null;

            visibleNodeVisuals.Clear();
            nodeVisuals.Clear();
            wireVisuals.Clear();

            if (ItemsSource != null)
            {
                ItemsSource.Entities.CollectionChanged += Entities_CollectionChanged;
                ItemsSource.PropertyConnections.CollectionChanged += PropertyConnections_CollectionChanged;
                ItemsSource.EventConnections.CollectionChanged += EventConnections_CollectionChanged;
                ItemsSource.LinkConnections.CollectionChanged += LinkConnections_CollectionChanged;

                GenerateNodes(ItemsSource.Entities);
                GenerateInterface();
                GenerateWires(ItemsSource.LinkConnections, 0);
                GenerateWires(ItemsSource.EventConnections, 1);
                GenerateWires(ItemsSource.PropertyConnections, 2);

                // @temp: Remove any node spatial entity that doesnt have a recorded connection. Need to find a better
                // way to handle this for eliminating nodes that should not appear

                SchematicsLayout? layout = SchematicsLayoutManager.Instance.GetLayout(ItemsSource.BlueprintGuid);
                if (layout.HasValue)
                {
                    scale = layout.Value.CanvasScale;
                    offset = new TranslateTransform(layout.Value.CanvasPosition.X, layout.Value.CanvasPosition.Y);
                    UpdateScaleParameters();
                }

                for (int i = nodeVisuals.Count - 1; i >= 0; i--)
                {
                    if (nodeVisuals[i] is BaseNodeVisual)
                    {
                        BaseNodeVisual nodeVisual = nodeVisuals[i] as BaseNodeVisual;
                        //if (!nodeVisual.IsValid())
                        //{
                        //    nodeVisuals.RemoveAt(i);
                        //}
                        //else
                        {
                            if (layout.HasValue)
                            {
                                nodeVisual.ApplyLayout(layout.Value, this);
                            }

                            // update the node to adjust for the collapsed state
                            nodeVisual.Update();
                        }
                    }
                }

                if (layout.HasValue)
                {
                    if (layout.Value.Wires != null)
                    {
                        foreach (SchematicsLayout.Wire wireLayout in layout.Value.Wires)
                        {
                            wireVisuals[wireLayout.Index].ApplyLayout(wireLayout, nodeVisuals);
                        }
                    }
                    if (layout.Value.Comments != null)
                    {
                        List<BaseVisual> comments = new List<BaseVisual>();
                        foreach (SchematicsLayout.Comment commentLayout in layout.Value.Comments)
                        {
                            List<BaseVisual> children = new List<BaseVisual>();
                            foreach (Guid guid in commentLayout.Children)
                            {
                                children.Add(nodeVisuals.Find(n => n.UniqueId == guid));
                            }

                            CommentNodeVisual commentVisual = new CommentNodeVisual(
                                new CommentNodeData()
                                {
                                    Color = new FrostySdk.Ebx.Vec4() { x = commentLayout.Color.R / 255.0f, y = commentLayout.Color.G / 255.0f, z = commentLayout.Color.B / 255.0f, w = 1.0f },
                                    CommentText = commentLayout.Text

                                }, children, commentLayout.Position.X, commentLayout.Position.Y);
                            commentVisual.UniqueId = commentLayout.UniqueId;
                            comments.Add(commentVisual);
                        }
                        nodeVisuals.AddRange(comments);
                    }
                }
            }

            InvalidateVisual();
        }

        protected override void OnPreviewKeyUp(KeyEventArgs e)
        {
            if (selectedNodes.Count == 1 && selectedNodes[0] is InterfaceNodeVisual && !(selectedNodes[0] is InterfaceShortcutNodeVisual))
            {
                if (e.Key == Key.S)
                {
                    InterfaceNodeVisual node = selectedNodes[0] as InterfaceNodeVisual;
                    if (node.Self.ShortcutNode == null)
                    {
                        GenerateShortcuts(node, node.Self);
                    }

                    e.Handled = true;
                }
            }
            else if (e.Key == Key.Delete)
            {
                UndoContainer container = new UndoContainer("Delete Wire Point(s)");
                //bool invalidate = false;
                for (int i = selectedNodes.Count - 1; i >= 0; i--)
                {
                    BaseVisual node = selectedNodes[i];
                    if (node is WirePointVisual)
                    {
                        WirePointVisual wirePoint = node as WirePointVisual;
                        container.Add(new GenericUndoUnit("",
                            (o) =>
                            {
                                wirePoint.Wire.RemoveWirePoint(wirePoint);
                                nodeVisuals.Remove(wirePoint);
                            }, 
                            (o) =>
                            {
                                wirePoint.Wire.AddWirePoint(wirePoint, force: true);
                                nodeVisuals.Add(wirePoint);
                            }));

                        selectedNodes.RemoveAt(i);
                        selectedOffsets.RemoveAt(i);
                    }
                    
                }

                if (container.HasItems)
                {
                    container.Add(new GenericUndoUnit("", o => InvalidateVisual(), o => ClearSelection()));
                    UndoManager.Instance.CommitUndo(container);
                }

                e.Handled = true;
            }
            else if (e.Key == Key.G && Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                if (selectedNodes.Count > 0)
                {
                    CommentNodeVisual commentNode = new CommentNodeVisual(new CommentNodeData() { Color = new FrostySdk.Ebx.Vec4() { x = 0.25f, y = 0.0f, z = 0.0f, w = 1.0f }, CommentText = "@todo" }, selectedNodes, 0, 0) { UniqueId = Guid.NewGuid() };
                    UndoManager.Instance.CommitUndo(new GenericUndoUnit("Create Comment Group",
                        (o) =>
                        {
                            AddNode(commentNode);
                            InvalidateVisual();
                        },
                        (o) =>
                        {
                            nodeVisuals.Remove(commentNode);
                            ClearSelection();
                        }));
                }

                e.Handled = true;
            }

            base.OnPreviewKeyUp(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Focus();

                MatrixTransform m = GetWorldMatrix();
                Point mousePos = m.Inverse.Transform(e.GetPosition(this));

                if (editingWire != null)
                {
                    editingWire.OnMouseMove(mousePos);
                    editingWire.UpdateEditing(mousePos, visibleNodeVisuals.Where(n => n is BaseNodeVisual));
                    InvalidateVisual();
                    return;
                }

                if (hoveredNode != null && hoveredNode is NodeVisual)
                {
                    if ((hoveredNode as NodeVisual).CollapseHover)
                        return;
                    if ((hoveredNode as NodeVisual).HightlightedPort != null)
                        return;
                }

                if (selectedNodes.Count > 0 && selectedNodes.Contains(hoveredNode))
                {
                    if (!UndoManager.Instance.IsUndoing && UndoManager.Instance.PendingUndoUnit == null)
                    {
                        List<BaseVisual> prevSelection = new List<BaseVisual>();
                        prevSelection.AddRange(selectedNodes);

                        List<Point> prevPositions = new List<Point>();
                        foreach (BaseVisual node in selectedNodes)
                            prevPositions.Add(node.Rect.Location);
                        
                        UndoManager.Instance.PendingUndoUnit = new GenericUndoUnit("Move Nodes",
                            (o) =>
                            {
                                // do nothing for now
                            },
                            (o) =>
                            {
                                for (int i = 0; i < prevSelection.Count; i++)
                                {
                                    prevSelection[i].Move(prevPositions[i]);
                                }
                                ClearSelection();
                            });
                    }

                    for (int i = 0; i < selectedNodes.Count; i++)
                    {
                        double newX = mousePos.X + selectedOffsets[i].X;
                        double newY = mousePos.Y + selectedOffsets[i].Y;

                        newX -= (newX % 5);
                        newY -= (newY % 5);

                        selectedNodes[i].Move(new Point(newX, newY));
                    }
                    InvalidateVisual();
                    return;
                }

                if (isMarqueeSelecting)
                {
                    marqueeSelectionCurrent = mousePos;

                    double width = marqueeSelectionCurrent.X - marqueeSelectionStart.X;
                    double height = marqueeSelectionCurrent.Y - marqueeSelectionStart.Y;
                    Point startPos = marqueeSelectionStart;

                    if (width <= 0)
                    {
                        width = marqueeSelectionStart.X - marqueeSelectionCurrent.X;
                        startPos.X = marqueeSelectionCurrent.X;
                    }
                    if (height <= 0)
                    {
                        height = marqueeSelectionStart.Y - marqueeSelectionCurrent.Y;
                        startPos.Y = marqueeSelectionCurrent.Y;
                    }

                    Rect selectionRect = new Rect(startPos, new Size(width, height));

                    for (int i = selectedNodes.Count - 1; i >= 0; i--)
                    {
                        if (!selectionRect.Contains(selectedNodes[i].Rect))
                        {
                            selectedNodes[i].IsSelected = false;
                            selectedNodes.RemoveAt(i);
                            selectedOffsets.RemoveAt(i);
                        }
                    }

                    foreach (BaseVisual node in nodeVisuals)
                    {
                        if (selectionRect.Contains(node.Rect))
                        {
                            if (!selectedNodes.Contains(node))
                            {
                                node.IsSelected = true;
                                selectedNodes.Add(node);
                                selectedOffsets.Add(new Point(node.Rect.Location.X - mousePos.X, node.Rect.Location.Y - mousePos.Y));
                            }
                        }
                    }

                    InvalidateVisual();
                }
            }
            else
            {
                MatrixTransform m = GetWorldMatrix();
                Point mousePos = m.Inverse.Transform(e.GetPosition(this));

                foreach (BaseVisual visual in visibleNodeVisuals)
                {
                    if (visual.Rect.Contains(mousePos) && visual.HitTest(mousePos))
                    {
                        bool invalidate = false;
                        if (hoveredNode != null && hoveredNode != visual)
                        {
                            hoveredNode.OnMouseLeave();
                            invalidate = true;
                        }

                        hoveredNode = visual;
                        Mouse.OverrideCursor = Cursors.SizeAll;

                        if (visual.OnMouseOver(mousePos))
                        {
                            invalidate = true;
                        }

                        if (invalidate)
                        {
                            InvalidateVisual();
                        }

                        return;
                    }
                }

                if (hoveredNode != null)
                {
                    if (hoveredNode.OnMouseLeave())
                        InvalidateVisual();

                    hoveredNode = null;
                    Mouse.OverrideCursor = null;
                }
            }

            base.OnMouseMove(e);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            MatrixTransform m = GetWorldMatrix();
            Point mousePos = m.Inverse.Transform(e.GetPosition(this));

            foreach (BaseVisual visual in visibleNodeVisuals)
            {
                if (visual.OnMouseDown(mousePos, e.ChangedButton))
                {
                    e.Handled = true;
                    InvalidateVisual();
                    return;
                }
            }

            if (e.ChangedButton == MouseButton.Left)
            {
                prevMousePos = mousePos;

                if (hoveredNode != null)
                {
                    if (hoveredNode is BaseNodeVisual && (hoveredNode as BaseNodeVisual).HightlightedPort != null)
                    {
                        BaseNodeVisual node = hoveredNode as BaseNodeVisual;
                        BaseNodeVisual sourceNode = (node.HightlightedPort.PortDirection == 0) ? node : null;
                        BaseNodeVisual targetNode = (node.HightlightedPort.PortDirection == 1) ? node : null;

                        editingWire = new WireVisual(
                            sourceNode,
                            (sourceNode != null) ? sourceNode.HightlightedPort.Name : "",
                            (sourceNode != null) ? sourceNode.HightlightedPort.NameHash : 0,
                            targetNode,
                            (targetNode != null) ? targetNode.HightlightedPort.Name : "",
                            (targetNode != null) ? targetNode.HightlightedPort.NameHash : 0,
                            node.HightlightedPort.PortType
                            );
                        editingWire.OnMouseMove(mousePos);

                        InvalidateVisual();
                        return;
                    }

                    foreach (BaseVisual visual in visibleNodeVisuals)
                    {
                        if (visual.Rect.Contains(mousePos) && visual.HitTest(mousePos))
                        {
                            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                            {
                                Mouse.Capture(this);

                                if (!selectedNodes.Contains(visual))
                                {
                                    selectedNodes.Add(visual);
                                    selectedOffsets.Add(new Point(visual.Rect.Location.X - mousePos.X, visual.Rect.Location.Y - mousePos.Y));

                                    visual.IsSelected = true;

                                    if (visual.Data != null)
                                    {
                                        SelectedNodeChangedCommand?.Execute(visual.Data);
                                    }

                                    InvalidateVisual();
                                }
                            }
                            else
                            {
                                if (!selectedNodes.Contains(visual))
                                {
                                    if (selectedNodes.Count > 0)
                                    {
                                        foreach (BaseVisual node in selectedNodes)
                                        {
                                            node.IsSelected = false;
                                        }

                                        selectedNodes.Clear();
                                        selectedOffsets.Clear();
                                    }

                                    selectedNodes.Add(visual);
                                    selectedOffsets.Add(new Point(visual.Rect.Location.X - mousePos.X, visual.Rect.Location.Y - mousePos.Y));

                                    visual.IsSelected = true;

                                    if (visual.Data != null)
                                    {
                                        SelectedNodeChangedCommand?.Execute(visual.Data);
                                    }
                                    InvalidateVisual();
                                }
                            }

                            for (int i = 0; i < selectedOffsets.Count; i++)
                            {
                                selectedOffsets[i] = new Point(selectedNodes[i].Rect.Location.X - mousePos.X, selectedNodes[i].Rect.Location.Y - mousePos.Y);
                            }

                            return;
                        }
                    }
                }
                else
                {
                    Mouse.Capture(this);
                    isMarqueeSelecting = hoveredNode == null;
                    marqueeSelectionStart = mousePos;
                    marqueeSelectionCurrent = mousePos;
                }

                InvalidateVisual();
            }

            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            MatrixTransform m = GetWorldMatrix();
            Point mousePos = m.Inverse.Transform(e.GetPosition(this));

            if (editingWire != null)
            {
                if (editingWire.HoveredPort != null)
                {
                    BaseNodeVisual.Port sourcePort = (editingWire.Source != null) ? editingWire.SourcePort : editingWire.HoveredPort;
                    BaseNodeVisual.Port targetPort = (editingWire.Target != null) ? editingWire.TargetPort : editingWire.HoveredPort;

                    WireAddedCommand?.Execute(new WireAddedEventArgs(
                        (sourcePort.Owner is NodeVisual) ? (sourcePort.Owner as NodeVisual).Entity : null, 
                        (targetPort.Owner is NodeVisual) ? (targetPort.Owner as NodeVisual).Entity : null, 
                        sourcePort.NameHash, targetPort.NameHash, editingWire.WireType
                        ));
                }

                editingWire.OnMouseUp(mousePos, e.ChangedButton);
                editingWire.Disconnect();
                editingWire = null;

                InvalidateVisual();
                return;
            }

            foreach (BaseVisual visual in visibleNodeVisuals)
            {
                if (visual.OnMouseUp(mousePos, e.ChangedButton))
                {
                    e.Handled = true;
                    InvalidateVisual();
                    return;
                }
            }

            if (e.ChangedButton == MouseButton.Left)
            {
                //prevMousePos = mousePos;
                Mouse.Capture(null);

                if (UndoManager.Instance.PendingUndoUnit != null)
                {
                    // commit any pending moves
                    UndoManager.Instance.CommitUndo(UndoManager.Instance.PendingUndoUnit);
                }

                foreach (WireVisual wire in wireVisuals)
                {
                    if (wire.OnMouseUp(mousePos, e.ChangedButton))
                    {
                        WirePointVisual wirePoint = new WirePointVisual(wire, mousePos.X, mousePos.Y) { UniqueId = Guid.NewGuid() };
                        WireVisual wireToModify = wire;

                        UndoManager.Instance.CommitUndo(new GenericUndoUnit("Add Wire Point",
                            (o) =>
                            {
                                wireToModify.AddWirePoint(wirePoint);
                                AddNode(wirePoint);

                                InvalidateVisual();
                            },
                            (o) =>
                            {
                                wireToModify.RemoveWirePoint(wirePoint);
                                nodeVisuals.Remove(wirePoint);

                                ClearSelection();
                            }));

                        return;
                    }
                }

                if (hoveredNode != null)
                {
                    if (PointEqualsWithTolerance(mousePos, prevMousePos))
                    {
                        if (selectedNodes.Count > 0 && !(Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)))
                        {
                            foreach (BaseVisual node in selectedNodes)
                            {
                                node.IsSelected = false;
                            }

                            selectedNodes.Clear();
                            selectedOffsets.Clear();

                            SelectedNodeChangedCommand?.Execute(null);
                        }

                        if (!selectedNodes.Contains(hoveredNode))
                        {
                            selectedNodes.Add(hoveredNode);
                            selectedOffsets.Add(new Point(hoveredNode.Rect.Location.X - mousePos.X, hoveredNode.Rect.Location.Y - mousePos.Y));

                            hoveredNode.IsSelected = true;

                            if (hoveredNode.Data != null)
                            {
                                SelectedNodeChangedCommand?.Execute(hoveredNode.Data);
                            }
                        }
                        InvalidateVisual();
                    }
                }
                else
                {
                    if (!isMarqueeSelecting || PointEqualsWithTolerance(mousePos, prevMousePos))
                    {
                        if (selectedNodes.Count > 0 && !(Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)))
                        {
                            foreach (BaseVisual node in selectedNodes)
                            {
                                node.IsSelected = false;
                            }

                            selectedNodes.Clear();
                            selectedOffsets.Clear();

                            SelectedNodeChangedCommand?.Execute(null);
                        }
                    }
                }

                if (isMarqueeSelecting && e.ChangedButton == MouseButton.Left)
                {
                    isMarqueeSelecting = false;
                    Mouse.Capture(null);

                    InvalidateVisual();
                    return;
                }
            }

            base.OnMouseUp(e);
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            Mouse.OverrideCursor = null;
        }

        protected override void Render(DrawingContext drawingContext)
        {
            if (ItemsSource == null)
                return;

            UpdateVisibileNodes();

            if (ItemsSource.World.IsSimulationRunning)
            {
                DrawingContext drawingContext2 = debugLayerVisual.RenderOpen();
                RenderDebug(drawingContext2);
                drawingContext2.Close();

                drawingContext2 = debugWireVisual.RenderOpen();
                RenderDebugWires(drawingContext2);
                drawingContext2.Close();
            }

            DrawingContextState state = new DrawingContextState(this, drawingContext);
            {
                // draw wires
                for (int i = 0; i < wireVisuals.Count; i++)
                {
                    wireVisuals[i].Render(state);
                }
                if (editingWire != null)
                {
                    editingWire.Render(state);
                }

                if (ItemsSource.World.IsSimulationRunning)
                {
                    drawingContext.DrawDrawing(debugWireVisual.Drawing);
                }

                // draw nodes
                for (int i = visibleNodeVisuals.Count - 1; i >= 0; i--)
                {
                    visibleNodeVisuals[i].Render(state);
                }

                if (ItemsSource.World.IsSimulationRunning)
                {
                    drawingContext.DrawDrawing(debugLayerVisual.Drawing);
                }

                DrawSelectionRect(drawingContext);
            }
        }

        protected void RenderDebugWires(DrawingContext drawingContext)
        {
            DrawingContextState state = new DrawingContextState(this, drawingContext);

            for (int i = 0; i < wireVisuals.Count; i++)
            {
                wireVisuals[i].RenderDebug(state);
            }
        }

        protected void RenderDebug(DrawingContext drawingContext)
        {
            DrawingContextState state = new DrawingContextState(this, drawingContext);

            for (int i = visibleNodeVisuals.Count - 1; i >= 0; i--)
            {
                visibleNodeVisuals[i].RenderDebug(state);
            }

            drawingContext.PushOpacity(0.25);
            drawingContext.DrawText(simulationText, new Point(ActualWidth - simulationText.Width - 10, ActualHeight - simulationText.Height));
            drawingContext.Pop();
        }

        protected override void UpdateScaleParameters()
        {
            base.UpdateScaleParameters();

            largeFont.Update(scale);
            smallFont.Update(scale);
        }

        private void ClearSelection()
        {
            foreach (BaseVisual node in selectedNodes)
            {
                node.IsSelected = false;
            }
            selectedNodes.Clear();
            selectedOffsets.Clear();

            InvalidateVisual();
        }

        private void AddNode(BaseVisual nodeToAdd)
        {
            foreach (BaseVisual node in nodeVisuals.Where(n => n is CommentNodeVisual))
            {
                if (node.Rect.Contains(nodeToAdd.Rect))
                {
                    CommentNodeVisual commentNode = node as CommentNodeVisual;
                    commentNode.AddNode(nodeToAdd);
                }
            }
            nodeVisuals.Add(nodeToAdd);
        }

        private void DrawSelectionRect(DrawingContext drawingContext)
        {
            if (!isMarqueeSelecting)
                return;

            MatrixTransform m = GetWorldMatrix();

            double width = marqueeSelectionCurrent.X - marqueeSelectionStart.X;
            double height = marqueeSelectionCurrent.Y - marqueeSelectionStart.Y;

            if (width == 0 && height == 0)
                return;

            Point startPos = marqueeSelectionStart;

            if (width <= 0)
            {
                width = marqueeSelectionStart.X - marqueeSelectionCurrent.X;
                startPos.X = marqueeSelectionCurrent.X;
            }
            if (height <= 0)
            {
                height = marqueeSelectionStart.Y - marqueeSelectionCurrent.Y;
                startPos.Y = marqueeSelectionCurrent.Y;
            }

            drawingContext.DrawRectangle(null, marqueePen, new Rect(m.Transform(startPos), new Size(width * scale, height * scale)));
        }

        private void UpdateVisibileNodes()
        {
            MatrixTransform m = GetWorldMatrix();
            Point topLeft = m.Inverse.Transform(new Point(0, 0));
            Point bottomRight = m.Inverse.Transform(new Point(ActualWidth, ActualHeight));
            Rect fustrum = new Rect(topLeft, bottomRight);

            visibleNodeVisuals.Clear();
            foreach (BaseVisual visual in nodeVisuals)
            {
                if (fustrum.IntersectsWith(visual.Rect) || visual.IsSelected)
                {
                    if (visual.IsSelected) visibleNodeVisuals.Insert(0, visual);
                    else visibleNodeVisuals.Add(visual);
                }
            }
        }

        private void GenerateNodes(IEnumerable<Entity> entities)
        {
            ContextMenu portContextMenu = new ContextMenu();
            MenuItem makeShortcutItem = new MenuItem() { Header = "Make shortcut" };
            makeShortcutItem.Click += (o, e) =>
            {
                BaseNodeVisual.Port port = (o as MenuItem).DataContext as BaseNodeVisual.Port;
                GenerateShortcuts(port.Owner, port);
            };
            portContextMenu.Items.Add(makeShortcutItem);

            Random r = new Random(2);
            foreach (Entity entity in entities)
            {
                ILogicEntity logicEntity = entity as ILogicEntity;
                ContextMenu nodeContextMenu = new ContextMenu();

                if (logicEntity is LogicEntity)
                {
                    LogicEntity l = logicEntity as LogicEntity;
                    foreach (ConnectionDesc conn in l.Events)
                    {
                        if (conn.IsTriggerable)
                        {
                            MenuItem triggerMenuItem = new MenuItem() { Header = $"Trigger {conn.Name}" };
                            triggerMenuItem.Click += (o, e) => { l.EventToTrigger = Frosty.Hash.Fnv1.HashString(conn.Name); };
                            nodeContextMenu.Items.Add(triggerMenuItem);
                        }
                    }
                }

                double x = ((int)(r.NextDouble() * 2000)) - 1000;
                double y = ((int)(r.NextDouble() * 2000)) - 1000;
                bool isCollapsed = entity.UserData == null;

                x = x - (x % 10);
                y = y - (y % 10);

                NodeVisual node = new NodeVisual(logicEntity, x, y) 
                { 
                    GlyphWidth = OriginalAdvanceWidth, 
                    NodeContextMenu = (nodeContextMenu.Items.Count > 0) ? nodeContextMenu : null,
                    PortContextMenu = portContextMenu, 
                    UniqueId = Guid.NewGuid() 
                };

                nodeVisuals.Add(node);

                node.IsCollapsed = isCollapsed;
                node.Update();

                if (entity.UserData != null && entity.UserData is Point)
                {
                    Point mousePos = GetWorldMatrix().Inverse.Transform((Point)entity.UserData);

                    x = mousePos.X;
                    y = mousePos.Y;

                    node.IsCollapsed = false;
                    node.Rect.X = x - node.Rect.Width * 0.5;
                    node.Rect.Y = y - node.Rect.Height * 0.5;
                }
            }
        }

        private void GenerateInterface()
        {
            if (ItemsSource.InterfaceDescriptor == null)
                return;

            InterfaceDescriptor interfaceDesc = ItemsSource.InterfaceDescriptor as InterfaceDescriptor;
            Random r = new Random(1);

            foreach (DataField dataField in interfaceDesc.Data.Fields)
            {
                double x = ((int)(r.NextDouble() * 2000)) - 1000;
                double y = ((int)(r.NextDouble() * 2000)) - 1000;

                x = x - (x % 10);
                y = y - (y % 10);

                switch (dataField.AccessType)
                {
                    case FrostySdk.Ebx.FieldAccessType.FieldAccessType_Source: nodeVisuals.Add(new InterfaceNodeVisual(ItemsSource.BlueprintGuid, interfaceDesc, 1, dataField, x, y) { GlyphWidth = OriginalAdvanceWidth, UniqueId = Guid.NewGuid() }); break;
                    case FrostySdk.Ebx.FieldAccessType.FieldAccessType_Target: nodeVisuals.Add(new InterfaceNodeVisual(ItemsSource.BlueprintGuid, interfaceDesc, 0, dataField, x, y) { GlyphWidth = OriginalAdvanceWidth, UniqueId = Guid.NewGuid() }); break;
                    case FrostySdk.Ebx.FieldAccessType.FieldAccessType_SourceAndTarget:
                        nodeVisuals.Add(new InterfaceNodeVisual(ItemsSource.BlueprintGuid, interfaceDesc, 1, dataField, x, y) { GlyphWidth = OriginalAdvanceWidth });
                        nodeVisuals.Add(new InterfaceNodeVisual(ItemsSource.BlueprintGuid, interfaceDesc, 0, dataField, x, y) { GlyphWidth = OriginalAdvanceWidth });
                        break;
                }
            }

            foreach (DynamicEvent eventField in interfaceDesc.Data.InputEvents)
            {
                double x = ((int)(r.NextDouble() * 2000)) - 1000;
                double y = ((int)(r.NextDouble() * 2000)) - 1000;

                x = x - (x % 10);
                y = y - (y % 10);

                nodeVisuals.Add(new InterfaceNodeVisual(ItemsSource.BlueprintGuid, interfaceDesc, 0, eventField, x, y) { GlyphWidth = OriginalAdvanceWidth, UniqueId = Guid.NewGuid() });
            }
            foreach (DynamicEvent eventField in interfaceDesc.Data.OutputEvents)
            {
                double x = ((int)(r.NextDouble() * 2000)) - 1000;
                double y = ((int)(r.NextDouble() * 2000)) - 1000;

                x = x - (x % 10);
                y = y - (y % 10);

                nodeVisuals.Add(new InterfaceNodeVisual(ItemsSource.BlueprintGuid, interfaceDesc, 1, eventField, x, y) { GlyphWidth = OriginalAdvanceWidth, UniqueId = Guid.NewGuid() });
            }

            foreach (DynamicLink linkField in interfaceDesc.Data.InputLinks)
            {
                double x = ((int)(r.NextDouble() * 2000)) - 1000;
                double y = ((int)(r.NextDouble() * 2000)) - 1000;

                x = x - (x % 10);
                y = y - (y % 10);

                nodeVisuals.Add(new InterfaceNodeVisual(ItemsSource.BlueprintGuid, interfaceDesc, 0, linkField, x, y) { GlyphWidth = OriginalAdvanceWidth, UniqueId = Guid.NewGuid() });
            }
            foreach (DynamicLink linkField in interfaceDesc.Data.OutputLinks)
            {
                double x = ((int)(r.NextDouble() * 2000)) - 1000;
                double y = ((int)(r.NextDouble() * 2000)) - 1000;

                x = x - (x % 10);
                y = y - (y % 10);

                nodeVisuals.Add(new InterfaceNodeVisual(ItemsSource.BlueprintGuid, interfaceDesc, 1, linkField, x, y) { GlyphWidth = OriginalAdvanceWidth, UniqueId = Guid.NewGuid() });
            }
        }

        private void GenerateWires(IEnumerable<object> connections, int connectionType)
        {
            IEnumerable<BaseNodeVisual> nodes = nodeVisuals.Where(n => n is BaseNodeVisual).Select(n => n as BaseNodeVisual);
            foreach (object obj in connections)
            {
                FrostySdk.Ebx.PointerRef source = new FrostySdk.Ebx.PointerRef();
                FrostySdk.Ebx.PointerRef target = new FrostySdk.Ebx.PointerRef();
                int sourceId = -1;
                int targetId = -1;
                string sourceName = null;
                string targetName = null;

                switch (connectionType)
                {
                    case 0:
                        {
                            LinkConnection linkConnection = obj as FrostySdk.Ebx.LinkConnection;
                            source = MakeRef(linkConnection.Source, ItemsSource.BlueprintGuid);
                            target = MakeRef(linkConnection.Target, ItemsSource.BlueprintGuid);
                            sourceId = linkConnection.SourceFieldId;
                            targetId = linkConnection.TargetFieldId;
                            sourceName = linkConnection.SourceField;
                            targetName = linkConnection.TargetField;
                        }
                        break;
                    case 1:
                        {
                            EventConnection eventConnection = obj as FrostySdk.Ebx.EventConnection;
                            source = MakeRef(eventConnection.Source, ItemsSource.BlueprintGuid);
                            target = MakeRef(eventConnection.Target, ItemsSource.BlueprintGuid);
                            sourceId = eventConnection.SourceEvent.Id;
                            targetId = eventConnection.TargetEvent.Id;
                            sourceName = eventConnection.SourceEvent.Name;
                            targetName = eventConnection.TargetEvent.Name;
                        }
                        break;
                    case 2:
                        {
                            PropertyConnection propertyConnection = obj as FrostySdk.Ebx.PropertyConnection;
                            source = MakeRef(propertyConnection.Source, ItemsSource.BlueprintGuid);
                            target = MakeRef(propertyConnection.Target, ItemsSource.BlueprintGuid);
                            sourceId = propertyConnection.SourceFieldId;
                            targetId = propertyConnection.TargetFieldId;
                            sourceName = propertyConnection.SourceField;
                            targetName = propertyConnection.TargetField;
                        }
                        break;
                }

                BaseVisual sourceVisual = nodes.FirstOrDefault(v => v.Matches(source, sourceId, 0));
                BaseVisual targetVisual = nodes.FirstOrDefault(v => v.Matches(target, targetId, 1));

                if (sourceVisual == null || targetVisual == null)
                    continue;

                WireVisual wire = new WireVisual(
                    sourceVisual as BaseNodeVisual,
                    sourceName,
                    sourceId,
                    targetVisual as BaseNodeVisual,
                    targetName,
                    targetId,
                    connectionType);
                wire.Data = obj;

                if (wire.SourcePort == null || wire.TargetPort == null)
                    continue;

                wireVisuals.Add(wire);
            }
        }

        private void Entities_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                GenerateNodes(e.NewItems.OfType<Entity>());
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (object entity in e.OldItems)
                {
                    nodeVisuals.Remove(nodeVisuals.Find(n => n.Data == entity));
                    // @todo: cleanup wires
                }
            }

            InvalidateVisual();
        }

        private void LinkConnections_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                GenerateWires(e.NewItems.OfType<object>(), 0);
            }

            InvalidateVisual();
        }

        private void EventConnections_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                GenerateWires(e.NewItems.OfType<object>(), 1);
            }

            InvalidateVisual();
        }

        private void PropertyConnections_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                GenerateWires(e.NewItems.OfType<object>(), 2);
            }

            InvalidateVisual();
        }

        private FrostySdk.Ebx.PointerRef MakeRef(FrostySdk.Ebx.PointerRef pr, Guid guid)
        {
            if (pr.Type == PointerRefType.External)
                return pr;

            return new FrostySdk.Ebx.PointerRef(new EbxImportReference() { FileGuid = guid, ClassGuid = pr.GetInstanceGuid() });
        }

        private bool PointEqualsWithTolerance(Point a, Point b)
        {
            return a.X >= b.X - 0.5 && a.Y >= b.Y - 0.5 && a.X <= b.X + 0.5 && a.Y <= b.Y + 0.5;
        }

        private static void OnEntitiesChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            SchematicsCanvas canvas = o as SchematicsCanvas;
            canvas.Update();
        }

        private static void OnDataModified(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null)
                return;

            SchematicsCanvas canvas = o as SchematicsCanvas;
            foreach (BaseVisual node in canvas.visibleNodeVisuals)
                node.Update();
            canvas.InvalidateVisual();
        }

        private static void OnDebugUpdateModified(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null)
                return;

            SchematicsCanvas canvas = o as SchematicsCanvas;
            canvas.UpdateDebugLayer();
        }
    }
}
