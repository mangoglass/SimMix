using Valve.VR;
using UnityEngine;

public class VRInputParser : MonoBehaviour, IInputParser {

    public enum Controller { left, right };

    public Controller controller;
    public SteamVR_Action_Boolean toolClick;
    public SteamVR_Action_Boolean teleportClick;
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

    public bool isLeftController() 
    {
        return controller == Controller.left;
    }

    public bool isRightController() 
    {
        return controller == Controller.right;
    }

    public bool MenuClickBool() 
    {
        return menuClick.GetState(m_Pose.inputSource);
    }

    public bool MenuDisplayBool() 
    {
        return menuDisplay.GetState(m_Pose.inputSource);
    }

    public bool MenuDisplayBoolDown() {
        return menuDisplay.GetStateDown(m_Pose.inputSource);
    }

    public bool MenuDisplayBoolUp() {
        return menuDisplay.GetStateUp(m_Pose.inputSource);
    }

    public bool SwapBool() 
    {
        return swapClick.GetState(m_Pose.inputSource);
    }

    public bool TeleportBool() 
    {
        return teleportClick.GetState(m_Pose.inputSource);
    }

    public bool TeleportBoolDown() {
        return teleportClick.GetStateDown(m_Pose.inputSource);
    }

    public bool TeleportBoolUp() {
        return teleportClick.GetStateUp(m_Pose.inputSource);
    }

    public bool ToolBool() 
    {
        return toolClick.GetState(m_Pose.inputSource);
    }

    public bool ToolBoolUp() 
    {
        return toolClick.GetStateUp(m_Pose.inputSource);
    }

    public bool ToolBoolDown() {
        return toolClick.GetStateDown(m_Pose.inputSource);
    }

    public bool ToolTriggerValueChanged() 
    {
        return toolVariable.GetChanged(m_Pose.inputSource);
    }

    public float ToolTriggerValue() 
    {
        return toolVariable.GetAxis(m_Pose.inputSource);
    }

    public Vector2 MenuPointerLocation() 
    {
        return menuLocation.GetAxis(m_Pose.inputSource);
    }
}
