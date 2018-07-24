using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotation : MonoBehaviour {
    //Скрипт не используется в игре, так как эффект получился "на любителя"
    public bool rotateForward;
    public bool rotateBack;
    private Block block;
    Camera cam;
    // Use this for initialization
    void Start () {
        rotateForward = true;
        cam = GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update () {
        if (rotateForward)
        {
            if (transform.rotation.eulerAngles.z < 180)
            {
                RotateCam(Vector3.forward);
            }
            else
            {
                rotateForward = false;
                //rotateBack = true;
                transform.position = new Vector3(0, 0, transform.position.z);
            }
        }

        if (rotateBack)
        {
            Debug.Log(transform.rotation.eulerAngles.z);
            if (transform.rotation.eulerAngles.z > 0 && transform.rotation.eulerAngles.z < 181)
            {
                RotateCam(Vector3.back);
            }
            else
            {
                rotateBack = false;
                transform.position = new Vector3(0, 0, transform.position.z);
                Debug.Log(transform.position);
            }
        }

    }

    void RotateCam(Vector3 dir)
    {
        // поворачивает камеру в заданном направлении с изменением размера и положения
        cam.orthographicSize = 2 + System.Math.Abs(90 - transform.rotation.eulerAngles.z) / 30;
        transform.Rotate(dir * 2F);

        float radAngle = (float)(transform.rotation.eulerAngles.z / 180 * System.Math.PI);

        Vector3 tempPos = Vector3.zero;
        tempPos.y -= (float)System.Math.Sin(radAngle) * 5;
        tempPos.z = transform.position.z;
        transform.position = tempPos;
    }
}
