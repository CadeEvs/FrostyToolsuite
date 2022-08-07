using Frosty.Core;
using Frosty.Core.Viewport;
using LevelEditorPlugin.Editors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using LevelEditorPlugin.Render;
using LevelEditorPlugin.Render.Proxies;
using LevelEditorPlugin.Managers;
using System.IO;
using Frosty.Core.Managers;
using FrostySdk.Attributes;
using FrostySdk.IO;
using LevelEditorPlugin.Data;

namespace LevelEditorPlugin.Entities
{
    public class StaticModelGroupElementMoveEntityUndoUnit : IUndoUnit
    {
        public string Text => "Move Entities";

        private StaticModelGroupElementEntity entity;
        private Matrix originalTransform;

        public StaticModelGroupElementMoveEntityUndoUnit(StaticModelGroupElementEntity inEntity, Matrix inOriginalTransform)
        {
            entity = inEntity;
            originalTransform = inOriginalTransform;
        }

        public void Do()
        {
            (entity.Parent as StaticModelGroupEntity).UpdateData(entity);
        }

        public void Undo()
        {
            (entity.Parent as StaticModelGroupEntity).UndoData(entity);
            (entity as ISpatialEntity).SetTransform(originalTransform, suppressUpdate: true);
            entity.RequiresTransformUpdate = true;
        }
    }

    [EntityBinding(DataType = typeof(StaticModelGroupElementEntityData))]
    public class StaticModelGroupElementEntity : Entity, IEntityData<StaticModelGroupElementEntityData>, ISpatialEntity
    {
        public StaticModelGroupElementEntityData Data => data as StaticModelGroupElementEntityData;
        public override bool RequiresTransformUpdate 
        {
            get => base.RequiresTransformUpdate;
            set
            {
                entity.RequiresTransformUpdate = value;
            }
        }
        public override string DisplayName => Path.GetFileName(blueprint.Name);

        private Assets.ObjectBlueprint blueprint;
        private Entity entity;

        public StaticModelGroupElementEntity(StaticModelGroupElementEntityData inData, Entity inParent)
            : base(inData, inParent)
        {
            blueprint = LoadedAssetManager.Instance.LoadAsset<Assets.ObjectBlueprint>(new FrostySdk.IO.EbxImportReference() { FileGuid = Data.InternalBlueprint.External.FileGuid, ClassGuid = Guid.Empty });
            entity = CreateEntity(blueprint.Data.Object.GetObjectAs<FrostySdk.Ebx.GameObjectData>());

            // For visibility in the property grid, will show the actual blueprint as opposed to the entity data
            Data.Blueprint = new FrostySdk.Ebx.PointerRef(new EbxImportReference() { FileGuid = blueprint.FileGuid, ClassGuid = blueprint.InstanceGuid });
        }

        public virtual Matrix GetTransform()
        {
            Matrix m = Matrix.Identity;
            if (parent != null && parent is ISpatialEntity)
                m = (parent as ISpatialEntity).GetTransform();
            return SharpDXUtils.FromLinearTransform(Data.Transform) * m;
        }

        public Matrix GetLocalTransform()
        {
            return SharpDXUtils.FromLinearTransform(Data.Transform);
        }

        public void SetTransform(Matrix m, bool suppressUpdate)
        {
            if (suppressUpdate)
            {
                if (!UndoManager.Instance.IsUndoing && UndoManager.Instance.PendingUndoUnit == null)
                {
                    UndoManager.Instance.PendingUndoUnit = new StaticModelGroupElementMoveEntityUndoUnit(this, GetLocalTransform());
                }
            }
            else
            {
                UndoManager.Instance.CommitUndo(UndoManager.Instance.PendingUndoUnit);
            }

            Data.Transform = MakeLinearTransform(m);
            NotifyEntityModified("Transform");
        }

        public override void CreateRenderProxy(List<RenderProxy> proxies, RenderCreateState state)
        {
            entity.CreateRenderProxy(proxies, state);
        }

