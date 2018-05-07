using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerfectTimer : MonoBehaviour
{
    // private bool isStop;
    private float timer = 0;
    public float Timer
    {
        set { timer = value; }
        get { return timer; }
    }
    void Awake()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (timer > 0)
            timer -= Time.deltaTime;
    }
}