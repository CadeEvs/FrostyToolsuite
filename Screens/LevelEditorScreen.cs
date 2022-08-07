using Frosty.Core.Screens;
using Frosty.Core.Viewport;
using Frosty.Core.Viewport.DXUT;
using LevelEditorPlugin.Entities;
using LevelEditorPlugin.Managers;
using LevelEditorPlugin.Render;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using D3D11 = SharpDX.Direct3D11;
using FrostySdk;
using LevelEditorPlugin.Render.Proxies;
using Frosty.Core.Windows;
using System.Windows;
using System.Runtime.InteropServices;
using System.IO;
using LevelEditorPlugin.Library.Image;

namespace LevelEditorPlugin.Screens
{
    public delegate void RenderAction(RenderCreateState state);

    public class LevelEditorCamera : FirstPersonCamera
    {
        public float FOV => fov;
        public float AspectRatio => aspectRatio;

        public Matrix WorldMatrix
        {
            get
            {
                return Matrix.RotationYawPitchRoll(-cameraYawAngle, cameraPitchAngle, 0) * Matrix.Translation(cameraWorldMatrix.TranslationVector * new Vector3(-1, 1, 1));
            }
        }

        public LevelEditorCamera()
        {
            rotationScaler = 0.015f;
            moveScaler = 10.0f;
        }
    }

    public class GizmoRenderProxy : MeshRenderBase
    {
        public Matrix Transform { get; set; }
        public float Scale { get; protected set; }

        private ObjRenderable renderData;

        private ShaderPermutation permutation;
        private D3D11.Buffer pixelParameters;
        private List<D3D11.ShaderResourceView> pixelTextures = new List<D3D11.ShaderResourceView>();
        private Matrix originalTransform;

        private List<ShaderParameter> materialParameters = new List<ShaderParameter>();
        private List<ShaderParameter> materialTextures = new List<ShaderParameter>();

        private static GeometryDeclarationDesc GeometryDecl = GeometryDeclarationDesc.Create(new GeometryDeclarationDesc.Element[]
        {
            new GeometryDeclarationDesc.Element
            {
                Usage = VertexElementUsage.Pos,
                Format = VertexElementFormat.Float3
            },
            new GeometryDeclarationDesc.Element
            {
                Usage = VertexElementUsage.Normal,
                Format = VertexElementFormat.Float3
            },
            new GeometryDeclarationDesc.Element
            {
                Usage = VertexElementUsage.TexCoord0,
                Format = VertexElementFormat.Float2
            }
        });

        public GizmoRenderProxy(RenderCreateState state, ObjRenderable meshData)
        {
            renderData = meshData;
            Transform = Matrix.Identity;
            originalTransform = Transform;

            CreateMaterial(state);
        }

        public override void Render(D3D11.DeviceContext context, MeshRenderPath renderPath)
        {
            Matrix cameraTransform = Screens.LevelEditorScreen.EditorCamera.WorldMatrix;
            
            float distToCamera = (Transform.TranslationVector - cameraTransform.TranslationVector).Length();
            Scale = Math.Max(distToCamera / 5.0f, 0.125f);

            Transform = Matrix.Scaling(Scale) * Matrix.Translation(Transform.TranslationVector);

            if (renderPath == MeshRenderPath.Forward)
            {
                permutation.SetState(context, renderPath);
                context.PixelShader.SetShaderResources(1, pixelTextures.ToArray());
                context.PixelShader.SetConstantBuffer(2, pixelParameters);

                renderData.Render(context, renderPath);
            }
        }

        public virtual MeshRenderInstance GetInstance(float distToCamera)
        {
            return new MeshRenderInstance() { RenderMesh = this, Transform = Transform };
        }

        public void UpdateSelectIndex(RenderCreateState state, int newSelectIndex)
        {
            materialParameters.Clear();
            materialParameters.Add(new ShaderParameter("SelectIndex", ShaderParameterType.Float4, new[] { (float)newSelectIndex, 0, 0, 0 }));
            permutation.AssignParameters(state, materialParameters, materialTextures, ref pixelParameters, ref pixelTextures);
        }

