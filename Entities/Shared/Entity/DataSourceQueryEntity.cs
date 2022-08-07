using LevelEditorPlugin.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DataSourceQueryEntityData))]
	public class DataSourceQueryEntity : LogicEntity, IEntityData<FrostySdk.Ebx.DataSourceQueryEntityData>
	{
		protected readonly int Property_InData = Frosty.Hash.Fnv1.HashString("InData");
		protected readonly int Property_ArrayIndex = Frosty.Hash.Fnv1.HashString("ArrayIndex");

		public new FrostySdk.Ebx.DataSourceQueryEntityData Data => data as FrostySdk.Ebx.DataSourceQueryEntityData;
		public override string DisplayName => "DataSourceQuery";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
        public override IEnumerable<ConnectionDesc> Properties
        {
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("InData", Direction.In),
				new ConnectionDesc("ArrayIndex", Direction.In, typeof(int))
			};
		}

		protected IProperty inDataProperty;
		protected Property<int> arrayIndexProperty;
		protected List<IProperty> outProperties = new List<IProperty>();
		protected List<Assets.Asset> loadedAssets = new List<Assets.Asset>();

        public DataSourceQueryEntity(FrostySdk.Ebx.DataSourceQueryEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
			SetFlags(EntityFlags.HasLogic);
			inDataProperty = new Property<object>(this, Property_InData);
			arrayIndexProperty = new Property<int>(this, Property_ArrayIndex);
		}

        public override IProperty GetProperty(int nameHash)
        {
			var property = properties.Find(p => p.NameHash == nameHash);
			if (property == null)
			{
				property = new Property<object>(this, nameHash);
				outProperties.Add(property);
			}
			return property;
        }

        public override void OnPropertyChanged(int propertyHash)
        {
			if (propertyHash == inDataProperty.NameHash || propertyHash == arrayIndexProperty.NameHash)
			{
				object dataValue = inDataProperty.Value;
				if (dataValue == null)
					return;

				Type dataType = dataValue.GetType();
				if (dataType.GetInterface("IList") != null)
				{
					IList listValue = (IList)dataValue;
					dataValue = listValue[arrayIndexProperty.Value];

					if (dataValue is FrostySdk.Ebx.PointerRef)
					{
						dataValue = ((FrostySdk.Ebx.PointerRef)dataValue).Internal;
					}

					dataType = dataValue.GetType();
				}

				var propertyInfos = dataType.GetProperties();
				foreach (var property in outProperties)
				{
					var pi = propertyInfos.FirstOrDefault(p => property.NameHash == Frosty.Hash.Fnv1.HashString(p.Name));
					if (pi != null)
					{
						object propValue = pi.GetValue(dataValue);
						if (pi.PropertyType == typeof(FrostySdk.Ebx.PointerRef))
						{
							var asset = LoadedAssetManager.Instance.LoadAsset<Assets.Asset>(this, (FrostySdk.Ebx.PointerRef)propValue);
							loadedAssets.Add(asset);
							propValue = asset;
						}
						property.Value = propValue;
					}
				}
				return;
			}

            base.OnPropertyChanged(propertyHash);
        }

        public override void Destroy()
        {
			foreach (var asset in loadedAssets)
			{
				LoadedAssetManager.Instance.UnloadAsset(asset);
			}
            base.Destroy();
        }
    }
}

