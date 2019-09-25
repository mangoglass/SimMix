using UnityEngine;

internal class PlayerScaleTool : ITool 
{
    private Vector3 oldPos;
    private float scalingFactor;
    private float clickScalingFactor;
    private float scalingTriggerThreshhold;
    
    public PlayerScaleTool(Vector3 startPos) 
    {
        oldPos = startPos;

        Globals globals = Object.FindObjectOfType<Globals>();
        scalingFactor = globals.scalingFactor;
        clickScalingFactor = globals.clickScalingFactor;
        scalingTriggerThreshhold = globals.scalingTriggerThreshold;
    }

    public void Apply(IInputParser input) 
    {
        Vector3 pos = input.GetTransform().position;
        float toolValue = input.ToolTriggerValue();
        bool toolClick = input.ToolBool();

        if(toolValue > scalingTriggerThreshhold) 
        {
            Vector3 diffVector = pos - oldPos;
            diffVector = new Vector3(0, 0, diffVector.z);

            if (toolClick) 
            {
                
            }

            else 
            {

            }
        }

        oldPos = pos;
    }
    
}
