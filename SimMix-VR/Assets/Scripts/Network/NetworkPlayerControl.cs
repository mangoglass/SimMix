using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.XR;

//public class SyncListVector3 : SyncList<Vector3> {}

public class NetworkPlayerControl : NetworkBehaviour
{
    // Theses are the actuall head and hands of the user. 
    public Transform vr_camera;
    public Transform vr_left;
    public Transform vr_right;
    // These are the representations of the user's head and hands.
    public Transform head;
    public Transform left_hand;
    public Transform right_hand;
    // GameObjects to be destroyed.
    public GameObject camera_rig;
    public GameObject debug_camera;
    public GameObject right_hand_camera;
    public GameObject spectator_camera;
    // Spawnable objects
    public GameObject cubePrefab;   //1
    public GameObject icoPrefab;    //2

    //[SerializeField]
    private MeshManager meshManager;

    //DEBUG!!!
    //public GameObject spawnObject;
    //public TextMesh debugText;

    private float sendDelta = 1f/2f; // this is in seconds (not ms)!
    private float currentTime = 0f;

    void Start()
    {
        // If this instance is not the local player. 
        if (!isLocalPlayer)
        {
            Destroy(camera_rig);
            Destroy(spectator_camera);
            Destroy(right_hand_camera);
        }
        // If this instance is the local player. 
        else
        {
            GameObject serverControl = GameObject.FindWithTag("ServerControl");
            serverControl.GetComponent<ServerControl>().AddLocalPlayer(gameObject);
            
            // Disable the hand representations if this is local player.
            left_hand.GetComponentInChildren<MeshRenderer>().enabled = false;
            right_hand.GetComponentInChildren<MeshRenderer>().enabled = false;
        }
        // If a VR headset is missing, use this camera instead. 
        if (isLocalPlayer && !XRDevice.isPresent)
        {
            Destroy(camera_rig);
            Destroy(spectator_camera);
            Destroy(right_hand_camera);
            debug_camera.SetActive(true);
        }
        // Get the MeshManger of this client regardless of whether or not this is the localplayer. 
        meshManager = GameObject.FindWithTag("MeshManager").GetComponent<MeshManager>();

        
        
        //DEBUG!!
        //Debug.Log("isServer: " + isServer);
        //Debug.Log("isClient: " + isClient);
        //Debug.Log("isLocalPlayer: " + isLocalPlayer);
        //Debug.Log("XRDevice.isPresent: " + XRDevice.isPresent);
    }

    [ClientCallback]
    void Update()
    {
        if (isLocalPlayer)
        {
            if (Input.GetKeyDown(KeyCode.D))
            {
                //CmdChangeSyncVar();
            }

            if (XRDevice.isPresent)
            {
                // Update transform of head and hands. 
                UpdatePositions();
            }
            else
            {
                //DEBUG!!!
                if (Input.GetKeyDown(KeyCode.H))
                {
                    //CmdCreateObjectOnServerFromNonVr();
                    CmdSpawnObject(1, new Vector3(0, 1, 0));
                    //Debug.Log("Called CmdCreateObjectOnServerFromNonVr...");
                }
            }
        }
    }

    private void UpdatePositions()
    {
        head.transform.SetPositionAndRotation(vr_camera.position, vr_camera.rotation);
        left_hand.transform.SetPositionAndRotation(vr_left.position, vr_left.rotation);
        right_hand.transform.SetPositionAndRotation(vr_right.position, vr_right.rotation);
    }

    /**
     * This function is called from the server.
     */
    [Command]
    public void CmdSpawnObject(int shape, Vector3 position)
    {
        
        // It's a cube
        if (shape == 1)
        {
            GameObject meshObject = Instantiate(cubePrefab);
            meshObject.transform.position = position;
            NetworkServer.SpawnWithClientAuthority(meshObject,connectionToClient);
            RpcRegisterMesh(meshObject);
        } 
        // It's a icosphere
        else if (shape == 2)
        {
            GameObject meshObject = Instantiate(icoPrefab);
            meshObject.transform.position = position;
            NetworkServer.SpawnWithClientAuthority(meshObject, connectionToClient);
            RpcRegisterMesh(meshObject);
        }
    }

    [ClientRpc] // OBS! This function is only called for THIS player instance, not all player instances!
    public void RpcRegisterMesh(GameObject meshObject)
    {
        meshManager.RegisterMesh(meshObject);
    }


    private void OnDestroy()
    {
        //Todo: Call Mesh Manager and remove both controllers.
    }
}
