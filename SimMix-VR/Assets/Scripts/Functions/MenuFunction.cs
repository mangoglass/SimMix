using UnityEngine;

public class MenuFunction : IFunction 
{
    private float radius;
    private float menuThreshold;
    private int nrOfMenuElements;
    private int hooverElement;
    private int selectedElement;

    private Material selectedMat;
    private Material hoverMat;
    private Material unselectedMat;

    private GameObject wrapper;
    private GameObject pointer;
    private GameObject[] menuElements;

    private ToolFunction toolRef;
    private MeshManager meshManager;

    public MenuFunction(int o_nrOfMenuElements, Transform controllerTransform, ToolFunction o_toolRef) 
    {
        nrOfMenuElements = o_nrOfMenuElements;
        toolRef = o_toolRef;
        hooverElement = -1;
        selectedElement = (int)toolRef.equippedTool;

        Globals globals = Object.FindObjectOfType<Globals>();
        
        wrapper = Object.Instantiate(new GameObject(), controllerTransform);
        wrapper.transform.localScale = new Vector3(globals.menuScale, globals.menuScale, globals.menuScale);
        wrapper.transform.localPosition = globals.menuRelativePosition;
        wrapper.transform.localRotation = Quaternion.Euler(globals.menuRelativeRotation);

        radius = globals.menuRadius;
        menuThreshold = globals.menuThreshold;
        selectedMat = globals.MenuSelectedMaterial;
        hoverMat = globals.MenuHooverMaterial;
        unselectedMat = globals.MenuUnselectedMaterial;
        meshManager = globals.meshManager;

        menuElements = new GameObject[nrOfMenuElements];

        float angle = Mathf.PI / 2;
        float deltaAngle = 2 * Mathf.PI / nrOfMenuElements;


        for (int i = 0; i < nrOfMenuElements; i++) 
        {
            GameObject primitive = GameObject.CreatePrimitive(globals.menuElementType);
            GameObject menuElement = Object.Instantiate(primitive, wrapper.transform);
            Object.Destroy(primitive);

            menuElement.transform.localPosition = new Vector3(radius * Mathf.Cos(angle), 0, radius * Mathf.Sin(angle));
            menuElement.transform.localScale = globals.menuElementLocalScale;
            menuElement.GetComponent<MeshRenderer>().material = unselectedMat;

            menuElements[i] = menuElement;
            angle += deltaAngle;


            GameObject tool_text_wrapper = new GameObject();
            tool_text_wrapper.transform.localEulerAngles = new Vector3(90f, 0, 0);
            //test.transform.localPosition = new Vector3(1.2f * Mathf.Cos(currentAngel), 0, 1.2f * Mathf.Sin(currentAngel));
            tool_text_wrapper.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            tool_text_wrapper.transform.localPosition = new Vector3(0, 0, -0.5f);

            GameObject text_go = Object.Instantiate(tool_text_wrapper, menuElement.transform);

            TextMesh tm = text_go.AddComponent<TextMesh>();
            tm.anchor = TextAnchor.UpperCenter;
            tm.color = Color.white;
            tm.fontSize = 60;
            tm.text = o_toolRef.toolnames[i];
        }

        GameObject mode_wrapper = new GameObject();
        mode_wrapper.transform.localEulerAngles = new Vector3(90f, 0, 0);
        mode_wrapper.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        mode_wrapper.transform.localPosition = new Vector3(0, 0, radius + 1f);


        Debug.Log("selected init: " + selectedElement);
        menuElements[selectedElement].GetComponent<MeshRenderer>().material = selectedMat;

        GameObject pointerPrimitive = GameObject.CreatePrimitive(globals.pointerElementType);
        pointer = Object.Instantiate(pointerPrimitive, wrapper.transform);
        Object.Destroy(pointerPrimitive);

        pointer.GetComponent<MeshRenderer>().material = globals.MenuPointerMaterial;

        wrapper.SetActive(false);


    }

    private string ModeToString(EditMode edit_mode)
    {
        switch (edit_mode)
        {
            case EditMode.Object:
                return "Object";
            case EditMode.Face:
                return "Face";
            case EditMode.Vertex:
                return "Vertex";
        }
        return " - ";
    }

    public bool Call(IInputParser input) 
    {
        bool menuDisplay = input.MenuDisplayBool();

        if(menuDisplay) 
        {

            if (!wrapper.activeSelf) { wrapper.SetActive(true); }
            Vector2 pos = input.MenuPointerLocation();
            Vector3 pointerPos = new Vector3(radius * pos.x, 0, radius * pos.y);
            pointer.transform.localPosition = pointerPos;
            float angle = Mathf.Atan2(pos.y, pos.x) * Mathf.Rad2Deg;

            if (pointerPos.magnitude >= radius * menuThreshold) 
            {
                int element = GetElementIndex(Mathf.Deg2Rad * angle);

                if (hooverElement != -1 && hooverElement != element && hooverElement != selectedElement) 
                {
                    menuElements[hooverElement].GetComponent<MeshRenderer>().material = unselectedMat;
                }

                hooverElement = element;

                if (hooverElement != selectedElement) 
                {
                    menuElements[hooverElement].GetComponent<MeshRenderer>().material = hoverMat;
                }

                if (input.MenuClickBool() && hooverElement != selectedElement) 
                {
                    menuElements[hooverElement].GetComponent<MeshRenderer>().material = selectedMat;
                    Debug.Log("selected: " + selectedElement);
                    menuElements[selectedElement].GetComponent<MeshRenderer>().material = unselectedMat;

                    selectedElement = hooverElement;
                    toolRef.equippedTool = (ToolFunction.ToolEnum)(selectedElement);
                }
            }

            else if (hooverElement != -1) 
            {
                if (hooverElement != selectedElement) 
                {
                    menuElements[hooverElement].GetComponent<MeshRenderer>().material = unselectedMat;
                }

                hooverElement = -1;
            }
        }

        else if(input.MenuDisplayBoolUp()) 
        {
            wrapper.SetActive(false);
        }

        return menuDisplay;
    }

    private int GetElementIndex(float angle) 
    {
        float elementSubDivision = 2f * Mathf.PI / menuElements.Length;

        angle = angle - (Mathf.PI / 2f) + (elementSubDivision / 2f);
        if (angle < 0) { angle += 2f * Mathf.PI; }

        return (int)(angle / elementSubDivision);
    }
}
