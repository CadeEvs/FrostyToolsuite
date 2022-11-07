using SharpDX;
using System.Collections.Generic;

namespace MeshSetPlugin.Render
{
    public class MeshRenderAnim
    {
        public class Keyframe<T>
        {
            public int FrameTime;
            public T Value;
        }
        public class Bone
        {
            public int NameHash;
            public List<Keyframe<Quaternion>> Rotations = new List<Keyframe<Quaternion>>();
            public List<Keyframe<Vector3>> Translations = new List<Keyframe<Vector3>>();
            public List<Keyframe<Vector3>> Scales = new List<Keyframe<Vector3>>();
        }
        private struct InterpolatedValue
        {
            public Quaternion? Rotation;
            public Vector3? Translation;
            public Vector3? Scale;
            public int LastRotIndex;
            public int LastTransIndex;
            public int LastScaleIndex;
        }

        private List<Bone> bones = new List<Bone>();
        private InterpolatedValue[] interpolatedValues = null;
        private int frameCount;

        private double currentTime = 0.0;
        private int frame = 0;
        private const float Speed = (1 / 30.0f);

        public MeshRenderAnim(int inFrameCount)
        {
            frameCount = inFrameCount;
        }

        public void AddBones(IEnumerable<Bone> inBones)
        {
            bones.AddRange(inBones);
            interpolatedValues = new InterpolatedValue[bones.Count];
        }

        public void Update(double deltaTime)
        {
            currentTime += deltaTime;
            if (currentTime > Speed)
            {
                frame++;
                if (frame >= frameCount)
                {
                    frame = 0;
                }

                currentTime = 0.0;
            }

            for (int i = 0; i < bones.Count; i++)
            {
                Bone bone = bones[i];
                interpolatedValues[i].Rotation = InterpolateRotation(frame, bone.Rotations, ref interpolatedValues[i].LastRotIndex);
                interpolatedValues[i].Translation = Interpolate(frame, bone.Translations, ref interpolatedValues[i].LastTransIndex);
                interpolatedValues[i].Scale = Interpolate(frame, bone.Scales, ref interpolatedValues[i].LastScaleIndex);
            }
        }

        public void UpdateSkeleton(MeshRenderSkeleton skeleton)
        {
            foreach (MeshRenderSkeleton.Bone skelBone in skeleton.Bones)
            {
                int index = bones.FindIndex((Bone a) => a.NameHash == skelBone.NameHash);
                if (index == -1)
                {
                    continue;
                }

                Quaternion? rotation = interpolatedValues[index].Rotation;
                Vector3? translation = interpolatedValues[index].Translation;
                Vector3? scale = interpolatedValues[index].Scale;

                skelBone.LocalPose.Decompose(out Vector3 skelScale, out Quaternion skelRotation, out Vector3 skelTranslation);

                if (!rotation.HasValue)
                {
                    rotation = skelRotation;
                }

                if (!translation.HasValue)
                {
                    translation = skelTranslation;
                }

                if (!scale.HasValue)
                {
                    scale = skelScale;
                }

                skeleton.UpdateBone(skeleton.GetBoneId(skelBone.NameHash), localPose: Matrix.Scaling(scale.Value) * Matrix.RotationQuaternion(rotation.Value) * Matrix.Translation(translation.Value));
            }
        }

        private Vector3? Interpolate(int index, List<Keyframe<Vector3>> list, ref int lastIndex)
        {
            GetKeyframes(index, list, out Keyframe<Vector3> first, out Keyframe<Vector3> second, out float interp, ref lastIndex);
            if (first == null && second == null)
            {
                return null;
            }

            return Vector3.Lerp(first.Value, second.Value, interp);
        }

        private Quaternion? InterpolateRotation(int index, List<Keyframe<Quaternion>> list, ref int lastIndex)
        {
            GetKeyframes(index, list, out Keyframe<Quaternion> first, out Keyframe<Quaternion> second, out float interp, ref lastIndex);
            if (first == null && second == null)
            {
                return null;
            }

            return Quaternion.Slerp(first.Value, second.Value, interp);
        }

        private void GetKeyframes<T>(int index, List<Keyframe<T>> list, out Keyframe<T> first, out Keyframe<T> second, out float interp, ref int lastIndex)
        {
            first = null;
            second = null;
            interp = 0;

            if (list.Count == 0)
            {
                return;
            }

            if (list.Count == 1)
            {
                first = list[0];
                second = list[0];
                return;
            }

            if (lastIndex > index)
            {
                lastIndex = 0;
            }

            for (int i = lastIndex; i < list.Count; i++)
            {
                if (first == null)
                {
                    if (list[i].FrameTime > index)
                    {
                        first = list[i - 1];
                        lastIndex = i - 1;
                    }
                }
                if (second == null && first != null)
                {
                    if (list[i].FrameTime > index && list[i].FrameTime > first.FrameTime)
                    {
                        second = list[i];
                    }
                }
                if (first != null && second != null)
                {
                    break;
                }
            }

            if (first == null && second == null)
            {
                // just set to the last key
                first = list[list.Count - 2];
                second = list[list.Count - 1];
            }

            interp = (index - first.FrameTime) / (float)(second.FrameTime - first.FrameTime);
        }
    }
}
