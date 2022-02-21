using System;
using System.Runtime.InteropServices;
using SharpDX;

namespace Frosty.Core.Viewport
{
    namespace DXUT
    {
        public enum D3DUtil_CameraKeys
        {
            CAM_STRAFE_LEFT = 0,
            CAM_STRAFE_RIGHT,
            CAM_MOVE_FORWARD,
            CAM_MOVE_BACKWARD,
            CAM_MOVE_UP,
            CAM_MOVE_DOWN,
            CAM_RESET,
            CAM_CONTROLDOWN,
            CAM_SPEED,
            CAM_MAX_KEYS,
            CAM_UNKNOWN = 0xFF
        };

        public abstract class BaseCamera
        {
            [DllImport("User32.dll", EntryPoint = "SetCursorPos")]
            protected static extern bool SetCursorPos(int X, int Y);

            [DllImport("User32.dll", EntryPoint = "GetCursorPos")]
            protected static extern void GetCursorPos(ref Point point);

            protected const int KEY_WAS_DOWN_MASK = 0x80;
            protected const int KEY_IS_DOWN_MASK = 0x01;

            protected const int MOUSE_LEFT_BUTTON = 0x01;
            protected const int MOUSE_MIDDLE_BUTTON = 0x02;
            protected const int MOUSE_RIGHT_BUTTON = 0x04;
            protected const int MOUSE_WHEEL_BUTTON = 0x08;

            public BaseCamera()
            {
                framesToSmoothMouseData = 2.0f;
                totalDragTimeToZero = 0.25f;
                rotationScaler = 0.1f;
                moveScaler = 5.0f;
                bEnablePositionMovement = true;
                bEnableYAxisMovement = true;

                SetViewParams(Vector3.Zero, Vector3.UnitY);
                SetProjParams((float)(Math.PI / 4.0f), 1.0f, 1.0f, 1000.0f);

                GetCursorPos(ref lastMousePosition);

                dragRect = new Rectangle(int.MinValue, int.MinValue, int.MaxValue, int.MaxValue);
                minBoundary = new Vector3(-1, -1, -1);
                maxBoundary = new Vector3(1, 1, 1);
            }

            public virtual void KeyDown(System.Windows.Input.Key key)
            {
                D3DUtil_CameraKeys mappedKey = MapKey(key);
                if (mappedKey != D3DUtil_CameraKeys.CAM_UNKNOWN)
                {
                    if (!IsKeyDown(keyStates[(int)mappedKey]))
                    {
                        keyStates[(int)mappedKey] = KEY_WAS_DOWN_MASK | KEY_IS_DOWN_MASK;
                        ++numKeysDown;
                    }
                }
            }

            public virtual void KeyUp(System.Windows.Input.Key key)
            {
                D3DUtil_CameraKeys mappedKey = MapKey(key);
                if (mappedKey != D3DUtil_CameraKeys.CAM_UNKNOWN && (int)mappedKey < (int)D3DUtil_CameraKeys.CAM_MAX_KEYS)
                {
                    keyStates[(int)mappedKey] &= ~KEY_IS_DOWN_MASK;
                    --numKeysDown;
                }
            }

            public virtual void MouseButtonDown(int x, int y, MouseButton button)
            {
                Point cursorPt = new Point(x, y);
                if (button == MouseButton.Left/* && dragRect.Contains(cursorPt.X, cursorPt.Y)*/)
                {
                    if (bMouseLButtonDown == false && initialMousePosition.X == 0 && initialMousePosition.Y == 0)
                        GetCursorPos(ref initialMousePosition);

                    bMouseLButtonDown = true;
                    currentButtonMask |= MOUSE_LEFT_BUTTON;
                }
                if (button == MouseButton.Middle/* && dragRect.Contains(cursorPt.X, cursorPt.Y)*/)
                {
                    if (bMouseMButtonDown == false && initialMousePosition.X == 0 && initialMousePosition.Y == 0)
                        GetCursorPos(ref initialMousePosition);

                    bMouseMButtonDown = true;
                    currentButtonMask |= MOUSE_MIDDLE_BUTTON;
                }
                if (button == MouseButton.Right/* && dragRect.Contains(cursorPt.X, cursorPt.Y)*/)
                {
                    if (bMouseRButtonDown == false && initialMousePosition.X == 0 && initialMousePosition.Y == 0)
                        GetCursorPos(ref initialMousePosition);

                    bMouseRButtonDown = true;
                    currentButtonMask |= MOUSE_RIGHT_BUTTON;
                }

                if (bResetCursorAfterMove)
                    System.Windows.Input.Mouse.OverrideCursor = System.Windows.Input.Cursors.None;
                GetCursorPos(ref lastMousePosition);

            }

