using UnityEngine;

public class PrimitiveTool : ITool 
{
    private enum PrimitiveEnum { cube = 0 };
    private GameObject[] primitives;
    private float minToolVisibility;
    private float objectDistance;
    private MeshManager meshManager;

    public PrimitiveTool() 
    {
        Globals globals = Object.FindObjectOfType<Globals>();

        minToolVisibility = globals.minToolVisibility;
        objectDistance = globals.objectDistanceFromController;
        meshManager = globals.meshManager;

        primitives = new GameObject[]
        {
            Resources.Load("Prefabs/Primitives/Cube") as GameObject
        };

        for (int i = 0; i < primitives.Length; i++) 
        {
            GameObject primitive = Object.Instantiate(primitives[i]);
            primitive.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
            primitive.transform.localRotation = Quaternion.Euler(Vector3.zero);
            primitive.SetActive(false);

            primitives[i] = primitive;
        }
    }

    public void Apply(IInputParser input, bool isFirstFrame) 
    {
        float triggerValue = input.ToolTriggerValue();

        if (triggerValue > 0) {
            GameObject cube = primitives[(int)PrimitiveEnum.cube];
            Transform transform = input.GetTransform();

            if (input.ToolTriggerValueChanged()) { cube.SetActive(true); }

            Vector3 pos = transform.position + objectDistance * transform.forward;
            //cube.transform.SetPositionAndRotation(pos, transform.rotation);
            cube.transform.position = pos;

            Color c = cube.GetComponent<MeshRenderer>().material.color;
            c.a = minToolVisibility + (triggerValue * (1 - minToolVisibility));
            cube.GetComponent<MeshRenderer>().material.color = c;

            if (input.ToolBoolUp()) {
                meshManager.CreateCube(pos, 0.15f);
            }
        }

        else if (input.ToolTriggerValueChanged()) {
            primitives[(int)PrimitiveEnum.cube].SetActive(false);
        }
    }

    
}
