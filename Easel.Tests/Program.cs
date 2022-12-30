using Easel;
using Easel.Tests;
using Easel.Tests.TestScenes;

GameSettings settings = new GameSettings();

using TestGame game = new TestGame(settings, new GameResolutionTest());
game.Run();