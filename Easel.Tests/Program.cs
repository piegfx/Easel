/*using System;
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
using Pie.Windowing;*/

using System;
using System.Numerics;
using Easel.Math;

Vector2 numericsVector = new Vector2(-13, 20);
Vector2T<float> easelVector = new Vector2T<float>(-13, 20);

Console.WriteLine($"Numerics: {numericsVector}");
Console.WriteLine($"Easel: {easelVector}");

Console.WriteLine();
Console.WriteLine($"Numerics normalized: {Vector2.Normalize(numericsVector)}");
Console.WriteLine($"Easel normalized: {easelVector.Normalize()}");

Console.WriteLine();
Console.WriteLine($"Numerics length: {numericsVector.Length()}");
Console.WriteLine($"Easel length: {easelVector.Length()}");

Console.WriteLine();
Console.WriteLine($"Numerics clamp: {Vector2.Clamp(numericsVector, new Vector2(-15, 15), new Vector2(-5, 25))}");
Console.WriteLine($"Easel clamp: {Vector2T.Clamp(easelVector, new Vector2T<float>(-15, 15), new Vector2T<float>(-5, 25))}");

Console.WriteLine();
Console.WriteLine($"Numerics min: {Vector2.Min(numericsVector, new Vector2(-25, 26))}");
Console.WriteLine($"Easel min: {Vector2T.Min(easelVector, new Vector2T<float>(-25, 26))}");

Console.WriteLine();
Console.WriteLine($"Numerics max: {Vector2.Max(numericsVector, new Vector2(-25, 26))}");
Console.WriteLine($"Easel max: {Vector2T.Max(easelVector, new Vector2T<float>(-25, 26))}");

Console.WriteLine();
const float amount = 0.6532f;
Console.WriteLine($"Numerics lerp: {Vector2.Lerp(numericsVector, Vector2.Zero, amount)}");
Console.WriteLine($"Easel lerp: {Vector2T.Lerp(easelVector, Vector2T<float>.Zero, amount)}");

Console.WriteLine();
Console.WriteLine($"Numerics abs: {Vector2.Abs(numericsVector)}");
Console.WriteLine($"Easel abs: {Vector2T.Abs(easelVector)}");

Console.WriteLine();
Console.WriteLine($"Numerics sqrt: {Vector2.SquareRoot(numericsVector)}");
Console.WriteLine($"Easel sqrt: {Vector2T.Sqrt(easelVector)}");

Console.WriteLine();
Console.WriteLine($"Numerics distance: {Vector2.Distance(numericsVector, new Vector2(20, 30))}");
Console.WriteLine($"Easel distance: {Vector2T.Distance(easelVector, new Vector2T<float>(20, 30))}");

Console.WriteLine();
Console.WriteLine($"Numerics reflect: {Vector2.Reflect(numericsVector, Vector2.Normalize(new Vector2(134, 456)))}");
Console.WriteLine($"Easel reflect: {Vector2T.Reflect(easelVector, new Vector2T<float>(134, 456).Normalize())}");

/*GameSettings settings = new GameSettings()
{
    Border = WindowBorder.Resizable,
    //AutoGenerateContentDirectory = null
};

Logger.UseConsoleLogs();

using TestGame game = new TestGame(settings, new TestShadow());
game.Run();*/

/*QuaternionT<float> quat = QuaternionT.FromEuler(1f, 0.5f, 0.25f);
Console.WriteLine(quat);

Quaternion quat2 = Quaternion.CreateFromYawPitchRoll(1f, 0.5f, 0.25f);
Console.WriteLine(quat2);

Console.WriteLine(MatrixT.FromQuaternion(quat));

Console.WriteLine(Matrix4x4.CreateFromQuaternion(quat2));*/