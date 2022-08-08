using FrostySdk.Attributes;
using LevelEditorPlugin.Editors;
using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	public class UILootDataProviderMockData
	{
		[EbxFieldMeta(FrostySdk.IO.EbxFieldType.Int32)]
		public int CurrentWeight { get; set; }
		[EbxFieldMeta(FrostySdk.IO.EbxFieldType.Int32)]
		public int MaxWeight { get; set; }
		[EbxFieldMeta(FrostySdk.IO.EbxFieldType.Pointer, "ItemLootList")]
		public FrostySdk.Ebx.PointerRef LootList { get; set; }
		[EbxFieldMeta(FrostySdk.IO.EbxFieldType.Pointer, "EquippedItems")]
		public FrostySdk.Ebx.PointerRef Weapons { get; set; }
		[EbxFieldMeta(FrostySdk.IO.EbxFieldType.Pointer, "EquippedItems")]
		public FrostySdk.Ebx.PointerRef Consumables { get; set; }
	}

	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UILootDataProviderEntityData))]
	public class UILootDataProviderEntity : LogicEntity, IEntityData<FrostySdk.Ebx.UILootDataProviderEntityData>
	{
		protected readonly int Property_Weapons = Frosty.Hash.Fnv1.HashString("Weapons");
		protected readonly int Property_Consumables = Frosty.Hash.Fnv1.HashString("Consumables");
		protected readonly int Property_LootList = Frosty.Hash.Fnv1.HashString("LootList");

		public new FrostySdk.Ebx.UILootDataProviderEntityData Data => data as FrostySdk.Ebx.UILootDataProviderEntityData;
		public override string DisplayName => "UILootDataProvider";
        public override IEnumerable<ConnectionDesc> Properties
        {
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("ItemIndex", Direction.In, typeof(int)),
				new ConnectionDesc("EquipWeaponSlotIndex", Direction.In, typeof(int)),
				new ConnectionDesc("EquipConsumableSlotIndex", Direction.In, typeof(int)),
				new ConnectionDesc("Weapons", Direction.Out),
				new ConnectionDesc("Consumables", Direction.Out),
				new ConnectionDesc("LootList", Direction.Out)
			};
        }

        protected IProperty weaponsProperty;
		protected IProperty consumablesProperty;
		protected IProperty lootListProperty;
		protected Property<int> currentWeightProperty;
		protected Property<int> maxWeightProperty;

		protected Event<InputEvent> event_ae630fe0;

		public UILootDataProviderEntity(FrostySdk.Ebx.UILootDataProviderEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
			SetFlags(EntityFlags.HasLogic);
			mockDataObject = new UILootDataProviderMockData();

			currentWeightProperty = new Property<int>(this, -1397242580);
			maxWeightProperty = new Property<int>(this, -1523989015);
			weaponsProperty = new Property<object>(this, Property_Weapons);
			consumablesProperty = new Property<object>(this, Property_Consumables);
			lootListProperty = new Property<object>(this, Property_LootList);

			event_ae630fe0 = new Event<InputEvent>(this, -1369239584);
		}

        public override void OnEvent(int eventHash)
        {
			if (eventHash == event_ae630fe0.NameHash)
			{
				var dataProvider = mockDataObject as UILootDataProviderMockData;
				
				currentWeightProperty.Value = dataProvider.CurrentWeight;
				maxWeightProperty.Value = dataProvider.MaxWeight;
				weaponsProperty.Value = dataProvider.Weapons.GetObjectAs<object>();
				consumablesProperty.Value = dataProvider.Consumables.GetObjectAs<object>();
				lootListProperty.Value = dataProvider.LootList.GetObjectAs<object>();
				return;
			}

            base.OnEvent(eventHash);
        }
    }
}

