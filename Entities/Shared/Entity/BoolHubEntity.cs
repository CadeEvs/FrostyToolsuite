using FrostySdk;
using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BoolHubEntityData))]
	public class BoolHubEntity : LogicEntity, IEntityData<FrostySdk.Ebx.BoolHubEntityData>
	{
		public new FrostySdk.Ebx.BoolHubEntityData Data => data as FrostySdk.Ebx.BoolHubEntityData;
		public override string DisplayName => "BoolHub";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
		public override IEnumerable<ConnectionDesc> Properties
		{
			get
			{
				List<ConnectionDesc> outProperties = new List<ConnectionDesc>();
#if !GW2
				for (int i = 0; i < Data.HashedInput.Count; i++)
				{
					outProperties.Add(new ConnectionDesc() { Name = Utils.GetString((int)Data.HashedInput[i]), Direction = Direction.In });
				}
				outProperties.Add(new ConnectionDesc("Out", Direction.Out));
#endif
				return outProperties;
			}
		}

		public BoolHubEntity(FrostySdk.Ebx.BoolHubEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

