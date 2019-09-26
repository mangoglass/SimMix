using UnityEngine;

/**
* The Camera movement is based on a camera from Babylon.js
* https://doc.babylonjs.com/babylon101/cameras#arc-rotate-camera
*/
public class SpectatorCameraControl : MonoBehaviour
{
    // Add (VR) camera rig here!
    public Transform target; 
    // Only used when zooming.
    public GameObject spectatorCamera; 

    public float radius = 2.5f;
    public float scrollSensitivity = 0.25f;
    public float minRadius = 2f;
    public float maxRadius = 5f;

    public float scrollSensibility = 300f;
    //private readonly float minBeta = 90f;

    private readonly int invert = -1;
    

    void Start()
    {
        // set up camera settings here!  

        // Set start position of spectator camera. 
        spectatorCamera.transform.localPosition = new Vector3(0, 0, -radius);

        // Set this position to camera rig position.
        transform.position = new Vector3(target.position.x, transform.position.y, target.position.z);
        
    }

    // Update is called once per frame
    void Update()
    {
        Zoom();
        Rotate();
        Move();
    }

    private void Zoom()
    {
        float deltaScroll = Input.mouseScrollDelta.y;
        if (deltaScroll != 0)
        {
            radius -= deltaScroll * scrollSensitivity;
            if (radius < minRadius || radius > maxRadius)
            {
                radius += deltaScroll * scrollSensitivity;
            }
            else
            {
                spectatorCamera.transform.localPosition = new Vector3(0, 0, -radius);
            }

        }
    }

    private void Rotate()
    {

        // 0 is left mouse button.
        if(Input.GetMouseButton(0))
        {
            // Alpha rotation
            float deltaMouseX = Input.GetAxis("Mouse X");
            float deltaAlpha = deltaMouseX * scrollSensibility * Time.deltaTime;
            transform.RotateAround(transform.position, Vector3.up, deltaAlpha);

            // Beta rotation
            float deltaMouseY = Input.GetAxis("Mouse Y");
            float deltaBeta = invert * deltaMouseY * scrollSensibility * Time.deltaTime;
            transform.Rotate(Vector3.right, deltaBeta, Space.Self);

            //Debug.Log(transform.localEulerAngles);
            //Debug.Log(transform.localEulerAngles.z);
        }
        
    }

    private void Move()
    {
        if(transform.position.x != target.position.x || transform.position.z != target.position.z)
        {
            transform.position = new Vector3(target.position.x, transform.position.y, target.position.z);
        }
    }
}
