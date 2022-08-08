using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AutoSizedContainerData))]
	public class AutoSizedContainer : WSUIContainerEntity, IEntityData<FrostySdk.Ebx.AutoSizedContainerData>
	{
		public new FrostySdk.Ebx.AutoSizedContainerData Data => data as FrostySdk.Ebx.AutoSizedContainerData;
		public override string DisplayName => "AutoSizedContainer";

		public AutoSizedContainer(FrostySdk.Ebx.AutoSizedContainerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