        private void CreateMaterial(RenderCreateState state)
        {
            materialTextures.Add(new ShaderParameter("Texture", ShaderParameterType.Tex2d, $"Resources/Textures/TranslateGizmo.dds"));
            materialTextures.Add(new ShaderParameter("SelectMask", ShaderParameterType.Tex2d, $"Resources/Textures/TranslateGizmoSelected.dds"));

            permutation = state.ShaderLibrary.GetUserShader("GizmoShader", GeometryDecl);
            permutation.IsTwoSided = true;
            permutation.LoadShaders(state.Device);
            permutation.AssignParameters(state, materialParameters, materialTextures, ref pixelParameters, ref pixelTextures);
        }

        public void Dispose()
        {
            renderData.Dispose();
            permutation.Dispose();
            pixelParameters?.Dispose();
            pixelTextures.Clear();
        }
    }

    public class Gizmo
    {
        public bool IsVisible { get; set; }
        public Matrix Transform
        {
            get => renderProxy.Transform;
            set 
            {
                if (renderProxy.Transform != value)
                {
                    renderProxy.Transform = value;
                }
            }
        }
        public BoundingBox BoundingBox { get; protected set; }

        protected ObjRenderable meshData;
        protected string meshFilename;

        protected GizmoRenderProxy renderProxy;

        public Gizmo()
        {
            IsVisible = false;
        }

        public void CreateRenderProxy(RenderCreateState state)
        {
            meshData = LoadedMeshManager.Instance.LoadMesh(state, "TranslateGizmo");
            renderProxy = new GizmoRenderProxy(state, meshData);
        }

        public MeshRenderInstance GetInstance()
        {
            return renderProxy.GetInstance(0.0f);
        }

        public virtual bool HitTest(Ray hitTestRay)
        {
            return IsVisible && hitTestRay.Intersects(BoundingBox);
        }

        public virtual void Update(RenderCreateState state)
        {
        }

        public virtual bool MouseDown(int x, int y, Ray rayCast, MouseButton button)
        {
            return false;
        }

        public virtual bool MouseMove(int x, int y, Ray rayCast)
        {
            return true;
        }

        public virtual bool MouseUp(int x, int y, Ray rayCast, MouseButton button)
        {
            return false;
        }

        public void Dispose()
        {
            renderProxy.Dispose();
            LoadedMeshManager.Instance.UnloadMesh(meshData);
        }
    }

    public class TranslateGizmo : Gizmo
    {
        public bool IsMoving => moved;

        private BoundingBox[] axisBoundingBoxes = new BoundingBox[3]
        {
            new BoundingBox(Vector3.Zero, new Vector3(0.25f, 0.25f, 1.0f)),
            new BoundingBox(Vector3.Zero, new Vector3(0.25f, 1.0f, 0.25f)),
            new BoundingBox(Vector3.Zero, new Vector3(1.0f, 0.25f, 0.25f)),
        };
        private int selectedAxis = -1;
        private bool leftMouseDown;
        private bool moved;

        private float offset;

        public TranslateGizmo()
        {
            meshFilename = "TranslateGizmo";
        }

        public override void Update(RenderCreateState state)
        {
            if (!IsVisible)
                return;

            BoundingBox = BoundingBox.FromSphere(new BoundingSphere(Transform.TranslationVector, renderProxy.Scale * 0.5f));

            float smallAxisAmount = 0.0625f * renderProxy.Scale;
            float largeAxisAmount = 0.5f * renderProxy.Scale;
            Vector3 translation = Transform.TranslationVector;

            axisBoundingBoxes[0] = new BoundingBox(new Vector3(-smallAxisAmount + translation.X, -smallAxisAmount + translation.Y, translation.Z), new Vector3(smallAxisAmount + translation.X, smallAxisAmount + translation.Y, largeAxisAmount + translation.Z));
            axisBoundingBoxes[1] = new BoundingBox(new Vector3(-smallAxisAmount + translation.X, translation.Y, -smallAxisAmount + translation.Z), new Vector3(smallAxisAmount + translation.X, largeAxisAmount + translation.Y, smallAxisAmount + translation.Z));
            axisBoundingBoxes[2] = new BoundingBox(new Vector3(-largeAxisAmount + translation.X, -smallAxisAmount + translation.Y, -smallAxisAmount + translation.Z), new Vector3(translation.X, smallAxisAmount + translation.Y, smallAxisAmount + translation.Z));

            renderProxy.UpdateSelectIndex(state, selectedAxis);
        }

