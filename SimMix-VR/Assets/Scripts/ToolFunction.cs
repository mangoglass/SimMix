
public class ToolFunction : IFunction {

    public enum ToolEnum { createPrimitive=0, translate=1,rotate=2,scale=3,extrude=4,color=5,magic=6,inset=7,switchMode=8};
    public ToolEnum equippedTool;

    private ITool[] tools;

    public int player_id;
    private MeshManager mesh_manager;

    public ToolFunction(float minToolVisibillity, int player_id, MeshManager mesh_manager)
    {
        this.player_id = player_id;
        this.mesh_manager = mesh_manager;

        equippedTool = ToolEnum.translate;

        tools = new ITool[]
        {
            new PrimitiveTool(minToolVisibillity),
            new TranslateTool(player_id),
            new RotateTool(player_id),
            new ScaleTool(player_id),
            new ExtrudeTool(player_id),
            new ColorTool(player_id),
            new MagicTool(player_id),
            new InsetTool(player_id),
            new SwitchModeTool(player_id)
        };


    }

    public bool Call(IInputParser input) 
    {
        //switch (equippedTool) 
        //{
        //    case ToolEnum.createPrimitive:
        //        tools[(int)ToolEnum.createPrimitive].Apply(input);
        //        break;

        //    case ToolEnum.translate:
        //        tools[(int)ToolEnum.translate].Apply(input);
        //        break;
        //}
        tools[(int)equippedTool].Apply(input);

        bool maintainState = true;

        if (input.ToolTriggerValue() == 0 && (input.SwapBool() || input.MenuDisplayBool() || input.TeleportBool())) 
        {
            maintainState = false;

        }
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
