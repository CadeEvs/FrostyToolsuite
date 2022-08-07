using LevelEditorPlugin.Editors;
using LevelEditorPlugin.Managers;
using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GameComponentEntityData))]
	public class GameComponentEntity : ComponentEntity, IEntityData<FrostySdk.Ebx.GameComponentEntityData>
	{
		public new FrostySdk.Ebx.GameComponentEntityData Data => data as FrostySdk.Ebx.GameComponentEntityData;

		//protected List<Assets.Asset> additionalAssets = new List<Assets.Asset>();

		public GameComponentEntity(FrostySdk.Ebx.GameComponentEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