            public virtual void MouseButtonUp(MouseButton button)
            {
                if (button == MouseButton.Left)
                {
                    bMouseLButtonDown = false;
                    currentButtonMask &= ~MOUSE_LEFT_BUTTON;
                }
                if (button == MouseButton.Middle)
                {
                    bMouseMButtonDown = false;
                    currentButtonMask &= ~MOUSE_MIDDLE_BUTTON;
                }
                if (button == MouseButton.Right)
                {
                    bMouseRButtonDown = false;
                    currentButtonMask &= ~MOUSE_RIGHT_BUTTON;
                }

                if (!bMouseLButtonDown && !bMouseMButtonDown && !bMouseRButtonDown)
                {
                    if (bResetCursorAfterMove)
                    {
                        System.Windows.Input.Mouse.OverrideCursor = null;
                        initialMousePosition = new Point(0, 0);
                    }
                }
            }

            public virtual void MouseWheel(int delta) => mouseWheelDelta += delta;

            public virtual void MouseMove(int x, int y)
            {
            }

            public virtual void FrameMove(float elapsedTime)
            {
                prevViewProjMatrix = GetViewProjMatrix();
                prevCrViewProjMatrix = GetCrViewProjMatrix();
            }

            public virtual void Reset() => SetViewParams(defaultEye, defaultLookAt);

            public virtual void SetViewParams(Vector3 eyePt, Vector3 lookAtPt)
            {
                currentEye = eyePt;
                defaultEye = eyePt;

                currentLookAt = lookAtPt;
                defaultLookAt = lookAtPt;

                viewMatrix = Matrix.LookAtLH(eyePt, lookAtPt, Vector3.UnitY);

                Matrix invView = viewMatrix;
                invView.Invert();

                Vector4 zBasis = invView.Row3;
                cameraYawAngle = (float)Math.Atan2(zBasis.X, zBasis.Z);
                float len = (float)Math.Sqrt(zBasis.Z * zBasis.Z + zBasis.X * zBasis.X);
                cameraPitchAngle = (float)-Math.Atan2(zBasis.Y, len);
            }

            public void SetProjParams(float inFov, float inAspectRatio, float inNearPlane, float inFarPlane)
            {
                fov = inFov;
                aspectRatio = inAspectRatio;
                nearPlane = inNearPlane;
                farPlane = inFarPlane;

                projMatrix = Matrix.PerspectiveFovLH(fov, aspectRatio, nearPlane, farPlane);
            }

            public virtual void SetDragRect(Rectangle rc) { dragRect = rc; }
            public void SetInvertPitch(bool inInvertPitch) { bInvertPitch = inInvertPitch; }
            public void SetDrag(bool inMovementDrag, float inTotalDragTimeToZero = 0.25f)
            {
                bMovementDrag = inMovementDrag;
                totalDragTimeToZero = inTotalDragTimeToZero;
            }
            public void SetEnableYAxisMovement(bool inEnableYAxisMovement) { bEnableYAxisMovement = inEnableYAxisMovement; }
            public void SetEnablePositionMovement(bool inEnablePositionMovement) { bEnablePositionMovement = inEnablePositionMovement; }
            public void SetClipToBoundary(bool inClipToBoundary, Vector3? inMinBoundary, Vector3? inMaxBoundary)
            {
                bClipToBoundary = inClipToBoundary;
                if (inMinBoundary.HasValue)
                    minBoundary = inMinBoundary.Value;
                if (inMaxBoundary.HasValue)
                    maxBoundary = inMaxBoundary.Value;
            }
            public void SetScalers(float inRotationScaler=0.01f, float inMoveScaler = 5.0f)
            {
                rotationScaler = inRotationScaler;
                moveScaler = inMoveScaler;
            }
            public void SetMoveScaler(float inMoveScaler)
            {
                moveScaler = inMoveScaler;
            }
            public void SetNumberOfFramesToSmoothMouseData(int inNumFrames) { if (inNumFrames > 0) framesToSmoothMouseData = (float)inNumFrames; }
            public void SetResetCursorAfterMove(bool inResetCursorAfterMove) { bResetCursorAfterMove = inResetCursorAfterMove; }

