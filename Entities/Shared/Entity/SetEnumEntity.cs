using FrostySdk;
using System;
using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SetEnumEntityData))]
	public class SetEnumEntity : ExplicitEnumTypeLogicEntity, IEntityData<FrostySdk.Ebx.SetEnumEntityData>
	{
		public new FrostySdk.Ebx.SetEnumEntityData Data => data as FrostySdk.Ebx.SetEnumEntityData;
		public override string DisplayName => "SetEnum";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
		public override IEnumerable<ConnectionDesc> Events
		{
			get
			{
				List<ConnectionDesc> outEvents = new List<ConnectionDesc>();
				string[] names = Enum.GetNames(enumType);

				for (int i = 0; i < Data.InputEvents.Count; i++)
				{
					outEvents.Add(new ConnectionDesc() { Name = $"Set_{names[i]}", Direction = Direction.In });
				}

				return outEvents;
			}
		}
		public override IEnumerable<ConnectionDesc> Properties
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Out", Direction.Out)
			};
		}

		public SetEnumEntity(FrostySdk.Ebx.SetEnumEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

