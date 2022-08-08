using LevelEditorPlugin.Managers;
using System.Collections.Generic;
using ButtonLegendLayout = FrostySdk.Ebx.ButtonLegendLayout;
using ButtonLegendButtonItem = FrostySdk.Ebx.ButtonLegendButtonItem;
using Frosty.Core;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIButtonLegendLayoutDataProviderData))]
	public class UIButtonLegendLayoutDataProvider : LogicEntity, IEntityData<FrostySdk.Ebx.UIButtonLegendLayoutDataProviderData>
	{
		protected readonly int Property_Layout = Frosty.Hash.Fnv1.HashString("Layout");

		public new FrostySdk.Ebx.UIButtonLegendLayoutDataProviderData Data => data as FrostySdk.Ebx.UIButtonLegendLayoutDataProviderData;
		public override string DisplayName => "UIButtonLegendLayoutDataProvider";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
        public override IEnumerable<ConnectionDesc> Properties
        {
			get
            {
				List<ConnectionDesc> outProperties = new List<ConnectionDesc>();
				outProperties.Add(new ConnectionDesc("Layout", Direction.Out));
				return outProperties;
            }
        }

        protected Assets.UIButtonLegendActionMappings mappings;
		protected ButtonLegendLayout layout;

		protected Property<ButtonLegendLayout> layoutProperty;

		public UIButtonLegendLayoutDataProvider(FrostySdk.Ebx.UIButtonLegendLayoutDataProviderData inData, Entity inParent)
			: base(inData, inParent)
		{
			SetFlags(EntityFlags.HasLogic);
			mappings = LoadedAssetManager.Instance.LoadAsset<Assets.UIButtonLegendActionMappings>(this, Data.Mappings);
			layout = new ButtonLegendLayout() { Alignment = Data.Alignment };

			foreach (var item in Data.Items)
			{
				var buttonItem = new ButtonLegendButtonItem();
				var actionMapping = mappings.GetActionMapping(item.InputConcepts);

				buttonItem.Disabled = item.Disabled;
				buttonItem.Highlighted = item.Highlighted;
				buttonItem.InputConcepts.AddRange(item.InputConcepts);
				buttonItem.Label = LocalizedStringDatabase.Current.GetString((uint)item.Label.StringId);
				buttonItem.MarkupString = actionMapping.MarkupString;
				buttonItem.ControlVisibility = actionMapping.ControlVisibility;
				buttonItem.TextureId = actionMapping.TextureId;
				buttonItem.UseDoubleWidthIcon = actionMapping.UseDoubleWidthIcon;

				layout.Buttons.Add(new FrostySdk.Ebx.PointerRef(buttonItem));
			}

			layoutProperty = new Property<ButtonLegendLayout>(this, Property_Layout);
		}

        public override void BeginSimulation()
        {
            base.BeginSimulation();
			layoutProperty.Value = layout;
        }

        public override void Destroy()
        {
			LoadedAssetManager.Instance.UnloadAsset(mappings);
            base.Destroy();
        }
    }
}

