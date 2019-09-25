using UnityEngine;

public class InsetTool : ITool
{
    private MeshManager mesh_manager;
    private int player_id;

    private float prev_dist;
    private Vector3 center;

    public InsetTool(int player_id)
    {
        Globals glob = Object.FindObjectOfType<Globals>();
        mesh_manager = glob.meshManager;
        this.player_id = player_id;
    }

    public void Apply(IInputParser input)
    {
        Vector3 pos = input.GetTransform().position;
        float dist;

        if (input.ToolBoolDown())
        {
            mesh_manager.Extrude(player_id);

            center = mesh_manager.GetCenter(player_id);
            dist = Vector3.Magnitude(pos - center);
        }
        else
        {
            dist = Vector3.Magnitude(pos - center);
            mesh_manager.Scale(player_id, dist / prev_dist);
        }

        prev_dist = dist;
    }

}
