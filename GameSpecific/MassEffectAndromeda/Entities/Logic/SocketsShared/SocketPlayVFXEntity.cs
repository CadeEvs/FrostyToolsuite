using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SocketPlayVFXEntityData))]
	public class SocketPlayVFXEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SocketPlayVFXEntityData>
	{
		public new FrostySdk.Ebx.SocketPlayVFXEntityData Data => data as FrostySdk.Ebx.SocketPlayVFXEntityData;
		public override string DisplayName => "SocketPlayVFX";
		public override IEnumerable<ConnectionDesc> Events
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Start", Direction.In)
			};
		}
		public override IEnumerable<ConnectionDesc> Properties
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("RawTransformEffectLocation", Direction.In)
			};
		}

		public SocketPlayVFXEntity(FrostySdk.Ebx.SocketPlayVFXEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

