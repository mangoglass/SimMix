using UnityEngine;

public class MagicTool : ITool 
{
    private MeshManager mesh_manager;
    private int player_id;

    private Vector3 last_rotation;
    private Vector3 last_pos;

    public MagicTool(int player_id) 
    {
        Globals glob = Object.FindObjectOfType<Globals>();
        mesh_manager = glob.meshManager;
        this.player_id = player_id;
    }

    public void Apply( IInputParser input, bool isFirstFrame) 
    {
        Vector3 pos = input.GetTransform().position;
        Vector3 rotation = input.GetTransform().eulerAngles;

        if (!isFirstFrame) 
        {
            mesh_manager.Translate(player_id, pos - last_pos);
            //mesh_manager.Rotate(player_id, rotation - last_rotation);
        }

        last_pos = pos;
        last_rotation = rotation;
    }

}
