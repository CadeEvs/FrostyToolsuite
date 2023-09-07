using Frosty.Core;
using Frosty.Core.Controls;
using Frosty.Core.Controls.Editors;
using FrostySdk;
using FrostySdk.Ebx;
using FrostySdk.IO;
using FrostySdk.Managers;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;

namespace ConnectionPlugin.Editors
{
    public class PropertyConnectionEditor : FrostyTypeEditor<PropertyConnectionControl>
    {
        public PropertyConnectionEditor()
        {
            ValueProperty = PropertyConnectionControl.ValueProperty;
            NotifyOnTargetUpdated = true;
        }
    }
    [TemplatePart(Name = "PART_StackPanel", Type = typeof(StackPanel))]
    public class PropertyConnectionControl : Control
    {
        #region -- Properties --

        #region -- Value --
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(object), typeof(PropertyConnectionControl), new FrameworkPropertyMetadata(null));
        public object Value
        {
            get => GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }
        #endregion

        #endregion

        static PropertyConnectionControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyConnectionControl), new FrameworkPropertyMetadata(typeof(PropertyConnectionControl)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Loaded += PropertyConnectionControl_Loaded;
        }

        private bool firstTime = true;
        private void PropertyConnectionControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (firstTime)
            {
                TargetUpdated += PropertyConnectionControl_TargetUpdated;
                RefreshUI();

                firstTime = false;
            }
        }

        private void PropertyConnectionControl_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            RefreshUI();
        }

        protected virtual void RefreshUI()
        {
            dynamic propConnection = Value;
            StackPanel sp = GetTemplateChild("PART_StackPanel") as StackPanel;

            (sp.Children[0] as TextBlock).Text = GetEntity(propConnection.Source);
            (sp.Children[4] as TextBlock).Text = GetEntity(propConnection.Target);
            (sp.Children[2] as TextBlock).Text = Sanitize(propConnection.Source, propConnection.SourceField);
            (sp.Children[6] as TextBlock).Text = Sanitize(propConnection.Target, propConnection.TargetField);
            // This has to be in a try block because earlier games don't have property connection flags
            try
            {
                TextBlock realmTextBox = (sp.Children[7] as TextBlock);
                GetRealmText(propConnection.Flags & 0x7, realmTextBox);
            }
            catch (RuntimeBinderException) { }
        }

        protected void GetRealmText(uint realm, TextBlock textBlock)
        {
            textBlock.Inlines.Clear();
            switch(realm)
            {
                case 1: // ClientAndServer
                    textBlock.Inlines.Add(new Run("N") { Foreground = Brushes.SlateGray });
                    textBlock.Inlines.Add(new Run("C") { Foreground = Brushes.Yellow });
                    textBlock.Inlines.Add(new Run("S") { Foreground = Brushes.Yellow });
                    break;
                case 2: // Client
                    textBlock.Inlines.Add(new Run("N") { Foreground = Brushes.SlateGray });
                    textBlock.Inlines.Add(new Run("C") { Foreground = Brushes.Yellow });
                    textBlock.Inlines.Add(new Run("S") { Foreground = Brushes.SlateGray });
                    break;
                case 3: // Server
                    textBlock.Inlines.Add(new Run("N") { Foreground = Brushes.SlateGray });
                    textBlock.Inlines.Add(new Run("C") { Foreground = Brushes.SlateGray });
                    textBlock.Inlines.Add(new Run("S") { Foreground = Brushes.Yellow });
                    break;
                case 4: // NetworkedClient
                    textBlock.Inlines.Add(new Run("N") { Foreground = Brushes.Yellow });
                    textBlock.Inlines.Add(new Run("C") { Foreground = Brushes.Yellow });
                    textBlock.Inlines.Add(new Run("S") { Foreground = Brushes.SlateGray });
                    break;
                case 5: // NetworkedClientAndServer
                    textBlock.Inlines.Add(new Run("N") { Foreground = Brushes.Yellow });
                    textBlock.Inlines.Add(new Run("C") { Foreground = Brushes.Yellow });
                    textBlock.Inlines.Add(new Run("S") { Foreground = Brushes.Yellow });
                    break;
                default: // Invalid or unknown
                    textBlock.Inlines.Add(new Run("N") { Foreground = Brushes.Red });
                    textBlock.Inlines.Add(new Run("C") { Foreground = Brushes.Red });
                    textBlock.Inlines.Add(new Run("S") { Foreground = Brushes.Red });
                    break;
            }
        }

        protected object ResolvePointer(PointerRef pr, out EbxAssetEntry externalAssetEntry)
        {
            externalAssetEntry = null;

            if (pr.Type is PointerRefType.Internal)
            {
                return pr.Internal;
            }
            else if (pr.Type is PointerRefType.External)
            {
                EbxImportReference ebxRef = pr.External;

                externalAssetEntry = App.AssetManager.GetEbxEntry(ebxRef.FileGuid);

                if(externalAssetEntry == null)
                {
                    return null;
                }

                if(ebxRef.ClassGuid == Guid.Empty)
                {
                    return externalAssetEntry;
                }

                return App.AssetManager.GetEbx(externalAssetEntry).GetObject(ebxRef.ClassGuid);
            }

            return null;
        }

        protected virtual string GetEntity(PointerRef pr)
        {
            if (pr.Type == PointerRefType.Null)
            {
                return "(null)";
            }

            object resolvedValue = ResolvePointer(pr, out EbxAssetEntry externalAssetEntry);

            if (resolvedValue == null)
            {
                return "(null)";
            }

            string objId = ((dynamic)resolvedValue).__Id;

            foreach (string clutterName in ClutterNames)
            {
                if (objId.EndsWith(clutterName))
                {
                    objId = objId.Remove(objId.Length - clutterName.Length);
                    break;
                }
            }

            return pr.Type is PointerRefType.External ? $"{externalAssetEntry.Filename}/{objId}" : objId;
        }

        protected virtual string Sanitize(PointerRef pr, string value)
        {
            string retVal = value;
            if (value.StartsWith("0x"))
                retVal = value.Remove(0, 2);
            if (value == "00000000")
                retVal = "Self";

            if (pr.Type == PointerRefType.Internal && TypeLibrary.IsSubClassOf(pr.Internal, "InterfaceDescriptorData"))
            {
                dynamic interfaceDesc = pr.Internal;
                foreach (dynamic field in interfaceDesc.Fields)
                {
                    if (field.Name == value)
                    {
                        if (field.ValueRef.Type != PointerRefType.Null)
                        {
                            var entry = App.AssetManager.GetEbxEntry(field.ValueRef.External.FileGuid);
                            retVal += " (" + entry.Type + " '" + entry.Filename + "')";
                        }
                        else
                        {
                            if (ProfilesLibrary.DataVersion == (int)ProfileVersion.PlantsVsZombiesBattleforNeighborville || ProfilesLibrary.DataVersion == (int)ProfileVersion.NeedForSpeedHeat)
                            {
                                string val = field.BoxedValue.ToString();
                                if (val != "(null)")
                                {
                                    retVal += " (" + val + ")";
                                }
                            }
                            else
                            {
                                if (field.Value != "")
                                {
                                    retVal += " (" + field.Value + ")";
                                }
                            }
                        }
                        break;
                    }
                }
            }
            return retVal;
        }

        private readonly string[] ClutterNames =
        {
            "EntityData",
            "ObjectData",
            "ComponentData",
            "DescriptorData",
            "Data",
        };

        private FrostyAssetEditor GetParentEditor()
        {
            DependencyObject parent = System.Windows.Media.VisualTreeHelper.GetParent(this);
            while (!(parent.GetType().IsSubclassOf(typeof(FrostyAssetEditor)) || parent is FrostyAssetEditor))
                parent = System.Windows.Media.VisualTreeHelper.GetParent(parent);
            return (parent as FrostyAssetEditor);
        }
    }
}
