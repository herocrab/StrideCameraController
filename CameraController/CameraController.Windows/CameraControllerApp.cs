using Stride.Engine;
// ReSharper disable UnusedParameter.Local

namespace CameraController
{
    class CameraControllerApp
    {
        static void Main(string[] args)
        {
            using (var game = new Game())
            {
                game.Run();
            }
        }
    }
}
