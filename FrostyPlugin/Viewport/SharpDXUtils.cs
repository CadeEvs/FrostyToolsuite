using SharpDX;
using System;

namespace Frosty.Core.Viewport
{
    public class SharpDXUtils
    {
        public static Matrix FromQuaternion(Quaternion quat)
        {
            float num = 2f * quat.X * quat.X;
            float num2 = 2f * quat.X * quat.Y;
            float num3 = 2f * quat.X * quat.W;
            float num4 = 2f * quat.Y * quat.Y;
            float num5 = 2f * quat.Y * quat.Z;
            float num6 = 2f * quat.Y * quat.W;
            float num7 = 2f * quat.Z * quat.X;
            float num8 = 2f * quat.Z * quat.Z;
            float num9 = 2f * quat.Z * quat.W;
            Matrix result;
            result.M11 = (float)(1.0 - ((double)num4 + (double)num8));
            result.M12 = num2 + num9;
            result.M13 = num7 - num6;
            result.M14 = 0f;
            result.M21 = num2 - num9;
            result.M22 = (float)(1.0 - ((double)num8 + (double)num));
            result.M23 = num5 + num3;
            result.M24 = 0f;
            result.M31 = num7 + num6;
            result.M32 = num5 - num3;
            result.M33 = (float)(1.0 - ((double)num4 + (double)num));
            result.M34 = 0f;
            result.M41 = 0f;
            result.M42 = 0f;
            result.M43 = 0f;
            result.M44 = 1f;
            return result;
        }

        public static SharpDX.Vector3 ExtractEulerAngles(SharpDX.Matrix m)
        {
            float piOver180 = (float)(Math.PI / 180.0);
            SharpDX.Vector3 eulerRotation = new SharpDX.Vector3();
            float num = (float)Math.Sqrt((double)m.M11 * (double)m.M11 + (double)m.M12 * (double)m.M12);

            if ((double)num > 1.0 / 1000.0)
            {
                eulerRotation.X = (float)Math.Atan2((double)m.M23, (double)m.M33);
                eulerRotation.Y = (float)Math.Atan2(-(double)m.M13, (double)num);
                eulerRotation.Z = (float)Math.Atan2((double)m.M12, (double)m.M11);
            }
            else
            {
                eulerRotation.X = (float)Math.Atan2(-(double)m.M32, (double)m.M22);
                eulerRotation.Y = (float)Math.Atan2(-(double)m.M13, (double)num);
                eulerRotation.Z = 0.0f;
            }

            eulerRotation.X /= piOver180;
            eulerRotation.Y /= piOver180;
            eulerRotation.Z /= piOver180;

            return eulerRotation;
        }

        public static Quaternion CreateFromEulerAngles(float x, float y, float z)
        {
            float num = (float)Math.Sin((double)x * 0.5);
            float num2 = (float)Math.Cos((double)x * 0.5);
            float num3 = (float)Math.Sin((double)y * 0.5);
            float num4 = (float)Math.Cos((double)y * 0.5);
            float num5 = (float)Math.Sin((double)z * 0.5);
            float num6 = (float)Math.Cos((double)z * 0.5);
            Quaternion result;
            result.X = (float)((double)num * (double)num4 * (double)num6 - (double)num2 * (double)num3 * (double)num5);
            result.Y = (float)((double)num2 * (double)num3 * (double)num6 + (double)num * (double)num4 * (double)num5);
            result.Z = (float)((double)num2 * (double)num4 * (double)num5 - (double)num * (double)num3 * (double)num6);
            result.W = (float)((double)num2 * (double)num4 * (double)num6 + (double)num * (double)num3 * (double)num5);
            return result;
        }

        public static Matrix FromLinearTransform(dynamic transform)
        {
            return new SharpDX.Matrix(
                transform.right.x, transform.right.y, transform.right.z, 0.0f,
                transform.up.x, transform.up.y, transform.up.z, 0.0f,
                transform.forward.x, transform.forward.y, transform.forward.z, 0.0f,
                transform.trans.x, transform.trans.y, transform.trans.z, 1.0f
                );
        }

        public static Vector3 FromVec3(dynamic vec)
        {
            return new Vector3(vec.x, vec.y, vec.z);
        }

        public static Vector4 FromVec4(dynamic vec)
        {
            return new Vector4(vec.x, vec.y, vec.z, vec.w);
        }
    }
}
