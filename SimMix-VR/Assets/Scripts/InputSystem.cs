using System;
using UnityEngine;
using Valve.VR;

public class InputSystem : MonoBehaviour
{
    public bool forceReadInput = true;

    public enum Controller {left, right};
    private enum State {none, action, teleport, swap, menu};
    private enum Tool {placeCube, translate};

    public Controller controller;
    public SteamVR_Action_Boolean toolActionClick;
    public SteamVR_Action_Boolean teleport;
    public SteamVR_Action_Boolean headsetIsOn;
    public SteamVR_Action_Boolean swapAction;
    public SteamVR_Action_Boolean menuDisplay;
    public SteamVR_Action_Boolean menuClick;

    public SteamVR_Action_Single toolActionVariable;
    public SteamVR_Action_Vector2 menuLocation;
    public GameObject[] objects = new GameObject[3];

    private SteamVR_Behaviour_Pose m_Pose;
    private Tool equippedTool;
    private State state;


    void Awake() 
    {
        m_Pose = GetComponent<SteamVR_Behaviour_Pose>();
        equippedTool = Tool.placeCube;
        state = State.none;
        objects[0] = Instantiate(objects[0]);
        objects[0].SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (forceReadInput || headsetIsOn.GetState(SteamVR_Input_Sources.Head)) 
        {
            SteamVR_Input_Sources inSrc = m_Pose.inputSource;
            StateHandler(inSrc);
        }

        else if(headsetIsOn.GetStateUp(SteamVR_Input_Sources.Head))
        {
            Debug.Log("\n\n---- Player has taken off the headset: input reading turned off ----\n\n");
        }
    }

    private void StateHandler(SteamVR_Input_Sources inSrc) 
    {
        switch (state) 
        {
            case State.none:
                if (toolActionVariable.GetAxis(inSrc) > 0) 
                {
                    state = State.action;
                    Debug.Log(controller.ToString() + " entering tool action state");
                    ToolAction(inSrc);
                }

                else if (menuDisplay.GetState(inSrc)) 
                {
                    state = State.menu;
                    Debug.Log(controller.ToString() + " entering menu action state");
                    MenuAction(inSrc);
                }

                else if (teleport.GetState(inSrc)) {
                    state = State.teleport;
                    Debug.Log(controller.ToString() + " entering teleport action state");
                    TeleportAction(inSrc);
                }

                else if (swapAction.GetState(inSrc)) {
                    state = State.swap;
                    Debug.Log(controller.ToString() + " entering swaping action state");
                    SwapingAction(inSrc);
                }

                break;

            case State.action:
                ToolAction(inSrc);
                break;

            case State.menu:
                MenuAction(inSrc);
                break;

            case State.teleport:
                TeleportAction(inSrc);
                break;

            case State.swap:
                SwapingAction(inSrc);
                break;
        }
    }

    private void ToolAction(SteamVR_Input_Sources inSrc) 
    {
        switch (equippedTool) 
        {
            case Tool.placeCube:
                if (toolActionVariable.GetAxis(inSrc) > 0) 
                {
                    GameObject cube = objects[0];

                    if (toolActionVariable.GetChanged(inSrc)) 
                    {
                        cube.SetActive(true);
                    }

                    Vector3 pos = m_Pose.transform.position + 0.2f * m_Pose.transform.forward;
                    cube.transform.SetPositionAndRotation(pos, m_Pose.transform.rotation);

                    
                    Color c = cube.GetComponent<MeshRenderer>().material.color;
                    c.a = 0.1f + (toolActionVariable.GetAxis(inSrc) * 0.9f);
                    cube.GetComponent<MeshRenderer>().material.color = c;
                    

                    if (toolActionClick.GetStateUp(inSrc)) 
                    {
                        GameObject cubeClone = Instantiate(cube);
                        cubeClone.GetComponent<MeshRenderer>().material = new Material(cube.GetComponent<MeshRenderer>().material.shader);
                    }
                }

                if(toolActionVariable.GetChanged(inSrc) && toolActionVariable.GetAxis(inSrc) == 0) 
                {
                    objects[0].SetActive(false);
                }

                break;

            case Tool.translate:

                break;
        }

        if (toolActionVariable.GetAxis(inSrc) == 0) 
        {
            state = State.none;
        }
    }

    private void MenuAction(SteamVR_Input_Sources inSrc) 
    {
        if (!menuDisplay.GetState(inSrc)) {
            state = State.none;
        }
    }

    private void TeleportAction(SteamVR_Input_Sources inSrc) 
    {
        if (!teleport.GetState(inSrc)) {
            state = State.none;
        }
    }

    private void SwapingAction(SteamVR_Input_Sources inSrc) 
    {
        if (!swapAction.GetState(inSrc)) {
            state = State.none;
        }
    }
}
