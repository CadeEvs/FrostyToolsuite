
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.VaultComponentData))]
	public class VaultComponent : GameComponent, IEntityData<FrostySdk.Ebx.VaultComponentData>
	{
		public new FrostySdk.Ebx.VaultComponentData Data => data as FrostySdk.Ebx.VaultComponentData;
		public override string DisplayName => "VaultComponent";

		public VaultComponent(FrostySdk.Ebx.VaultComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

