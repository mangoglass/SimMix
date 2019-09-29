using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EditMode
{
    Object,
    Face,
    Vertex

}

public class Player
{
    public Transform transform;
    public int selected_object;
    public int selected_secondary;
    public bool update_selected;
    public EditMode edit_mode;

}

public class MeshManager : MonoBehaviour
{

    public List<Player> players;


    public double min_select_dist;


    public Material default_material;
    public Dictionary<int, MyTriMesh> trimeshes;
    int last_mesh_id;

    void OnGUI()
    {
        /*
        GUIStyle gs = new GUIStyle();
        gs.fontSize = 100;
        Player p = players[0];
        switch (p.edit_mode)
        {
            case EditMode.Object:
                GUILayout.Label("OBJECT MODE", gs);
                break;
            case EditMode.Face:
                GUILayout.Label("FACE MODE", gs);
                break;
            case EditMode.Vertex:
                GUILayout.Label("VERTEX MODE", gs);
                break;
        }
        */
    }

    // Start is called before the first frame update
    void Awake()
    {
        players = new List<Player>();
        min_select_dist = 0.3;
        trimeshes = new Dictionary<int, MyTriMesh>();

        last_mesh_id = 0;

    }

    private void ResetScene()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        min_select_dist = 0.3;
        last_mesh_id = 0;
        trimeshes = new Dictionary<int, MyTriMesh>();
        CreateCube(new Vector3(0, 0, 0), 0.3f);
        CreateCube(new Vector3(0.5f, 0, 0.5f), 0.3f);


    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetScene();
        }
    }

    void Start()
    {
        //CreateCube(new Vector3(0, 0, 0), 0.3f);
        ResetScene();
        //mesh_manager.CreateCube(new Vector3(2, 0, 2), 1.0f);
        //         int num = 10;
        // float step = (Mathf.PI * 2) / num;
        // float xoffset = 0.5f;
        // float yoffset = 0.5f;

        // for (int x = 0; x < num; x++)
        // {
        //     for (int y = 0; y < num; y++)
        //     {

        //         float siny = (((Mathf.Cos(y * step) + 1.0f) / 4.0f)) + 0.2f;
        //         float cosx = (((Mathf.Sin(x * step) + 1.0f) / 4.0f)) + 0.2f;

        //         CreateCube(new Vector3(x / 5.0f + xoffset, siny + cosx, y / 5.0f + yoffset), 0.15f);

        //     }
        // }
    }

    // Update is called once per frame
    void LateUpdate()
    {

        for (int i = 0; i < players.Count; i++)
        {
            Deselect(i);

            if (players[i].update_selected)
            {
                (players[i].selected_object, players[i].selected_secondary) = GetClosest(i);
            }

            Select(i);
        }


    }


    public int RegisterPlayer(Transform transform)
    {
        Player player = new Player();
        player.selected_object = -1;
        player.selected_secondary = -1;
        player.transform = transform;
        player.update_selected = true;
        player.edit_mode = EditMode.Object;

        players.Add(player);
        return players.Count - 1;
    }


    public void SetUpdateSelected(int player_id, bool update_selected)
    {
        players[player_id].update_selected = update_selected;
    }

    public EditMode GetMode(int player_id)
    {
        return players[player_id].edit_mode;
    }

    public void ToggleMode(int player_id, EditMode edit_mode)
    {
        Player p = players[player_id];
        switch (p.edit_mode)
        {
            case EditMode.Object:
                DeselectObject(p.selected_object);
                break;
            case EditMode.Face:
                DeselectFace(p.selected_object, p.selected_secondary);
                break;
            case EditMode.Vertex:
                break;
        }


        players[player_id].selected_object = -1;
        players[player_id].selected_secondary = -1;

        //players[player_id].edit_mode = (EditMode)((int)(players[player_id].edit_mode + 1) % System.Enum.GetNames(typeof(EditMode)).Length);
        players[player_id].edit_mode = edit_mode;
    }
    /////////////////////////////////////////////////////////////////////

    private MyTriMesh InitMesh(string mesh_name)
    {
        GameObject mesh_go = new GameObject(mesh_name);
        MyTriMesh mesh = mesh_go.AddComponent<MyTriMesh>() as MyTriMesh;
        MeshRenderer mesh_renderer = mesh_go.AddComponent<MeshRenderer>() as MeshRenderer;
        mesh_renderer.material = default_material;
        mesh_go.transform.SetParent(gameObject.transform);

        return mesh;
    }

    public void CreateCube(Vector3 position, float side_length)
    {

        MyTriMesh mesh = InitMesh("Cuby");

        float half_side = side_length / 2.0f;
        Vector3 e0 = new Vector3(half_side, 0, 0);
        Vector3 e1 = new Vector3(0, half_side, 0);
        Vector3 e2 = new Vector3(0, 0, half_side);

        int p1 = mesh.CreateVertex(position + e0 + e1 + e2);
        int p2 = mesh.CreateVertex(position + e0 + e1 - e2);
        int p3 = mesh.CreateVertex(position + e0 - e1 + e2);
        int p4 = mesh.CreateVertex(position + e0 - e1 - e2);
        int p5 = mesh.CreateVertex(position - e0 + e1 + e2);
        int p6 = mesh.CreateVertex(position - e0 + e1 - e2);
        int p7 = mesh.CreateVertex(position - e0 - e1 + e2);
        int p8 = mesh.CreateVertex(position - e0 - e1 - e2);

        mesh.CreateFace(p2, p1, p5);
        mesh.CreateFace(p2, p5, p6);
        mesh.CreateFace(p1, p3, p7);
        mesh.CreateFace(p1, p7, p5);
        mesh.CreateFace(p4, p3, p1);
        mesh.CreateFace(p4, p1, p2);
        mesh.CreateFace(p4, p2, p6);
        mesh.CreateFace(p4, p6, p8);
        mesh.CreateFace(p6, p5, p7);
        mesh.CreateFace(p6, p7, p8);
        mesh.CreateFace(p3, p4, p8);
        mesh.CreateFace(p3, p8, p7);

        last_mesh_id++;
        trimeshes.Add(last_mesh_id, mesh);

    }




    public void Select(int player_id)
    {
        Player p = players[player_id];
        switch (p.edit_mode)
        {
            case EditMode.Object:
                SelectObject(p.selected_object);
                break;
            case EditMode.Face:
                SelectFace(p.selected_object, p.selected_secondary);
                break;
            case EditMode.Vertex:
                break;
        }
    }

    public void SelectObject(int obj_id)
    {
        if (obj_id < 0) return;
        MyTriMesh mesh = trimeshes[obj_id];
        mesh.Select();
    }

    public void SelectFace(int obj_id, int face_id)
    {
        if (obj_id < 0) return;

        MyTriMesh mesh = trimeshes[obj_id];
        mesh.SelectFace(face_id);
    }

    public void Deselect(int player_id)
    {
        Player p = players[player_id];
        switch (p.edit_mode)
        {
            case EditMode.Object:
                DeselectObject(p.selected_object);
                break;
            case EditMode.Face:
                DeselectFace(p.selected_object, p.selected_secondary);
                break;
            case EditMode.Vertex:
                break;
        }
    }

    public void DeselectObject(int obj_id)
    {
        if (obj_id < 0) return;

        MyTriMesh mesh = trimeshes[obj_id];
        mesh.Deselect();
    }

    public void DeselectFace(int obj_id, int face_id)
    {
        if (obj_id < 0) return;

        MyTriMesh mesh = trimeshes[obj_id];
        mesh.DeselectFace(face_id);
    }


    public void SetColor(int player_id, Color color)
    {
        Player p = players[player_id];

        MyTriMesh mesh;
        if (!trimeshes.TryGetValue(p.selected_object, out mesh)) return;

        switch (p.edit_mode)
        {
            case EditMode.Object:
                mesh.SetColor(color);
                break;
            case EditMode.Face:
                mesh.SetColor(p.selected_secondary, color);
                break;
            case EditMode.Vertex:
                break;
        }

    }

    public void Translate(int player_id, Vector3 diff)
    {
        Player p = players[player_id];

        MyTriMesh mesh;
        if (!trimeshes.TryGetValue(p.selected_object, out mesh)) return;
        switch (p.edit_mode)
        {
            case EditMode.Object:
                mesh.Translate(diff);
                break;
            case EditMode.Face:
                mesh.TranslateFace(p.selected_secondary, diff);
                break;
            case EditMode.Vertex:
                mesh.Translate(p.selected_secondary, diff);
                break;
        }

    }

    public void Scale(int player_id, float factor)
    {
        Player p = players[player_id];

        MyTriMesh mesh;
        if (!trimeshes.TryGetValue(p.selected_object, out mesh)) return;
        switch (p.edit_mode)
        {
            case EditMode.Object:
                mesh.Scale(factor);
                break;
            case EditMode.Face:
                mesh.ScaleFace(p.selected_secondary, factor);
                break;
            case EditMode.Vertex:
                break;
        }

    }

    public void Rotate(int player_id, Vector3 euler_angles)
    {
        Player p = players[player_id];

        MyTriMesh mesh;
        if (!trimeshes.TryGetValue(p.selected_object, out mesh)) return;
        switch (p.edit_mode)
        {
            case EditMode.Object:
                mesh.Rotate(euler_angles);
                break;
            case EditMode.Face:
                mesh.RotateFace(p.selected_secondary, euler_angles);
                break;
            case EditMode.Vertex:
                break;
        }

    }

    public void Extrude(int player_id)
    {
        Player p = players[player_id];
        if (p.edit_mode != EditMode.Face) return;

        MyTriMesh mesh;
        if (!trimeshes.TryGetValue(p.selected_object, out mesh)) return;
        mesh.Extrude(p.selected_secondary);
    }

	public Vector3 GetCenter(int player_id)
	{
		Player p = players[player_id];

		MyTriMesh mesh;
		if (!trimeshes.TryGetValue(p.selected_object, out mesh)) return new Vector3(0,0,0);
		switch (p.edit_mode)
		{
			case EditMode.Object:
				return mesh.GetCenter();
			case EditMode.Face:
				return mesh.GetCenterFace(p.selected_secondary);
			case EditMode.Vertex:
				return mesh.GetVert(p.selected_secondary);
		}
        return new Vector3(0, 0, 0);

    }

	/////////////////////////////////////////////////////////////////////
	///

	public (int, int, float) GetClosestVertex(Vector3 position)
    {
        float min_dist = float.MaxValue;
        int min_obj_id = 0;
        int min_vert_id = 0;

        foreach (KeyValuePair<int, MyTriMesh> mesh_entry in trimeshes)
        {

            (int vert_id, float dist_to) = mesh_entry.Value.GetClosestVertex(position);
            if (dist_to < min_dist)
            {
                min_dist = dist_to;
                min_obj_id = mesh_entry.Key;
                min_vert_id = vert_id;
            }
        }

        return (min_obj_id, min_vert_id, min_dist);
    }

    public (int, int, float) GetClosestFace(Vector3 position)
    {
        float min_dist = float.MaxValue;
        int min_obj_id = 0;
        int min_face_id = 0;

        foreach (KeyValuePair<int, MyTriMesh> mesh_entry in trimeshes)
        {

            (int face_id, float dist_to) = mesh_entry.Value.GetClosestFace(position);
            if (dist_to < min_dist)
            {
                min_dist = dist_to;
                min_obj_id = mesh_entry.Key;
                min_face_id = face_id;
            }
        }

        return (min_obj_id, min_face_id, min_dist);
    }


    private (int, int) GetClosestVertexId(Vector3 position)
    {
        (int mesh, int vertex, float dist) = GetClosestVertex(position);
        return (dist < min_select_dist) ? (mesh, vertex) : (-1, -1);
    }

    private (int, int) GetClosestFaceId(Vector3 position)
    {
        (int mesh, int face, float dist) = GetClosestFace(position);
        return (dist < min_select_dist) ? (mesh, face) : (-1, -1);
    }


    public (int, int) GetClosest(int player_id)
    {
        Player p = players[player_id];
        switch (p.edit_mode)
        {
            case EditMode.Object:
                return GetClosestFaceId(p.transform.position);
            case EditMode.Face:
                return GetClosestFaceId(p.transform.position);
            case EditMode.Vertex:
                return GetClosestVertexId(p.transform.position);
        }
        return (-1, -1);
    }
}