            public Matrix GetViewMatrix() { return viewMatrix; }
            public Matrix GetViewAtOriginMatrix() { return viewMatrixAtOrigin; }
            public Matrix GetProjMatrix() { return projMatrix; }
            public Matrix GetProjMatrix(float[] jitter)
            {
                Matrix jitteredProjMatrix = projMatrix;
                jitteredProjMatrix.M31 = jitter[0];
                jitteredProjMatrix.M32 = jitter[1];
                return jitteredProjMatrix;
            }
            public Matrix GetViewProjMatrix() { return viewMatrix * projMatrix; }
            public Matrix GetViewProjMatrix(float[] jitter)
            {
                Matrix jitteredProjMatrix = projMatrix;
                jitteredProjMatrix.M31 = jitter[0];
                jitteredProjMatrix.M32 = jitter[1];
                return viewMatrix * jitteredProjMatrix;
            }
            public Matrix GetCrViewProjMatrix() { return viewMatrixAtOrigin * projMatrix; }
            public Matrix GetCrViewProjMatrix(float[] jitter)
            {
                Matrix jitteredProjMatrix = projMatrix;
                jitteredProjMatrix.M31 = jitter[0];
                jitteredProjMatrix.M32 = jitter[1];
                return viewMatrixAtOrigin * jitteredProjMatrix;
            }
            public Matrix GetPrevViewProjMatrix() { return prevViewProjMatrix; }
            public Matrix GetPrevCrViewProjMatrix() { return prevCrViewProjMatrix; }
            public virtual Vector3 GetEyePt() { return currentEye; }
            public Vector3 GetLookAtPt() { return currentLookAt; }
            public float GetNearClip() { return nearPlane; }
            public float GetFarClip() { return farPlane; }

            public bool IsBeingDragged() { return bMouseLButtonDown || bMouseMButtonDown || bMouseRButtonDown; }
            public bool IsMouseLButtonDown() { return bMouseLButtonDown; }
            public bool IsMouseMButtonDown() { return bMouseMButtonDown; }
            public bool IsMouseRButtonDown() { return bMouseRButtonDown; }

            protected D3DUtil_CameraKeys MapKey(System.Windows.Input.Key inKey)
            {
                switch (inKey)
                {
                    case System.Windows.Input.Key.LeftCtrl: return D3DUtil_CameraKeys.CAM_CONTROLDOWN;
                    case System.Windows.Input.Key.RightCtrl: return D3DUtil_CameraKeys.CAM_CONTROLDOWN;
                    case System.Windows.Input.Key.Left: return D3DUtil_CameraKeys.CAM_STRAFE_LEFT;
                    case System.Windows.Input.Key.Right: return D3DUtil_CameraKeys.CAM_STRAFE_RIGHT;
                    case System.Windows.Input.Key.Up: return D3DUtil_CameraKeys.CAM_MOVE_FORWARD;
                    case System.Windows.Input.Key.Down: return D3DUtil_CameraKeys.CAM_MOVE_BACKWARD;
                    case System.Windows.Input.Key.Prior: return D3DUtil_CameraKeys.CAM_MOVE_UP;
                    case System.Windows.Input.Key.Next: return D3DUtil_CameraKeys.CAM_MOVE_DOWN;

                    case System.Windows.Input.Key.A: return D3DUtil_CameraKeys.CAM_STRAFE_LEFT;
                    case System.Windows.Input.Key.D: return D3DUtil_CameraKeys.CAM_STRAFE_RIGHT;
                    case System.Windows.Input.Key.W: return D3DUtil_CameraKeys.CAM_MOVE_FORWARD;
                    case System.Windows.Input.Key.S: return D3DUtil_CameraKeys.CAM_MOVE_BACKWARD;
                    case System.Windows.Input.Key.Q: return D3DUtil_CameraKeys.CAM_MOVE_DOWN;
                    case System.Windows.Input.Key.E: return D3DUtil_CameraKeys.CAM_MOVE_UP;

                    case System.Windows.Input.Key.LeftShift: return D3DUtil_CameraKeys.CAM_SPEED;
                    case System.Windows.Input.Key.Home: return D3DUtil_CameraKeys.CAM_RESET;
                }

                return D3DUtil_CameraKeys.CAM_UNKNOWN;
            }

