using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditorPlugin.Entities
{
    [EntityBinding(DataType = typeof(FrostySdk.Ebx.LayerReferenceObjectData))]
    public class LayerReferenceObject : ReferenceObject, IEntityData<FrostySdk.Ebx.LayerReferenceObjectData>, ILayerEntity
    {
        public new FrostySdk.Ebx.LayerReferenceObjectData Data => data as FrostySdk.Ebx.LayerReferenceObjectData;
        public new Assets.Layer Blueprint => blueprint as Assets.Layer;

        public LayerReferenceObject(FrostySdk.Ebx.LayerReferenceObjectData inData, Entity inParent)
            : base(inData, inParent)
        {
        }

        public Layers.SceneLayer GetLayer()
        {
            if (blueprint == null)
                return null;

            //if (IsLogicLayer)
            //    return null;

            string layerName = $"{Path.GetFileName(blueprint.Name)}";
            Layers.SceneLayer layer = new Layers.SceneLayer(this, layerName, new SharpDX.Color4(1.0f, 0.0f, 0.0f, 1.0f));

            foreach (var entity in entities)
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
