using System.Windows;
using System.Windows.Controls;
using System;
using System.Windows.Data;

namespace Frosty.Core.Controls.Editors
{
    public enum FrostyVectorControlType
    {
        Vec2,
        Vec3,
        Vec4
    }
    [TemplatePart(Name = PART_X, Type = typeof(TextBox))]
    [TemplatePart(Name = PART_Y, Type = typeof(TextBox))]
    [TemplatePart(Name = PART_Z, Type = typeof(TextBox))]
    [TemplatePart(Name = PART_W, Type = typeof(TextBox))]
    public abstract class FrostyVectorControl : Control
    {
        private const string PART_X = "PART_X";
        private const string PART_Y = "PART_Y";
        private const string PART_Z = "PART_Z";
        private const string PART_W = "PART_W";

        protected TextBox textBoxX;
        protected TextBox textBoxY;
        protected TextBox textBoxZ;
        protected TextBox textBoxW;
        public FrostyPropertyGridItemData Item;

        #region -- Properties --

        #region -- Result --
        public static readonly DependencyProperty ResultProperty = DependencyProperty.Register("Result", typeof(object), typeof(FrostyVectorControl), new FrameworkPropertyMetadata(null, OnResultChanged));
        public object Result
        {
            get => GetValue(ResultProperty);
            set => SetValue(ResultProperty, value);
        }
        public static void OnResultChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            FrostyVectorControl ctrl = o as FrostyVectorControl;
            ctrl.ResultChanged(e.NewValue);
        }
        #endregion

        #region -- ControlType --
        public static readonly DependencyProperty ControlTypeProperty = DependencyProperty.Register("ControlType", typeof(FrostyVectorControlType), typeof(FrostyVectorControl), new FrameworkPropertyMetadata(FrostyVectorControlType.Vec3));
        public FrostyVectorControlType ControlType
        {
            get => (FrostyVectorControlType)GetValue(ControlTypeProperty);
            set => SetValue(ControlTypeProperty, value);
        }
        #endregion

        #region -- Labels --

        public static readonly DependencyProperty XLabelProperty = DependencyProperty.Register("XLabel", typeof(string), typeof(FrostyVectorControl), new FrameworkPropertyMetadata("X"));
        public string XLabel
        {
            get => (string)GetValue(XLabelProperty);
            set => SetValue(XLabelProperty, value);
        }
        public static readonly DependencyProperty YLabelProperty = DependencyProperty.Register("YLabel", typeof(string), typeof(FrostyVectorControl), new FrameworkPropertyMetadata("Y"));
        public string YLabel
        {
            get => (string)GetValue(YLabelProperty);
            set => SetValue(YLabelProperty, value);
        }
        public static readonly DependencyProperty ZLabelProperty = DependencyProperty.Register("ZLabel", typeof(string), typeof(FrostyVectorControl), new FrameworkPropertyMetadata("Z"));
        public string ZLabel
        {
            get => (string)GetValue(ZLabelProperty);
            set => SetValue(ZLabelProperty, value);
        }
        public static readonly DependencyProperty WLabelProperty = DependencyProperty.Register("WLabel", typeof(string), typeof(FrostyVectorControl), new FrameworkPropertyMetadata("W"));
        public string WLabel
        {
            get => (string)GetValue(WLabelProperty);
            set => SetValue(WLabelProperty, value);
        }

        #endregion

        #endregion

        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float W { get; set; }

