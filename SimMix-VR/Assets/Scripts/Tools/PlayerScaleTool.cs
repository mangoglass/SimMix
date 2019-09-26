using UnityEngine;
using Valve.VR;

internal class PlayerScaleTool : ITool 
{
    private Vector3 oldPos;
    private float scale;
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
        Vector3 pos = input.GetTransform().localPosition;
        float toolValue = input.ToolTriggerValue();
        bool toolClick = input.ToolBool();

        if(toolValue > scalingTriggerThreshhold && input.ToolLastTriggerValue() != 0) 
        {
            Transform cameraRig = SteamVR_Render.Top().origin;
            scale = cameraRig.localScale.x;
            Vector3 diffVector = pos - oldPos;
            float z = diffVector.y;
            z *= scale;
            z *= (toolClick ? clickScalingFactor : scalingFactor);
            scale -= z;
            cameraRig.localScale = new Vector3(scale, scale, scale);
        }

        oldPos = pos;
    }
    
}
