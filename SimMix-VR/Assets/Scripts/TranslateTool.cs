using UnityEngine;

public class TranslateTool : ITool 
{
    public double min_select_dist;
    public Color mark_color;

    public Vector3 last_pos;
    public int selected_mesh_id;
    int prev_marked_mesh_id;

    private MeshManager mesh_manager;

    public TranslateTool() 
    {
        Globals glob = Object.FindObjectOfType<Globals>();

        mesh_manager = glob.meshManager;
        min_select_dist = 0.3f;
        mark_color = new Color(0.9622642f, 0.4039694f, 0.8832362f);
        prev_marked_mesh_id = -1;
    }

    public void Apply(IInputParser input) 
    {
        Vector3 pos = input.GetTransform().position;
        UnmarkAll();

        if (input.ToolBoolDown()) {
            selected_mesh_id = GetClosestId(pos);
            last_pos = pos;
        }

        if (input.ToolBool() && selected_mesh_id > -1) {
            mesh_manager.Translate(selected_mesh_id, pos - last_pos);
            last_pos = pos;
            MarkSelected();
        }
        else {
            MarkClosest(pos);
        }
    }

    private void UnmarkAll() {
        if (prev_marked_mesh_id > -1) {
            mesh_manager.SetColor(prev_marked_mesh_id, Color.white);
        }
    }

    private void MarkSelected() {
        if (selected_mesh_id > -1) {
            mesh_manager.SetColor(selected_mesh_id, mark_color);
        }

        prev_marked_mesh_id = selected_mesh_id;
    }

    private void MarkClosest(Vector3 position) {
        int mesh_id = GetClosestId(position);

        if (mesh_id > -1) {
            mesh_manager.SetColor(mesh_id, mark_color);
        }

        prev_marked_mesh_id = mesh_id;
    }

    private int GetClosestId(Vector3 position) {
        (int mesh, float dist) = mesh_manager.GetClosestId(position);
        return (dist < min_select_dist) ? mesh : -1;
    }
}
