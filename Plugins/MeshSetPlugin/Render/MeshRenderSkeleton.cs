using Frosty.Core.Viewport;
using Frosty.Hash;
using SharpDX;
using System;
using System.Collections.Generic;

namespace MeshSetPlugin.Render
{
    public class MeshRenderSkeleton
    {
#if FROSTY_DEVELOPER
        public class ExpressionValue<T>
        {
            protected T value;
            public ExpressionValue(T inValue)
            {
                value = inValue;
            }

            public virtual T Evaluate(int hash)
            {
                return value;
            }
        }
        public class BoneQueryExpressionValue : ExpressionValue<Matrix>
        {
            private int WorldTransformHash = Fnv1.HashString("WorldTransform");

            protected MeshRenderSkeleton skeleton;
            protected int boneId;

            public BoneQueryExpressionValue(MeshRenderSkeleton inSkeleton, int inBoneId)
                : base(Matrix.Identity)
            {
                skeleton = inSkeleton;
                boneId = inBoneId;
            }

            public override Matrix Evaluate(int hash)
            {
                if (hash == WorldTransformHash)
                {
                    return skeleton.GetBoneWorldMatrix(boneId);
                }

                return skeleton.GetBone(boneId).LocalPose;
            }
        }
        public class TestRollBoneExpression
        {
            private int WorldTransformHash = Fnv1.HashString("WorldTransform");
            private int ValueHash = Fnv1.HashString("Value");

            private ExpressionValue<Vector3> param1;
            private ExpressionValue<float> param2;
            private ExpressionValue<Matrix> param3;
            private ExpressionValue<Matrix> param4;
            private ExpressionValue<Matrix> param5;

            public TestRollBoneExpression(ExpressionValue<Vector3> inParam1, ExpressionValue<float> inParam2, ExpressionValue<Matrix> inParam3, ExpressionValue<Matrix> inParam4, ExpressionValue<Matrix> inParam5)
            {
                param1 = inParam1;
                param2 = inParam2;
                param3 = inParam3;
                param4 = inParam4;
                param5 = inParam5;
            }

            public Matrix Evaluate(Matrix input)
            {
                Matrix tmp00 = input * param3.Evaluate(WorldTransformHash);
                tmp00.Decompose(out Vector3 tmp40, out Quaternion tmpD0, out Vector3 tmp50);
                param4.Evaluate(WorldTransformHash).Decompose(out Vector3 tmp60, out Quaternion tmpE0, out Vector3 tmp70);
                param5.Evaluate(WorldTransformHash).Decompose(out Vector3 tmp80, out Quaternion tmpF0, out Vector3 tmp90);
                Quaternion tmp100 = Quaternion.Invert(tmpF0);
                Quaternion tmp110 = tmpE0 * tmp100;
                Vector3 tmpA0 = SharpDXUtils.ExtractEulerAngles(SharpDXUtils.FromQuaternion(tmp110)) * new Vector3((float)(Math.PI / 180.0));
                Vector3 tmpB0 = tmpA0 * param1.Evaluate(ValueHash);
                Vector3 tmpC0 = tmpB0 + new Vector3(0, 0, param2.Evaluate(ValueHash));
                Quaternion tmp120 = Quaternion.Normalize(SharpDXUtils.CreateFromEulerAngles(tmpC0.X, tmpC0.Y, tmpC0.Z));
                Quaternion tmp130 = tmp120 * tmpF0;
                return Matrix.Scaling(tmp40) * Matrix.RotationQuaternion(tmp130) * Matrix.Translation(tmp50);
            }
        }
#endif

        public class Bone
        {
            public int NameHash;
            public Matrix ModelPose;
            public Matrix LocalPose;
            public int ParentBoneId;
            public bool IsProcedural;
        }

        public int BoneCount => proceduralBoneIndex;
        public IEnumerable<Bone> Bones => bones;

        private List<Bone> bones = new List<Bone>();
        private int proceduralBoneIndex = -1;

#if FROSTY_DEVELOPER
        private Dictionary<int, TestRollBoneExpression> expressions = new Dictionary<int, TestRollBoneExpression>();
        public void AddExpression(int boneId, TestRollBoneExpression expr)
        {
            expressions.Add(boneId, expr);
        }
#endif

        public void AddBone(Bone bone)
        {
            bones.Add(bone);
            if (bone.IsProcedural && proceduralBoneIndex == -1)
            {
                proceduralBoneIndex = bones.Count - 1;
            }
        }

        public Matrix GetBoneWorldMatrix(int idx)
        {
            if (idx >= bones.Count)
            {
                return Matrix.Identity;
            }

            Matrix boneMatrix = bones[idx].LocalPose;
            while (idx != -1)
            {
                idx = bones[idx].ParentBoneId;
                if (idx != -1)
                {
                    boneMatrix *= bones[idx].LocalPose;
                }
            }

            return boneMatrix;
        }

        public Matrix GetBoneMatrix(int idx)
        {
            if (idx >= bones.Count)
            {
                return Matrix.Identity;
            }

            Matrix invBoneMatrix = bones[idx].ModelPose;
            Matrix boneMatrix = GetBoneWorldMatrix(idx);

#if FROSTY_DEVELOPER
            if (expressions.ContainsKey(idx))
            {
                boneMatrix = expressions[idx].Evaluate(bones[idx].LocalPose);
            }
#endif

            boneMatrix = invBoneMatrix * boneMatrix;
            return boneMatrix;
        }

        public Bone GetBone(int boneId)
        {
            return bones[boneId];
        }

        public int GetBoneId(int nameHash)
        {
            return bones.FindIndex((Bone a) => a.NameHash == nameHash);
        }

        public void UpdateBone(int boneId, Matrix? modelPose = null, Matrix? localPose = null)
        {
            if (boneId >= bones.Count)
            {
                return;
            }

            if (modelPose.HasValue)
            {
                bones[boneId].ModelPose = modelPose.Value;
            }

            if (localPose.HasValue)
            {
                bones[boneId].LocalPose = localPose.Value;
            }
        }

        public void UpdateBone(string boneName, Matrix? modelPose = null, Matrix? localPose = null)
        {
            int hash = Fnv1.HashString(boneName);
            int boneId = bones.FindIndex((Bone a) => a.NameHash == hash);
            if (boneId == -1)
            {
                return;
            }

            UpdateBone(boneId, modelPose, localPose);
        }
    }
}
