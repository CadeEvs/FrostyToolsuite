using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ContactProjectileAudioComponentData))]
	public class ContactProjectileAudioComponent : GameComponent, IEntityData<FrostySdk.Ebx.ContactProjectileAudioComponentData>
	{
		public new FrostySdk.Ebx.ContactProjectileAudioComponentData Data => data as FrostySdk.Ebx.ContactProjectileAudioComponentData;
		public override string DisplayName => "ContactProjectileAudioComponent";

		public ContactProjectileAudioComponent(FrostySdk.Ebx.ContactProjectileAudioComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

