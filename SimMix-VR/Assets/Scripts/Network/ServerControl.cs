using UnityEngine;
using UnityEngine.Networking;
public class ServerControl : NetworkBehaviour
{

    private GameObject currentLocalPlayer;

    void Start()
    {
        currentLocalPlayer = null;
        //DEBUG!!!
        Debug.Log("From ServerControl, isServer: " + isServer);
        Debug.Log("hasAuthority: "+hasAuthority);
    }

    [Client]
    public void AddLocalPlayer(GameObject player)
    {
        currentLocalPlayer = player;
    }

    [Client]
    public void AddToScene(int shape, Vector3 position)
    {
        if (currentLocalPlayer != null)
        {
            currentLocalPlayer.GetComponent<NetworkPlayerControl>().CmdSpawnObject(shape, position);
        }
    }
}
