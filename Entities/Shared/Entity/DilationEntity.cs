using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DilationEntityData))]
	public class DilationEntity : LogicEntity, IEntityData<FrostySdk.Ebx.DilationEntityData>
	{
		public new FrostySdk.Ebx.DilationEntityData Data => data as FrostySdk.Ebx.DilationEntityData;
		public override string DisplayName => "Dilation";
#if MASS_EFFECT
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
#endif
		public override IEnumerable<ConnectionDesc> Events
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Activate", Direction.In),
				new ConnectionDesc("Deactivate", Direction.In)
			};
		}

		public DilationEntity(FrostySdk.Ebx.DilationEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

