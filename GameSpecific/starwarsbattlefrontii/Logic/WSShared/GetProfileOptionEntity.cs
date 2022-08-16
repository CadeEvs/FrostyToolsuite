using System.Collections.Generic;
using System.Windows.Shapes;
using LevelEditorPlugin.Managers;
using Path = System.IO.Path;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GetProfileOptionEntityData))]
	public class GetProfileOptionEntity : ProfileOptionEntity, IEntityData<FrostySdk.Ebx.GetProfileOptionEntityData>
	{
		public new FrostySdk.Ebx.GetProfileOptionEntityData Data => data as FrostySdk.Ebx.GetProfileOptionEntityData;
		public override string DisplayName => "GetProfileOption";

		public override IEnumerable<ConnectionDesc> Events
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Update", Direction.In),
			};
		}
		public override IEnumerable<ConnectionDesc> Properties
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("OptionData", Direction.Out),
				new ConnectionDesc("IntValue", Direction.Out, typeof(int)),
				new ConnectionDesc("BoolValue", Direction.Out, typeof(bool)),
				new ConnectionDesc("FloatValue", Direction.Out, typeof(float))
			};
		}

		private Assets.ProfileOptionData m_optionsAsset;
		
		public override IEnumerable<string> HeaderRows
		{
			get
			{
				List<string> outHeaderRows = new List<string>();
				if (m_optionsAsset != null)
				{
					outHeaderRows.Add($"Option: {Path.GetFileName(m_optionsAsset.Name)}");
				}
				return outHeaderRows;
			}
		}
		
		public GetProfileOptionEntity(FrostySdk.Ebx.GetProfileOptionEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
			m_optionsAsset = LoadedAssetManager.Instance.LoadAsset<Assets.ProfileOptionData>(this, Data.OptionData);
		}

		public override void Destroy()
		{
			LoadedAssetManager.Instance.UnloadAsset(m_optionsAsset);
		}
	}
}

