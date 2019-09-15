using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshManager : MonoBehaviour {
    public Dictionary<int, GameObject> meshes;
    int curr_index;

    // Start is called before the first frame update
    void Awake() {
        meshes = new Dictionary<int, GameObject>();
        curr_index = 0;
    }

    void Start() {
        CreateCube(new Vector3(0.0f, 0.5f, 0.0f), 1);
        CreateCube(new Vector3(2.0f, 0.5f, 0.0f), 1);
        CreateCube(new Vector3(0.0f, 0.5f, 2.0f), 1);
        CreateCube(new Vector3(2.0f, 0.5f, 2.0f), 1);
    }

    // Update is called once per frame
    void Update() { }

    public void CreateCube(Vector3 position, float side_length) {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = position;
        cube.transform.localScale = new Vector3(side_length, side_length, side_length);
        meshes.Add(curr_index, cube);
        curr_index += 1;
    }

    public void SetColor(int id, Color color) {
        GameObject mesh = meshes[id];
        Renderer rend = mesh.GetComponent<Renderer>();
        rend.material.color = color;
    }

    public (int, float) GetClosestId(Vector3 position) {
        float min_dist = float.MaxValue;
        int min_i = 0;

        foreach (KeyValuePair<int, GameObject> mesh_entry in meshes) {
            float dist_to = Vector3.Distance(position, mesh_entry.Value.transform.position);
            if (dist_to < min_dist) {
                min_dist = dist_to;
                min_i = mesh_entry.Key;
            }
        }

        return (min_i, min_dist);
    }

    public void Translate(int id, Vector3 diff) {
        GameObject mesh = meshes[id];
        mesh.transform.Translate(diff);
    }
}
