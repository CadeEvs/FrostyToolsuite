using Frosty.Core;
using Frosty.Core.Controls;
using Frosty.Core.Controls.Editors;
using FrostySdk;
using FrostySdk.Ebx;
using FrostySdk.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

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
            (sp.Children[2] as TextBlock).Text = AdditionalSanitize(propConnection.Source, propConnection.SourceField);
            (sp.Children[6] as TextBlock).Text = AdditionalSanitize(propConnection.Target, propConnection.TargetField);
        }

        protected virtual string GetEntity(PointerRef pr)
        {
            string val = "";
            if (pr.Type == PointerRefType.Null)
                return "(null)";
            else if (pr.Type == PointerRefType.External)
            {
                EbxAsset asset = GetParentEditor().GetDependentObject(pr.External.FileGuid);
                val = App.AssetManager.GetEbxEntry(pr.External.FileGuid).Filename + "/" + asset.GetObject(pr.External.ClassGuid).GetType().Name;
            }
            else if (pr.Type == PointerRefType.Internal)
                val = ((dynamic)pr.Internal).__Id;

            if (val.EndsWith("EntityData") || val.EndsWith("ObjectData"))
                return val.Remove(val.Length - 10);
            else if (val.EndsWith("ComponentData"))
                return val.Remove(val.Length - 13);
            else if (val.EndsWith("DescriptorData"))
                return val.Remove(val.Length - 14);
            else if (val.EndsWith("Data"))
                return val.Remove(val.Length - 4);

            return val;
        }

        protected virtual string Sanitize(PointerRef pr, string value)
        {
            string retVal = value;
            if (value.StartsWith("0x"))
                retVal = value.Remove(0, 2);
            if (value == "00000000")
                retVal = "Self";
          
            if (pr.Type == FrostySdk.IO.PointerRefType.Internal && TypeLibrary.IsSubClassOf(pr.Internal, "InterfaceDescriptorData"))
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
                            if (ProfilesLibrary.IsLoaded(ProfileVersion.PlantsVsZombiesBattleforNeighborville, ProfileVersion.NeedForSpeedHeat,
                                ProfileVersion.Battlefield2042, ProfileVersion.NeedForSpeedUnbound, ProfileVersion.DeadSpace))
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

        protected virtual string AdditionalSanitize(PointerRef pr, string value)
        {
            if (value.StartsWith("0x"))
                value = value.Remove(0, 2);
            if (value == "00000000")
                value = "Self";
            return value;
        }

        private FrostyAssetEditor GetParentEditor()
        {
            DependencyObject parent = System.Windows.Media.VisualTreeHelper.GetParent(this);
            while (!(parent.GetType().IsSubclassOf(typeof(FrostyAssetEditor)) || parent is FrostyAssetEditor))
                parent = System.Windows.Media.VisualTreeHelper.GetParent(parent);
            return (parent as FrostyAssetEditor);
        }
    }
}
