using Easel;
using Easel.Tests;
using Easel.Tests.TestScenes;
using Pie.Windowing;

GameSettings settings = new GameSettings()
{
    Border = WindowBorder.Resizable
};

using TestGame game = new TestGame(settings, new Test3D());
game.Run();