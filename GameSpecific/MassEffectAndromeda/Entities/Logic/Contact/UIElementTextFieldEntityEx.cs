using Frosty.Core;
using LevelEditorPlugin.Managers;
using LevelEditorPlugin.Render.Proxies;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using CString = FrostySdk.Ebx.CString;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIElementTextFieldEntityExData))]
	public class UIElementTextFieldEntityEx : UIWidgetsTextFieldEntity, IEntityData<FrostySdk.Ebx.UIElementTextFieldEntityExData>
	{
		protected readonly int Property_FieldText = Frosty.Hash.Fnv1.HashString("FieldText");

		public new FrostySdk.Ebx.UIElementTextFieldEntityExData Data => data as FrostySdk.Ebx.UIElementTextFieldEntityExData;
		public override string DisplayName => "UIElementTextFieldEntityEx";
        public override IEnumerable<ConnectionDesc> Properties
        {
			get
            {
				List<ConnectionDesc> outProperties = new List<ConnectionDesc>();
				outProperties.AddRange(base.Properties);
				outProperties.Add(new ConnectionDesc("FieldText", Direction.In, typeof(CString)));
				return outProperties;
            }
        }
        public Assets.UIElementFontStyle FontStyle => fontStyle;
		public string FieldText => fieldTextProperty.Value;

		protected Assets.UIElementFontStyle fontStyle;

		protected Property<CString> fieldTextProperty;

		public UIElementTextFieldEntityEx(FrostySdk.Ebx.UIElementTextFieldEntityExData inData, Entity inParent)
			: base(inData, inParent)
		{
			SetFlags(EntityFlags.HasLogic);
			fontStyle = LoadedAssetManager.Instance.LoadAsset<Assets.UIElementFontStyle>(this, Data.FontStyle);
			fieldTextProperty = new Property<CString>(this, Property_FieldText, Data.FieldText);

			if (Data.LocalizedTextString.StringId != 0)
			{
				fieldTextProperty.Value = LocalizedStringDatabase.Current.GetString((uint)Data.LocalizedTextString.StringId);
			}
		}

        public override void OnPropertyChanged(int propertyHash)
        {
			base.OnPropertyChanged(propertyHash);
		}

        public override WidgetProxy CreateRenderProxy()
        {
			SetFlags(EntityFlags.RenderProxyGenerated);
			return new UIElementTextFieldExProxy(this);
        }

        public override void Destroy()
        {
			LoadedAssetManager.Instance.UnloadAsset(fontStyle);
            base.Destroy();
        }

        public override void SetDefaultValues()
        {
            base.SetDefaultValues();

			Data.Size = new FrostySdk.Ebx.Vec2() { x = 200, y = 50 };
        }

        public override void OnDataModified()
        {
            base.OnDataModified();

			if (fontStyle != null)
			{
				LoadedAssetManager.Instance.UnloadAsset(fontStyle);
			}

			fontStyle = LoadedAssetManager.Instance.LoadAsset<Assets.UIElementFontStyle>(this, Data.FontStyle);
			fieldTextProperty.Value = Data.FieldText;

			if (Data.LocalizedTextString.StringId != 0)
			{
				fieldTextProperty.Value = LocalizedStringDatabase.Current.GetString((uint)Data.LocalizedTextString.StringId);
			}
		}
    }
}

