using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.InteractionContextProviderEntityData))]
	public class InteractionContextProviderEntity : ContextProvider, IEntityData<FrostySdk.Ebx.InteractionContextProviderEntityData>
	{
		public new FrostySdk.Ebx.InteractionContextProviderEntityData Data => data as FrostySdk.Ebx.InteractionContextProviderEntityData;
		public override string DisplayName => "InteractionContextProvider";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public InteractionContextProviderEntity(FrostySdk.Ebx.InteractionContextProviderEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

