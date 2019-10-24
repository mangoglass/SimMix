
using UnityEngine;

public class ToolFunction : IFunction {


    public enum ToolEnum { createPrimitive=0, translate=1, rotate=2, scale=3, extrude=4, color=5, inset=6, userScale=7,copy=8,delete=9 };
    public string[] toolnames =
    { "Create","Translate","Rotate","Scale","Extrude","Color","Inset","User Scale","Copy","Delete" };


    public ToolEnum equippedTool;

    private ITool[] tools;
    private float triggerThreshold;
    private int player_id;
    private MeshManager mesh_manager;
    private bool isFirstFrame;

    public ToolFunction(Vector3 toolStartPos, int player_id, MeshManager mesh_manager)
    {
        this.player_id = player_id;
        this.mesh_manager = mesh_manager;
        isFirstFrame = true;

        equippedTool = ToolEnum.translate;

        tools = new ITool[]
        {
            new PrimitiveTool(),
            new TranslateTool(player_id),
            new RotateTool(player_id),
            new ScaleTool(player_id),
            new ExtrudeTool(player_id),
            new ColorTool(player_id),
            new InsetTool(player_id),
            new PlayerScaleTool(toolStartPos),
            new CopyTool(player_id),
            new DeleteTool(player_id),

        };

        Globals globals = Object.FindObjectOfType<Globals>();
        triggerThreshold = globals.toolTriggerThreshold;
    }

    public bool Call(IInputParser input) 
    {
        tools[(int)equippedTool].Apply(input, isFirstFrame);

        bool maintainState = input.ToolTriggerValue() > triggerThreshold;
        mesh_manager.SetUpdateSelected(player_id, !maintainState);
        isFirstFrame = !maintainState;

        return maintainState; 
    }

    public ITool Swap(ITool tool, ToolEnum type) 
    {
        ITool equiped = tools[(int)equippedTool];
        ITool outTool = equiped;

        return outTool;
    }
}
