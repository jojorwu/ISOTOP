using Arch.Core;
using Isotope.Core.Components;
using Isotope.Core.Map;
using Raylib_cs;
using System.Numerics;

namespace Isotope.Client.Rendering;

/// <summary>
/// A rendering pass that handles lighting and shadows.
/// </summary>
public class LightingPass
{
    private RenderTexture2D _lightMap;
    private ShadowCaster _shadowCaster;

    /// <summary>
    /// Initializes a new instance of the <see cref="LightingPass"/> class.
    /// </summary>
    /// <param name="width">The width of the light map.</param>
    /// <param name="height">The height of the light map.</param>
    public LightingPass(int width, int height)
    {
        _lightMap = Raylib.LoadRenderTexture(width, height);
        _shadowCaster = new ShadowCaster();
    }

    /// <summary>
    /// Unloads the light map texture.
    /// </summary>
    public void Unload()
    {
        Raylib.UnloadRenderTexture(_lightMap);
    }

    /// <summary>
    /// Draws all light sources in the world to the light map.
    /// </summary>
    /// <param name="world">The game world.</param>
    /// <param name="map">The world map.</param>
    /// <param name="cam">The game camera.</param>
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

    /// <summary>
    /// Renders the light map to the screen.
    /// </summary>
    public void RenderToScreen()
    {
        Raylib.BeginBlendMode(BlendMode.Multiply);
        Rectangle src = new Rectangle(0, 0, _lightMap.texture.width, -_lightMap.texture.height);
        Raylib.DrawTextureRec(_lightMap.texture, src, Vector2.Zero, Color.White);
        Raylib.EndBlendMode();
    }
}
