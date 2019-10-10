using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class to provide mouse movement for the camera
/// </summary>
public class Movement : MonoBehaviour
{
    // Things to run when the camera initializes
    void Start()
    {

    }

    public float movespeed = 13.0f;
    public float scrollspeed = 20.0f;


    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Debug.Log("Registered a mouse movement");
            if (Input.GetAxis("Mouse X") > 0)
            {
                transform.position += new Vector3(Input.GetAxisRaw("Mouse X") * Time.deltaTime * -movespeed,
                                           Input.GetAxisRaw("Mouse Y") * Time.deltaTime * -movespeed, 0);
            }
            else if (Input.GetAxis("Mouse X") < 0)
            {
                transform.position += new Vector3(Input.GetAxisRaw("Mouse X") * Time.deltaTime * -movespeed,
                                           Input.GetAxisRaw("Mouse Y") * Time.deltaTime * -movespeed, 0);
            }
            else if (Input.GetAxis("Mouse Y") > 0)
            {
                transform.position += new Vector3(Input.GetAxisRaw("Mouse X") * Time.deltaTime * -movespeed,
                                           Input.GetAxisRaw("Mouse Y") * Time.deltaTime * -movespeed, 0);
            }
            else if (Input.GetAxis("Mouse Y") < 0)
            {
                transform.position += new Vector3(Input.GetAxisRaw("Mouse X") * Time.deltaTime * -movespeed,
                                           Input.GetAxisRaw("Mouse Y") * Time.deltaTime * -movespeed, 0);
            }
        }
        if (Input.mouseScrollDelta.y != 0)
        {
            float newscrollpos = Input.mouseScrollDelta.y * Time.deltaTime * scrollspeed;
            if ((transform.position.z + newscrollpos) <= -2 && (transform.position.z + newscrollpos) >= -15)
            {
                Debug.Log("Registered a scroll");
                transform.position += new Vector3(0, 0, newscrollpos);
            }
        }

    }
}