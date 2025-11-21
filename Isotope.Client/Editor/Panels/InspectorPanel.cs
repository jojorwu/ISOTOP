using ImGuiNET;
using Arch.Core;
using System.Numerics;
using Isotope.Core.Components;

namespace Isotope.Client.Editor.Panels
{
    public class InspectorPanel
    {
        private EditorContext _ctx;

        public InspectorPanel(EditorContext ctx) { _ctx = ctx; }

        public void Draw()
        {
            ImGui.Begin("Inspector");

            if (_ctx.SelectedEntity == Entity.Null || !_ctx.World.IsAlive(_ctx.SelectedEntity))
            {
                ImGui.TextDisabled("Select an entity to view properties.");
                ImGui.End();
                return;
            }

            Entity e = _ctx.SelectedEntity;

            ImGui.Text($"ID: {e.Id}");
            ImGui.Separator();

            if (_ctx.World.Has<NameComponent>(e))
            {
                ref var c = ref _ctx.World.Get<NameComponent>(e);
                string name = c.Name ?? "";
                if (ImGui.InputText("Name", ref name, 64)) c.Name = name;
            }

            if (_ctx.World.Has<TransformComponent>(e))
            {
                if (ImGui.CollapsingHeader("Transform", ImGuiTreeNodeFlags.DefaultOpen))
                {
                    ref var t = ref _ctx.World.Get<TransformComponent>(e);
                    ImGui.DragFloat2("Position", ref t.Position);
                }
            }

            if (_ctx.World.Has<SpriteComponent>(e))
            {
                if (ImGui.CollapsingHeader("Sprite", ImGuiTreeNodeFlags.DefaultOpen))
                {
                    ref var s = ref _ctx.World.Get<SpriteComponent>(e);
                    string path = s.TexturePath ?? "";
                    if (ImGui.InputText("Texture", ref path, 128)) s.TexturePath = path;
                    Vector4 col = new Vector4(s.Tint.R/255f, s.Tint.G/255f, s.Tint.B/255f, s.Tint.A/255f);
                    if (ImGui.ColorEdit4("Tint", ref col))
                    {
                        s.Tint = new Raylib_cs.Color((byte)(col.X*255), (byte)(col.Y*255), (byte)(col.Z*255), (byte)(col.W*255));
                    }
                }
            }

            ImGui.Separator();
            if (ImGui.Button("Add Component"))
            {
                ImGui.OpenPopup("AddComponentPopup");
            }

            if (ImGui.BeginPopup("AddComponentPopup"))
            {
                if (ImGui.MenuItem("Sprite")) _ctx.World.Add(e, new SpriteComponent());
                if (ImGui.MenuItem("Light Source")) _ctx.World.Add(e, new LightSource());
                ImGui.EndPopup();
            }

            if (_ctx.World.Has<LightSource>(e))
            {
                ImGui.Separator();
                bool open = ImGui.CollapsingHeader("Light Source", ImGuiTreeNodeFlags.DefaultOpen);

                if (ImGui.BeginPopupContextItem())
                {
                    if (ImGui.MenuItem("Remove Light")) _ctx.World.Remove<LightSource>(e);
                    ImGui.EndPopup();
                }

                if (open)
                {
                    ref var l = ref _ctx.World.Get<LightSource>(e);

                    Vector3 col = new Vector3(l.Color.R/255f, l.Color.G/255f, l.Color.B/255f);
                    if (ImGui.ColorEdit3("Color", ref col))
                    {
                        l.Color = new Raylib_cs.Color((byte)(col.X*255), (byte)(col.Y*255), (byte)(col.Z*255), 255);
                    }

                    float radius = l.Radius;
                    if (ImGui.DragFloat("Radius", ref radius, 1.0f, 0.0f, 1000.0f))
                    {
                        l.Radius = radius;
                    }
                }
            }

            if (_ctx.World.Has<DoorComponent>(e))
            {
                ImGui.Separator();
                if (ImGui.CollapsingHeader("Door", ImGuiTreeNodeFlags.DefaultOpen))
                {
                    ref var d = ref _ctx.World.Get<DoorComponent>(e);
                    if (ImGui.Checkbox("Is Open", ref d.IsOpen))
                    {
                        // Manually trigger the state change
                        ref var visual = ref _ctx.World.Get<SpriteStateComponent>(e);
                        visual.CurrentState = d.IsOpen ? "open" : "closed";
                    }
                    ImGui.Checkbox("Is Locked", ref d.IsLocked);
                    ImGui.DragFloat("Open Speed", ref d.OpenSpeed, 0.1f, 0.1f, 5.0f);
                }
            }

            ImGui.End();
        }
    }
}
