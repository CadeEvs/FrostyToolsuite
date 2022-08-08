using Frosty.Core.Viewport;
using LevelEditorPlugin.Editors;
using LevelEditorPlugin.Managers;
using LevelEditorPlugin.Render.Proxies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
#if MASS_EFFECT
    [EntityBinding(DataType = typeof(FrostySdk.Ebx.MESoldierEntityData))]
    public class MESoldierEntity : SoldierEntity, IEntityData<FrostySdk.Ebx.MESoldierEntityData>
    {
        public new FrostySdk.Ebx.MESoldierEntityData Data => data as FrostySdk.Ebx.MESoldierEntityData;
        public override bool RequiresTransformUpdate 
        { 
            get => base.RequiresTransformUpdate; 
            set   
            {
                foreach (var entity in entities)
                    entity.RequiresTransformUpdate = value;
            }
        }

        private List<Assets.AppearancePreset> presets = new List<Assets.AppearancePreset>();
        private List<Entity> entities = new List<Entity>();
        private List<Assets.CommonAppearanceItem> items = new List<Assets.CommonAppearanceItem>();
        private Assets.GenericAsset<FrostySdk.Ebx.AssetTemplate> assetTemplate;

        public MESoldierEntity(FrostySdk.Ebx.MESoldierEntityData inData, Entity inParent)
            : base(inData, inParent)
        {
            var characterSpawnEntity = inParent as CharacterSpawnReferenceObject;
            if (characterSpawnEntity != null)
            {
                assetTemplate = LoadedAssetManager.Instance.LoadAsset<Assets.GenericAsset<FrostySdk.Ebx.AssetTemplate>>(characterSpawnEntity.Blueprint, characterSpawnEntity.Data.Template);
                if (assetTemplate != null)
                {
                    SpawnFromTemplate(assetTemplate.Data);
                }
            }

            if (presets.Count == 0)
            {
                SpawnFromComponents(Data.Components);
            }
        }

        public override void CreateRenderProxy(List<RenderProxy> proxies, RenderCreateState state)
        {
            foreach (var entity in entities)
            {
                entity.CreateRenderProxy(proxies, state);
            }
        }

        private void SpawnFromComponents(List<FrostySdk.Ebx.PointerRef> componentList)
        {
            foreach (var componentRef in componentList)
            {
                var component = componentRef.GetObjectAs<FrostySdk.Ebx.GameObjectData>();
                if (component is FrostySdk.Ebx.SoldierBodyComponentData)
                {
                    var soldierBodyComponent = component as FrostySdk.Ebx.SoldierBodyComponentData;
                    SpawnFromComponents(soldierBodyComponent.Components);
                    break;
                }
                else if (component is FrostySdk.Ebx.CharacterMeshComponentData)
                {
                    var characterMeshComponent = component as FrostySdk.Ebx.CharacterMeshComponentData;
                    entities.Add(CreateEntity(new FrostySdk.Ebx.MeshProxyEntityData() { Mesh = characterMeshComponent.Mesh }));
                }
                else if (component is FrostySdk.Ebx.MEAppearanceComponentData)
                {
                    var appearanceComponent = component as FrostySdk.Ebx.MEAppearanceComponentData;
                    foreach (var itemRef in appearanceComponent.DefaultItems)
                    {
                        if (itemRef.Type != FrostySdk.IO.PointerRefType.Null)
                        {
                            var appearanceItem = LoadedAssetManager.Instance.LoadAsset<Assets.CommonAppearanceItem>(this, itemRef);
                            if (appearanceItem != null)
                            {
                                items.Add(appearanceItem);

                                var entity = CreateEntity(appearanceItem.GenerateEntityData());
                                if (entity != null)
                                    entities.Add(entity);
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine(itemRef.External.FileGuid.ToString());
                            }
                        }
                    }
                }
                else if (component is FrostySdk.Ebx.AssignAppearancePresetEntityData)
                {
                    var assignAppearancePreset = component as FrostySdk.Ebx.AssignAppearancePresetEntityData;
                    var assetPreset = LoadedAssetManager.Instance.LoadAsset<Assets.AppearancePreset>(this, assignAppearancePreset.Preset);

                    if (assetPreset != null)
                    {
                        SpawnFromPreset(assetPreset);
                    }
                    break;
                }
            }
        }

        private void SpawnFromTemplate(FrostySdk.Ebx.AssetTemplate template)
        {
            foreach (var templateComponent in template.Components)
            {
                var preset = templateComponent.Template.GetObjectAs<FrostySdk.Ebx.Template>();
                if (preset is FrostySdk.Ebx.AssignAppearancePresetEntityDataTemplate)
                {
                    var presetTemplate = preset as FrostySdk.Ebx.AssignAppearancePresetEntityDataTemplate;
                    var assetPreset = LoadedAssetManager.Instance.LoadAsset<Assets.AppearancePreset>(this, presetTemplate.Preset);

                    if (assetPreset != null)
                    {
                        SpawnFromPreset(assetPreset);
                    }
                }
            }
        }

        private void SpawnFromPreset(Assets.AppearancePreset assetPreset)
        {
            presets.Add(assetPreset);
            foreach (var item in assetPreset.Items)
            {
                var entity = CreateEntity(item.GenerateEntityData());
                if (entity != null)
                    entities.Add(entity);
            }
        }

        public override void SetOwner(Entity newOwner)
        {
            base.SetOwner(newOwner);
            foreach (var entity in entities)
                entity.SetOwner(newOwner);
        }

        public override void SetVisibility(bool newVisibility)
        {
            if (newVisibility != isVisible)
            {
                isVisible = newVisibility;
                foreach (var entity in entities)
                    entity.SetVisibility(newVisibility);
            }
        }

        public override void Destroy()
        {
            foreach (var entity in entities)
                entity.Destroy();

            foreach (var item in items)
                LoadedAssetManager.Instance.UnloadAsset(item);

            foreach (var preset in presets)
                LoadedAssetManager.Instance.UnloadAsset(preset);

            LoadedAssetManager.Instance.UnloadAsset(assetTemplate);
        }
    }
#endif
}
