
using UnityEngine;

public class ToolFunction : IFunction {


    public enum ToolEnum { createPrimitive=0, translate=1,rotate=2,scale=3,extrude=4,color=5,magic=6,inset=7,switchMode=8,userScale=9};
    public ToolEnum equippedTool;

    private ITool[] tools;

    public int player_id;
    private MeshManager mesh_manager;

    public ToolFunction(Vector3 toolStartPos, int player_id, MeshManager mesh_manager)
    {
        this.player_id = player_id;
        this.mesh_manager = mesh_manager;

        equippedTool = ToolEnum.translate;

        tools = new ITool[]
        {
            new PrimitiveTool(),
            new TranslateTool(player_id),
            new RotateTool(player_id),
            new ScaleTool(player_id),
            new ExtrudeTool(player_id),
            new ColorTool(player_id),
            new MagicTool(player_id),
            new InsetTool(player_id),
            new SwitchModeTool(player_id),
            new PlayerScaleTool(toolStartPos)
        };
    }

    public bool Call(IInputParser input) 
    {
        tools[(int)equippedTool].Apply(input);

        bool maintainState = input.ToolTriggerValue() > 0;
        mesh_manager.SetUpdateSelected(player_id, !maintainState);

        return maintainState; 
    }

    public ITool Swap(ITool tool, ToolEnum type) 
    {
        ITool equiped = tools[(int)equippedTool];
        ITool outTool = equiped;

        return outTool;
    }
}
