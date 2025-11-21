using ImGuiNET;
using Arch.Core;
using Isotope.Core.Components;

namespace Isotope.Client.Editor.Panels
{
    public class HierarchyPanel
    {
        private EditorContext _ctx;
        private QueryDescription _query = new QueryDescription().WithAll<TransformComponent, NameComponent>();

        public HierarchyPanel(EditorContext ctx) { _ctx = ctx; }

        public void Draw()
        {
            ImGui.Begin("Hierarchy");

            if (ImGui.Button("+ Create Entity"))
            {
                var e = _ctx.World.Create(
                    new TransformComponent { Position = new System.Numerics.Vector2(0,0) },
                    new NameComponent { Name = "New Entity" }
                );
                _ctx.Select(e);
            }

            ImGui.Separator();

            _ctx.World.Query(in _query, (Entity entity, ref NameComponent name) =>
            {
                ImGuiTreeNodeFlags flags = ImGuiTreeNodeFlags.OpenOnArrow | ImGuiTreeNodeFlags.SpanAvailWidth;
                if (_ctx.SelectedEntity == entity) flags |= ImGuiTreeNodeFlags.Selected;

                bool opened = ImGui.TreeNodeEx($"#{entity.Id}: {name.Name}", flags | ImGuiTreeNodeFlags.Leaf);

                if (ImGui.IsItemClicked())
                {
                    _ctx.Select(entity);
                }

            if (ImGui.BeginPopupContextItem())
            {
                if (ImGui.MenuItem("Delete"))
                {
                    _ctx.World.Destroy(entity);
                    if (_ctx.SelectedEntity == entity) _ctx.Select(Entity.Null);
                }
                ImGui.EndPopup();
            }

            if (ImGui.BeginDragDropSource())
            {
                unsafe { int id = entity.Id; ImGui.SetDragDropPayload("ENTITY_DRAG", (IntPtr)(&id), sizeof(int)); }
                ImGui.Text(name.Name);
                ImGui.EndDragDropSource();
            }

            if (ImGui.BeginDragDropTarget())
            {
                var payload = ImGui.AcceptDragDropPayload("ENTITY_DRAG");
                if (payload.NativePtr != null)
                {
                    int draggedId = unsafe { *(int*)payload.Data };
                    var draggedEntity = _ctx.World.Get(draggedId);
                    if(draggedEntity != Entity.Null)
                    {
                        ref var t = ref _ctx.World.Get<TransformComponent>(draggedEntity);
                        t.Parent = entity;
                    }
                }
                ImGui.EndDragDropTarget();
            }

                if (opened) ImGui.TreePop();
            });

            ImGui.End();
        }
    }
}
