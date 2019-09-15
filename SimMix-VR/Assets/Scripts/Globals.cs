using UnityEngine;

public class Globals : MonoBehaviour
{
    [Header("Mesh Manager global")]
    public MeshManager meshManager;

    [Header("Menu Variables")]
    public float menuScale;
    public float menuRadius;
    [Range(0.01f,1f)]
    public float menuThreshold;
    public Vector3 menuRelativePosition;
    public Vector3 menuRelativeRotation;
    public PrimitiveType menuElementType;
    public Vector3 menuElementLocalScale;
    public PrimitiveType pointerElementType;
    public Material pointerMaterial;
    public Material unselectedMaterial;
    public Material hooverMaterial;
    public Material selectedMaterial;
}
