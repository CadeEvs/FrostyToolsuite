using LevelEditorPlugin.Managers;
using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIScreenRenderEntityData))]
	public class UIScreenRenderEntity : LogicReferenceObject, IEntityData<FrostySdk.Ebx.UIScreenRenderEntityData>
	{
		public new FrostySdk.Ebx.UIScreenRenderEntityData Data => data as FrostySdk.Ebx.UIScreenRenderEntityData;
        public override IEnumerable<ConnectionDesc> Events
        {
			get
            {
				List<ConnectionDesc> outEvents = new List<ConnectionDesc>();
				outEvents.AddRange(base.Events);
				outEvents.Add(new ConnectionDesc("Enable", Direction.In));
				outEvents.Add(new ConnectionDesc("Disable", Direction.In));
				return outEvents;
			}
        }
		public override IEnumerable<ConnectionDesc> Properties
		{
			get
			{
				List<ConnectionDesc> outProperties = new List<ConnectionDesc>();
				outProperties.AddRange(base.Properties);
				outProperties.Add(new ConnectionDesc("Alpha", Direction.In));
				outProperties.Add(new ConnectionDesc("Visible", Direction.In));
				return outProperties;
			}
		}

		public UIScreenRenderEntity(FrostySdk.Ebx.UIScreenRenderEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}

        protected override void Initialize()
        {
			blueprint = LoadedAssetManager.Instance.LoadAsset<Assets.UIWidgetBlueprint>(this, Data.Blueprint);
        }

        public override void Destroy()
        {
			LoadedAssetManager.Instance.UnloadAsset(blueprint);
        }
    }
}

