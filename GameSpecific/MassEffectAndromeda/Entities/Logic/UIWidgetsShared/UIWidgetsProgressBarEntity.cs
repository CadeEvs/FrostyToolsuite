using LevelEditorPlugin.Managers;
using LevelEditorPlugin.Render.Proxies;
using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIWidgetsProgressBarEntityData))]
	public class UIWidgetsProgressBarEntity : UIWidgetsElementEntity, IEntityData<FrostySdk.Ebx.UIWidgetsProgressBarEntityData>
	{
		protected readonly int Property_CurrentValue = Frosty.Hash.Fnv1.HashString("CurrentValue");
		protected readonly int Property_MaximumValue = Frosty.Hash.Fnv1.HashString("MaximumValue");

		public new FrostySdk.Ebx.UIWidgetsProgressBarEntityData Data => data as FrostySdk.Ebx.UIWidgetsProgressBarEntityData;
		public override string DisplayName => "UIWidgetsProgressBar";
        public override IEnumerable<ConnectionDesc> Properties
        {
			get
            {
				List<ConnectionDesc> outProperties = new List<ConnectionDesc>();
				outProperties.AddRange(base.Properties);
				outProperties.Add(new ConnectionDesc("CurrentValue", Direction.In, typeof(float)));
				outProperties.Add(new ConnectionDesc("MaximumValue", Direction.In, typeof(float)));
				return outProperties;
			}
        }
		public Assets.UIWidgetsAppearance Background => backgroundAsset; 
        public Assets.UIWidgetsAppearance Fill => fillAsset;
		public float CurrentValue => currentValueProperty.Value;
		public float MaximumValue => maximumValueProperty.Value;

		protected Assets.UIWidgetsAppearance backgroundAsset;
		protected Assets.UIWidgetsAppearance fillAsset;

		protected Property<float> currentValueProperty;
		protected Property<float> maximumValueProperty;

		public UIWidgetsProgressBarEntity(FrostySdk.Ebx.UIWidgetsProgressBarEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
			SetFlags(EntityFlags.HasLogic);
			backgroundAsset = LoadedAssetManager.Instance.LoadAsset<Assets.UIWidgetsAppearance>(this, Data.Background);
			if (backgroundAsset != null)
			{
				backgroundAsset.InitializeResources(FindAncestor<UIWidgetEntity>());
			}

			fillAsset = LoadedAssetManager.Instance.LoadAsset<Assets.UIWidgetsAppearance>(this, Data.Fill);
			if (fillAsset != null)
			{
				fillAsset.InitializeResources(FindAncestor<UIWidgetEntity>());
			}

			currentValueProperty = new Property<float>(this, Property_CurrentValue, Data.CurrentValue);
			maximumValueProperty = new Property<float>(this, Property_MaximumValue, Data.MaximumValue);
		}

		public override WidgetProxy CreateRenderProxy()
		{
			SetFlags(EntityFlags.RenderProxyGenerated);
			return new UIWidgetsProgressBarProxy(this);
		}

		public override void Destroy()
		{
			LoadedAssetManager.Instance.UnloadAsset(backgroundAsset);
			LoadedAssetManager.Instance.UnloadAsset(fillAsset);
			base.Destroy();
		}
	}
}

