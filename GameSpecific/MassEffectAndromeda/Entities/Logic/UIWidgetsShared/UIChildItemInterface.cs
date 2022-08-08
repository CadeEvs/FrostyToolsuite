using System;
using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIChildItemInterfaceData))]
	public class UIChildItemInterface : LogicEntity, IEntityData<FrostySdk.Ebx.UIChildItemInterfaceData>
	{
		public new FrostySdk.Ebx.UIChildItemInterfaceData Data => data as FrostySdk.Ebx.UIChildItemInterfaceData;
		public override string DisplayName => "UIChildItemInterface";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
        public override IEnumerable<string> HeaderRows
        {
			get
            {
				List<string> outHeaderRows = new List<string>();
				if (dataType != null)
				{
					outHeaderRows.Add(dataType.Name);
				}
				return outHeaderRows;
            }
        }
		public object DataSource
        {
			get => outDataProperty.Value;
			set => outDataProperty.Value = value;
        }

		protected IProperty outDataProperty;
        protected Type dataType;

		public UIChildItemInterface(FrostySdk.Ebx.UIChildItemInterfaceData inData, Entity inParent)
			: base(inData, inParent)
		{
			dataType = FrostySdk.TypeLibrary.GetType(Data.DynamicInputDataType);
			outDataProperty = new Property<object>(this, 0x4a600066);
			mockDataObject = Activator.CreateInstance(dataType);
		}

        public override void BeginSimulation()
        {
            base.BeginSimulation();

			if (outDataProperty.IsUnset)
			{
				outDataProperty.Value = mockDataObject;
			}
        }
    }
}

