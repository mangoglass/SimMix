using UnityEngine;

public class PrimitiveTool : ITool 
{
    private enum PrimitiveEnum { cube = 0 };
    private GameObject[] primitives;
    private float m_minToolVisibility;
    private MeshManager meshManager;

    public PrimitiveTool(float minToolVisibility) 
    {
        m_minToolVisibility = minToolVisibility;
        Globals globals = Object.FindObjectOfType<Globals>();
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

    public void Apply(IInputParser input) 
    {
        float triggerValue = input.ToolTriggerValue();

        if (triggerValue > 0) {
            GameObject cube = primitives[(int)PrimitiveEnum.cube];
            Transform transform = input.GetTransform();

            if (input.ToolTriggerValueChanged()) { cube.SetActive(true); }

            Vector3 pos = transform.position + 0.2f * transform.forward;
            //cube.transform.SetPositionAndRotation(pos, transform.rotation);
            cube.transform.position = pos;

            Color c = cube.GetComponent<MeshRenderer>().material.color;
            c.a = m_minToolVisibility + (triggerValue * (1 - m_minToolVisibility));
            cube.GetComponent<MeshRenderer>().material.color = c;

            if (input.ToolBoolUp()) {
                /*
                GameObject cubeClone = Object.Instantiate(cube);
                Material cloneMat = new Material(cube.GetComponent<MeshRenderer>().material.shader);
                cloneMat.SetColor("Red", new Color(1, 0, 0));
                cubeClone.GetComponent<MeshRenderer>().material = cloneMat;
                */

                meshManager.CreateCube(pos, 0.15f);
            }
        }

        else if (input.ToolTriggerValueChanged()) {
            primitives[(int)PrimitiveEnum.cube].SetActive(false);
        }
    }

    
}
