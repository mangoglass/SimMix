using UnityEngine;

public class InputSystem : MonoBehaviour
{
    public enum Controller { left, right };
    private enum FunctionEnum { tool=0, teleport=1, swap=2, menu=3, none };

    public bool forceReadInput = true;
    public Controller controller;

    [Range(0f,1f)]
    public float minToolPrimitiveVisibility;

    private IInputParser inputParser;
    private IFunction[] functions;
    private FunctionEnum state;

    void Awake() 
    {
        inputParser = GetComponent<IInputParser>();
        state = FunctionEnum.none;

        functions = new IFunction[]
        {
            new ToolFunction(minToolPrimitiveVisibility)
        };
    }

    // Update is called once per frame
    void Update()
    {
        if (forceReadInput || inputParser.HeadsetBoolCheck()) 
        {
            StateHandler();
        }

        else if (inputParser.HeadsetBoolReleased()) 
        {
            Debug.Log("Player has taken off headset, pausing input");
        }
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
                    Debug.Log(controller.ToString() + " entering tool action state");
                }

                else if (inputParser.MenuDisplayBoolCheck()) 
                {
                    state = FunctionEnum.menu;
                    Debug.Log(controller.ToString() + " entering menu action state");
                }

                else if (inputParser.TeleportBoolCheck()) 
                {
                    state = FunctionEnum.teleport;
                    Debug.Log(controller.ToString() + " entering teleport action state");
                }

                else if (inputParser.SwapBoolCheck()) 
                {
                    state = FunctionEnum.swap;
                    Debug.Log(controller.ToString() + " entering swaping action state");
                }

                break;

            case FunctionEnum.tool:
                maintainState = functions[(int)FunctionEnum.tool].Call(inputParser);
                if (!maintainState) { state = FunctionEnum.none; }
                break;

            case FunctionEnum.menu:
                maintainState = functions[(int)FunctionEnum.menu].Call(inputParser);
                if (!maintainState) { state = FunctionEnum.none; }
                break;

            case FunctionEnum.teleport:
                maintainState = functions[(int)FunctionEnum.teleport].Call(inputParser);
                if (!maintainState) { state = FunctionEnum.none; }
                break;

            case FunctionEnum.swap:
                maintainState = functions[(int)FunctionEnum.swap].Call(inputParser);
                if (!maintainState) { state = FunctionEnum.none; }
                break;
        }
    }
}
