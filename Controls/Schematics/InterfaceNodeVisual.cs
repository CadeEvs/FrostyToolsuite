using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using LevelEditorPlugin.Entities;
using LevelEditorPlugin.Managers;

namespace LevelEditorPlugin.Controls
{
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

        public override Port Connect(WireVisual wire, string name, int nameHash, int portType, int direction)
        {
            Self.IsConnected = true;
            Self.ConnectionCount++;
            Self.Connections.Add(wire);

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
                HighlightedPort = Self;
                Self.IsHighlighted = true;
                changedHighlight = true;
            }
            else if (Self.IsHighlighted)
            {
                HighlightedPort = null;
                Self.IsHighlighted = false;
                changedHighlight = true;
            }

            return changedHighlight;
        }

        public override bool OnMouseLeave()
        {
            if (HighlightedPort != null)
            {
                HighlightedPort.IsHighlighted = false;
                HighlightedPort = null;
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
}