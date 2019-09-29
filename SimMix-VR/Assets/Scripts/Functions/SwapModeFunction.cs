
using UnityEngine;

public class ChangeModeFunction : IFunction {
    private MeshManager meshManager;
    private EditMode mode;
    private Controller controller;
    private GameObject modeMenuWrapper;
    private GameObject modeMenuPointer;
    private GameObject selectedTextWrapper;
    private GameObject controllerObject;
    private Material pointerMat;
    private Color[] modeColors;
    private string[] modeNames;
    private int player_id;
    private int nrOfModes;
    private float maxPointerY;
    private float minPointerY;

    private struct ModeMenuElement 
    {
        public GameObject shape;
        public Outline outline;
    }

    private ModeMenuElement[] elements;

    public ChangeModeFunction(int player_id, Transform controllerTransform, Controller controller, GameObject controllerObject) 
    {
        this.player_id = player_id;
        this.controller = controller;
        this.controllerObject = controllerObject;

        modeMenuWrapper = Object.Instantiate(new GameObject(), controllerTransform);
        modeMenuWrapper.transform.localPosition = new Vector3(0, 0, 0);
        modeMenuWrapper.SetActive(false);

        Globals globals = Object.FindObjectOfType<Globals>();

        meshManager = globals.meshManager;
        pointerMat = globals.swapPointerMaterial;
        minPointerY = globals.pointerMinY;
        maxPointerY = globals.pointerMaxY;
        modeColors = globals.modeColor;

        mode = meshManager.GetMode(player_id);

        // Pointer
        modeMenuPointer = Resources.Load("Prefabs/ChangeModePointer") as GameObject;
        modeMenuPointer = Object.Instantiate(modeMenuPointer, modeMenuWrapper.transform);
        Transform mmpt = modeMenuPointer.transform;
        mmpt.localPosition = globals.modePointerOffset;
        if(controller == Controller.right) 
        {
            mmpt.localRotation = Quaternion.Euler(mmpt.rotation.eulerAngles.x, mmpt.rotation.eulerAngles.y + 180f, mmpt.rotation.eulerAngles.z);
            mmpt.localPosition = new Vector3(-mmpt.localPosition.x, mmpt.localPosition.y, mmpt.localPosition.z);
        }

        GameObject primitive = GameObject.CreatePrimitive(globals.swapMenuTypes);
        Object.Destroy(primitive.GetComponent<Collider>());
        float scale = globals.swapMenuElementScale;
        primitive.transform.localScale = new Vector3(scale, scale, scale);

        GameObject textPrefab = new GameObject();
        textPrefab.transform.localEulerAngles = new Vector3(90f, 0, 0);
        textPrefab.transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);

        modeNames = System.Enum.GetNames(typeof(EditMode));
        nrOfModes = modeNames.Length;
        elements = new ModeMenuElement[nrOfModes];

        for(int i = 0; i < nrOfModes; i++) 
        {
            ModeMenuElement element = new ModeMenuElement();
            element.shape = Object.Instantiate(primitive, modeMenuWrapper.transform);
            float xOffset = globals.swapMenuXOffset;
            xOffset = (controller == Controller.left ? xOffset : -xOffset);
            float yOffset = globals.swapMenuYOffset + (i * globals.swapMenuTextPadding) - (globals.swapMenuTextPadding * nrOfModes / 2f);
            element.shape.transform.localPosition = new Vector3(xOffset, 0, yOffset);
            element.shape.GetComponent<MeshRenderer>().material = ModeColor((EditMode)i, globals);

            // text displayed next to modes
            GameObject textObject = Object.Instantiate(textPrefab, modeMenuWrapper.transform);
            float textXOffset = 0.015f * (controller == Controller.left ? 1 : -1);
            textObject.transform.localPosition = new Vector3(xOffset + textXOffset, 0, yOffset);
            TextMesh elementTM = textObject.AddComponent<TextMesh>();
            elementTM.anchor = (controller == Controller.left ? TextAnchor.MiddleLeft : TextAnchor.MiddleRight);
            elementTM.color = globals.swapMenuTextColor;
            elementTM.fontSize = globals.swapMenuTextFontSize;
            elementTM.text = modeNames[i] + " mode";

            // outline for the selected mode
            element.outline = Utility.CreateOutline(element.shape, globals.selectedColor, (i == (int)mode));
            elements[i] = element;
        }

        selectedTextWrapper = Object.Instantiate(textPrefab, controllerTransform);
        Transform selT = selectedTextWrapper.transform;
        selT.localPosition = globals.modeSelectedTextOffset;
        if(controller == Controller.right) 
        {
            selT.localPosition = new Vector3(-selT.localPosition.x, selT.localPosition.y, selT.localPosition.z);
        }

        TextMesh wrapperTM = selectedTextWrapper.AddComponent<TextMesh>();
        wrapperTM.anchor = (controller == Controller.left ? TextAnchor.LowerLeft : TextAnchor.LowerRight);
        wrapperTM.color = globals.swapMenuTextColor;
        wrapperTM.fontSize = (int)(globals.swapMenuTextFontSize * 1.5f);
        wrapperTM.text = modeNames[(int)mode] + " mode";

        Object.Destroy(primitive);
    }

    private Material ModeColor(EditMode i, Globals globals) {
        switch(i) 
        {
            case EditMode.Object:
                return globals.objectMaterial;
            case EditMode.Face:
                return globals.faceMaterial;
            case EditMode.Vertex:
                return globals.vertexMaterial;
        }

        return globals.objectMaterial;
    }

    public bool Call(IInputParser input) 
    {
        bool swapButtonPressed = input.SwapBool();

        if(swapButtonPressed) 
        {
            if (input.SwapBoolDown()) 
            {
                modeMenuWrapper.SetActive(true);
                selectedTextWrapper.SetActive(false);
            }

            // value between minPointerY and MaxPointerY based on the current mode
            float yPos = minPointerY + ((maxPointerY - minPointerY) * (float)mode / (nrOfModes - 1));
            int modeIndex = (int)(mode);

            if (input.MenuDisplayBool()) 
            {
                float pointerPos = (1f + input.MenuPointerLocation().y) / 2f;
                yPos = minPointerY + ((maxPointerY - minPointerY) * pointerPos);
                modeIndex = (int)((pointerPos == 1 ? 0.99f : pointerPos) * nrOfModes);
            }

            Vector3 pos = modeMenuPointer.transform.localPosition;
            modeMenuPointer.transform.localPosition = new Vector3(pos.x, pos.y, yPos);

            if ((int)mode != modeIndex) 
            {
                elements[(int)mode].outline.enabled = false;
                elements[modeIndex].outline.enabled = true;
                mode = (EditMode) modeIndex;
                selectedTextWrapper.GetComponent<TextMesh>().text = modeNames[modeIndex] + " mode";
                meshManager.ToggleMode(player_id, mode);
                controllerObject.GetComponent<Outline>().OutlineColor = modeColors[modeIndex];
            }
        }

        else if(input.SwapBoolUp()) 
        {
            modeMenuWrapper.SetActive(false);
            selectedTextWrapper.SetActive(true);
        }

        return swapButtonPressed;
    }
}
