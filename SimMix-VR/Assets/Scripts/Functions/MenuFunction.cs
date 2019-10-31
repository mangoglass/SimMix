using System.Xml;
using UnityEngine;

public class MenuFunction : IFunction 
{
    private enum ItemType { menu, tool, back }

    private abstract class MenuItem 
    {
        public string name;
        public GameObject gameObject;
        public ItemType type;
    }

    private class SubMenu : MenuItem 
    {
        public int menuId;
        public int parentMenu;
        public MenuItem[] subItems;
        public GameObject menuContainer;

        public SubMenu() { type = ItemType.menu; }
    }

    private class Tool : MenuItem 
    {
        public int toolId;
        public bool canUseInObjectMode;
        public bool canUseInFaceMode;
        public bool canUseInVertexMode;

        public Tool() { type = ItemType.tool; }
    }

    private class Back : MenuItem 
    {
        public Back() {
            name = "Back";
            gameObject = Resources.Load("Prefabs/Tools/back") as GameObject;
            type = ItemType.back;
        }
    }

    private float radius;
    private float menuThreshold;
    private int currentMenu;
    private MenuItem hooverMenuItem;
    private int equipedToolId;

    private Material selectedMat;
    private Material hoverMat;
    private Material unselectedMat;

    private GameObject wrapper;
    private GameObject pointer;
    private SubMenu[] subMenus;
    private Tool[] tools;

    private ToolFunction toolRef;
    private MeshManager meshManager;
    private Transform controllerTransform;

    public MenuFunction(int nrTools, Transform o_controllerTransform, ToolFunction o_toolRef) 
    {
        controllerTransform = o_controllerTransform;
        toolRef = o_toolRef;
        tools = new Tool[nrTools];

        hooverMenuItem = null;
        equipedToolId = (int)toolRef.equippedTool;

        Globals globals = Object.FindObjectOfType<Globals>();
        
        wrapper = Object.Instantiate(new GameObject("Menu Wrapper"), controllerTransform);
        wrapper.transform.localScale = new Vector3(globals.menuScale, globals.menuScale, globals.menuScale);
        wrapper.transform.localPosition = globals.menuRelativePosition;
        wrapper.transform.localRotation = Quaternion.Euler(globals.menuRelativeRotation);

        radius = globals.menuRadius;
        menuThreshold = globals.menuThreshold;
        selectedMat = globals.MenuSelectedMaterial;
        hoverMat = globals.MenuHooverMaterial;
        unselectedMat = globals.MenuUnselectedMaterial;
        meshManager = globals.meshManager;

        initMenus(globals);

        currentMenu = 0;
        subMenus[currentMenu].menuContainer.SetActive(true);

        /*
        GameObject mode_wrapper = new GameObject();
        mode_wrapper.transform.localEulerAngles = new Vector3(90f, 0, 0);
        mode_wrapper.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        mode_wrapper.transform.localPosition = new Vector3(0, 0, radius + 1f);
        */

        //menuElements[selectedElement].GetComponent<MeshRenderer>().material = selectedMat;

        GameObject pointerPrimitive = GameObject.CreatePrimitive(globals.pointerElementType);
        pointer = Object.Instantiate(pointerPrimitive, wrapper.transform);
        Object.Destroy(pointerPrimitive);

        pointer.GetComponent<MeshRenderer>().material = globals.MenuPointerMaterial;

        wrapper.SetActive(false);
    }

    private void initMenus(Globals globals) 
    {
        int nrMenus = 0;
        XmlReader reader = XmlReader.Create(globals.menuSystemFile);

        while (reader.Read()) 
        {
            if (reader.NodeType == XmlNodeType.Element && reader.Name == "menu") 
            {
                nrMenus++;
            }
        }

        subMenus = new SubMenu[nrMenus];
        for (int i = 0; i < nrMenus; i++) { subMenus[i] = new SubMenu(); }
        reader = XmlReader.Create(globals.menuSystemFile); // Reset the reader to start of file
        reader.Read();  // read first line (first menu)

        menuRecursive(reader, 0, 0);
        initMenuContainers(nrMenus, globals);
    }

