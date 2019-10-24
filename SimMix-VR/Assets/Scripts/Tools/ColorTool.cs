using UnityEngine;

public class ColorTool : ITool
{
    private MeshManager mesh_manager;
    private int player_id;

    public ColorTool(int player_id)
    {
        Globals glob = Object.FindObjectOfType<Globals>();
        mesh_manager = glob.meshManager;
        this.player_id = player_id;
    }

    public void Apply(IInputParser input, bool isFirstFrame)
    {
        //if (input.ToolBoolDown())
        //{
        //    mesh_manager.SetColor(player_id, Random.ColorHSV(0f, 1f, 0f, 0.5f, 1f, 1f));
        //}
        Vector3 ea = input.GetTransform().eulerAngles;
        mesh_manager.SetColor(player_id, new Color(ea.x / 360.0f,ea.y / 360.0f, ea.z / 360.0f));
    }

}