        public override void SetOwner(Entity newOwner)
        {
            base.SetOwner(newOwner);
            entity.SetOwner(newOwner);
        }

        public override void SetVisibility(bool newVisibility)
        {
            if (newVisibility != isVisible)
            {
                isVisible = newVisibility;
                entity.SetVisibility(newVisibility);
            }
        }

        public override void Destroy()
        {
            LoadedAssetManager.Instance.UnloadAsset(blueprint);
            entity.Destroy();
        }
    }

    [EntityBinding(DataType = typeof(FrostySdk.Ebx.StaticModelGroupEntityData))]
    public class StaticModelGroupEntity : Entity, IEntityData<FrostySdk.Ebx.StaticModelGroupEntityData>, ISpatialReferenceEntity, ILayerEntity
    {
        public FrostySdk.Ebx.StaticModelGroupEntityData Data => data as FrostySdk.Ebx.StaticModelGroupEntityData;

        private Resources.HavokPhysicsData physicsData;
        private List<Entity> entities = new List<Entity>();

        public StaticModelGroupEntity(FrostySdk.Ebx.StaticModelGroupEntityData inData, Entity inParent)
            : base(inData, inParent)
        {
            physicsData = GetPhysicsData(inData);

            int instCount = 0;
            int totalInstCount = 0;

            foreach (var member in inData.MemberDatas)
            {
                for (int i = 0; i < member.InstanceCount; i++)
                {
                    StaticModelGroupElementEntityData entityData = new StaticModelGroupElementEntityData();
                    entityData.InternalBlueprint = member.MemberType;

                    if (member.InstanceTransforms.Count > 0 || physicsData != null)
                    {
                        if (member.InstanceTransforms.Count > 0)
                        {
                            entityData.Transform = member.InstanceTransforms[i];
                            entityData.Index = totalInstCount;
                            entityData.HavokShapeType = "None";
                        }
                        else if (physicsData != null)
                        {
                            uint index = (uint)(member.PhysicsPartRange.First + i);

                            entityData.Transform = MakeLinearTransform(physicsData.GetTransform(instCount++));
                            entityData.Index = instCount - 1;
                            entityData.IsHavok = true;
                            entityData.HavokShapeType = physicsData.GetPhysicsShapeType(instCount - 1);
                        }

                        if (i < member.InstanceObjectVariation.Count)
                        {
                            var currentLayer = Parent;
                            entityData.ObjectVariationHash = member.InstanceObjectVariation[i];

                            if (entityData.ObjectVariationHash != 0)
                            {
                                while (!(currentLayer is SubWorldReferenceObject))
                                    currentLayer = currentLayer.Parent;

                                var meshVariatationDb = (currentLayer as SubWorldReferenceObject).MeshVariationDatabase;
                                if (meshVariatationDb != null)
                                {
                                    entityData.ObjectVariation = meshVariatationDb.GetVariation(entityData.ObjectVariationHash);
                                }
                            }
                            
                        }
                        if (i < member.InstanceRenderingOverrides.Count) entityData.RenderingOverrides = member.InstanceRenderingOverrides[i];
                        if (i < member.InstanceRadiosityTypeOverride.Count) entityData.RadiosityTypeOverride = member.InstanceRadiosityTypeOverride[i];
                        if (i < member.InstanceTerrainShaderNodesEnable.Count) entityData.TerrainShaderNodesEnable = member.InstanceTerrainShaderNodesEnable[i];

                        totalInstCount++;
                    }

                    entities.Add(CreateEntity(entityData));
                }
            }
        }

        public virtual Matrix GetTransform()
        {
            Matrix m = Matrix.Identity;
            if (parent != null && parent is ISpatialEntity)
                m = (parent as ISpatialEntity).GetTransform();
            return m;
        }

        public Matrix GetLocalTransform()
        {
            return Matrix.Identity;
        }

        public void SetTransform(Matrix m, bool suppressUpdate)
        {
            // do nothing
        }

