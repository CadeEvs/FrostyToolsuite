using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClothInstanceObserverEntityData))]
	public class ClothInstanceObserverEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ClothInstanceObserverEntityData>
	{
		public new FrostySdk.Ebx.ClothInstanceObserverEntityData Data => data as FrostySdk.Ebx.ClothInstanceObserverEntityData;
		public override string DisplayName => "ClothInstanceObserver";

		public ClothInstanceObserverEntity(FrostySdk.Ebx.ClothInstanceObserverEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

