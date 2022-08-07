using LevelEditorPlugin.Editors;
using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.EffectEntityData))]
	public class EffectEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.EffectEntityData>, IComponentEntity
	{
		public new FrostySdk.Ebx.EffectEntityData Data => data as FrostySdk.Ebx.EffectEntityData;
		public IEnumerable<Entity> Components => components;

		protected List<Entity> components = new List<Entity>();

		public EffectEntity(FrostySdk.Ebx.EffectEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}

		public void SpawnComponents()
		{
            foreach (var objPointer in Data.Components)
            {
                if (objPointer.Type == FrostySdk.IO.PointerRefType.External)
                {
                    // make sure the external asset is already loaded and add a ref to it
                    //additionalAssets.Add(LoadedAssetManager.Instance.LoadAsset<Assets.Asset>(objPointer.External.FileGuid));
                    System.Diagnostics.Debug.Assert(false);
                }

                var objectData = objPointer.GetObjectAs<FrostySdk.Ebx.GameObjectData>();
                var component = CreateEntity(objectData);

                if (component != null)
                {
                    components.Add(component);
                    if (component is Component)
                    {
                        var gameComponent = component as Component;
                        gameComponent.SpawnComponents();
                    }
                }
            }
        }

        public override void Destroy()
        {
			foreach (var component in components)
			{
				component.Destroy();
			}
            base.Destroy();
        }
    }
}