    private int menuRecursive(XmlReader reader, int menuIndex, int parentIndex) 
    {
        SubMenu menu = subMenus[menuIndex];
        menu.parentMenu = parentIndex;
        menu.menuId = menuIndex;
        int subItem = (menuIndex > 0 ? 1 : 0); // if menu is not root, leave first space to back button
        int nextMenuIndex = menuIndex + 1;

        while (reader.Read()) {
            if (reader.Name == "menu" && reader.NodeType == XmlNodeType.EndElement) // end of menu
            {
                return nextMenuIndex; // return the next viable menu index
            } 

            if (reader.NodeType != XmlNodeType.Element) continue; // not start of xml element

            switch (reader.Name) {
                case "name":
                    reader.Read();
                    menu.name = reader.Value;
                    break;

                case "path":
                    reader.Read();
                    menu.gameObject = Resources.Load(reader.Value) as GameObject;
                    break;

                case "items":
                    reader.Read();
                    int nrSubItems = int.Parse(reader.Value) + (menuIndex > 0 ? 1 : 0);
                    menu.subItems = new MenuItem[nrSubItems];
                    break;

                case "menu":
                    menu.subItems[subItem] = subMenus[nextMenuIndex]; // set subitem to found submenu
                    nextMenuIndex = menuRecursive(reader, nextMenuIndex, menuIndex); // recursive call
                    subItem++;
                    break;

                case "tool":
                    menu.subItems[subItem] = createTool(reader);
                    subItem++;
                    break;

                default:
                    break;
            }
        }

        return nextMenuIndex;
    }

    private MenuItem createTool(XmlReader reader) {
        Tool tool = new Tool();

        while (reader.Read() && !(reader.NodeType == XmlNodeType.EndElement && reader.Name == "tool")) // as long as we havve not reached the end of the tool
        {
            if (reader.NodeType != XmlNodeType.Element) continue;

            switch (reader.Name) 
            {
                case "name":
                    reader.Read();
                    tool.name = reader.Value;
                    break;

                case "path":
                    reader.Read();
                    tool.gameObject = Resources.Load(reader.Value) as GameObject;
                    break;

                case "id":
                    reader.Read();
                    int id = int.Parse(reader.Value);
                    tool.toolId = id;
                    tools[id] = tool;
                    break;

                case "omode":
                    reader.Read();
                    tool.canUseInObjectMode = reader.Value.Equals("true");
                    break;

                case "fmode":
                    reader.Read();
                    tool.canUseInFaceMode = reader.Value.Equals("true");
                    break;

                case "vmode":
                    reader.Read();
                    tool.canUseInVertexMode = reader.Value.Equals("true");
                    break;

                default:
                    break;
            }
        }

        return tool;
    }

