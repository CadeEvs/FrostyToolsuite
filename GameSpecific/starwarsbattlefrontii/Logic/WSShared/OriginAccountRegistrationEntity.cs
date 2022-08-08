using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.OriginAccountRegistrationEntityData))]
	public class OriginAccountRegistrationEntity : LogicEntity, IEntityData<FrostySdk.Ebx.OriginAccountRegistrationEntityData>
	{
		public new FrostySdk.Ebx.OriginAccountRegistrationEntityData Data => data as FrostySdk.Ebx.OriginAccountRegistrationEntityData;
		public override string DisplayName => "OriginAccountRegistration";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public OriginAccountRegistrationEntity(FrostySdk.Ebx.OriginAccountRegistrationEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

