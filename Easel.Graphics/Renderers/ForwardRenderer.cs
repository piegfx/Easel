using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using Easel.Graphics.Lighting;
using Easel.Graphics.Renderers.Structs;
using Easel.Math;
using Pie;
using Color = Easel.Math.Color;

namespace Easel.Graphics.Renderers;

public sealed class ForwardRenderer : IRenderer
{
    private List<TransformedRenderable> _opaques;
    private List<TransformedRenderable> _translucents;
    private List<Sprite> _opaqueSprites;

    private ProjViewModel _projViewModel;
    private GraphicsBuffer _projViewModelBuffer;

    private SceneInfo _sceneInfo;
    private GraphicsBuffer _sceneInfoBuffer;

    private DepthStencilState _depthState;

    private EffectLayout _shadowEffect;
    private Pie.RasterizerState _shadowRasterizer;

    private Effect _postProcessEffect;
    
    public ForwardRenderer(EaselGraphics graphics, Size<int> initialResolution)
    {
        _opaques = new List<TransformedRenderable>();
        _translucents = new List<TransformedRenderable>();
        _opaqueSprites = new List<Sprite>();
        _projViewModel = new ProjViewModel();

        MainTarget = new RenderTarget(initialResolution, autoDispose: false);
        
        graphics.SwapchainResized += GraphicsOnSwapchainResized;

        GraphicsDevice device = graphics.PieGraphics;

        _projViewModelBuffer = device.CreateBuffer(BufferType.UniformBuffer, _projViewModel, true);

        _sceneInfo = new SceneInfo();
        _sceneInfoBuffer = device.CreateBuffer(BufferType.UniformBuffer, _sceneInfo, true);

        _depthState = device.CreateDepthStencilState(DepthStencilStateDescription.LessEqual);

        InputLayout layout =
            device.CreateInputLayout(new InputLayoutDescription(Format.R32G32B32_Float, 0, 0, InputType.PerVertex));

        _shadowEffect =
            new EffectLayout(Effect.FromPath("Easel.Graphics.Shaders.Shadow_vert.spv", "Easel.Graphics.Shaders.Shadow_frag.spv"), 
                layout, VertexPositionTextureNormalTangent.SizeInBytes);

        _shadowRasterizer = device.CreateRasterizerState(new RasterizerStateDescription(CullFace.Front,
            CullDirection.CounterClockwise, FillMode.Solid, false));

        //_postProcessEffect = Effect.FromPath("Easel.Graphics.Shaders.PostProcess.PostProcess_vert.spv",
        //    "Easel.Graphics.Shaders.PostProcess.PostProcess_frag.spv");
    }

    private void GraphicsOnSwapchainResized(Size<int> size)
    {
        MainTarget.Dispose();
        MainTarget = new RenderTarget(size, autoDispose: false);
    }
    
    public DirectionalLight? DirectionalLight { get; set; }
    public RenderTarget MainTarget { get; set; }

    public void Draw(in Renderable renderable, in Matrix4x4 world)
    {
        if (renderable.Material.IsTranslucent)
            _translucents.Add(new TransformedRenderable(renderable, world));
        else
            _opaques.Add(new TransformedRenderable(renderable, world));
    }

    public void DrawSprite(in Sprite sprite)
    {
        _opaqueSprites.Add(sprite);
    }

    public void NewFrame()
    {
        _opaques.Clear();
        _translucents.Clear();
        _opaqueSprites.Clear();

        EaselGraphics graphics = EaselGraphics.Instance;
        graphics.SetRenderTarget(MainTarget);
    }

    public void DoneFrame()
    {
        EaselGraphics graphics = EaselGraphics.Instance;
        
        // TODO: EaselGraphics main RT.
        graphics.SetRenderTarget(null);
        
        graphics.Clear(Color.Black);
        graphics.SpriteRenderer.Begin(blendState: BlendState.DisabledRgbMask /*effect: _postProcessEffect*/);
        graphics.SpriteRenderer.Draw(MainTarget, Vector2T<float>.Zero, null, Color.White, 0, Vector2T<float>.Zero, Vector2T<float>.One);
        graphics.SpriteRenderer.End();
    }

