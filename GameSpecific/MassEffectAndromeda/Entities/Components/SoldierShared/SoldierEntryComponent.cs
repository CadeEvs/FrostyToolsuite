using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SoldierEntryComponentData))]
	public class SoldierEntryComponent : CharacterEntryComponent, IEntityData<FrostySdk.Ebx.SoldierEntryComponentData>
	{
		public new FrostySdk.Ebx.SoldierEntryComponentData Data => data as FrostySdk.Ebx.SoldierEntryComponentData;
		public override string DisplayName => "SoldierEntryComponent";

		public SoldierEntryComponent(FrostySdk.Ebx.SoldierEntryComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

