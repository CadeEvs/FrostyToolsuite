using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.VisualEnvironmentReferenceObjectData))]
	public class VisualEnvironmentReferenceObject : LogicReferenceObject, IEntityData<FrostySdk.Ebx.VisualEnvironmentReferenceObjectData>
	{
		public new FrostySdk.Ebx.VisualEnvironmentReferenceObjectData Data => data as FrostySdk.Ebx.VisualEnvironmentReferenceObjectData;
		public new Assets.VisualEnvironmentBlueprint Blueprint => blueprint as Assets.VisualEnvironmentBlueprint;

		public VisualEnvironmentReferenceObject(FrostySdk.Ebx.VisualEnvironmentReferenceObjectData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

