using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MyTriMesh : MonoBehaviour
{
    Dictionary<int, MyMeshFace> faces;
    Dictionary<int, Vector3> verti;

    int last_face;
    int last_vertex;

    Mesh mesh;
    bool rerender;

    Outline outline;

    public void Awake()
    {
        faces = new Dictionary<int, MyMeshFace>();
        verti = new Dictionary<int, Vector3>();
        rerender = false;
    }

    public void Start()
    {
        mesh = new Mesh();
        gameObject.AddComponent<MeshFilter>().mesh = mesh;


        ////////
        outline = gameObject.AddComponent<Outline>();
        outline.OutlineMode = Outline.Mode.OutlineAll;
        outline.enabled = false;
        outline.OutlineColor = (Color.red + Color.yellow) / 2.0f;
        outline.OutlineWidth = 6.0f;
    }

    public void Update()
    {
        if (rerender)
        {
            Render();
            rerender = false;
        }
    }

    public void CloneFrom(MyTriMesh og_tri_mesh)
    {
        rerender = true;
        last_face = og_tri_mesh.last_face;
        last_vertex = og_tri_mesh.last_vertex;

        verti = new Dictionary<int, Vector3>(og_tri_mesh.verti);

        faces = new Dictionary<int, MyMeshFace>();
        foreach (KeyValuePair<int, MyMeshFace> entry in og_tri_mesh.faces)
        {
            faces.Add(entry.Key, entry.Value.Clone());
        }

    }

    public void Render()
    {
        Vector3[] vertices_array = new Vector3[faces.Count * 3];
        int[] faces_array = new int[faces.Count * 3];
        Color[] colors_array = new Color[faces.Count * 3];

        int curr_index = 0;
        foreach (KeyValuePair<int, MyMeshFace> face_entry in faces)
        {
            faces_array[curr_index] = curr_index;
            faces_array[curr_index + 1] = curr_index + 1;
            faces_array[curr_index + 2] = curr_index + 2;

            vertices_array[curr_index] = verti[face_entry.Value.vertices[2]];
            vertices_array[curr_index + 1] = verti[face_entry.Value.vertices[1]];
            vertices_array[curr_index + 2] = verti[face_entry.Value.vertices[0]];


            Color color = face_entry.Value.color;
            if (face_entry.Value.selected)
            {
                color = (color + Color.magenta) / 2.0f;
            }

            colors_array[curr_index] = color;
            colors_array[curr_index + 1] = color;
            colors_array[curr_index + 2] = color;

            curr_index += 3;

        }

        mesh.vertices = vertices_array;
        mesh.triangles = faces_array;
        mesh.colors = colors_array;
        mesh.RecalculateNormals();
    }


    public int CreateVertex(Vector3 pos)
    {
        rerender = true;
        last_vertex += 1;

        verti.Add(last_vertex, pos);
        return last_vertex;
    }

    public int CreateFace(int v0, int v1, int v2)
    {
        rerender = true;

        MyMeshFace face = new MyMeshFace(v0, v1, v2);
        last_face += 1;
        faces.Add(last_face, face);
        return last_face;
    }

    public int CreateFace(int v0, int v1, int v2, Color color)
    {
        rerender = true;

        MyMeshFace face = new MyMeshFace(v0, v1, v2);
        face.color = color;
        last_face += 1;
        faces.Add(last_face, face);
        return last_face;
    }


    public Vector3[] GetFaceVerti(int face_id)
    {
        return faces[face_id].vertices.Select(x => verti[x]).ToArray();
    }

    public (int, float) GetClosestVertex(Vector3 position)
    {
        float min_dist = float.MaxValue;
        int min_vert_id = 0;

        foreach (KeyValuePair<int, Vector3> vert_entry in verti)
        {

            float dist_to = Vector3.Distance(position, vert_entry.Value);
            if (dist_to < min_dist)
            {
                min_dist = dist_to;
                min_vert_id = vert_entry.Key;
            }
        }
        return (min_vert_id, min_dist);
    }

    public (int, float) GetClosestFace(Vector3 position)
    {
        float min_dist = float.MaxValue;
        int min_face_id = 0;

        foreach (KeyValuePair<int, MyMeshFace> face_entry in faces)
        {
            float dist_to = face_entry.Value.ClosestDistance(position, verti);
            if (dist_to < min_dist)
            {
                min_dist = dist_to;
                min_face_id = face_entry.Key;
            }
        }
        return (min_face_id, min_dist);
    }


    public Vector3 GetCenter()
    {
        return GetCenter(verti.Keys.ToArray());
    }

    private Vector3 GetCenter(int[] vert_ids)
    {
        Vector3 sum = new Vector3(0, 0, 0);

        foreach (int vert_id in vert_ids)
        {
            sum += verti[vert_id];
        }

        return sum / vert_ids.Count();
    }

    public Vector3 GetCenterFace(int face_id)
    {
        return GetCenter(faces[face_id].vertices);
    }

    public Vector3[] GetDisplacedFaceVerti(int face_id, float displacement)
    {
        Vector3 v0 = verti[faces[face_id].vertices[0]];
        Vector3 v1 = verti[faces[face_id].vertices[1]];
        Vector3 v2 = verti[faces[face_id].vertices[2]];
        Vector3 n = Vector3.Normalize(Vector3.Cross(v2 - v0, v1 - v0));
        return new Vector3[3] { v0 + displacement * n, v1 + displacement * n, v2 + displacement * n };


    }


    public void Translate(int vert_id, Vector3 diff)
    {
        rerender = true;
        verti[vert_id] += diff;
    }

    public void Translate(Vector3 diff)
    {
        rerender = true;
        Translate(verti.Keys.ToArray(), diff);
    }

    public void Translate(int[] vert_ids, Vector3 diff)
    {
        rerender = true;
        foreach (int vert_id in vert_ids)
        {
            Translate(vert_id, diff);
        }
    }

    public void TranslateFace(int face_id, Vector3 diff)
    {
        rerender = true;
        Translate(faces[face_id].vertices, diff);
    }


    public void Scale(float factor)
    {
        rerender = true;
        Scale(verti.Keys.ToArray(), factor);
    }

    public void Scale(int[] vert_ids, float factor)
    {
        rerender = true;
        Vector3 center = GetCenter(vert_ids);
        foreach (int vert_id in vert_ids)
        {
            verti[vert_id] = factor * (verti[vert_id] - center) + center;
        }
    }

    public void ScaleFace(int face_id, float factor)
    {
        rerender = true;
        Scale(faces[face_id].vertices, factor);
    }

    public void Rotate(Quaternion rotation)
    {
        rerender = true;
        Rotate(verti.Keys.ToArray(), rotation);
    }

    public void Rotate(int[] vert_ids, Quaternion rotation)
    {
        rerender = true;

        Vector3 center = GetCenter(vert_ids);
        //Quaternion rotation = Quaternion.Euler(euler_angles.x, euler_angles.y, euler_angles.z);
        Matrix4x4 m = Matrix4x4.Rotate(rotation);

        foreach (int vert_id in vert_ids)
        {
            verti[vert_id] = m.MultiplyPoint3x4(verti[vert_id] - center) + center;
        }
    }

    public void RotateFace(int face_id, Quaternion rotation)
    {
        rerender = true;
        Rotate(faces[face_id].vertices, rotation);
    }

    public void Extrude(int face_id)
    {
        rerender = true;
        int v0 = faces[face_id].vertices[0];
        int v1 = faces[face_id].vertices[1];
        int v2 = faces[face_id].vertices[2];

        int new_v0 = CreateVertex(verti[v0]);
        int new_v1 = CreateVertex(verti[v1]);
        int new_v2 = CreateVertex(verti[v2]);

        Color og_color = faces[face_id].color;

        CreateFace(new_v0, v0, v1, og_color);
        CreateFace(new_v0, v1, new_v1, og_color);
        CreateFace(new_v1, v1, v2, og_color);
        CreateFace(new_v1, v2, new_v2, og_color);
        CreateFace(new_v2, v2, v0, og_color);
        CreateFace(new_v2, v0, new_v0, og_color);
        faces[face_id].vertices = new int[] { new_v0, new_v1, new_v2 };
    }


    public void SetColor(Color color)
    {
        rerender = true;
        SetColor(faces.Keys.ToArray(), color);
    }

    public void SetColor(int[] face_ids, Color color)
    {
        rerender = true;
        foreach (int face_id in face_ids)
        {
            SetColor(face_id, color);
        }
    }

    public void SetColor(int face_id, Color color)
    {
        rerender = true;
        faces[face_id].color = color;
    }


    public void Select()
    {
        rerender = true;
        //kan inte göra såhär
        //SelectFace(faces.Keys.ToArray());

        outline.enabled = true;

    }

    public void SelectFace(int[] face_ids)
    {
        rerender = true;
        foreach (int face_id in face_ids)
        {
            SelectFace(face_id);
        }
    }

    public void SelectFace(int face_id)
    {
        rerender = true;
        faces[face_id].selected = true;
    }


    public void Deselect()
    {
        rerender = true;

        outline.enabled = false;
        //kan inte göra såhär
        //DeselectFace(faces.Keys.ToArray());
    }

    public void DeselectFace(int[] face_ids)
    {
        rerender = true;
        foreach (int face_id in face_ids)
        {
            DeselectFace(face_id);
        }
    }

    public void DeselectFace(int face_id)
    {
        rerender = true;
        faces[face_id].selected = false;
    }

    public Vector3 GetVert(int vert_id)
    {
        return verti[vert_id];
    }
}

