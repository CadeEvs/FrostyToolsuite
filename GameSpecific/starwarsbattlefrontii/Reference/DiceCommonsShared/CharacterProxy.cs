using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CharacterProxyData))]
	public class CharacterProxy : BlueprintProxy, IEntityData<FrostySdk.Ebx.CharacterProxyData>
	{
		public new FrostySdk.Ebx.CharacterProxyData Data => data as FrostySdk.Ebx.CharacterProxyData;

		public CharacterProxy(FrostySdk.Ebx.CharacterProxyData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

