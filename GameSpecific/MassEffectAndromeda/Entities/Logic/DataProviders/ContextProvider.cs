using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ContextProviderData))]
	public class ContextProvider : LogicEntity, IEntityData<FrostySdk.Ebx.ContextProviderData>
	{
		public new FrostySdk.Ebx.ContextProviderData Data => data as FrostySdk.Ebx.ContextProviderData;
		public override string DisplayName => "ContextProvider";

		public ContextProvider(FrostySdk.Ebx.ContextProviderData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

