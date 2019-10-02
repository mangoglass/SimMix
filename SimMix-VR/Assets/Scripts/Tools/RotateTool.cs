using UnityEngine;

public class RotateTool : ITool 
{
    private MeshManager mesh_manager;
    private int player_id;

    private Vector3 last_pos;
    public Vector3 center;


    public RotateTool(int player_id) 
    {
        Globals glob = Object.FindObjectOfType<Globals>();
        mesh_manager = glob.meshManager;
        this.player_id = player_id;
    }

    public void Apply( IInputParser input) 
    {
        Vector3 position = input.GetTransform().position;

        if (input.ToolLastTriggerValue() == 0) 
        {
            center = mesh_manager.GetCenter(player_id);

        }else
        {
            Vector3 w1 = last_pos - center;
            Vector3 w2 = position - center;

            Vector3 a = Vector3.Cross(w1, w2);
            float w = Mathf.Sqrt(Vector3.Dot(w1, w1) * Vector3.Dot(w2, w2)) + Vector3.Dot(w1, w2);
            Quaternion q = new Quaternion(a.x, a.y, a.z, w);
            q.Normalize();
            mesh_manager.Rotate(player_id, q);
        }
        last_pos = position;


    }

}