            protected bool IsKeyDown(int inKey) { return (inKey & KEY_IS_DOWN_MASK) != 0; }
            protected bool WasKeyDown(int inKey) { return (inKey & KEY_WAS_DOWN_MASK) != 0; }

            protected Vector3 ConstrainToBoundary(Vector3 v)
            {
                return Vector3.Clamp(v, minBoundary, maxBoundary);
            }

            protected virtual Point UpdateMouseDelta()
            {
                Point curMousePt = new Point();
                GetCursorPos(ref curMousePt);

                Point curMouseDelta = new Point
                {
                    X = curMousePt.X - lastMousePosition.X,
                    Y = curMousePt.Y - lastMousePosition.Y
                };

                if (curMouseDelta.X > 10) curMouseDelta.X = 10;
                else if (curMouseDelta.X < -10) curMouseDelta.X = -10;
                if (curMouseDelta.Y > 10) curMouseDelta.Y = 10;
                else if (curMouseDelta.Y < -10) curMouseDelta.Y = -10;

                Point mouseMoveDelta = curMouseDelta;

                if (bResetCursorAfterMove)
                {
                    SetCursorPos(initialMousePosition.X, initialMousePosition.Y);
                    lastMousePosition = initialMousePosition;
                }

                float percentOfNew = 1.0f / framesToSmoothMouseData;
                float percentOfOld = 1.0f - percentOfNew;

                mouseDelta.X = mouseDelta.X * percentOfOld + curMouseDelta.X * percentOfNew;
                mouseDelta.Y = mouseDelta.Y * percentOfOld + curMouseDelta.Y * percentOfNew;

                rotateVelocity.X = mouseDelta.X * rotationScaler;
                rotateVelocity.Y = mouseDelta.Y * rotationScaler;

                return mouseMoveDelta;
            }

            protected void UpdateVelocity(float elapsedTime)
            {
                Vector2 rotVelocity = mouseDelta * rotationScaler;
                Vector3 acceleration = keyboardDirection;

                float multiplier = 1.0f;
                if (IsKeyDown(keyStates[(int)D3DUtil_CameraKeys.CAM_SPEED]))
                    multiplier = 4.0f;

                acceleration.Normalize();
                acceleration *= moveScaler * multiplier;

                if (bMovementDrag)
                {
                    if (acceleration.LengthSquared() > 0)
                    {
                        velocity = acceleration;
                        dragTimer = totalDragTimeToZero;
                        velocityDrag = acceleration / dragTimer;
                    }
                    else
                    {
                        if (dragTimer > 0)
                        {
                            velocity -= velocityDrag * elapsedTime;
                            dragTimer -= elapsedTime;
                        }
                        else
                        {
                            velocity = Vector3.Zero;
                        }
                    }
                }
                else
                {
                    velocity = acceleration;
                }
            }

