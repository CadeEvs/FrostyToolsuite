using FrostySdk;
using System;
using System.Windows;
using System.Windows.Data;
using System.Globalization;
using FrostySdk.Attributes;
using Frosty.Core.Viewport;
using SharpDX;

namespace Frosty.Core.Controls.Editors
{
    [ClassConverter(typeof(LinearTransformConverter))]
    public class LinearTransformOverride : BaseTypeOverride
    {
    }

    public class FrostyRotationEditor : FrostyTypeEditor<FrostyRotationControl>
    {
        public FrostyRotationEditor()
        {
            ValueProperty = FrostyVectorControl.ResultProperty;
        }
    }

    public class FrostyRotationControl : FrostyVectorControl
    {
        static FrostyRotationControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FrostyRotationControl), new FrameworkPropertyMetadata(typeof(FrostyRotationControl)));
            ControlTypeProperty.OverrideMetadata(typeof(FrostyRotationControl), new FrameworkPropertyMetadata(FrostyVectorControlType.Vec3));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        protected override void UpdateValues()
        {
            dynamic vector = Result;
            if (X != vector.x || Y != vector.y || Z != vector.z)
            {
                dynamic newVector = TypeLibrary.CreateObject("Vec3");

                if (X < -180.0f) X = -180.0f;
                else if (X > 180.0f) X = 180.0f;
                if (Y < -180.0f) Y = -180.0f;
                else if (Y > 180.0f) Y = 180.0f;
                if (Z < -180.0f) Z = -180.0f;
                else if (Z > 180.0f) Z = 180.0f;

                newVector.x = X;
                newVector.y = Y;
                newVector.z = Z;

                Result = newVector;

                textBoxX.Text = X.ToString();
                textBoxY.Text = Y.ToString();
                textBoxZ.Text = Z.ToString();
            }
        }
    }

    [DisplayName("EditorLinearTransform")]
    public class LinearTransformConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            dynamic obj = value;
            EditorLinearTransform trns = new EditorLinearTransform();

            if (obj.Rotation.x >= float.MaxValue)
            {
                // first time convert from raw matrix values

                Matrix matrix = new Matrix(
                        obj.right.x, obj.right.y, obj.right.z, 0.0f,
                        obj.up.x, obj.up.y, obj.up.z, 0.0f,
                        obj.forward.x, obj.forward.y, obj.forward.z, 0.0f,
                        obj.trans.x, obj.trans.y, obj.trans.z, 1.0f
                        );


                matrix.Decompose(out Vector3 scale, out Quaternion rotation, out Vector3 translation);
                Vector3 euler = SharpDXUtils.ExtractEulerAngles(matrix);

                trns.Translation.x = translation.X;
                trns.Translation.y = translation.Y;
                trns.Translation.z = translation.Z;

                trns.Scale.x = scale.X;
                trns.Scale.y = scale.Y;
                trns.Scale.z = scale.Z;

                trns.Rotation.x = -euler.Y;
                trns.Rotation.y = euler.X;
                trns.Rotation.z = euler.Z;
            }
            else
            {
                // grab values directly

                trns.Translation.x = obj.Translate.x;
                trns.Translation.y = obj.Translate.y;
                trns.Translation.z = obj.Translate.z;

                trns.Rotation.x = obj.Rotation.x;
                trns.Rotation.y = obj.Rotation.y;
                trns.Rotation.z = obj.Rotation.z;

                trns.Scale.x = obj.Scale.x;
                trns.Scale.y = obj.Scale.y;
                trns.Scale.z = obj.Scale.z;
            }

            return trns;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            dynamic trns = parameter;
            EditorLinearTransform obj = (EditorLinearTransform)value;

            // store directly...

            trns.Translate.x = obj.Translation.x;
            trns.Translate.y = obj.Translation.y;
            trns.Translate.z = obj.Translation.z;

            trns.Rotation.x = obj.Rotation.x;
            trns.Rotation.y = obj.Rotation.y;
            trns.Rotation.z = obj.Rotation.z;

            trns.Scale.x = obj.Scale.x;
            trns.Scale.y = obj.Scale.y;
            trns.Scale.z = obj.Scale.z;

            // then convert...

            float val = (float)(Math.PI / 180.0);
            Matrix m = Matrix.RotationYawPitchRoll(-obj.Rotation.x * val, obj.Rotation.y * val, obj.Rotation.z * val);
            m = m * Matrix.Scaling(obj.Scale.x, obj.Scale.y, obj.Scale.z);

            trns.trans.x = obj.Translation.x;
            trns.trans.y = obj.Translation.y;
            trns.trans.z = obj.Translation.z;

            trns.right.x = m.M11;
            trns.right.y = m.M12;
            trns.right.z = m.M13;

            trns.up.x = m.M21;
            trns.up.y = m.M22;
            trns.up.z = m.M23;

            trns.forward.x = m.M31;
            trns.forward.y = m.M32;
            trns.forward.z = m.M33;

            return trns;
        }
    }
    public class EditorLinearTransform
    {
        // vec3 translation
        public dynamic Translation { get; set; }

        // vec3 (roll/pitch/yaw) rotation
        [Editor(typeof(FrostyRotationEditor))]
        public dynamic Rotation { get; set; }

        // vec3 scale
        public dynamic Scale { get; set; }

        public EditorLinearTransform()
        {
            Translation = TypeLibrary.CreateObject("Vec3");
            Rotation = TypeLibrary.CreateObject("Vec3");
            Scale = TypeLibrary.CreateObject("Vec3");

            Scale.x = 1.0f;
            Scale.y = 1.0f;
            Scale.z = 1.0f;
        }

        public override string ToString()
        {
            return "LinearTransform";
        }
    }
}
