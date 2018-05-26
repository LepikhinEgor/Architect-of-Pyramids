using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    [SerializeField]
    private float speed = 10.0F;

    private Transform target;
    [SerializeField]
    public bool isFollow = true;

    private void Awake()
    {
         target = GameObject.FindGameObjectWithTag("Block").transform;
    }

    void Update()
    {
        if (isFollow)
        {
            Vector3 cameraPos = target.position;
            cameraPos.x = 0;
            cameraPos.z -= 10F;
            cameraPos.y += 3F;

            transform.position = Vector3.Lerp(transform.position, cameraPos, speed * Time.deltaTime);
        }
    }
}
