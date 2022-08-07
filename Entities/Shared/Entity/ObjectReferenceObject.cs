using LevelEditorPlugin.Editors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditorPlugin.Entities
{
    [EntityBinding(DataType = typeof(FrostySdk.Ebx.ObjectReferenceObjectData))]
    public class ObjectReferenceObject : SpatialReferenceObject, IEntityData<FrostySdk.Ebx.ObjectReferenceObjectData>
    {
        public new FrostySdk.Ebx.ObjectReferenceObjectData Data => data as FrostySdk.Ebx.ObjectReferenceObjectData;
        public new Assets.ObjectBlueprint Blueprint => blueprint as Assets.ObjectBlueprint;
        public override string DisplayName => (blueprint != null) ? Path.GetFileName(blueprint.Name) : base.DisplayName;

        public ObjectReferenceObject(FrostySdk.Ebx.ObjectReferenceObjectData inData, Entity inParent, EntityWorld inWorld)
            : base(inData, inParent, inWorld)
        {
        }

        public ObjectReferenceObject(FrostySdk.Ebx.ObjectReferenceObjectData inData, Entity inParent)
            : this(inData, inParent, null)
        {
        }
    }
}
