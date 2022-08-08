using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.OnlineArubaManagerData))]
	public class OnlineArubaManager : LogicEntity, IEntityData<FrostySdk.Ebx.OnlineArubaManagerData>
	{
		public new FrostySdk.Ebx.OnlineArubaManagerData Data => data as FrostySdk.Ebx.OnlineArubaManagerData;
		public override string DisplayName => "OnlineArubaManager";

		public OnlineArubaManager(FrostySdk.Ebx.OnlineArubaManagerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

