using LevelEditorPlugin.Editors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditorPlugin.Entities
{
    [EntityBinding(DataType = typeof(FrostySdk.Ebx.SpatialPrefabReferenceObjectData))]
    public class SpatialPrefabReferenceObject : SpatialReferenceObject, IEntityData<FrostySdk.Ebx.SpatialPrefabReferenceObjectData>
    {
        public new FrostySdk.Ebx.SpatialPrefabReferenceObjectData Data => data as FrostySdk.Ebx.SpatialPrefabReferenceObjectData;
        public new Assets.SpatialPrefabBlueprint Blueprint => blueprint as Assets.SpatialPrefabBlueprint;

        public SpatialPrefabReferenceObject(FrostySdk.Ebx.SpatialPrefabReferenceObjectData inData, Entity inParent, EntityWorld inWorld)
            : base(inData, inParent, inWorld)
        {
        }

        public SpatialPrefabReferenceObject(FrostySdk.Ebx.SpatialPrefabReferenceObjectData inData, Entity inParent)
            : this(inData, inParent, null)
        {
        }

        public Layers.SceneLayer GetLayer()
        {
            if (blueprint == null)
                return null;

            string layerName = Path.GetFileName(blueprint.Name);
            Layers.SceneLayer layer = new Layers.SceneLayer(this, layerName, new SharpDX.Color4(0.0f, 0.5f, 0.0f, 1.0f));

            foreach (Entity entity in entities)
            {
                if (entity is ILayerEntity)
                {
                    ILayerEntity entityLayer = entity as ILayerEntity;
                    Layers.SceneLayer childLayer = entityLayer.GetLayer();
                    if (childLayer != null)
                        layer.ChildLayers.Add(childLayer);
                }
                else
                {
                    layer.AddEntity(entity);
                    entity.SetOwner(entity);
                }
            }
            return layer;
        }
    }
}
