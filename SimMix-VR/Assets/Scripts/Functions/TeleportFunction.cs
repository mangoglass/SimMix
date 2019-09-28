using UnityEngine;

public class TeleportFunction : IFunction 
{
    private TeleportHandler teleportRef;
    private Transform controllerTransform;
    private GameObject line;
    private GameObject cursor;
    private float cursorHooverDist;
    private float lineMaxLength;
    private float fadeTime;
    private int rayLayer;
    private bool canTeleport;

    public TeleportFunction(Transform o_controllerTransform) 
    {
        controllerTransform = o_controllerTransform;
        canTeleport = false;

        line = Resources.Load("Prefabs/TeleportLine") as GameObject;
        cursor = Resources.Load("Prefabs/TeleportCursor") as GameObject;

        line = Object.Instantiate(line, controllerTransform);
        cursor = Object.Instantiate(cursor);

        line.SetActive(false);
        cursor.SetActive(false);

        Globals globals = Object.FindObjectOfType<Globals>();

        teleportRef = globals.teleportReference;
        cursorHooverDist = globals.cursorHooverDistance;
        line.transform.localPosition = globals.lineOffset;
        lineMaxLength = globals.lineMaxLength;
        rayLayer = globals.rayLayer;
        fadeTime = globals.teleportFadeTime;
    }

    public bool Call(IInputParser input) 
    {
        bool teleportDisplay = input.TeleportBool();

        if(teleportDisplay) 
        {
            line.SetActive(true);
            SetLineAndCursor(input);
        }

        else 
        {
            if (canTeleport) 
            {
                Vector3 target = cursor.transform.position;
                target.y -= cursorHooverDist;
                teleportRef.Teleport(target, fadeTime);
            }

            line.SetActive(false);
            cursor.SetActive(false);
            canTeleport = false;
        }

        return teleportDisplay;
    }

    private void SetLineAndCursor(IInputParser input) 
    {
        Transform lineT = line.transform;
        Ray ray = new Ray(controllerTransform.position, controllerTransform.forward);
        RaycastHit hit = new RaycastHit();

        // If the ray cast from the controller forward hit something
        if(Physics.Raycast(ray, out hit, 50, 1 << rayLayer)) 
        {
            cursor.SetActive(true);
            cursor.transform.position = new Vector3(hit.point.x, hit.point.y + cursorHooverDist, hit.point.z);
            SetLineScale(Vector3.Distance(hit.point, ray.origin));
            canTeleport = true;
        }

        else 
        {
            cursor.SetActive(false);
            SetLineScale(lineMaxLength);
            canTeleport = false;
        }

    }

    void SetLineScale(float length) 
    {
        Transform lineTransform = line.transform.GetChild(0);

        Vector3 oPos = lineTransform.localPosition;
        Vector3 oScale = lineTransform.localScale;

        lineTransform.localScale = new Vector3(oScale.x, length, oScale.z);
        lineTransform.localPosition = new Vector3(oPos.x, oPos.y, length);
    }


}