            protected void GetInput(bool bGetKeyboardInput, bool bGetMouseInput, out Point mouseMoveDelta)
            {
                keyboardDirection = Vector3.Zero;
                if (bGetKeyboardInput)
                {
                    if (IsKeyDown(keyStates[(int)D3DUtil_CameraKeys.CAM_MOVE_FORWARD]))
                        keyboardDirection.Z += 1.0f;
                    if (IsKeyDown(keyStates[(int)D3DUtil_CameraKeys.CAM_MOVE_BACKWARD]))
                        keyboardDirection.Z -= 1.0f;
                    if (bEnableYAxisMovement)
                    {
                        if (IsKeyDown(keyStates[(int)D3DUtil_CameraKeys.CAM_MOVE_UP]))
                            keyboardDirection.Y += 1.0f;
                        if (IsKeyDown(keyStates[(int)D3DUtil_CameraKeys.CAM_MOVE_DOWN]))
                            keyboardDirection.Y -= 1.0f;
                    }
                    if (IsKeyDown(keyStates[(int)D3DUtil_CameraKeys.CAM_STRAFE_RIGHT]))
                        keyboardDirection.X += 1.0f;
                    if (IsKeyDown(keyStates[(int)D3DUtil_CameraKeys.CAM_STRAFE_LEFT]))
                        keyboardDirection.X -= 1.0f;
                }

                mouseMoveDelta = new Point();
                if (bGetMouseInput)
                    mouseMoveDelta = UpdateMouseDelta();
            }

            protected Matrix viewMatrix;
            protected Matrix projMatrix;

            protected Matrix viewMatrixAtOrigin;
            protected Matrix prevViewProjMatrix;
            protected Matrix prevCrViewProjMatrix;

            protected int numKeysDown;
            protected int[] keyStates = new int[(int)D3DUtil_CameraKeys.CAM_MAX_KEYS];
            protected Vector3 keyboardDirection;
            protected Point lastMousePosition;
            protected Point initialMousePosition;
            protected int currentButtonMask;
            protected int mouseWheelDelta;
            protected float framesToSmoothMouseData;
            protected Vector2 mouseDelta;
            protected Vector3 defaultEye;
            protected Vector3 defaultLookAt;
            protected Vector3 currentEye;
            protected Vector3 currentLookAt;
            protected float cameraYawAngle;
            protected float cameraPitchAngle;

            protected Rectangle dragRect;
            protected Vector3 velocity;
            protected Vector3 velocityDrag;
            protected float dragTimer;
            protected float totalDragTimeToZero;
            protected Vector2 rotateVelocity;

            protected float fov;
            protected float aspectRatio;
            protected float nearPlane;
            protected float farPlane;

            protected float rotationScaler;
            protected float moveScaler;

            protected bool bMouseLButtonDown;
            protected bool bMouseMButtonDown;
            protected bool bMouseRButtonDown;
            protected bool bMovementDrag;
            protected bool bInvertPitch;
            protected bool bEnablePositionMovement;
            protected bool bEnableYAxisMovement;
            protected bool bClipToBoundary;
            protected bool bResetCursorAfterMove;

            protected Vector3 minBoundary;
            protected Vector3 maxBoundary;
        }

        public class FirstPersonCamera : BaseCamera
        {
            public FirstPersonCamera()
            {
                activeButtonMask = 0x07;
                bMovementDrag = true;
                rotationScaler = 0.002f;
                bResetCursorAfterMove = true;
            }

