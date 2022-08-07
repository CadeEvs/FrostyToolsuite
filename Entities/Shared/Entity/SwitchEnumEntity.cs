using FrostySdk;
using System;
using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SwitchEnumEntityData))]
	public class SwitchEnumEntity : ImpliedEnumTypeLogicEntity, IEntityData<FrostySdk.Ebx.SwitchEnumEntityData>
	{
		public new FrostySdk.Ebx.SwitchEnumEntityData Data => data as FrostySdk.Ebx.SwitchEnumEntityData;
		public override string DisplayName => $"SwitchEnum";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
		public override IEnumerable<ConnectionDesc> Events
		{
			get
			{
				List<ConnectionDesc> outEvents = new List<ConnectionDesc>();
				string[] names = Enum.GetNames(enumType);

				for (int i = 0; i < Data.OutputEvents.Count; i++)
				{
					outEvents.Add(new ConnectionDesc() { Name = names[i], Direction = Direction.Out });
				}

				outEvents.Add(new ConnectionDesc("Default", Direction.Out));
				return outEvents;
			}
		}
		public override IEnumerable<ConnectionDesc> Properties
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("In", Direction.In)
			};
		}
		public override IEnumerable<string> HeaderRows
		{
			get => new List<string>()
			{
				enumType.Name
			};
		}

		public SwitchEnumEntity(FrostySdk.Ebx.SwitchEnumEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

