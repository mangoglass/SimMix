using UnityEngine;
using Valve.VR;

internal class PlayerScaleTool : ITool 
{
    private Vector3 oldPos;
    private Transform cameraRef;
    private float oldScale;
    private float newScale;
    private float scalingFactor;
    private float clickScalingFactor;
    private float scalingTriggerThreshhold;
    private float minScale;
    private float maxScale;
    
    public PlayerScaleTool(Vector3 startPos) 
    {
        oldPos = startPos;
        Globals globals = Object.FindObjectOfType<Globals>();
        scalingFactor = globals.scalingFactor;
        clickScalingFactor = globals.clickScalingFactor;
        scalingTriggerThreshhold = globals.scalingTriggerThreshold;
        cameraRef = globals.vrCameraRef;
        minScale = globals.minScale;
        maxScale = globals.maxScale;
    }

    public void Apply(IInputParser input, bool isFirstFrame) 
    {
        Vector3 pos = input.GetTransform().localPosition;
        float toolValue = input.ToolTriggerValue();
        bool toolClick = input.ToolBool();

        if(toolValue > scalingTriggerThreshhold && !isFirstFrame)
        {
            Transform cameraRig = SteamVR_Render.Top().origin;
            oldScale = cameraRig.localScale.x;
            Vector3 diffVector = pos - oldPos;
            float z = diffVector.y;
            z *= oldScale;
            z *= (toolClick ? clickScalingFactor : scalingFactor);
            newScale = oldScale - z;

            newScale = newScale < minScale ? minScale : (newScale > maxScale ? maxScale : newScale);
            cameraRig.localScale = new Vector3(newScale, newScale, newScale);

            float scaleDiff = oldScale - newScale;
            Vector3 posChange = cameraRef.localPosition * scaleDiff;
            Vector3 clp = cameraRig.localPosition;
            cameraRig.localPosition = new Vector3(clp.x + posChange.x, clp.y, clp.z + posChange.z);
        }

        oldPos = pos;
    }
}
