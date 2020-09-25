using Stride.Core.Diagnostics;
using Stride.Engine;

namespace CameraController.Scripts
{
    public class CameraAgent : StartupScript
    {
        public override void Start()
        {
            RegisterCamera();
        }

        private void RegisterCamera()
        {
            Log.ActivateLog(LogMessageType.Debug);

            var cameraDb = Services.GetService<CameraDb>();
            var cameraComponent = Entity.Get<CameraComponent>();

            if (cameraComponent == null) {
                Log.Error($"{Entity.Name} attempted to register a camera when no camera component is attached.");
                return;
            }

            cameraDb?.RegisterCamera(Entity.Get<CameraComponent>());
        }
    }
}
