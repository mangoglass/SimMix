using System;
using System.Collections;
using UnityEngine;

public enum Controller 
{
    left,
    right
}

public class InputSystem : MonoBehaviour
{
    private enum FunctionEnum { tool=0, teleport=1, swap=2, menu=3, none };

    public Controller controller;

    [Range(0f,1f)]
    public float minToolPrimitiveVisibility;

    private IInputParser inputParser;
    private IFunction[] functions;
    private FunctionEnum state;

    void Start() 
    {
        Globals glob = FindObjectOfType<Globals>();

        MeshManager mesh_manager = glob.meshManager;
        int player_id = mesh_manager.RegisterPlayer(gameObject.transform);

        inputParser = GetComponent<IInputParser>();
        state = FunctionEnum.none;
        int nrOfTools = Enum.GetNames(typeof(ToolFunction.ToolEnum)).Length;

        GameObject controllerModel = gameObject.transform.Find("Model").gameObject;
        ToolFunction toolFunction = new ToolFunction(inputParser.GetTransform().position,player_id,mesh_manager);

        functions = new IFunction[]
        {
            toolFunction,
            new TeleportFunction(inputParser.GetTransform()),
            new ChangeModeFunction(player_id, inputParser.GetTransform(), controller, controllerModel),
            new MenuFunction(nrOfTools, inputParser.GetTransform(), toolFunction)
        };

        StartCoroutine(OutLineCreator(1f, controllerModel, glob.modeColor[0]));
    }

    // Update is called once per frame
    void Update()
    {
         StateHandler();
    }

    private void StateHandler() 
    {
        bool maintainState;

        switch (state) 
        {
            case FunctionEnum.none:
                if (inputParser.ToolTriggerValue() > 0) 
                {
                    state = FunctionEnum.tool;
                    maintainState = functions[(int)FunctionEnum.tool].Call(inputParser);
                    Debug.Log(controller.ToString() + " entering tool action state");
                }

                else if (inputParser.MenuDisplayBool()) 
                {
                    state = FunctionEnum.menu;
                    maintainState = functions[(int)FunctionEnum.menu].Call(inputParser);
                    Debug.Log(controller.ToString() + " entering menu action state");
                }

                else if (inputParser.TeleportBool()) 
                {
                    state = FunctionEnum.teleport;
                    maintainState = functions[(int)FunctionEnum.teleport].Call(inputParser);
                    Debug.Log(controller.ToString() + " entering teleport action state");
                }

                else if (inputParser.SwapBool()) 
                {
                    state = FunctionEnum.swap;
                    maintainState = functions[(int)FunctionEnum.swap].Call(inputParser);
                    Debug.Log(controller.ToString() + " entering swaping action state");
                }

                break;

            default:
                maintainState = functions[(int)state].Call(inputParser);
                if (!maintainState) { state = FunctionEnum.none; }
                break;
        }
    }

    private IEnumerator OutLineCreator(float waitTime, GameObject go, Color color) 
    {
        yield return new WaitForSeconds(waitTime);
        Utility.CreateOutline(go, color, true);
    }
}