// :)
public class MyMeshFace
{
    public int[] vertices;
    public Color color;
    public bool selected;

    public MyMeshFace Clone()
    {
        MyMeshFace mmf = new MyMeshFace(vertices[0], vertices[1], vertices[2]);
        mmf.color = color;
        return mmf;
    }

    public MyMeshFace(int v0, int v1, int v2)
    {
        selected = false;
        vertices = new int[] { v0, v1, v2 };
        //color = Color.gray;
        //color = Random.ColorHSV(0f, 1f, 0f, 0.5f, 1f, 1f);
        color = Random.ColorHSV(0f, 1f, 0f, 0.5f, 0.7f, 1f);

    }


    private Vector3 CartesianToBarycentric(Vector3 v1, Vector3 v2, Vector3 vp)
    {
        float d00 = Vector3.Dot(v1, v1);
        float d01 = Vector3.Dot(v1, v2);
        float d11 = Vector3.Dot(v2, v2);
        float d20 = Vector3.Dot(vp, v1);
        float d21 = Vector3.Dot(vp, v2);
        float denom = d00 * d11 - d01 * d01;

        float v = (d11 * d20 - d01 * d21) / denom;
        float w = (d00 * d21 - d01 * d20) / denom;
        float u = 1.0f - v - w;
        return new Vector3(v, w, u);
    }

    private Vector3 BarycentricToCartesian(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 b)
    {
        return b.x * p0 + b.y * p1 + b.z * p2;
    }

    private Vector3 Clamp01(Vector3 v)
    {
        return new Vector3(Mathf.Clamp01(v.x),
                           Mathf.Clamp01(v.y),
                           Mathf.Clamp01(v.z));
    }

    public float ClosestDistance(Vector3 position, Dictionary<int, Vector3> verti)
    {
        Vector3 p0 = verti[vertices[0]];
        Vector3 p1 = verti[vertices[1]];
        Vector3 p2 = verti[vertices[2]];

        Vector3 v1 = p1 - p0;
        Vector3 v2 = p2 - p0;
        Vector3 vp = position - p0;

        Vector3 n = Vector3.Normalize(Vector3.Cross(v1, v2));
        Vector3 proj_n = Vector3.Dot(vp, n) * n;
        Vector3 nearest_point_on_plane = position - proj_n;
        vp = nearest_point_on_plane - verti[vertices[0]];

        Vector3 bary = CartesianToBarycentric(v1, v2, vp);
        bary = Clamp01(bary);
        Vector3 nearest_point = BarycentricToCartesian(p0, p1, p2, bary);
        return (position - nearest_point).magnitude;
    }

}