        public void UpdateData(StaticModelGroupElementEntity entity)
        {
            if (entity.Data.IsHavok)
            {
                physicsData.UpdateData(entity.Data.Index, entity.GetLocalTransform());
            }
            else
            {
                int instanceIndex = 0;
                bool ebxNeedsUpdating = false;

                foreach (var member in Data.MemberDatas)
                {
                    for (int i = 0; i < member.InstanceCount; i++)
                    {
                        if (member.InstanceTransforms.Count > 0)
                        {
                            if (member.InstanceTransforms.Count > 0)
                            {
                                if (instanceIndex == entity.Data.Index)
                                {
                                    member.InstanceTransforms[i] = MakeLinearTransform(entity.GetLocalTransform());
                                    ebxNeedsUpdating = true;
                                }
                            }
                        }

                        instanceIndex++;
                    }
                }

                if (ebxNeedsUpdating)
                {
                    LoadedAssetManager.Instance.UpdateAsset((Owner.Parent as ReferenceObject).Blueprint);
                }
            }
        }

        public void UndoData(StaticModelGroupElementEntity entity)
        {
            if (entity.Data.IsHavok)
            {
                physicsData.UndoData(entity.Data.Index, entity.GetLocalTransform());
            }
            else
            {
                int instanceIndex = 0;
                bool ebxNeedsUpdating = false;

                foreach (var member in Data.MemberDatas)
                {
                    for (int i = 0; i < member.InstanceCount; i++)
                    {
                        if (member.InstanceTransforms.Count > 0)
                        {
                            if (member.InstanceTransforms.Count > 0)
                            {
                                if (instanceIndex == entity.Data.Index)
                                {
                                    member.InstanceTransforms[i] = MakeLinearTransform(entity.GetLocalTransform());
                                    ebxNeedsUpdating = true;
                                }
                            }
                        }

                        instanceIndex++;
                    }
                }

                if (ebxNeedsUpdating)
                {
                    LoadedAssetManager.Instance.UndoUpdate((Owner.Parent as ReferenceObject).Blueprint);
                }
            }
        }

        public Layers.SceneLayer GetLayer()
        {
            Layers.SceneLayer layer = new Layers.SceneLayer(this, "static_instances", new SharpDX.Color4(0.0f, 0.0f, 0.5f, 1.0f));
            foreach (var entity in entities)
            {
                layer.AddEntity(entity);
                entity.SetOwner(entity);
            }

            return layer;
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

        public override void CreateRenderProxy(List<RenderProxy> proxies, RenderCreateState state)
        {
            foreach (var entity in entities)
                entity.CreateRenderProxy(proxies, state);
        }

        public override void Destroy()
        {
            foreach (var entity in entities)
                entity.Destroy();
        }

        private Resources.HavokPhysicsData GetPhysicsData(FrostySdk.Ebx.StaticModelGroupEntityData inData)
        {
            foreach (var component in inData.Components)
            {
                FrostySdk.Ebx.GameObjectData gameObjectData = component.GetObjectAs<FrostySdk.Ebx.GameObjectData>();
                if (gameObjectData is FrostySdk.Ebx.StaticModelGroupPhysicsComponentData)
                {
                    FrostySdk.Ebx.StaticModelGroupPhysicsComponentData physicsComponentData = (FrostySdk.Ebx.StaticModelGroupPhysicsComponentData)gameObjectData;
                    foreach (var body in physicsComponentData.PhysicsBodies)
                    {
                        FrostySdk.Ebx.GroupRigidBodyData bodyData = body.GetObjectAs<FrostySdk.Ebx.GroupRigidBodyData>();
                        FrostySdk.Ebx.GroupHavokAsset havokAsset = bodyData.Asset.GetObjectAs<FrostySdk.Ebx.GroupHavokAsset>();

                        if (havokAsset != null)
                        {
                            return App.AssetManager.GetResAs<Resources.HavokPhysicsData>(App.AssetManager.GetResEntry(havokAsset.Resource));
                        }
                    }
                }
            }

            return null;
        }
    }
}
