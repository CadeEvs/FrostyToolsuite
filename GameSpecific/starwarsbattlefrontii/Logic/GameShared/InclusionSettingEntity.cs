using System.Collections.Generic;
using FrostySdk.Ebx;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.InclusionSettingEntityData))]
	public class InclusionSettingEntity : LogicEntity, IEntityData<FrostySdk.Ebx.InclusionSettingEntityData>
	{
		public new FrostySdk.Ebx.InclusionSettingEntityData Data => data as FrostySdk.Ebx.InclusionSettingEntityData;
		public override string DisplayName => "InclusionSetting";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public override IEnumerable<ConnectionDesc> Properties
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Out", Direction.Out)
			};
		}

		public override IEnumerable<string> HeaderRows
		{
			get
			{
				List<string> outHeaderRows = new List<string>();
				foreach (CString setting in Data.Settings)
				{
					outHeaderRows.Add($"Setting: {setting}");
				}
				return outHeaderRows;
			}
		}
		
		public InclusionSettingEntity(FrostySdk.Ebx.InclusionSettingEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

