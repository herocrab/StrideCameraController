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
            var cameraDb = Services.GetService<CameraDb>();
            cameraDb?.RegisterCamera(Entity.Get<CameraComponent>());
        }
    }
}
