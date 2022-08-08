
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WSShieldComponentData))]
	public class WSShieldComponent : ShieldComponent, IEntityData<FrostySdk.Ebx.WSShieldComponentData>
	{
		public new FrostySdk.Ebx.WSShieldComponentData Data => data as FrostySdk.Ebx.WSShieldComponentData;
		public override string DisplayName => "WSShieldComponent";

		public WSShieldComponent(FrostySdk.Ebx.WSShieldComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

