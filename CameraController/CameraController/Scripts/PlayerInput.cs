using System.Linq;
using Stride.Core.Collections;
using Stride.Core.Diagnostics;
using Stride.Core.Mathematics;
using Stride.Input;
using Stride.Engine;
using Stride.Physics;

namespace CameraController.Scripts
{
    public class PlayerInput : SyncScript
    {
        private Entity sphere;
        private CameraDb cameraDb;

        private CameraComponent cameraX;
        private CameraComponent cameraY;

        private CameraComponent playerInputCamera;

        private readonly FastList<HitResult> resultList = new FastList<HitResult>();
        private readonly Int2 debugTextPos = new Int2(10, 20);

        public override void Start()
        {
            InitializePlayerInput();
            InitializeCameras();
        }

        public void AttachCamera(CameraComponent cameraComponent)
        {
            playerInputCamera = cameraComponent;
        }

        private void InitializeCameras()
        {
            // This class is also acting as the trigger entity script
            cameraX = SceneSystem.SceneInstance.RootScene.Entities.FirstOrDefault(a => a.Name == "CameraX")?.Get<CameraComponent>();
            cameraY = SceneSystem.SceneInstance.RootScene.Entities.FirstOrDefault(a => a.Name == "CameraY")?.Get<CameraComponent>();

            if (cameraX == null || cameraY == null) {
                Log.Error($"One of the cameras could not be located in the scene.");
            }

            // Set the initial camera
            playerInputCamera = cameraX;
        }

        private void InitializePlayerInput()
        {
            Log.ActivateLog(LogMessageType.Debug);

            sphere = SceneSystem.SceneInstance.RootScene.Entities.FirstOrDefault(a => a.Name == "Sphere");
            if (sphere == null) {
                Log.Error($"Sphere entity could not be located in scene.");
            }
            else {
                Log.Debug($"Sphere has been attached to player input");
            }

            cameraDb = Services.GetService<CameraDb>();
            if (cameraDb == null) {
                Log.Error($"Camera db service could not be found in the service registry.");
            }
            else {
                Log.Debug($"Camera db service was found in the service registry.");
            }
        }

        public override void Update()
        {
            DisplayDebugInfo();
            EvaluatePlayerInput();
        }

        private void DisplayDebugInfo()
        {
            DebugText.Print("Press X or Y to change cameras. Left click to move.", debugTextPos);
        }

        private void EvaluatePlayerInput()
        {
            // This would all be handled by the trigger collider and event
            if (Input.IsKeyPressed(Keys.X) && playerInputCamera != cameraX) {
                cameraDb?.ActivateCamera(cameraX);
            }

            if (Input.IsKeyPressed(Keys.Y) && playerInputCamera != cameraY) {
                cameraDb?.ActivateCamera(cameraY);
            }

            if (Input.IsMouseButtonPressed(MouseButton.Left)) {
                MoveSphere(Input.MousePosition);
            }
        }

        private void MoveSphere(Vector2 mouseScreenPos)
        {
            // Borrowed from Stride documentation
            var invViewProj = Matrix.Invert(playerInputCamera.ViewProjectionMatrix);
            Vector3 sPos;
            sPos.X = mouseScreenPos.X * 2f - 1f;
            sPos.Y = 1f - mouseScreenPos.Y * 2f;
            sPos.Z = 0f;

            var vectorNear = Vector3.Transform(sPos, invViewProj);
            vectorNear /= vectorNear.W;
            sPos.Z = 1f;
            var vectorFar = Vector3.Transform(sPos, invViewProj);
            vectorFar /= vectorFar.W;

            resultList.Clear();
            this.GetSimulation().RaycastPenetrating(vectorNear.XYZ(), vectorFar.XYZ(),
                resultList, CollisionFilterGroups.DefaultFilter, CollisionFilterGroupFlags.CustomFilter10);

            if (!resultList.Any()) {
                Log.Debug($"Destination point could not be found, are you clicking ground?");
                return;
            }

            var targetPosition = resultList.First().Point;
            sphere.Transform.Position = new Vector3(targetPosition.X, .5f, targetPosition.Z);
            Log.Debug($"Sphere position has been updated.");
        }
    }
}
