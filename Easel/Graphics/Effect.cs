using System;
using Pie;

namespace Easel.Graphics;

public class Effect : IDisposable
{
    public Shader PieShader;

    

    public void Dispose()
    {
        PieShader.Dispose();
    }
}