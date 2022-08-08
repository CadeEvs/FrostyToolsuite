using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MEPlatformSelectorEntityData))]
	public class MEPlatformSelectorEntity : LogicEntity, IEntityData<FrostySdk.Ebx.MEPlatformSelectorEntityData>
	{
		public new FrostySdk.Ebx.MEPlatformSelectorEntityData Data => data as FrostySdk.Ebx.MEPlatformSelectorEntityData;
		public override string DisplayName => "MEPlatformSelector";

		protected Property<bool> property_b31afc73;

		public MEPlatformSelectorEntity(FrostySdk.Ebx.MEPlatformSelectorEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
			SetFlags(EntityFlags.HasLogic);
			property_b31afc73 = new Property<bool>(this, -1290077069);
		}

        public override void BeginSimulation()
        {
            base.BeginSimulation();
			property_b31afc73.Value = true;
        }
    }
}