    public void Perform3DPass(CameraInfo camera)
    {
        EaselGraphics graphics = EaselGraphics.Instance;
        GraphicsDevice device = graphics.PieGraphics;
        
        _sceneInfo.Sun = DirectionalLight?.ShaderDirLight ?? new ShaderDirLight();

        // TODO: Optimize this if possible
        IOrderedEnumerable<TransformedRenderable> opaques =
            _opaques.OrderBy(renderable => Vector3.Distance(renderable.Transform.Translation, camera.Position));
        
        IOrderedEnumerable<TransformedRenderable> translucents =
            _translucents.OrderBy(renderable => -Vector3.Distance(renderable.Transform.Translation, camera.Position));
        
        // First perform depth-only shadow pass

        /*if (DirectionalLight?.ShadowMap != null)
        {
            Matrix4x4 proj = Matrix4x4.CreateOrthographicOffCenter(-5.0f, 5.0f, -5.0f, 5.0f, 1.0f, 10.0f);
            Matrix4x4 view = Matrix4x4.CreateLookAt(
                -new Vector3(_sceneInfo.Sun.Direction.X, _sceneInfo.Sun.Direction.Y, _sceneInfo.Sun.Direction.Z),
                Vector3.Zero, Vector3.UnitY);
            Matrix4x4 lightSpace = proj * view;
            device.SetFramebuffer(DirectionalLight.Value.ShadowMap.Framebuffer);
            device.Clear(ClearFlags.Depth);
            // TODO: Optimize and set viewport to FB size.
            device.Viewport = new Rectangle(0, 0, 1024, 1024);

            CameraInfo info = new CameraInfo(proj, view); 
            _projViewModel.Projection = info.Projection;
            _projViewModel.View = info.View;
            _projViewModel.LightSpace = lightSpace;
            device.SetDepthState(_depthState);
            device.SetShader(_shadowEffect.Effect.PieShader);
            device.SetRasterizerState(_shadowRasterizer);
            
            foreach (TransformedRenderable renderable in _opaques)
                DrawShadowedRenderable(device, renderable);
            
            foreach (TransformedRenderable renderable in _translucents)
                DrawShadowedRenderable(device, renderable);
        }*/
        
        // Then perform main color pass.
        
        //graphics.SetRenderTarget(MainTarget);
        //device.Viewport = new Rectangle(0, 0, 1280, 720);

        if (camera.ClearColor.HasValue)
            EaselGraphics.Instance.Clear(camera.ClearColor.Value);
        
        _projViewModel.Projection = camera.Projection;
        _projViewModel.View = camera.View;

        device.SetPrimitiveType(PrimitiveType.TriangleList);
        device.SetDepthStencilState(_depthState);

        // Draw front-to-back for opaques.
        // This is to save a bit of GPU time so it doesn't process fragments that are covered by objects in front.
        foreach (TransformedRenderable renderable in opaques)
            DrawRenderable(device, camera, renderable);
        
        // Lastly draw the skybox.
        camera.Skybox?.Draw(camera.Projection, camera.View);
        
        foreach (TransformedRenderable renderable in translucents)
            DrawRenderable(device, camera, renderable);
    }
    
    private void DrawRenderable(GraphicsDevice device, in CameraInfo camera, in TransformedRenderable renderable)
    {
        _projViewModel.Model = renderable.Transform;
        device.UpdateBuffer(_projViewModelBuffer, 0, _projViewModel);

        _sceneInfo.Material = renderable.Renderable.Material.ShaderMaterial;
        _sceneInfo.CameraPos = new Vector4(camera.Position, 1.0f);
        device.UpdateBuffer(_sceneInfoBuffer, 0, _sceneInfo);

        device.SetShader(renderable.Renderable.Material.EffectLayout.Effect.PieShader);
        device.SetUniformBuffer(0, _projViewModelBuffer);
        device.SetUniformBuffer(1, _sceneInfoBuffer);
        if (DirectionalLight?.ShadowMap != null)
            device.SetTexture(2, DirectionalLight.Value.ShadowMap.Texture, DirectionalLight.Value.ShadowMap.SamplerState);
        renderable.Renderable.Material.ApplyTextures(device);
        device.SetRasterizerState(renderable.Renderable.Material.RasterizerState.PieRasterizerState);
        device.SetBlendState(renderable.Renderable.Material.BlendState.PieBlendState);

        device.SetVertexBuffer(0, renderable.Renderable.VertexBuffer,
            renderable.Renderable.Material.EffectLayout.Stride,
            renderable.Renderable.Material.EffectLayout.Layout);
        device.SetIndexBuffer(renderable.Renderable.IndexBuffer, IndexType.UInt);
        device.DrawIndexed(renderable.Renderable.NumIndices);
    }

    private void DrawShadowedRenderable(GraphicsDevice device, in TransformedRenderable renderable)
    {
        _projViewModel.Model = renderable.Transform;
        device.UpdateBuffer(_projViewModelBuffer, 0, _projViewModel);
        
        device.SetUniformBuffer(0, _projViewModelBuffer);

        device.SetVertexBuffer(0, renderable.Renderable.VertexBuffer, _shadowEffect.Stride, _shadowEffect.Layout);
        device.SetIndexBuffer(renderable.Renderable.IndexBuffer, IndexType.UInt);
        device.DrawIndexed(renderable.Renderable.NumIndices);
    }
    
    public void Perform2DPass(CameraInfo camera)
    {
        if (camera.ClearColor.HasValue)
            EaselGraphics.Instance.Clear(camera.ClearColor.Value);
        
        EaselGraphics graphics = EaselGraphics.Instance;
        _opaqueSprites.Sort((sprite, sprite1) => sprite.Position.Z.CompareTo(sprite1.Position.Z));
        
        graphics.SpriteRenderer.Begin(projection: camera.Projection, transform: camera.View);

        for (int i = 0; i < _opaqueSprites.Count; i++)
        {
            Sprite sprite = _opaqueSprites[i];
            graphics.SpriteRenderer.Draw(sprite.Texture, new Vector2T<float>(sprite.Position.X, sprite.Position.Y),
                sprite.Source, sprite.Tint, sprite.Rotation, sprite.Origin, sprite.Scale, sprite.Flip);
        }
        
        graphics.SpriteRenderer.End();
    }

    public void Dispose()
    {
        MainTarget.Dispose();
        _projViewModelBuffer.Dispose();
    }
}