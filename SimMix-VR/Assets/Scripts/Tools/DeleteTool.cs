using UnityEngine;

public class DeleteTool : ITool 
{
    private MeshManager mesh_manager;
    private int player_id;

    public DeleteTool(int player_id) 
    {
        Globals glob = Object.FindObjectOfType<Globals>();
        mesh_manager = glob.meshManager;
        this.player_id = player_id;
    }

    public void Apply( IInputParser input) 
    {

        if (input.ToolLastTriggerValue() == 0)
        {
            mesh_manager.Delete(player_id);
        }

    }
}
