using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectName : MonoBehaviour {
    private bool isGrowing;
    private Vector3 scale;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        scale = transform.localScale;
        if (isGrowing)
        {
            if (scale.x < 1.25)
            {
                scale.x += 0.8F * Time.deltaTime;
                scale.y = scale.x;
            }
            else
                isGrowing = false;
        }
        else
            if (scale.x > 0.75)
            {
                scale.x -= 0.8F * Time.deltaTime;
                scale.y = scale.x;
            }
            else
                isGrowing = true;
        transform.localScale = scale;
    }
}