        public override bool HitTest(Ray hitTestRay)
        {
            if (leftMouseDown)
                return true;

            if (base.HitTest(hitTestRay))
            {
                selectedAxis = -1;
                if (hitTestRay.Intersects(axisBoundingBoxes[0])) { selectedAxis = 0; }
                else if (hitTestRay.Intersects(axisBoundingBoxes[1])) { selectedAxis = 1; }
                else if (hitTestRay.Intersects(axisBoundingBoxes[2])) { selectedAxis = 2; }

                if (selectedAxis != -1)
                {
                    System.Windows.Input.Mouse.OverrideCursor = System.Windows.Input.Cursors.SizeAll;
                    return true;
                }
                else
                {
                    System.Windows.Input.Mouse.OverrideCursor = null;
                }
            }

            return false;
        }

        public override bool MouseDown(int x, int y, Ray rayCast, MouseButton button)
        {
            if (button == MouseButton.Left && selectedAxis != -1)
            {
                leftMouseDown = true;

                if (selectedAxis == 0)
                {
                    Vector3 outPosition;
                    Plane axisPlane = new Plane(Transform.TranslationVector, Vector3.UnitX);
                    rayCast.Intersects(ref axisPlane, out outPosition);
                    offset = Transform.TranslationVector.Z - outPosition.Z;
                }
                else if (selectedAxis == 1)
                {
                    Vector3 outPosition;
                    Plane axisPlane = new Plane(Transform.TranslationVector, Vector3.UnitZ);
                    rayCast.Intersects(ref axisPlane, out outPosition);
                    offset = Transform.TranslationVector.Y - outPosition.Y;
                }
                else if (selectedAxis == 2)
                {
                    Vector3 outPosition;
                    Plane axisPlane = new Plane(Transform.TranslationVector, Vector3.UnitY);
                    rayCast.Intersects(ref axisPlane, out outPosition);
                    offset = Transform.TranslationVector.X - outPosition.X;
                }

                return true;
            }

            return false;
        }

        public override bool MouseMove(int x, int y, Ray rayCast)
        {
            if (leftMouseDown)
            {
                moved = true;
                if (selectedAxis == 0)
                {
                    Matrix m = Transform;

                    Vector3 outPosition;
                    Plane axisPlane = new Plane(m.TranslationVector, Vector3.UnitX);
                    rayCast.Intersects(ref axisPlane, out outPosition);
                    
                    m.TranslationVector = new Vector3(Transform.TranslationVector.X, Transform.TranslationVector.Y, outPosition.Z + offset);
                    Transform = m;
                }
                else if (selectedAxis == 1)
                {
                    Matrix m = Transform;

                    Vector3 outPosition;
                    Plane axisPlane = new Plane(m.TranslationVector, Vector3.UnitZ);
                    rayCast.Intersects(ref axisPlane, out outPosition);

                    m.TranslationVector = new Vector3(Transform.TranslationVector.X, outPosition.Y + offset, Transform.TranslationVector.Z);
                    Transform = m;
                }
                else if (selectedAxis == 2)
                {
                    Matrix m = Transform;

                    Vector3 outPosition;
                    Plane axisPlane = new Plane(m.TranslationVector, Vector3.UnitY);
                    rayCast.Intersects(ref axisPlane, out outPosition);

                    m.TranslationVector = new Vector3(outPosition.X + offset, Transform.TranslationVector.Y, Transform.TranslationVector.Z);
                    Transform = m;
                }

                return true;
            }

            return false;
        }

        public override bool MouseUp(int x, int y, Ray rayCast, MouseButton button)
        {
            if (leftMouseDown)
            {
                bool gizmoMoved = moved;

                moved = false;
                leftMouseDown = false;
                selectedAxis = -1;
                System.Windows.Input.Mouse.OverrideCursor = null;

                return gizmoMoved;
            }

            return false;
        }
    }

    public class SelectedEntityChangedEventArgs : EventArgs
    {
        public Entity Entity { get; private set; }

