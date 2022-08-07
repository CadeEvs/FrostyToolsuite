using LevelEditorPlugin.Editors;
using LevelEditorPlugin.Managers;
using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LogicReferenceObjectData))]
	public class LogicReferenceObject : ReferenceObject, IEntityData<FrostySdk.Ebx.LogicReferenceObjectData>, INotRealSpatialEntity
	{
		public new FrostySdk.Ebx.LogicReferenceObjectData Data => data as FrostySdk.Ebx.LogicReferenceObjectData;

		public LogicReferenceObject(FrostySdk.Ebx.LogicReferenceObjectData inData, Entity inParent, EntityWorld inWorld)
			: base(inData, inParent, inWorld)
		{
		}

		public LogicReferenceObject(FrostySdk.Ebx.LogicReferenceObjectData inData, Entity inParent)
			: this(inData, inParent, null)
		{
		}

        public override void SetDefaultValues()
        {
            base.SetDefaultValues();
			Data.LocalPlayerId = FrostySdk.Ebx.LocalPlayerId.LocalPlayerId_Invalid;
		}
    }
}

