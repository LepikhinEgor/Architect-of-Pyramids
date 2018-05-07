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

    Player player;
    private void Awake()
    {
        player = FindObjectOfType<Player>();
        if (!target)
            target = FindObjectOfType<Block>().transform;
    }

    void Update()
    {
        if (isFollow)
        {
            Vector3 cameraPos = target.position;
            //if (player.LevelNum == 1)
            //    cameraPos.x = 3.6F;
            //else if (player.LevelNum == 0)
            //    cameraPos.x = 0;
            //else
            //    cameraPos.x = 0 + 3.6F + (player.LevelNum - 1) * 2.15F;
            cameraPos.x = 0;
            cameraPos.z -= 10F;
            cameraPos.y += 3F;
            transform.position = Vector3.Lerp(transform.position, cameraPos, speed * Time.deltaTime);
        }
    }
}
