using System.Numerics;
using Pie;
using Pie.Windowing;

namespace Easel.Extensions.Imgui;

public class ImGuiRenderer : IDisposable
{
    private int _windowWidth;
    private int _windowHeight;

    private bool _frameBegun;

    private GraphicsBuffer _vertexBuffer;
    private GraphicsBuffer _indexBuffer;

    private uint _vboSize;
    private uint _eboSize;

    private Shader _shader;

    private Texture _fontTexture;

    public Vector2 Scale;

    private readonly List<char> _pressedChars;

    private Keys[] _keysList;

    private uint _stride;
    private InputLayout _inputLayout;

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}