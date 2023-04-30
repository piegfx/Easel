using Easel.Headless.Scenes;

namespace Easel.Headless.EventArgs; 

public class InitializeSceneEventArgs
{
    public readonly Scene Scene;

    public InitializeSceneEventArgs(Scene scene) => Scene = scene;
}