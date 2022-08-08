using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DynamicTextureOutputElementData))]
	public class DynamicTextureOutputElement : WSUIElementEntity, IEntityData<FrostySdk.Ebx.DynamicTextureOutputElementData>
	{
		public new FrostySdk.Ebx.DynamicTextureOutputElementData Data => data as FrostySdk.Ebx.DynamicTextureOutputElementData;
		public override string DisplayName => "DynamicTextureOutputElement";

		public DynamicTextureOutputElement(FrostySdk.Ebx.DynamicTextureOutputElementData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