    private void initMenuContainers(int nrMenus, Globals globals) 
    {
        int nrMenuSubItems;

        for (int i = 0; i < nrMenus; i++) 
        {
            SubMenu menu = subMenus[i];
            nrMenuSubItems = menu.subItems.Length;

            if (i > 0) {
                menu.subItems[0] = new Back();
            }

            menu.menuContainer = Object.Instantiate(new GameObject("MenuContainer"), wrapper.transform);
            menu.menuContainer.SetActive(false);

            float angle = Mathf.PI / 2;
            float deltaAngle = 2 * Mathf.PI / nrMenuSubItems;
            float scale = globals.menuElementScale;

            for (int j = 0; j < nrMenuSubItems; j++) 
            {
                menu.subItems[j].gameObject = Object.Instantiate(menu.subItems[j].gameObject, menu.menuContainer.transform);

                GameObject gameObject = menu.subItems[j].gameObject;
                gameObject.transform.localPosition = new Vector3(radius * Mathf.Cos(angle), 0, radius * Mathf.Sin(angle));
                gameObject.transform.localScale = gameObject.transform.localScale * scale;
                gameObject.GetComponent<MeshRenderer>().material = unselectedMat;

                angle += deltaAngle;

                // Put the name bellow the menu item
                GameObject textWrapper = new GameObject("Menu Item Text");
                textWrapper.transform.SetParent(menu.menuContainer.transform);
                textWrapper.transform.localEulerAngles = new Vector3(90f, 0, 0);
                textWrapper.transform.localScale = new Vector3(0.14f, 0.14f, 0.14f);
                textWrapper.transform.localPosition = gameObject.transform.localPosition + new Vector3(0, 0, -1f);

                TextMesh tm = textWrapper.AddComponent<TextMesh>();
                tm.anchor = TextAnchor.UpperCenter;
                tm.color = Color.white;
                tm.fontSize = 60;
                tm.text = menu.subItems[j].name;
            }
        }
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
        int hooverToolId = -1;

        if(menuDisplay) 
        {

            if (!wrapper.activeSelf) { wrapper.SetActive(true); }
            Vector2 pos = input.MenuPointerLocation();
            Vector3 pointerPos = new Vector3(radius * pos.x, 0, radius * pos.y);
            pointer.transform.localPosition = pointerPos;
            float angle = Mathf.Atan2(pos.y, pos.x) * Mathf.Rad2Deg;

            if (pointerPos.magnitude >= radius * menuThreshold) 
            {
                MenuItem menuItem = subMenus[currentMenu].subItems[GetSubItemIndex(Mathf.Deg2Rad * angle)];
                hooverToolId = getHooverId();

                if (hooverMenuItem != null && hooverMenuItem != menuItem && hooverToolId != equipedToolId) 
                {
                    hooverMenuItem.gameObject.GetComponent<MeshRenderer>().material = unselectedMat;
                }

                hooverMenuItem = menuItem;
                hooverToolId = getHooverId();

                if (hooverToolId != equipedToolId) 
                {
                    hooverMenuItem.gameObject.GetComponent<MeshRenderer>().material = hoverMat;
                }

                if (input.MenuClickBoolDown() && hooverToolId != equipedToolId) 
                {
                    if (hooverMenuItem.type == ItemType.tool) // this is a tool
                    {
                        tools[hooverToolId].gameObject.GetComponent<MeshRenderer>().material = selectedMat;
                        tools[equipedToolId].gameObject.GetComponent<MeshRenderer>().material = unselectedMat;
                        equipedToolId = hooverToolId;
                        toolRef.equippedTool = (ToolFunction.ToolEnum)(equipedToolId); // change current tool to selected tool in menu
                    }

                    else if (hooverMenuItem.type == ItemType.menu)  // this is another menu
                    {
                        hooverMenuItem.gameObject.GetComponent<MeshRenderer>().material = unselectedMat;
                        subMenus[currentMenu].menuContainer.SetActive(false);
                        currentMenu = ((SubMenu)hooverMenuItem).menuId;
                        subMenus[currentMenu].menuContainer.SetActive(true);
                        hooverMenuItem = null;
                    }

                    else // this is a back button
                    {
                        hooverMenuItem.gameObject.GetComponent<MeshRenderer>().material = unselectedMat;
                        subMenus[currentMenu].menuContainer.SetActive(false);
                        currentMenu = subMenus[currentMenu].parentMenu;
                        subMenus[currentMenu].menuContainer.SetActive(true);
                        hooverMenuItem = null;
                    }
                }
            }

            else if (hooverMenuItem != null) // if we are not outside the threshold and a hoover mat has been applied to some item in the menu
            {
                hooverToolId = getHooverId();
                if (hooverToolId != equipedToolId) 
                {
                    hooverMenuItem.gameObject.GetComponent<MeshRenderer>().material = unselectedMat;
                }

                hooverMenuItem = null;
            }
        }

        else if(input.MenuDisplayBoolUp()) 
        {
            wrapper.SetActive(false);
        }

        return menuDisplay;
    }

    private int GetSubItemIndex(float angle) 
    {
        float elementSubDivision = 2f * Mathf.PI / subMenus[currentMenu].subItems.Length;

        angle = angle - (Mathf.PI / 2f) + (elementSubDivision / 2f);
        if (angle < 0) { angle += 2f * Mathf.PI; }

        return (int)(angle / elementSubDivision);
    }

    private int getHooverId() 
    {
        if (hooverMenuItem == null) return -1;
        if (hooverMenuItem.type != ItemType.tool) return -1;
        return ((Tool)hooverMenuItem).toolId;
    }
}
