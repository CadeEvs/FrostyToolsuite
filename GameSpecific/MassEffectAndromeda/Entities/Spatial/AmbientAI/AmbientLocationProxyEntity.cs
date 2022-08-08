using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AmbientLocationProxyEntityData))]
	public class AmbientLocationProxyEntity : AmbientLocationProxyBase, IEntityData<FrostySdk.Ebx.AmbientLocationProxyEntityData>
	{
		public new FrostySdk.Ebx.AmbientLocationProxyEntityData Data => data as FrostySdk.Ebx.AmbientLocationProxyEntityData;

		public AmbientLocationProxyEntity(FrostySdk.Ebx.AmbientLocationProxyEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

