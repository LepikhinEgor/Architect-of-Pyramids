using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreDiffMotion : MonoBehaviour {


    Vector3 targetPosition;
	// Use this for initialization

    public void SetTargetPos(Vector3 targetPos)
    {
        targetPosition = targetPos;
    }

	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = Vector3.Lerp(transform.position, targetPosition, 2*Time.deltaTime);
        if (System.Math.Abs(transform.position.y - targetPosition.y) < Screen.height / 160)
            Destroy(transform.gameObject);
	}
}