        public SelectedEntityChangedEventArgs(Entity entity)
        {
            Entity = entity;
        }
    }

    public class LevelEditorScreen : DeferredRenderScreen2
    {
        public bool ShowTaskWindow { get; set; }

        // @hack: so that entities have access to the camera. Primarily for sprites
        public static LevelEditorCamera EditorCamera { get; private set; }

        private List<MeshRenderInstance> sprites = new List<MeshRenderInstance>();

        private Queue<RenderAction> renderTasks = new Queue<RenderAction>();
        private List<RenderProxy> proxies = new List<RenderProxy>();

        private TranslateGizmo translateGizmo;
        private BindableDepthTexture gizmoDepthTexture;

        public event EventHandler<SelectedEntityChangedEventArgs> SelectedEntityChanged;

        public LevelEditorScreen(bool groundVisibleByDefault = false)
        {
            GroundVisible = groundVisibleByDefault;
            translateGizmo = new TranslateGizmo();

            renderTasks.Enqueue(state => 
            {
                translateGizmo.CreateRenderProxy(state);

            });
        }

        public void AddEntity(Entity entity)
        {
            renderTasks.Enqueue((RenderCreateState state) =>
            {
                List<RenderProxy> newProxies = new List<RenderProxy>();
                entity.CreateRenderProxy(newProxies, state);
                proxies.AddRange(newProxies);
            });
        }

        public override void Update(double timestep)
        {
            base.Update(timestep);

            translateGizmo.Update(RenderCreateState);
            if (!translateGizmo.IsMoving)
            {
                if (selectedProxies.Count > 0 && selectedProxies[0].OwnerEntity.RequiresTransformUpdate)
                {
                    translateGizmo.Transform = Matrix.Translation((selectedProxies[0].OwnerEntity as ISpatialEntity).GetTransform().TranslationVector);
                }
            }

            foreach (RenderProxy proxy in proxies)
                proxy.Update(RenderCreateState);
        }

        public void CaptureThumbnail(string path)
        {
            Render();

            {
                D3D11.Texture2D texture = Viewport.ColorBuffer;
                D3D11.Texture2DDescription description = texture.Description;

                D3D11.Texture2D textureCopy = new D3D11.Texture2D(Viewport.Device, new D3D11.Texture2DDescription
                {
                    Width = (int)description.Width,
                    Height = (int)description.Height,
                    MipLevels = 1,
                    ArraySize = 1,
                    Format = description.Format,
                    Usage = D3D11.ResourceUsage.Staging,
                    SampleDescription = new SharpDX.DXGI.SampleDescription(1, 0),
                    BindFlags = D3D11.BindFlags.None,
                    CpuAccessFlags = D3D11.CpuAccessFlags.Read,
                    OptionFlags = D3D11.ResourceOptionFlags.None
                });
                Viewport.Context.CopyResource(texture, textureCopy);

                DataStream dataStream;
                DataBox dataBox = Viewport.Context.MapSubresource(
                    textureCopy,
                    0,
                    0,
                    D3D11.MapMode.Read,
                    D3D11.MapFlags.None,
                    out dataStream);

                byte[] buffer = new byte[description.Width * description.Height * 4];
                for (int i = 0; i < description.Height; i++)
                {
                    dataStream.Read(buffer, i * description.Width * 4, description.Width * 4);
                    dataStream.Position += dataBox.RowPitch - (description.Width * 4);
                }
                for (int i = 3; i < buffer.Length; i += 4)
                {
                    buffer[i] = 0xFF;
                }

                ImageUtils.SaveToPNG(description.Width, description.Height, buffer, new FileStream(path, FileMode.Create));

                Viewport.Context.UnmapSubresource(textureCopy, 0);
                textureCopy.Dispose();
            }
        }

