using System.Collections;
using UnityEngine;
using Valve.VR;

public class TeleportHandler : MonoBehaviour
{
    private bool isTeleporting;

    public void Awake () 
    {
        isTeleporting = false;
    }

    public void Teleport(Vector3 target, float fadeTime) {
        if (isTeleporting) { return; }

        Transform cameraRig = SteamVR_Render.Top().origin;
        Vector3 headPos = SteamVR_Render.Top().head.position;

        Vector3 groundPos = new Vector3(headPos.x, cameraRig.position.y, headPos.z);
        Vector3 teleportVector = target - groundPos;

        StartCoroutine(MoveRig(cameraRig, teleportVector, fadeTime));
    }

    private IEnumerator MoveRig(Transform cameraRig, Vector3 teleportVector, float fadeTime) 
    {
        isTeleporting = true;

        SteamVR_Fade.Start(Color.black, fadeTime, true);

        //wait for fade
        yield return new WaitForSeconds(fadeTime);
        cameraRig.position += teleportVector;

        SteamVR_Fade.Start(Color.clear, fadeTime, true);

        isTeleporting = false;
    }

}
