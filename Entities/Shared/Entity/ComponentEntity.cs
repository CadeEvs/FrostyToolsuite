using Frosty.Core.Viewport;
using LevelEditorPlugin.Editors;
using System.Collections.Generic;
using FrostySdk.Ebx;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ComponentEntityData))]
	public class ComponentEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.ComponentEntityData>, IComponentEntity
	{
		public new FrostySdk.Ebx.ComponentEntityData Data => data as FrostySdk.Ebx.ComponentEntityData;
		public override bool RequiresTransformUpdate
		{
			get => requiresTransformUpdate;
			set
			{
				requiresTransformUpdate = value;
				foreach (Entity component in Components)
                {
					component.RequiresTransformUpdate = value;
                }
			}
		}
		public IEnumerable<Entity> Components => components;
		public override IEnumerable<ConnectionDesc> Events
		{
			get
			{
				List<ConnectionDesc> outEvents = new List<ConnectionDesc>();
				outEvents.AddRange(base.Events);
				outEvents.AddRange(new[]
				{ 
					new ConnectionDesc("Enable", Direction.In),
					new ConnectionDesc("Disable", Direction.In)
				});
				return outEvents;
			}
		}
		public override IEnumerable<ConnectionDesc> Properties
		{
			get
			{
				List<ConnectionDesc> outProperties = new List<ConnectionDesc>();
				outProperties.AddRange(base.Properties);
				outProperties.Add(new ConnectionDesc("Enabled", Direction.In));
				return outProperties;
			}
		}

		protected List<Entity> components = new List<Entity>();

		public ComponentEntity(FrostySdk.Ebx.ComponentEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}

		public override void CreateRenderProxy(List<Render.Proxies.RenderProxy> proxies, RenderCreateState state)
		{
			foreach (Entity component in Components)
			{
				component.CreateRenderProxy(proxies, state);
			}
		}

		public override void SetOwner(Entity newOwner)
		{
			base.SetOwner(newOwner);
			foreach (Entity entity in Components)
				entity.SetOwner(newOwner);
		}

		public override void SetVisibility(bool newVisibility)
		{
			if (newVisibility != isVisible)
			{
				isVisible = newVisibility;
				foreach (Entity entity in Components)
					entity.SetVisibility(newVisibility);
			}
		}

		public void SpawnComponents()
		{
			foreach (PointerRef objPointer in Data.Components)
			{
				if (objPointer.Type == FrostySdk.IO.PointerRefType.External)
				{
					// make sure the external asset is already loaded and add a ref to it
					//additionalAssets.Add(LoadedAssetManager.Instance.LoadAsset<Assets.Asset>(objPointer.External.FileGuid));
					System.Diagnostics.Debug.Assert(false);
				}

				GameObjectData objectData = objPointer.GetObjectAs<FrostySdk.Ebx.GameObjectData>();
				Entity component = CreateEntity(objectData);

				if (component != null)
				{
					components.Add(component);
					if (component is Component)
					{
						Component gameComponent = component as Component;
						gameComponent.SpawnComponents();
					}
				}
			}
		}

		public override void Destroy()
		{
			foreach (Entity component in components)
				component.Destroy();

			base.Destroy();
		}
	}
}

