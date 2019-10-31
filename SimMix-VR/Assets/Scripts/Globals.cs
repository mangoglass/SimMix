using UnityEngine;

public class Globals : MonoBehaviour
{
    [Header("Mesh Manager global")]
    public MeshManager meshManager;

    [Header("Cursor Variables")]
    public float cursorScale;
    public Material cursorMaterial;

    [Header("Tool Variables")]
    public float toolTriggerThreshold;

    [Header("Menu Variables")]
    public string menuSystemFile;
    public float menuScale;
    public float menuRadius;
    [Range(0.01f,1f)]
    public float menuThreshold;
    public Vector3 menuRelativePosition;
    public Vector3 menuRelativeRotation;
    public PrimitiveType menuElementType;
    public float menuElementScale;
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
    public float objectDistanceFromController;
    public Transform vrCameraRef;
    public float minScale;
    public float maxScale;

    [Header("Edit Mode Variables")]
    public Material objectMaterial;
    public Material faceMaterial;
    public Material vertexMaterial;
    public Color[] modeColor = new Color[3];
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
