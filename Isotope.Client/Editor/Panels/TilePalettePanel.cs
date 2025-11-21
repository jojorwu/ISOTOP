using ImGuiNET;
using Isotope.Client.Rendering;
using Isotope.Core.Tiles;
using System;
using System.Numerics;

namespace Isotope.Client.Editor.Panels
{
    public class TilePalettePanel
    {
        private EditorContext _ctx;

        public TilePalettePanel(EditorContext ctx)
        {
            _ctx = ctx;
        }

        public void Draw()
        {
            ImGui.Begin("Tile Palette");

            float windowVisibleX = ImGui.GetWindowPos().X + ImGui.GetWindowContentRegionMax().X;
            ImGuiStylePtr style = ImGui.GetStyle();

            var allTiles = TileRegistry.GetAllDefinitions();

            foreach (var tileDef in allTiles)
            {
                if(tileDef == null || tileDef.Id == 0) continue;

                var tex = ResourceManager.GetTexture(tileDef.TexturePath);
                IntPtr texId = (IntPtr)tex.Id;

                ImGui.PushID(tileDef.Id);

                if (_ctx.SelectedTileId == tileDef.Id)
                {
                    ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(1, 1, 0, 1));
                }

                if (ImGui.ImageButton(texId, new Vector2(32, 32)))
                {
                    _ctx.SelectedTileId = tileDef.Id;
                }

                if (_ctx.SelectedTileId == tileDef.Id)
                {
                    ImGui.PopStyleColor();
                }

                ImGui.PopID();

                float lastButtonX = ImGui.GetItemRectMax().X;
                float nextButtonX = lastButtonX + style.ItemSpacing.X + 32;

                if (nextButtonX < windowVisibleX)
                {
                    ImGui.SameLine();
                }
            }

            ImGui.End();
        }
    }
}
