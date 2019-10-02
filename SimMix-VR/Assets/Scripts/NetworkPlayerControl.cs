using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.XR;

public class NetworkPlayerControl : NetworkBehaviour
{
    public Transform vr_camera;
    public Transform vr_left;
    public Transform vr_right;
    public Transform head;
    public Transform left_hand;
    public Transform right_hand;
    public GameObject camera_rig;
    public GameObject debug_camera;
    public GameObject spectator_camera;
    //DEBUG!!!
    public GameObject spawnObject;
    public TextMesh debugText;
    [SyncVar]
    private int debugInt = 10;

    void Start()
    {

        if (!isLocalPlayer)
        {
            Destroy(camera_rig);
            Destroy(spectator_camera);
        }
        else
        {
            left_hand.GetComponentInChildren<MeshRenderer>().enabled = false;
            right_hand.GetComponentInChildren<MeshRenderer>().enabled = false;
        }

        if(!XRDevice.isPresent)
        {
            Destroy(camera_rig);
            debug_camera.SetActive(true);
        }

        debugText.text = debugInt.ToString();
    }

    void Update()
    {
        if (isLocalPlayer && XRDevice.isPresent)
        {
            UpdatePositions();

            //DEBUG!!!
            if (Input.GetKeyDown(KeyCode.H))
            {
                GameObject ob = Instantiate(spawnObject);
                ob.transform.SetPositionAndRotation(vr_right.position, vr_right.rotation);
                NetworkServer.Spawn(ob);
            }
            if(Input.GetKeyDown(KeyCode.D))
            {
                debugInt--;
                debugText.text = debugInt.ToString();
            }

        }
            
        
    }

    private void UpdatePositions()
    {        
        head.transform.SetPositionAndRotation(vr_camera.position, vr_camera.rotation);
        left_hand.transform.SetPositionAndRotation(vr_left.position, vr_left.rotation);
        right_hand.transform.SetPositionAndRotation(vr_right.position, vr_right.rotation);   
    }
}
