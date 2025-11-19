using Arch.Core;
using Isotope.Core.Components;
using Isotope.Core.Map;
using Raylib_cs;
using System.Numerics;

namespace Isotope.Client.Rendering;

public class LightingPass
{
    private RenderTexture2D _lightMap;
    private ShadowCaster _shadowCaster;

    public LightingPass(int width, int height)
    {
        _lightMap = Raylib.LoadRenderTexture(width, height);
        _shadowCaster = new ShadowCaster();
    }

    public void Unload()
    {
        Raylib.UnloadRenderTexture(_lightMap);
    }

    public void DrawLights(World world, WorldMap map, Camera2D cam)
    {
        Raylib.BeginTextureMode(_lightMap);
        Raylib.ClearBackground(new Color(30, 30, 40, 255));
        Raylib.BeginBlendMode(BlendMode.Additive);

        var query = new QueryDescription().WithAll<TransformComponent, LightSource>();

        world.Query(in query, (ref TransformComponent t, ref LightSource l) =>
        {
            Vector2[] lightPolygonPoints = _shadowCaster.CalculateLightPolygon(map, t.WorldPosition, l.Radius);

            if (lightPolygonPoints.Length < 3) return;

            var screenPolygon = new Vector2[lightPolygonPoints.Length];
            for(int i=0; i < lightPolygonPoints.Length; i++)
            {
                screenPolygon[i] = Raylib.GetWorldToScreen2D(lightPolygonPoints[i], cam);
            }

            Raylib.DrawTriangleFan(screenPolygon, screenPolygon.Length, l.Color);
        });

        Raylib.EndBlendMode();
        Raylib.EndTextureMode();
    }

    public void RenderToScreen()
    {
        Raylib.BeginBlendMode(BlendMode.Multiply);
        Rectangle src = new Rectangle(0, 0, _lightMap.texture.width, -_lightMap.texture.height);
        Raylib.DrawTextureRec(_lightMap.texture, src, Vector2.Zero, Color.White);
        Raylib.EndBlendMode();
    }
}
