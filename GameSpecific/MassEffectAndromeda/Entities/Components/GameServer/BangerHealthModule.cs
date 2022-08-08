using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BangerHealthModuleData))]
	public class BangerHealthModule : GameComponent, IEntityData<FrostySdk.Ebx.BangerHealthModuleData>
	{
		public new FrostySdk.Ebx.BangerHealthModuleData Data => data as FrostySdk.Ebx.BangerHealthModuleData;
		public override string DisplayName => "BangerHealthModule";

		public BangerHealthModule(FrostySdk.Ebx.BangerHealthModuleData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

