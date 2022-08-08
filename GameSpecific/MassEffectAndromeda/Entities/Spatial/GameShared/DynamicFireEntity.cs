using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DynamicFireEntityData))]
	public class DynamicFireEntity : GameComponentEntity, IEntityData<FrostySdk.Ebx.DynamicFireEntityData>
	{
		public new FrostySdk.Ebx.DynamicFireEntityData Data => data as FrostySdk.Ebx.DynamicFireEntityData;

		public DynamicFireEntity(FrostySdk.Ebx.DynamicFireEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