        public override void Render()
        {
            if (renderTasks.Count > 0)
            {
                if (ShowTaskWindow)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        FrostyTaskWindow.Show("Preparing scene", "", (task) =>
                        {
                            ProcessRenderTasks();
                            ShowTaskWindow = false;
                        });
                    });
                }
                else
                {
                    ProcessRenderTasks();
                }
            }

            gizmoDepthTexture.Clear(Viewport.Context, true, true, 1.0f, 0);
            base.Render();
        }

        public override void CreateSizeDependentBuffers()
        {
            base.CreateSizeDependentBuffers();

            D3D11.Texture2DDescription description = new D3D11.Texture2DDescription
            {
                ArraySize = 1,
                Format = SharpDX.DXGI.Format.R24G8_Typeless,
                Width = Viewport.ViewportWidth,
                Height = Viewport.ViewportHeight,
                MipLevels = 1,
                SampleDescription = new SharpDX.DXGI.SampleDescription(1, 0),
                Usage = D3D11.ResourceUsage.Default
            };
            D3D11.DepthStencilViewDescription dsViewDesc = new D3D11.DepthStencilViewDescription
            {
                Dimension = D3D11.DepthStencilViewDimension.Texture2D,
                Format = SharpDX.DXGI.Format.D24_UNorm_S8_UInt,
                Texture2D = default(D3D11.DepthStencilViewDescription.Texture2DResource)
            };
            gizmoDepthTexture = new BindableDepthTexture(Viewport.Device, description, false, dsViewDesc);
        }

        public override void DisposeSizeDependentBuffers()
        {
            base.DisposeSizeDependentBuffers();
            gizmoDepthTexture.Dispose();
        }

        public override void DisposeBuffers()
        {
            base.DisposeBuffers();

            translateGizmo.Dispose();

            foreach (RenderProxy proxy in proxies)
                proxy.Dispose();

            proxies.Clear();
            selectedProxies.Clear();
        }

        public override void CreateBuffers()
        {
            if (camera == null)
            {
                camera = new LevelEditorCamera();
                camera.SetViewParams(new Vector3(0, 0, 0), new Vector3(0, 0, 0));

                SetCamera();
            }

            base.CreateBuffers();

            ShadowsEnabled = false;
            HBAOEnabled = false;
        }

        // @hack: so that entities have access to the camera. Primarily for sprites
        public void SetCamera()
        {
            EditorCamera = camera as LevelEditorCamera;
        }

        List<RenderProxy> selectedProxies = new List<RenderProxy>();

        public void ClearSelection()
        {
            translateGizmo.IsVisible = false;
            renderTasks.Enqueue((state) =>
            {
                foreach (RenderProxy proxy in selectedProxies)
                    proxy.SetSelected(state, false);
                selectedProxies.Clear();
            });
        }

        public void SelectEntity(Entity entity)
        {
            ClearSelection();

            IEnumerable<RenderProxy> foundProxies = proxies.Where(rp => rp.OwnerEntity.Owner == entity);
            if (foundProxies.Count() == 0)
                return;

            translateGizmo.Transform = Matrix.Translation((entity as ISpatialEntity).GetTransform().TranslationVector);
            translateGizmo.IsVisible = true;

            renderTasks.Enqueue((state) =>
            {
                selectedProxies.AddRange(foundProxies);
                foreach (RenderProxy proxy in selectedProxies)
                    proxy.SetSelected(state, true);
            });
        }

        public void CenterOnSelection()
        {
            if (selectedProxies.Count == 0)
                return;

            BoundingBox totalBbox = new BoundingBox();
            int index = 0;

            foreach (RenderProxy proxy in selectedProxies)
            {
                if (index == 0) totalBbox = proxy.BoundingBox;
                else totalBbox = BoundingBox.Merge(totalBbox, proxy.BoundingBox);
                index++;
            }

            Vector3 center = totalBbox.Minimum + (totalBbox.Maximum - totalBbox.Minimum) * 0.5f;
            Vector3 offset = center + new Vector3(0, Math.Abs(totalBbox.Maximum.Y - totalBbox.Minimum.Y) / 1.25f, (totalBbox.Maximum - totalBbox.Minimum).Length() * 0.9f);

            (camera as FirstPersonCamera).SetViewParams(offset * new Vector3(-1, 1, 1), center * new Vector3(-1, 1, 1));
        }

        public override List<MeshRenderInstance> CollectMeshInstances()
        {
            LevelEditorCamera currentCamera = (LevelEditorCamera)camera;
            Matrix worldMatrix = currentCamera.WorldMatrix;

            BoundingFrustum cameraFrustum = BoundingFrustum.FromCamera(worldMatrix.TranslationVector, worldMatrix.Backward, worldMatrix.Down, currentCamera.FOV, currentCamera.GetNearClip(), currentCamera.GetFarClip(), currentCamera.AspectRatio);
            List<MeshRenderInstance> instancesInView = new List<MeshRenderInstance>();
            
            sprites.Clear();
            foreach (RenderProxy proxy in proxies)
            {
                float dist = (proxy.Transform.TranslationVector - worldMatrix.TranslationVector).Length();
                float width = cameraFrustum.GetWidthAtDepth(dist);
                float height = cameraFrustum.GetHeightAtDepth(dist);

                BoundingSphere bSphere = BoundingSphere.FromBox(proxy.BoundingBox);
                float wratio =  (bSphere.Radius * 2) / width;
                float hratio = (bSphere.Radius * 2) / height;
                float screenSize = (wratio > hratio) ? wratio : hratio;

                if (proxy.ShouldRender(dist, screenSize))
                {
                    ContainmentType containmentType = cameraFrustum.Contains(proxy.BoundingBox);
                    if (containmentType == ContainmentType.Contains || containmentType == ContainmentType.Intersects)
                    {
                        proxy.CurrentDistanceToCamera = dist;

                        if (proxy is SpriteRenderProxy) sprites.Add(proxy.GetInstance(dist));
                        else instancesInView.Add(proxy.GetInstance(dist));
                    }
                }
            }

            return instancesInView;
        }

        protected List<MeshRenderInstance> GetSortedVisibleList()
        {
            LevelEditorCamera currentCamera = (LevelEditorCamera)camera;
            Matrix worldMatrix = currentCamera.WorldMatrix;

            BoundingFrustum cameraFrustum = BoundingFrustum.FromCamera(worldMatrix.TranslationVector, worldMatrix.Backward, worldMatrix.Down, currentCamera.FOV, currentCamera.GetNearClip(), currentCamera.GetFarClip(), currentCamera.AspectRatio);
            List<MeshRenderInstance> instancesInView = new List<MeshRenderInstance>();

            foreach (RenderProxy proxy in proxies)
            {
                float dist = (proxy.Transform.TranslationVector - worldMatrix.TranslationVector).Length();
                float width = cameraFrustum.GetWidthAtDepth(dist);
                float height = cameraFrustum.GetHeightAtDepth(dist);

                BoundingSphere bSphere = BoundingSphere.FromBox(proxy.BoundingBox);
                float wratio = (bSphere.Radius * 2) / width;
                float hratio = (bSphere.Radius * 2) / height;
                float screenSize = (wratio > hratio) ? wratio : hratio;

                if (proxy.ShouldRender(dist, screenSize))
                {
                    ContainmentType containmentType = cameraFrustum.Contains(proxy.BoundingBox);
                    if (containmentType == ContainmentType.Contains || containmentType == ContainmentType.Intersects)
                    {
                        proxy.CurrentDistanceToCamera = dist;
                        instancesInView.Add(proxy.GetInstance(dist));
                    }
                }
            }

            instancesInView.Sort((a, b) =>
            {
                return (a.RenderMesh as RenderProxy).CurrentDistanceToCamera.CompareTo((b.RenderMesh as RenderProxy).CurrentDistanceToCamera);
            });

            return instancesInView;
        }

        private struct MouseButtonData
        {
            public int X;
            public int Y;
            public MouseButton Button;
            public bool MouseDown;
        }
        private MouseButtonData mouseButtonData;

        public override void MouseDown(int x, int y, MouseButton button)
        {
            if (translateGizmo.MouseDown(x, y, GetHitTestRay(x, y), button))
            {
                return;
            }

            if (button == MouseButton.Left)
            {
                if (!mouseButtonData.MouseDown)
                {
                    mouseButtonData = new MouseButtonData() { X = x, Y = y, Button = button, MouseDown = true };
                }
            }

            base.MouseDown(x, y, button);
        }

        public override void MouseUp(int x, int y, MouseButton button)
        {
            if (translateGizmo.MouseUp(x, y, GetHitTestRay(x, y), button))
            {
                Vector3 scale;
                Vector3 translation;
                Quaternion rotation;

                Matrix entityTransform = (selectedProxies[0].OwnerEntity.Owner as ISpatialEntity).GetLocalTransform();
                entityTransform.Decompose(out scale, out rotation, out translation);

                Matrix m = Matrix.Scaling(scale) * Matrix.RotationQuaternion(rotation) * Matrix.Translation(translateGizmo.Transform.TranslationVector);
                (selectedProxies[0].OwnerEntity.Owner as ISpatialEntity).SetTransform(m, suppressUpdate: false);

                foreach (RenderProxy proxy in selectedProxies)
                    proxy.OwnerEntity.RequiresTransformUpdate = true;

                return;
            }

            base.MouseUp(x, y, button);
            if (mouseButtonData.MouseDown)
            {
                if (x == mouseButtonData.X && y == mouseButtonData.Y && button == mouseButtonData.Button)
                {
                    Ray ray = GetHitTestRay(x, y);

                    List<MeshRenderInstance> visibleInstances = GetSortedVisibleList();
                    List<Tuple<MeshRenderInstance, float>> hits = new List<Tuple<MeshRenderInstance, float>>();

                    foreach (MeshRenderInstance instance in visibleInstances)
                    {
                        RenderProxy instProxy = instance.RenderMesh as RenderProxy;
                        BoundingBox bbox = instProxy.BoundingBox;

                        Vector3 outPosition;
                        if (ray.Intersects(ref bbox, out outPosition))
                        {
                            if (instProxy.HitTest(ray, out outPosition))
                            {
                                hits.Add(new Tuple<MeshRenderInstance, float>(instance, (outPosition - (camera as LevelEditorCamera).WorldMatrix.TranslationVector).Length()));
                            }
                        }
                    }

                    RenderProxy proxy = null;
                    RenderProxy prevSelection = selectedProxies.FirstOrDefault();

                    if (hits.Count > 0)
                    {
                        hits.Sort((a, b) => { return a.Item2.CompareTo(b.Item2); });
                        proxy = hits[0].Item1.RenderMesh as RenderProxy;                        
                    }

                    Entity entityToSelect = (proxy != null)
                            ? proxy.OwnerEntity.Owner
                            : null;

                    SelectedEntityChanged?.Invoke(this, new SelectedEntityChangedEventArgs(entityToSelect));
                }
            }

            mouseButtonData.MouseDown = false;
        }

        public override void MouseMove(int x, int y)
        {
            base.MouseMove(x, y);

            if (!translateGizmo.MouseMove(x, y, GetHitTestRay(x, y)))
            {
                Ray ray = GetHitTestRay(x, y);
                if (translateGizmo.HitTest(ray))
                {
                    // do something
                }
            }
            else
            {
                Vector3 scale;
                Vector3 translation;
                Quaternion rotation;

                Matrix entityTransform = (selectedProxies[0].OwnerEntity.Owner as ISpatialEntity).GetLocalTransform();
                entityTransform.Decompose(out scale, out rotation, out translation);

                Matrix m = Matrix.Scaling(scale) * Matrix.RotationQuaternion(rotation) * Matrix.Translation(translateGizmo.Transform.TranslationVector);
                (selectedProxies[0].OwnerEntity.Owner as ISpatialEntity).SetTransform(m, suppressUpdate: true);

                foreach (RenderProxy proxy in selectedProxies)
                    proxy.OwnerEntity.RequiresTransformUpdate = true;
            }
        }

        protected override void PostProcessEditorComposite()
        {
            base.PostProcessEditorComposite();

            sprites.Sort((a, b) =>
            {
                SpriteRenderProxy spriteA = a.RenderMesh as SpriteRenderProxy;
                SpriteRenderProxy spriteB = b.RenderMesh as SpriteRenderProxy;

                return spriteB.CurrentDistanceToCamera.CompareTo(spriteA.CurrentDistanceToCamera);
            });

            D3D11.DepthStencilView oldDsv;
            D3D11.RenderTargetView[] rts = Viewport.Context.OutputMerger.GetRenderTargets(6, out oldDsv);

            // Regular elements
            Viewport.Context.OutputMerger.SetRenderTargets(Viewport.DepthBufferDSV, rts);
            Viewport.Context.OutputMerger.DepthStencilState = D3DUtils.CreateDepthStencilState(true, D3D11.DepthWriteMask.Zero, depthComparison: D3D11.Comparison.LessEqual);
            Viewport.Context.OutputMerger.BlendState = D3DUtils.CreateBlendState(D3DUtils.CreateBlendStateRenderTarget(alphaBlend: true));

            Viewport.Context.VertexShader.SetConstantBuffer(0, viewConstants.Buffer);

            Viewport.Context.PixelShader.SetConstantBuffer(0, viewConstants.Buffer);
            Viewport.Context.PixelShader.SetShaderResource(0, normalBasisCubemapTexture.SRV);
            Viewport.Context.PixelShader.SetSampler(0, D3DUtils.CreateSamplerState(D3D11.TextureAddressMode.Clamp, filter: D3D11.Filter.MinMagMipPoint));

            RenderMeshes(MeshRenderPath.Forward, sprites);

            // Foreground elements
            Viewport.Context.OutputMerger.SetRenderTargets(gizmoDepthTexture.DSV, rts);
            Viewport.Context.OutputMerger.DepthStencilState = D3DUtils.CreateDepthStencilState(true, depthComparison: D3D11.Comparison.LessEqual);
            Viewport.Context.OutputMerger.BlendState = D3DUtils.CreateBlendState(D3DUtils.CreateBlendStateRenderTarget());

            List<MeshRenderInstance> instances = new List<MeshRenderInstance>();
            if (translateGizmo.IsVisible)
                instances.Add(translateGizmo.GetInstance());

            RenderMeshes(MeshRenderPath.Forward, instances);

            Viewport.Context.OutputMerger.SetRenderTargets(oldDsv, rts);
        }

        protected override void PostProcessSelectionOutline()
        {
            Viewport.Context.OutputMerger.SetRenderTargets(null, this.selectionOutlineTexture.RTV);
            Viewport.Context.OutputMerger.DepthStencilState = D3DUtils.CreateDepthStencilState(false, D3D11.DepthWriteMask.All);
            Viewport.Context.OutputMerger.BlendState = D3DUtils.CreateBlendState(D3DUtils.CreateBlendStateRenderTarget());
            Viewport.Context.Rasterizer.State = D3DUtils.CreateRasterizerState(D3D11.CullMode.None);
            Viewport.Context.InputAssembler.SetIndexBuffer(null, SharpDX.DXGI.Format.Unknown, 0);
            Viewport.Context.InputAssembler.SetVertexBuffers(0, new D3D11.VertexBufferBinding());
            Viewport.Context.InputAssembler.InputLayout = null;
            Viewport.Context.VertexShader.Set(vsFullscreenQuad);
            Viewport.Context.VertexShader.SetConstantBuffer(0, commonConstants.Buffer);
            Viewport.Context.PixelShader.Set(psResolve);
            Viewport.Context.PixelShader.SetShaderResources(0, new D3D11.ShaderResourceView[]
            {
                finalColorTexture.SRV,
                selectionDepthTexture.SRV
            });
            Viewport.Context.PixelShader.SetSampler(0, D3DUtils.CreateSamplerState(D3D11.TextureAddressMode.Clamp));
            Viewport.Context.Draw(6, 0);
        }

        private Ray GetHitTestRay(int x, int y)
        {
            Matrix viewProj = camera.GetViewProjMatrix();
            Ray ray = Ray.GetPickRay(x, y, new Viewport(0, 0, Viewport.ViewportWidth, Viewport.ViewportHeight, camera.GetNearClip(), camera.GetFarClip()), viewProj);

            ray.Position.X *= -1.0f;

            ray.Direction = (ray.Position - (camera.GetEyePt() * new Vector3(-1, 1, 1)));
            ray.Direction.Normalize();

            return ray;
        }

        private void ProcessRenderTasks()
        {
            while (renderTasks.Count > 0)
            {
                RenderAction action = renderTasks.Dequeue();
                action.Invoke(RenderCreateState);
            }
        }
    }
}