            public override void FrameMove(float elapsedTime)
            {
                base.FrameMove(elapsedTime);

                if (IsKeyDown(keyStates[(int)D3DUtil_CameraKeys.CAM_RESET]))
                    Reset();

                GetInput(currentButtonMask != 0, ((activeButtonMask & currentButtonMask) != 0) || bRotateWithoutButtonDown, out Point mouseMoveDelta);
                UpdateVelocity(elapsedTime);

                Vector3 posDelta = velocity/* * elapsedTime*/;
                Vector3 axisDelta = Vector3.Zero;

                // middle mouse pan around (or both left and right mouse)
                if ((currentButtonMask & 0x02) != 0 || (currentButtonMask & 0x05) == 0x05)
                {
                    if (mouseMoveDelta.X != 0)
                        posDelta.X += mouseMoveDelta.X / 1.0f;
                    if (mouseMoveDelta.Y != 0)
                        axisDelta.Y += -mouseMoveDelta.Y / 1.0f;
                }

                if ((currentButtonMask & 0x01) != 0)
                {
                    if ((currentButtonMask & 0x04) == 0)
                    {
                        // left mouse move forward/back
                        if (mouseMoveDelta.Y != 0)
                            axisDelta.Z += -mouseMoveDelta.Y / 1.0f;
                    }
                }

                // make sure Q/E move camera up and down on axis not camera direction
                axisDelta.Y += posDelta.Y;
                posDelta.Y = 0;

                // add mousewheel to forward motion
                posDelta.Z += mouseWheelDelta;
                mouseWheelDelta = 0;

                posDelta *= elapsedTime;
                axisDelta *= elapsedTime;

                // right mouse, look
                if ((currentButtonMask & 0x04) != 0)
                {
                    if ((currentButtonMask & 0x01) == 0)
                    {
                        float yawDelta = rotateVelocity.X;
                        float pitchDelta = rotateVelocity.Y;

                        if (bInvertPitch)
                            pitchDelta = -pitchDelta;

                        cameraPitchAngle += pitchDelta;
                        cameraYawAngle += yawDelta;

                        //cameraPitchAngle = (float)Math.Max(-Math.PI / 2.0f, cameraPitchAngle);
                        //cameraPitchAngle = (float)Math.Max(+Math.PI / 2.0f, cameraPitchAngle);
                    }
                }

                // left mouse, turn
                else if ((currentButtonMask & 0x01) != 0)
                {
                    float yawDelta = rotateVelocity.X;
                    cameraYawAngle += yawDelta;
                }

                Matrix cameraRot = Matrix.RotationYawPitchRoll(cameraYawAngle, cameraPitchAngle, 0);
                Matrix yawOnlyRot = Matrix.RotationYawPitchRoll(cameraYawAngle, 0, 0);

                Vector3 worldUp = Vector3.TransformCoordinate(Vector3.UnitY, cameraRot);
                Vector3 worldAhead = Vector3.TransformCoordinate(Vector3.UnitZ, cameraRot);

                if (!bEnableYAxisMovement)
                {
                    cameraRot = Matrix.RotationYawPitchRoll(cameraYawAngle, 0, 0);
                }

                Vector3 posDeltaWorld = Vector3.TransformCoordinate(posDelta, cameraRot);

                currentEye += posDeltaWorld;
                currentEye += Vector3.TransformCoordinate(axisDelta, yawOnlyRot);

                if (bClipToBoundary)
                {
                    // someday
                }

                currentLookAt = currentEye + worldAhead;
                viewMatrix = Matrix.LookAtLH(currentEye, currentLookAt, worldUp);

                cameraWorldMatrix = viewMatrix;
                cameraWorldMatrix.Invert();

                cameraWorldMatrix.DecomposeUniformScale(out float scale, out Quaternion rot, out Vector3 trans);

                viewMatrixAtOrigin = Matrix.RotationQuaternion(rot);
                viewMatrixAtOrigin.Invert();
            }

            public void SetRotateButtons(bool inLeft, bool inMiddle, bool inRight, bool inRotateWithoutButtonDown = false)
            {
                activeButtonMask = (inLeft ? MOUSE_LEFT_BUTTON : 0)
                    | (inMiddle ? MOUSE_MIDDLE_BUTTON : 0)
                    | (inRight ? MOUSE_RIGHT_BUTTON : 0);
                bRotateWithoutButtonDown = inRotateWithoutButtonDown;
            }

            public Matrix GetWorldMatrix() { return cameraWorldMatrix; }

            public Vector3 GetWorldRight() { return cameraWorldMatrix.Right; }
            public Vector3 GetWorldUp() { return cameraWorldMatrix.Up; }
            public Vector3 GetWorldAhead() { return cameraWorldMatrix.Forward; }
            public override Vector3 GetEyePt() { return cameraWorldMatrix.TranslationVector; }

            protected Matrix cameraWorldMatrix;
            protected int activeButtonMask;
            protected bool bRotateWithoutButtonDown;
        }

        public class ModelViewerCamera : BaseCamera
        {
            public ModelViewerCamera()
            {
                bResetCursorAfterMove = true;
                rotationScaler = 0.02f;
            }

