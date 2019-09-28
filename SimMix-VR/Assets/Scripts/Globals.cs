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
    public Material MenuPointerMaterial;
    public Material MenuUnselectedMaterial;
    public Material MenuHooverMaterial;
    public Material MenuSelectedMaterial;

    [Header("Teleport Variables")]
    public TeleportHandler teleportReference;
    public Vector3 lineOffset;
    [Range(10f, 10000f)]
    public float lineMaxLength;
    [Range(0f, 0.1f)]
    public float cursorHooverDistance;
    public float teleportFadeTime;
    public int rayLayer;

    [Header("Tool Variables")]
    [Range(0f, 1f)]
    public float minToolVisibility;
    public float scalingFactor;
    public float clickScalingFactor;
    [Range(0f, 1f)]
    public float scalingTriggerThreshold;

    [Header("Edit Mode Variables")]
    public Material objectMaterial;
    public Material faceMaterial;
    public Material vertexMaterial;
    public PrimitiveType swapMenuTypes;
    [Range(0f, 0.1f)]
    public float swapMenuElementScale;
    public float swapMenuXOffset;
    public float swapMenuYOffset;
    public float swapMenuTextPadding;
    public int swapMenuTextFontSize;
    public Color swapMenuTextColor;
    public Color selectedColor;
    public Material swapPointerMaterial;
    public Vector3 modeSelectedTextOffset;
    public Vector3 modePointerOffset;
    public float pointerMaxY;
    public float pointerMinY;
}