        static FrostyVectorControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FrostyVectorControl), new FrameworkPropertyMetadata(typeof(FrostyVectorControl)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Focusable = false;

            textBoxX = GetTemplateChild(PART_X) as TextBox;
            textBoxY = GetTemplateChild(PART_Y) as TextBox;
            textBoxZ = GetTemplateChild(PART_Z) as TextBox;
            textBoxW = GetTemplateChild(PART_W) as TextBox;

            if (textBoxX != null)
            {
                textBoxX.LostFocus += TextBox_LostFocus;
                textBoxX.GotKeyboardFocus += TextBox_GotKeyboardFocus;
            }
            if (textBoxY != null)
            {
                textBoxY.LostFocus += TextBox_LostFocus;
                textBoxY.GotKeyboardFocus += TextBox_GotKeyboardFocus;
            }
            if (textBoxZ != null)
            {
                textBoxZ.LostFocus += TextBox_LostFocus;
                textBoxZ.GotKeyboardFocus += TextBox_GotKeyboardFocus;
            }
            if (textBoxW != null)
            {
                textBoxW.LostFocus += TextBox_LostFocus;
                textBoxW.GotKeyboardFocus += TextBox_GotKeyboardFocus;
            }

            TargetUpdated += FrostyVectorControl_TargetUpdated;
        }

        private void FrostyVectorControl_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            dynamic vector = Result;

            X = vector.x;
            Y = vector.y;

            textBoxX.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
            textBoxY.GetBindingExpression(TextBox.TextProperty).UpdateTarget();

            if (ControlType >= FrostyVectorControlType.Vec3)
            {
                Z = vector.z;
                textBoxZ.GetBindingExpression(TextBox.TextProperty).UpdateTarget();

                if (ControlType >= FrostyVectorControlType.Vec4)
                {
                    W = vector.w;
                    textBoxW.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
                }
            }
        }

        private void TextBox_GotKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            tb.SelectAll();
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            UpdateValues();
        }

        protected virtual void UpdateValues()
        {
            dynamic vector = Result;

            if (ControlType == FrostyVectorControlType.Vec2)
            {
                if (X != vector.x || Y != vector.y)
                {
                    vector.x = X;
                    vector.y = Y;
                    Item.ForceValue(Result);
                }
            }
            else if (ControlType == FrostyVectorControlType.Vec3)
            {
                if (X != vector.x || Y != vector.y || Z != vector.z)
                {
                    vector.x = X;
                    vector.y = Y;
                    vector.z = Z;
                    Item.ForceValue(Result);
                }
            }
            else
            {
                if (X != vector.x || Y != vector.y || Z != vector.z || W != vector.w)
                {
                    vector.x = X;
                    vector.y = Y;
                    vector.z = Z;
                    vector.w = W;
                    Item.ForceValue(Result);
                }
            }
        }

        protected void ResultChanged(object newValue)
        {
            dynamic vector = newValue;
            Type type = vector.GetType();

            X = vector.x;
            Y = vector.y;

            if (ControlType >= FrostyVectorControlType.Vec3)
                Z = vector.z;
            if (ControlType >= FrostyVectorControlType.Vec4)
                W = vector.w;
        }
    }

    public class FrostyVec2Control : FrostyVectorControl
    {
        static FrostyVec2Control()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FrostyVec2Control), new FrameworkPropertyMetadata(typeof(FrostyVec2Control)));
            ControlTypeProperty.OverrideMetadata(typeof(FrostyVec2Control), new FrameworkPropertyMetadata(FrostyVectorControlType.Vec2));
        }
    }
    public class FrostyVec2Editor : FrostyTypeEditor<FrostyVec2Control>
    {
        public FrostyVec2Editor()
        {
            ValueProperty = FrostyVectorControl.ResultProperty;
            NotifyOnTargetUpdated = true;
        }

        protected override void CustomizeEditor(FrostyVec2Control editor, FrostyPropertyGridItemData item)
        {
            base.CustomizeEditor(editor, item);
            editor.Item = item;
        }
    }

    public class FrostyVec3Control : FrostyVectorControl
    {
        static FrostyVec3Control()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FrostyVec3Control), new FrameworkPropertyMetadata(typeof(FrostyVec3Control)));
            ControlTypeProperty.OverrideMetadata(typeof(FrostyVec3Control), new FrameworkPropertyMetadata(FrostyVectorControlType.Vec3));
        }
    }
    public class FrostyVec3Editor : FrostyTypeEditor<FrostyVec3Control>
    {
        public FrostyVec3Editor()
        {
            ValueProperty = FrostyVectorControl.ResultProperty;
            NotifyOnTargetUpdated = true;
        }

        protected override void CustomizeEditor(FrostyVec3Control editor, FrostyPropertyGridItemData item)
        {
            base.CustomizeEditor(editor, item);
            editor.Item = item;
        }
    }

    public class FrostyVec4Control : FrostyVectorControl
    {
        static FrostyVec4Control()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FrostyVec4Control), new FrameworkPropertyMetadata(typeof(FrostyVec4Control)));
            ControlTypeProperty.OverrideMetadata(typeof(FrostyVec4Control), new FrameworkPropertyMetadata(FrostyVectorControlType.Vec4));
        }
    }
    public class FrostyVec4Editor : FrostyTypeEditor<FrostyVec4Control>
    {
        public FrostyVec4Editor()
        {
            ValueProperty = FrostyVectorControl.ResultProperty;
            NotifyOnTargetUpdated = true;
        }

        protected override void CustomizeEditor(FrostyVec4Control editor, FrostyPropertyGridItemData item)
        {
            base.CustomizeEditor(editor, item);
            editor.Item = item;
        }
    }
}
