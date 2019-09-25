using UnityEngine;

public class RotateTool : ITool 
{
    private MeshManager mesh_manager;
    private int player_id;

    private Vector3 last_rotation;

    public RotateTool(int player_id) 
    {
        Globals glob = Object.FindObjectOfType<Globals>();
        mesh_manager = glob.meshManager;
        this.player_id = player_id;
    }

    public void Apply( IInputParser input) 
    {
        Vector3 rotation = input.GetTransform().eulerAngles;

        if (input.ToolLastTriggerValue() != 0) 
        {
            mesh_manager.Rotate(player_id, rotation - last_rotation);
        }

        last_rotation = rotation;
    }

}
