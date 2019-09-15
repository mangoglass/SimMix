using Valve.VR;
using UnityEngine;

public class VRInputParser : MonoBehaviour, IInputParser {

    public enum Controller { left, right };

    public Controller controller;
    public SteamVR_Action_Boolean toolClick;
    public SteamVR_Action_Boolean teleportClick;
    public SteamVR_Action_Boolean headsetIsOn;
    public SteamVR_Action_Boolean swapClick;
    public SteamVR_Action_Boolean menuDisplay;
    public SteamVR_Action_Boolean menuClick;
    public SteamVR_Action_Single toolVariable;
    public SteamVR_Action_Vector2 menuLocation;

    private SteamVR_Behaviour_Pose m_Pose;

    void Awake() 
    {
        m_Pose = GetComponent<SteamVR_Behaviour_Pose>();
    }

    public Transform GetTransform() 
    {
        return m_Pose.transform;
    }

    public bool HeadsetBoolCheck() 
    {
        return headsetIsOn.GetState(SteamVR_Input_Sources.Head);
    }

    public bool HeadsetBoolReleased() 
    {
        return headsetIsOn.GetStateUp(SteamVR_Input_Sources.Head);
    }

    public bool isLeftController() 
    {
        return controller == Controller.left;
    }

    public bool isRightController() 
    {
        return controller == Controller.right;
    }

    public bool MenuClickBoolCheck() 
    {
        return menuClick.GetState(m_Pose.inputSource);
    }

    public bool MenuDisplayBoolCheck() 
    {
        return menuDisplay.GetState(m_Pose.inputSource);
    }

    public bool SwapBoolCheck() 
    {
        return swapClick.GetState(m_Pose.inputSource);
    }

    public bool TeleportBoolCheck() 
    {
        return teleportClick.GetState(m_Pose.inputSource);
    }

    public bool ToolBoolCheck() 
    {
        return toolClick.GetState(m_Pose.inputSource);
    }

    public bool ToolBoolReleased() 
    {
        return toolClick.GetStateUp(m_Pose.inputSource);
    }

    public bool ToolTriggerValueChanged() 
    {
        return toolVariable.GetChanged(m_Pose.inputSource);
    }

    public float ToolTriggerValue() 
    {
        return toolVariable.GetAxis(m_Pose.inputSource);
    }

    public Vector2 MenuTrackLocation() 
    {
        return menuLocation.GetAxis(m_Pose.inputSource);
    }
}
