using System;
using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DynamicCastEntityData))]
	public class DynamicCastEntity : LogicEntity, IEntityData<FrostySdk.Ebx.DynamicCastEntityData>
	{
		protected readonly int Property_InData = Frosty.Hash.Fnv1.HashString("InData");
		protected readonly int Property_Out = Frosty.Hash.Fnv1.HashString("Out");

		public new FrostySdk.Ebx.DynamicCastEntityData Data => data as FrostySdk.Ebx.DynamicCastEntityData;
		public override string DisplayName => "DynamicCast";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
        public override IEnumerable<ConnectionDesc> Properties
        {
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("InData", Direction.In),
				new ConnectionDesc("Out", Direction.Out)
			};
        }
        public override IEnumerable<string> HeaderRows
        {
			get
            {
				List<string> outHeaderRows = new List<string>();
				outHeaderRows.Add(dynamicOutputDataType.Name);
				return outHeaderRows;
            }
        }

        protected Type dynamicOutputDataType;
		protected IProperty inDataProperty;
		protected IProperty outProperty;

        public DynamicCastEntity(FrostySdk.Ebx.DynamicCastEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
			dynamicOutputDataType = FrostySdk.TypeLibrary.GetType(Data.DynamicOutputDataType);
			inDataProperty = new Property<object>(this, Property_InData);
			outProperty = new Property<object>(this, Property_Out);
		}

        public override void OnPropertyChanged(int propertyHash)
        {
			if (propertyHash == inDataProperty.NameHash)
			{
				outProperty.Value = inDataProperty.Value;
				return;
			}

            base.OnPropertyChanged(propertyHash);
        }
    }
}

