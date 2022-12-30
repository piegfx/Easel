using System;
using System.Numerics;
using Easel.Entities;
using Easel.Entities.Components;
using Easel.Graphics;
using Easel.Graphics.Renderers;
using Easel.Math;
using Easel.Scenes;
using ImGuiNET;

namespace Easel.Tests.TestScenes;

public class GameResolutionTest : Scene
{
    private string[] _spriteRenderModes;
    private string[] _targetDrawModes;

    private int _currentSrm;
    private int _currentTdm;

    protected override void Initialize()
    {
        base.Initialize();

        Camera.Main.CameraType = CameraType.Orthographic;
        World.ClearColor = Color.CornflowerBlue;

        // Base entity to display
        Entity entity = new Entity();
        entity.AddComponent(new Sprite(Content.Load<Texture2D>("awesomeface.png")));
        AddEntity(entity);

        Graphics.PostProcessor.Resolution = new Size(640, 480);
        Graphics.PostProcessor.DrawMode = PostProcessor.TargetDrawMode.Fill;

        _spriteRenderModes = Enum.GetNames<SpriteRenderMode>();
        _targetDrawModes = Enum.GetNames<PostProcessor.TargetDrawMode>();
    }

    protected override void Update()
    {
        base.Update();

        Size size = Graphics.PostProcessor.Resolution.Value;

        if (ImGui.DragInt2("Resolution", ref size.Width, 1, 1))
            Graphics.PostProcessor.Resolution = size;

        if (ImGui.Combo("Scaling mode", ref _currentSrm, _spriteRenderModes, _spriteRenderModes.Length))
            Graphics.PostProcessor.ScalingMode = Enum.Parse<SpriteRenderMode>(_spriteRenderModes[_currentSrm]);
        
        if (ImGui.Combo("Draw mode", ref _currentTdm, _targetDrawModes, _targetDrawModes.Length))
            Graphics.PostProcessor.DrawMode = Enum.Parse<PostProcessor.TargetDrawMode>(_targetDrawModes[_currentTdm]);
    }
}