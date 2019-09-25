
public class ToolFunction : IFunction {

    public enum ToolEnum { createPrimitive=0, translate=1 };
    public ToolEnum equippedTool;

    private ITool[] tools;

    public ToolFunction(float minToolVisibillity)
    {
        equippedTool = ToolEnum.translate;

        tools = new ITool[]
        {
            new PrimitiveTool(minToolVisibillity),
            new TranslateTool()
        };
    }

    public bool Call(IInputParser input) 
    {
        switch (equippedTool) 
        {
            case ToolEnum.createPrimitive:
                tools[(int)ToolEnum.createPrimitive].Apply(input);
                break;

            case ToolEnum.translate:
                tools[(int)ToolEnum.translate].Apply(input);
                break;
        }

        return input.ToolTriggerValue() > 0 || (!input.SwapBool() && !input.MenuDisplayBool() && !input.TeleportBool()); 
    }

    public ITool Swap(ITool tool, ToolEnum type) 
    {
        ITool equiped = tools[(int)equippedTool];
        ITool outTool = equiped;

        return outTool;
    }
}
