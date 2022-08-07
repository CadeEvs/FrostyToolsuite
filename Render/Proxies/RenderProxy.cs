using Frosty.Core.Viewport;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using Frosty.Core.Screens;
using LevelEditorPlugin.Entities;
using System.Diagnostics.Contracts;

namespace LevelEditorPlugin.Render.Proxies
{
    public abstract class RenderProxy : MeshRenderBase
    {
        public float CurrentDistanceToCamera { get; set; }
        public Matrix Transform { get; protected set; }
        public BoundingBox BoundingBox { get; protected set; }
        public Entity OwnerEntity { get; protected set; }

        public RenderProxy(ISpatialEntity owner)
        {
            OwnerEntity = owner as Entity;
            Transform = owner.GetTransform();
        }

        public override void Render(DeviceContext context, MeshRenderPath renderPath)
        {
            throw new NotImplementedException();
        }

        public virtual void Update(RenderCreateState state)
        {
            if (OwnerEntity.RequiresTransformUpdate)
            {
                Transform = (OwnerEntity as ISpatialEntity).GetTransform();
                RecalculateBoundingBox();

                OwnerEntity.RequiresTransformUpdate = false;
            }
        }

        public virtual void RecalculateBoundingBox()
        {
            // do nothing
        }

        public virtual bool HitTest(Ray hitTestRay, out Vector3 hitLocation)
        {
            hitLocation = Transform.TranslationVector;
            return true;
        }

        public virtual bool ShouldRender(float distToCamera, float screenSize)
        {
            return true;
        }

        public virtual MeshRenderInstance GetInstance(float distToCamera)
        {
            return new MeshRenderInstance() { RenderMesh = this, Transform = Transform };
        }

        public virtual void SetSelected(RenderCreateState state, bool newSelected)
        {
        }

        public abstract void Dispose();
    }
}
