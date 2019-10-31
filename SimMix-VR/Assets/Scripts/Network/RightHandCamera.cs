using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightHandCamera : MonoBehaviour
{
    public Transform rightHandControl;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(rightHandControl != null)
        {
            transform.SetPositionAndRotation(rightHandControl.position, rightHandControl.rotation);
        }
    }
}
