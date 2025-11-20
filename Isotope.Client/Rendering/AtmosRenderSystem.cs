using System;
using Isotope.Core.Atmos;
using Raylib_cs;
using System.Numerics;

namespace Isotope.Client.Rendering
{
    public class AtmosRenderSystem
    {
        private GasMap _gasMap;

        public AtmosRenderSystem(GasMap gasMap)
        {
            _gasMap = gasMap;
        }

        public void Draw(Camera2D cam)
        {
            // Simple culling
            int startX = (int)(cam.Target.X - cam.Offset.X / cam.Zoom) / 32 - 1;
            int startY = (int)(cam.Target.Y - cam.Offset.Y / cam.Zoom) / 32 - 1;
            int endX = (int)(cam.Target.X + cam.Offset.X / cam.Zoom) / 32 + 2;
            int endY = (int)(cam.Target.Y + cam.Offset.Y / cam.Zoom) / 32 + 2;

            startX = Math.Clamp(startX, 0, _gasMap.Width);
            startY = Math.Clamp(startY, 0, _gasMap.Height);
            endX = Math.Clamp(endX, 0, _gasMap.Width);
            endY = Math.Clamp(endY, 0, _gasMap.Height);

            for (int y = startY; y < endY; y++)
            {
                for (int x = startX; x < endX; x++)
                {
                    int i = y * _gasMap.Width + x;

                    if (_gasMap.Plasma[i] > 10.0f)
                    {
                        float alpha = Math.Min(_gasMap.Plasma[i] / 100f, 0.8f);
                        Color color = new Color(255, 0, 255, (int)(alpha * 255));

                        Vector2 pos = new Vector2(x * 32, y * 32);
                        Raylib.DrawRectangleV(pos, new Vector2(32, 32), color);
                    }
                }
            }
        }
    }
}
