using System;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using Easel;
using Easel.Core;
using Easel.Tests;
using Easel.Tests.TestScenes;
using Pie;
using Pie.Windowing;


GameSettings settings = new GameSettings()
{
    Border = WindowBorder.Resizable,
    TitleBarFlags = TitleBarFlags.ShowFps | TitleBarFlags.ShowGraphicsApi,
    VSync = false,
    TargetFps = 0,
    Api = GraphicsApi.D3D11
    //AutoGenerateContentDirectory = null
};

Logger.UseConsoleLogs();

using TestGame game = new TestGame(settings, new TestManyEntities());
game.Run();

/*QuaternionT<float> quat = QuaternionT.FromEuler(1f, 0.5f, 0.25f);
Console.WriteLine(quat);

Quaternion quat2 = Quaternion.CreateFromYawPitchRoll(1f, 0.5f, 0.25f);
Console.WriteLine(quat2);

Console.WriteLine(MatrixT.FromQuaternion(quat));

Console.WriteLine(Matrix4x4.CreateFromQuaternion(quat2));*/