using UnityEngine;

public class PrimitiveTool : ITool 
{
    private enum PrimitiveEnum { cube = 0 };
    private GameObject[] primitives;
    private float m_minToolVisibility;

    public PrimitiveTool(float minToolVisibility) 
    {
        m_minToolVisibility = minToolVisibility;

        primitives = new GameObject[]
        {
            Resources.Load("Prefabs/Primitives/Cube") as GameObject
        };

        for (int i = 0; i < primitives.Length; i++) 
        {
            primitives[i] = Object.Instantiate(primitives[i]);
            primitives[i].SetActive(false);
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
            cube.transform.SetPositionAndRotation(pos, transform.rotation);

            Color c = cube.GetComponent<MeshRenderer>().material.color;
            c.a = m_minToolVisibility + (triggerValue * (1 - m_minToolVisibility));
            cube.GetComponent<MeshRenderer>().material.color = c;

            if (input.ToolBoolUp()) {
                GameObject cubeClone = Object.Instantiate(cube);
                Material cloneMat = new Material(cube.GetComponent<MeshRenderer>().material.shader);
                cloneMat.SetColor("Red", new Color(1, 0, 0));
                cubeClone.GetComponent<MeshRenderer>().material = cloneMat;
            }
        }

        else if (input.ToolTriggerValueChanged()) {
            primitives[(int)PrimitiveEnum.cube].SetActive(false);
        }
    }

    
}
