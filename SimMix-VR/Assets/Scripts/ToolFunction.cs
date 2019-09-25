
using UnityEngine;

public class ToolFunction : IFunction {

    public enum ToolEnum { createPrimitive=0, translate=1, scale=2 };
    public ToolEnum equippedTool;

    private ITool[] tools;

    public ToolFunction(Vector3 toolStartPos)
    {
        equippedTool = ToolEnum.translate;

        tools = new ITool[]
        {
            new PrimitiveTool(),
            new TranslateTool(),
            new PlayerScaleTool(toolStartPos)
        };
    }

    public bool Call(IInputParser input) 
    {
        tools[(int)equippedTool].Apply(input);

        return input.ToolTriggerValue() > 0 || (!input.SwapBool() && !input.MenuDisplayBool() && !input.TeleportBool()); 
    }

    public ITool Swap(ITool tool, ToolEnum type) 
    {
        ITool equiped = tools[(int)equippedTool];
        ITool outTool = equiped;

        return outTool;
    }
}
