using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MESoldierHealthComponentData))]
	public class MESoldierHealthComponent : SoldierHealthComponent, IEntityData<FrostySdk.Ebx.MESoldierHealthComponentData>
	{
		public new FrostySdk.Ebx.MESoldierHealthComponentData Data => data as FrostySdk.Ebx.MESoldierHealthComponentData;
		public override string DisplayName => "MESoldierHealthComponent";

		public MESoldierHealthComponent(FrostySdk.Ebx.MESoldierHealthComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

