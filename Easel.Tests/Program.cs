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
using Easel;
using Easel.Core;
using Easel.Math;
using Easel.Tests;
using Easel.Tests.TestScenes;
using Pie.Windowing;

/*const float amount = 0.6532f;

Console.WriteLine("Vector2------------------------------------------------------------------------------------------");

Vector2 numericsVector2 = new Vector2(-13, 20.4f);
Vector2 numericsVector22 = new Vector2(10.5f, -3.2f);
Vector2 easelVector2 = new Vector2(-13, 20.4f);
Vector2 easelVector22 = new Vector2(10.5f, -3.2f);

Console.WriteLine();
Console.WriteLine($"Numerics: {numericsVector2}");
Console.WriteLine($"Easel: {easelVector2}");

Console.WriteLine();
Console.WriteLine($"Numerics add: {numericsVector2 + numericsVector22}");
Console.WriteLine($"Easel add: {easelVector2 + easelVector22}");

Console.WriteLine();
Console.WriteLine($"Numerics sub: {numericsVector2 - numericsVector22}");
Console.WriteLine($"Easel sub: {easelVector2 - easelVector22}");

Console.WriteLine();
Console.WriteLine($"Numerics mul: {numericsVector2 * numericsVector22}");
Console.WriteLine($"Easel mul: {easelVector2 * easelVector22}");

Console.WriteLine();
Console.WriteLine($"Numerics mulvf: {numericsVector2 * 2.7f}");
Console.WriteLine($"Easel mulvf: {easelVector2 * 2.7f}");

Console.WriteLine();
Console.WriteLine($"Numerics mulfv: {2.7f * numericsVector2}");
Console.WriteLine($"Easel mulfv: {2.7f * easelVector2}");

Console.WriteLine();
Console.WriteLine($"Numerics div: {numericsVector2 / numericsVector22}");
Console.WriteLine($"Easel div: {easelVector2 / easelVector22}");

Console.WriteLine();
Console.WriteLine($"Numerics divvf: {numericsVector2 / 2.7f}");
Console.WriteLine($"Easel divvf: {easelVector2 / 2.7f}");

Console.WriteLine();
Console.WriteLine($"Numerics normalized: {Vector2.Normalize(numericsVector2)}");
Console.WriteLine($"Easel normalized: {easelVector2.Normalize()}");

Console.WriteLine();
Console.WriteLine($"Numerics length: {numericsVector2.Length()}");
Console.WriteLine($"Easel length: {easelVector2.Length()}");

Console.WriteLine();
Console.WriteLine($"Numerics clamp: {Vector2.Clamp(numericsVector2, new Vector2(-15, 15), new Vector2(-5, 25))}");
Console.WriteLine($"Easel clamp: {Vector2T.Clamp(easelVector2, new Vector2(-15, 15), new Vector2(-5, 25))}");

Console.WriteLine();
Console.WriteLine($"Numerics min: {Vector2.Min(numericsVector2, new Vector2(-25, 26))}");
Console.WriteLine($"Easel min: {Vector2T.Min(easelVector2, new Vector2(-25, 26))}");

Console.WriteLine();
Console.WriteLine($"Numerics max: {Vector2.Max(numericsVector2, new Vector2(-25, 26))}");
Console.WriteLine($"Easel max: {Vector2T.Max(easelVector2, new Vector2(-25, 26))}");

Console.WriteLine();
Console.WriteLine($"Numerics lerp: {Vector2.Lerp(numericsVector2, Vector2.Zero, amount)}");
Console.WriteLine($"Easel lerp: {Vector2T.Lerp(easelVector2, Vector2.Zero, amount)}");

Console.WriteLine();
Console.WriteLine($"Numerics abs: {Vector2.Abs(numericsVector2)}");
Console.WriteLine($"Easel abs: {Vector2T.Abs(easelVector2)}");

Console.WriteLine();
Console.WriteLine($"Numerics sqrt: {Vector2.SquareRoot(numericsVector2)}");
Console.WriteLine($"Easel sqrt: {Vector2T.Sqrt(easelVector2)}");

Console.WriteLine();
Console.WriteLine($"Numerics distance: {Vector2.Distance(numericsVector2, new Vector2(20, 30))}");
Console.WriteLine($"Easel distance: {Vector2T.Distance(easelVector2, new Vector2(20, 30))}");

Console.WriteLine();
Console.WriteLine($"Numerics reflect: {Vector2.Reflect(numericsVector2, Vector2.Normalize(new Vector2(134, 456)))}");
Console.WriteLine($"Easel reflect: {Vector2T.Reflect(easelVector2, new Vector2(134, 456).Normalize())}");

Console.WriteLine();
Console.WriteLine($"Numerics to easel: {(Vector2) numericsVector2}");
Console.WriteLine($"Easel to numerics: {(Vector2) easelVector2}");

Console.WriteLine("\nVector3------------------------------------------------------------------------------------------");

Vector3 numericsVector3 = new Vector3(2, -13, 20.4f);
Vector3 numericsVector32 = new Vector3(-20.24f, 10.5f, -3.2f);
Vector3 easelVector3 = new Vector3(2, -13, 20.4f);
Vector3 easelVector32 = new Vector3(-20.24f, 10.5f, -3.2f);

Console.WriteLine();
Console.WriteLine($"Numerics: {numericsVector3}");
Console.WriteLine($"Easel: {easelVector3}");

Console.WriteLine();
Console.WriteLine($"Numerics add: {numericsVector3 + numericsVector32}");
Console.WriteLine($"Easel add: {easelVector3 + easelVector32}");

Console.WriteLine();
Console.WriteLine($"Numerics sub: {numericsVector3 - numericsVector32}");
Console.WriteLine($"Easel sub: {easelVector3 - easelVector32}");

Console.WriteLine();
Console.WriteLine($"Numerics mul: {numericsVector3 * numericsVector32}");
Console.WriteLine($"Easel mul: {easelVector3 * easelVector32}");

Console.WriteLine();
Console.WriteLine($"Numerics mulvf: {numericsVector3 * 2.7f}");
Console.WriteLine($"Easel mulvf: {easelVector3 * 2.7f}");

Console.WriteLine();
Console.WriteLine($"Numerics mulfv: {2.7f * numericsVector3}");
Console.WriteLine($"Easel mulfv: {2.7f * easelVector3}");

Console.WriteLine();
Console.WriteLine($"Numerics div: {numericsVector3 / numericsVector32}");
Console.WriteLine($"Easel div: {easelVector3 / easelVector32}");

Console.WriteLine();
Console.WriteLine($"Numerics divvf: {numericsVector3 / 2.7f}");
Console.WriteLine($"Easel divvf: {easelVector3 / 2.7f}");

Console.WriteLine();
Console.WriteLine($"Numerics normalized: {Vector3.Normalize(numericsVector3)}");
Console.WriteLine($"Easel normalized: {easelVector3.Normalize()}");

Console.WriteLine();
Console.WriteLine($"Numerics length: {numericsVector3.Length()}");
Console.WriteLine($"Easel length: {easelVector3.Length()}");

Console.WriteLine();
Console.WriteLine($"Numerics clamp: {Vector3.Clamp(numericsVector3, new Vector3(2, -15, 15), new Vector3(4, -5, 25))}");
Console.WriteLine($"Easel clamp: {Vector3T.Clamp(easelVector3, new Vector3(2, -15, 15), new Vector3(4, -5, 25))}");

Console.WriteLine();
Console.WriteLine($"Numerics min: {Vector3.Min(numericsVector3, new Vector3(-4.56f, -25, 26))}");
Console.WriteLine($"Easel min: {Vector3T.Min(easelVector3, new Vector3(-4.56f, -25, 26))}");

Console.WriteLine();
Console.WriteLine($"Numerics max: {Vector3.Max(numericsVector3, new Vector3(-4.56f, -25, 26))}");
Console.WriteLine($"Easel max: {Vector3T.Max(easelVector3, new Vector3(-4.56f, -25, 26))}");

Console.WriteLine();
Console.WriteLine($"Numerics lerp: {Vector3.Lerp(numericsVector3, Vector3.Zero, amount)}");
Console.WriteLine($"Easel lerp: {Vector3T.Lerp(easelVector3, Vector3.Zero, amount)}");

Console.WriteLine();
Console.WriteLine($"Numerics abs: {Vector3.Abs(numericsVector3)}");
Console.WriteLine($"Easel abs: {Vector3T.Abs(easelVector3)}");

Console.WriteLine();
Console.WriteLine($"Numerics sqrt: {Vector3.SquareRoot(numericsVector3)}");
Console.WriteLine($"Easel sqrt: {Vector3T.Sqrt(easelVector3)}");

Console.WriteLine();
Console.WriteLine($"Numerics distance: {Vector3.Distance(numericsVector3, new Vector3(10, 20, 30))}");
Console.WriteLine($"Easel distance: {Vector3T.Distance(easelVector3, new Vector3(10, 20, 30))}");

Console.WriteLine();
Console.WriteLine($"Numerics reflect: {Vector3.Reflect(numericsVector3, Vector3.Normalize(new Vector3(-321, 134, 456)))}");
Console.WriteLine($"Easel reflect: {Vector3T.Reflect(easelVector3, new Vector3(-321, 134, 456).Normalize())}");

Console.WriteLine();
Console.WriteLine($"Numerics cross: {Vector3.Cross(numericsVector3, numericsVector32)}");
Console.WriteLine($"Easel cross: {Vector3T.Cross(easelVector3, easelVector32)}");

Console.WriteLine();
Console.WriteLine($"Numerics to easel: {(Vector3) numericsVector3}");
Console.WriteLine($"Easel to numerics: {(Vector3) easelVector3}");

Console.WriteLine("\nVector4------------------------------------------------------------------------------------------");

Vector4 numericsVector4 = new Vector4(2, -13, 20.4f, -1.75f);
Vector4 numericsVector42 = new Vector4(-20.24f, 10.5f, -3.2f, 12);
Vector4T<float> easelVector4 = new Vector4T<float>(2, -13, 20.4f, -1.75f);
Vector4T<float> easelVector42 = new Vector4T<float>(-20.24f, 10.5f, -3.2f, 12);

Console.WriteLine();
Console.WriteLine($"Numerics: {numericsVector4}");
Console.WriteLine($"Easel: {easelVector4}");

Console.WriteLine();
Console.WriteLine($"Numerics add: {numericsVector4 + numericsVector42}");
Console.WriteLine($"Easel add: {easelVector4 + easelVector42}");

Console.WriteLine();
Console.WriteLine($"Numerics sub: {numericsVector4 - numericsVector42}");
Console.WriteLine($"Easel sub: {easelVector4 - easelVector42}");

Console.WriteLine();
Console.WriteLine($"Numerics mul: {numericsVector4 * numericsVector42}");
Console.WriteLine($"Easel mul: {easelVector4 * easelVector42}");

Console.WriteLine();
Console.WriteLine($"Numerics mulvf: {numericsVector4 * 2.7f}");
Console.WriteLine($"Easel mulvf: {easelVector4 * 2.7f}");

Console.WriteLine();
Console.WriteLine($"Numerics mulfv: {2.7f * numericsVector4}");
Console.WriteLine($"Easel mulfv: {2.7f * easelVector4}");

Console.WriteLine();
Console.WriteLine($"Numerics div: {numericsVector4 / numericsVector42}");
Console.WriteLine($"Easel div: {easelVector4 / easelVector42}");

Console.WriteLine();
Console.WriteLine($"Numerics divvf: {numericsVector4 / 2.7f}");
Console.WriteLine($"Easel divvf: {easelVector4 / 2.7f}");

Console.WriteLine();
Console.WriteLine($"Numerics normalized: {Vector4.Normalize(numericsVector4)}");
Console.WriteLine($"Easel normalized: {easelVector4.Normalize()}");

Console.WriteLine();
Console.WriteLine($"Numerics length: {numericsVector4.Length()}");
Console.WriteLine($"Easel length: {easelVector4.Length()}");

Console.WriteLine();
Console.WriteLine($"Numerics clamp: {Vector4.Clamp(numericsVector4, new Vector4(2, -15, 15, 0), new Vector4(4, -5, 25, 1))}");
Console.WriteLine($"Easel clamp: {Vector4T.Clamp(easelVector4, new Vector4T<float>(2, -15, 15, 0), new Vector4T<float>(4, -5, 25, 1))}");

Console.WriteLine();
Console.WriteLine($"Numerics min: {Vector4.Min(numericsVector4, new Vector4(-4.56f, -25, 26, 1))}");
Console.WriteLine($"Easel min: {Vector4T.Min(easelVector4, new Vector4T<float>(-4.56f, -25, 26, 1))}");

Console.WriteLine();
Console.WriteLine($"Numerics max: {Vector4.Max(numericsVector4, new Vector4(-4.56f, -25, 26, 1))}");
Console.WriteLine($"Easel max: {Vector4T.Max(easelVector4, new Vector4T<float>(-4.56f, -25, 26, 1))}");

Console.WriteLine();
Console.WriteLine($"Numerics lerp: {Vector4.Lerp(numericsVector4, Vector4.Zero, amount)}");
Console.WriteLine($"Easel lerp: {Vector4T.Lerp(easelVector4, Vector4T<float>.Zero, amount)}");

Console.WriteLine();
Console.WriteLine($"Numerics abs: {Vector4.Abs(numericsVector4)}");
Console.WriteLine($"Easel abs: {Vector4T.Abs(easelVector4)}");

Console.WriteLine();
Console.WriteLine($"Numerics sqrt: {Vector4.SquareRoot(numericsVector4)}");
Console.WriteLine($"Easel sqrt: {Vector4T.Sqrt(easelVector4)}");

Console.WriteLine();
Console.WriteLine($"Numerics distance: {Vector4.Distance(numericsVector4, new Vector4(10, 20, 30, 40))}");
Console.WriteLine($"Easel distance: {Vector4T.Distance(easelVector4, new Vector4T<float>(10, 20, 30, 40))}");

Console.WriteLine();
Console.WriteLine($"Numerics to easel: {(Vector4T<float>) numericsVector4}");
Console.WriteLine($"Easel to numerics: {(Vector4) easelVector4}");*/

GameSettings settings = new GameSettings()
{
    Border = WindowBorder.Resizable,
    //AutoGenerateContentDirectory = null
};

Logger.UseConsoleLogs();

using TestGame game = new TestGame(settings, new TestShadow());
game.Run();

/*QuaternionT<float> quat = QuaternionT.FromEuler(1f, 0.5f, 0.25f);
Console.WriteLine(quat);

Quaternion quat2 = Quaternion.CreateFromYawPitchRoll(1f, 0.5f, 0.25f);
Console.WriteLine(quat2);

Console.WriteLine(MatrixT.FromQuaternion(quat));

Console.WriteLine(Matrix4x4.CreateFromQuaternion(quat2));*/