﻿using UnityEngine;

public class ExtrudeTool : ITool
{
    private MeshManager mesh_manager;
    private int player_id;

    private Vector3 last_pos;

    public ExtrudeTool(int player_id)
    {
        Globals glob = Object.FindObjectOfType<Globals>();
        mesh_manager = glob.meshManager;
        this.player_id = player_id;
    }

    public void Apply(IInputParser input)
    {
        Vector3 pos = input.GetTransform().position;

        if (input.ToolLastTriggerValue() == 0) 
        {
            mesh_manager.Extrude(player_id);
        }
        else
        {
            mesh_manager.Translate(player_id, pos - last_pos);
        }

        last_pos = pos;
    }

}
