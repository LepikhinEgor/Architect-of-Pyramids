using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PerfectCoef : MonoBehaviour {

    Vector2 userScreenSize;
    RectTransform rectTransform;
    private int scaleSpeed;
    private float scaleAcceleration;
    private Text text;

    // Use this for initialization

    private void Awake()
    {
        userScreenSize.x = Screen.width;
        userScreenSize.y = Screen.height;
        rectTransform = GetComponent<RectTransform>();
        scaleSpeed = (int)(userScreenSize.x / 40); // 20
        scaleAcceleration = scaleSpeed*50; //1000
        text = GetComponent<Text>();
        text.fontSize = (int)(userScreenSize.x/6.6F);// 120
    }
    void Start () {
    }
	
    public void SetPosition(Vector3 pos)
    {
        rectTransform.position = pos;
    }

    public void SetText(string text)
    {
        GetComponent<Text>().text = text;
    }

	// Update is called once per frame
	void Update () {
        if (text.fontSize > userScreenSize.x /20 /*40*/) {
            text.fontSize -= (int)(scaleSpeed * Time.deltaTime) +1;
            scaleSpeed += (int)(scaleAcceleration*Time.deltaTime) +1;
        }
        else
        {
            Destroy(this.rectTransform.gameObject);
        }
	}
}
