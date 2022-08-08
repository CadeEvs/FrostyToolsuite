using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LogicVisualEnvironmentReferenceObjectData))]
	public class LogicVisualEnvironmentReferenceObject : VisualEnvironmentReferenceObject, IEntityData<FrostySdk.Ebx.LogicVisualEnvironmentReferenceObjectData>
	{
		public new FrostySdk.Ebx.LogicVisualEnvironmentReferenceObjectData Data => data as FrostySdk.Ebx.LogicVisualEnvironmentReferenceObjectData;

		public LogicVisualEnvironmentReferenceObject(FrostySdk.Ebx.LogicVisualEnvironmentReferenceObjectData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

