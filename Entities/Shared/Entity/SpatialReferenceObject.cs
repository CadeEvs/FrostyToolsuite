using LevelEditorPlugin.Editors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using Frosty.Core.Viewport;

namespace LevelEditorPlugin.Entities
{
    [EntityBinding(DataType = typeof(FrostySdk.Ebx.SpatialReferenceObjectData))]
    public class SpatialReferenceObject : ReferenceObject, IEntityData<FrostySdk.Ebx.SpatialReferenceObjectData>
    {
        public new FrostySdk.Ebx.SpatialReferenceObjectData Data => data as FrostySdk.Ebx.SpatialReferenceObjectData;
        protected Entity RootEntity => (entities.Count > 0) ? entities[0] : null;

        public SpatialReferenceObject(FrostySdk.Ebx.SpatialReferenceObjectData inData, Entity inParent, EntityWorld inWorld)
            : base(inData, inParent, inWorld)
        {
        }

        public SpatialReferenceObject(FrostySdk.Ebx.SpatialReferenceObjectData inData, Entity inParent)
            : base(inData, inParent)
        {
        }

        protected override void SpawnEntities()
        {
            base.SpawnEntities();
            SpawnComponents();
        }

        protected virtual void SpawnComponents()
        {
            if (Blueprint != null)
            {
                if (Parent == null || RootEntity is IAllowComponentsInLevel)
                {

                    if (RootEntity is IComponentEntity)
                    {
                        IComponentEntity componentEntity = RootEntity as IComponentEntity;
                        componentEntity.SpawnComponents();
                    }
                }
            }
        }
    }
}