            public void SetLookAtPt(Vector3 newLookAt) => defaultLookAt = newLookAt;

            public void SetEyePt(Vector3 newEyePt) => defaultEye = newEyePt;

            public void SetRadius(float newRadius) => defaultRadius = radius = newRadius;

            public override void Reset()
            {
                base.Reset();

                radius = defaultRadius;
                currentPan = Vector3.Zero;
                cameraPitchAngle = defaultPitch;
                cameraYawAngle = defaultYaw;
            }

            public override void FrameMove(float elapsedTime)
            {
                base.FrameMove(elapsedTime);

                if (IsKeyDown(keyStates[(int)D3DUtil_CameraKeys.CAM_RESET]))
                    Reset();

                GetInput(false, (currentButtonMask & 0x07) != 0, out Point mouseMoveDelta);
                UpdateVelocity(elapsedTime);

                Vector3 posDelta = velocity;
                Vector3 axisDelta = Vector3.Zero;

                if ((currentButtonMask & 0x02) != 0)
                {
                    if (mouseMoveDelta.X != 0)
                        posDelta.X += mouseMoveDelta.X / 1.0f;
                    if (mouseMoveDelta.Y != 0)
                        axisDelta.Y += -mouseMoveDelta.Y / 1.0f;
                }

                if ((currentButtonMask & 0x01) != 0)
                {
                    float yawDelta = rotateVelocity.X;
                    cameraYawAngle += yawDelta;
                }

                if ((currentButtonMask & 0x04) != 0)
                {
                    float pitchDelta = rotateVelocity.Y;
                    cameraPitchAngle += pitchDelta;

                    cameraPitchAngle = Math.Min(cameraPitchAngle, maxPitch);
                    cameraPitchAngle = Math.Max(cameraPitchAngle, minPitch);
                }

                if (mouseWheelDelta != 0)
                    radius -= mouseWheelDelta * radius * 0.1f / 120.0f;
                radius = Math.Min(maxRadius, radius);
                radius = Math.Max(minRadius, radius);
                mouseWheelDelta = 0;

                posDelta *= elapsedTime;
                axisDelta *= elapsedTime;
                currentPan += posDelta + axisDelta;

                Matrix cameraRot = Matrix.RotationYawPitchRoll(cameraYawAngle, cameraPitchAngle, 0);
                Matrix cameraRotYaw = Matrix.RotationYawPitchRoll(cameraYawAngle, 0.0f, 0);

                Vector3 worldUp = Vector3.TransformCoordinate(Vector3.UnitY, cameraRot);
                Vector3 worldAhead = Vector3.TransformCoordinate(Vector3.UnitZ, cameraRot);

                currentLookAt = (defaultLookAt * Vector3.UnitY) + (worldAhead * (maxRadius - radius)) + Vector3.TransformCoordinate(currentPan, cameraRotYaw);
                currentEye = (defaultLookAt * Vector3.UnitY) + (worldAhead * -radius) + Vector3.TransformCoordinate(currentPan, cameraRotYaw);
                viewMatrix = Matrix.LookAtLH(currentEye, currentLookAt, Vector3.UnitY);

                Matrix cameraWorldMatrix = viewMatrix;
                cameraWorldMatrix.Invert();

                cameraWorldMatrix.DecomposeUniformScale(out float scale, out Quaternion rot, out Vector3 trans);

                viewMatrixAtOrigin = Matrix.RotationQuaternion(rot);
                viewMatrixAtOrigin.Invert();
            }

            protected float radius = 1.0f;
            protected float defaultRadius = 1.0f;
            protected float minRadius = 1.0f;
            protected float maxRadius = 10.0f;
            protected Vector3 currentPan = Vector3.Zero;
            protected float minPitch = -1.55334f; /* -89 degrees */
            protected float maxPitch = 1.55334f; /* 89 degrees */
            protected float defaultPitch = 0.785398f; /* 45 degrees */
            protected float defaultYaw = 1.5708f; /* 90 degrees */
        }
    }
}
