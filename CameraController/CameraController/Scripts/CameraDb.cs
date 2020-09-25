using System.Linq;
using Stride.Core.Collections;
using Stride.Core.Diagnostics;
using Stride.Engine;

namespace CameraController.Scripts
{
    public class CameraDb : StartupScript
    {
        private readonly FastList<CameraComponent> cameraDb = new FastList<CameraComponent>();

        private PlayerInput playerInput;

        public override void Start()
        {
            InitializeCameraDb();
            InitializePlayerInput();
        }

        public void RegisterCamera(CameraComponent cameraComponent)
        {
            if (!cameraDb.Contains(cameraComponent)) {
                cameraDb.Add(cameraComponent);
                Log.Info($"{cameraComponent.Entity.Name} camera has been registered with camera db.");
            }
        }

        public void ActivateCamera(CameraComponent cameraComponent)
        {
            // Lazy registration
            RegisterCamera(cameraComponent);

            foreach (var camera in cameraDb) {
                camera.Enabled = false;
            }

            cameraComponent.Enabled = true;
            Log.Info($"{cameraComponent.Entity.Name} has been activated.");

            playerInput?.AttachCamera(cameraComponent);
        }

        private void InitializeCameraDb()
        {
            Log.ActivateLog(LogMessageType.Debug);

            // Register this class as a service, this script must execute before camera agent scripts... it is set to 0 in game studio.
            Services.AddService(this);
        }

        private void InitializePlayerInput()
        {
            playerInput = SceneSystem.SceneInstance.RootScene.Entities.FirstOrDefault(a => a.Name == "PlayerInput")?.Get<PlayerInput>();

            if (playerInput == null) {
                Log.Error("Player Input script could not be found.");
                return;
            } 

            Log.Debug("Player Input script attached to camera db.");
        }
    }
}
