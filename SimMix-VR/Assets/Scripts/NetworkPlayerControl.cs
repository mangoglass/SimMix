using UnityEngine;
using UnityEngine.Networking;

public class NetworkPlayerControl : NetworkBehaviour
{
    public Transform head;
    public Transform vr_camera;
    public Transform left_hand;
    public Transform right_hand;
    public GameObject camera_rig;

    // Start is called before the first frame update
    void Start()
    {
        if(!isLocalPlayer)
        {
            Destroy(camera_rig);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(isLocalPlayer)
        {
            head.transform.SetPositionAndRotation(vr_camera.transform.position,vr_camera.transform.rotation);
        }
    }
}
