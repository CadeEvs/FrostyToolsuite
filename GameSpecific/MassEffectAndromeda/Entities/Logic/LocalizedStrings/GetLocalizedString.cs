using Frosty.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using CString = FrostySdk.Ebx.CString;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GetLocalizedStringData))]
	public class GetLocalizedString : LogicEntity, IEntityData<FrostySdk.Ebx.GetLocalizedStringData>
	{
		protected readonly int Property_StringIDOverride = Frosty.Hash.Fnv1.HashString("StringIDOverride");
		protected readonly int Property_OutString = Frosty.Hash.Fnv1.HashString("OutString");

		public new FrostySdk.Ebx.GetLocalizedStringData Data => data as FrostySdk.Ebx.GetLocalizedStringData;
		public override string DisplayName => "GetLocalizedString";
		public override IEnumerable<ConnectionDesc> Properties
		{
			get
			{
				List<ConnectionDesc> outProperties = new List<ConnectionDesc>();
				foreach (var input in Data.Inputs)
				{
					Type propType = GetInputType(input.InputType);
					outProperties.Add(new ConnectionDesc(FrostySdk.Utils.GetString((int)input.InputNameHash), Direction.In, propType));
				}
				outProperties.Add(new ConnectionDesc("StringIDOverride", Direction.In, typeof(int)));
				outProperties.Add(new ConnectionDesc("OutString", Direction.Out, typeof(CString)));
				return outProperties;
			}
		}

		protected Property<int> stringIdOverrideProperty;
		protected List<IProperty> inputProperties = new List<IProperty>();
		protected Property<CString> outStringProperty;

		public GetLocalizedString(FrostySdk.Ebx.GetLocalizedStringData inData, Entity inParent)
			: base(inData, inParent)
		{
			SetFlags(EntityFlags.HasLogic);
			foreach (var input in Data.Inputs)
			{
				switch (input.InputType)
                {
					case FrostySdk.Ebx.TokenizedStringInputType.TokenizedStringInputType_Float: inputProperties.Add(new Property<float>(this, (int)input.InputNameHash, 0.0f)); break;
					case FrostySdk.Ebx.TokenizedStringInputType.TokenizedStringInputType_Integer: inputProperties.Add(new Property<int>(this, (int)input.InputNameHash, 0)); break;
					case FrostySdk.Ebx.TokenizedStringInputType.TokenizedStringInputType_String: inputProperties.Add(new Property<CString>(this, (int)input.InputNameHash, "")); break;
				}
			}
			stringIdOverrideProperty = new Property<int>(this, Property_StringIDOverride, Data.StringIDOverride);
			outStringProperty = new Property<CString>(this, Property_OutString);
		}

        public override void BeginSimulation()
        {
            base.BeginSimulation();

			if (outStringProperty.IsUnset)
			{
				outStringProperty.Value = LocalizedStringDatabase.Current.GetString((uint)Data.Text.StringId);
			}
        }

        public override void OnPropertyChanged(int propertyHash)
        {
			if (inputProperties.FirstOrDefault(p => p.NameHash == propertyHash) != null || propertyHash == stringIdOverrideProperty.NameHash)
			{
				int stringId = (!stringIdOverrideProperty.IsUnset) ? stringIdOverrideProperty.Value : Data.Text.StringId;
				string textString = LocalizedStringDatabase.Current.GetString((uint)stringId);
				foreach (var input in Data.Inputs)
                {
					string pattern = $"{{CUSTOM{input.TokenId}}}";
					string replaceValue = "";

					var property = GetProperty((int)input.InputNameHash);
					if (input.InputType == FrostySdk.Ebx.TokenizedStringInputType.TokenizedStringInputType_String)
					{
						replaceValue = (CString)property.Value;
					}
					else if (input.InputType == FrostySdk.Ebx.TokenizedStringInputType.TokenizedStringInputType_Float)
					{
						replaceValue = ((float)property.Value).ToString($"F{input.NumericDecimalPlaces}");
					}
					else
                    {
						replaceValue = ((int)property.Value).ToString();
                    }

					textString = textString.Replace(pattern, replaceValue);
				}

				outStringProperty.Value = (CString)textString;
				return;
			}

            base.OnPropertyChanged(propertyHash);
        }

        private Type GetInputType(FrostySdk.Ebx.TokenizedStringInputType inputType)
        {
			Type propType = null;
			switch (inputType)
			{
				case FrostySdk.Ebx.TokenizedStringInputType.TokenizedStringInputType_Float: propType = typeof(float); break;
				case FrostySdk.Ebx.TokenizedStringInputType.TokenizedStringInputType_Integer: propType = typeof(int); break;
				case FrostySdk.Ebx.TokenizedStringInputType.TokenizedStringInputType_String: propType = typeof(CString); break;
			}
			return propType;
		}
    }
}

