using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundsVolumeController : MonoBehaviour {

    Slider soundsVolume;
    void Start () {
        soundsVolume = GetComponent<Slider>();
	}
	
	// Update is called once per frame
	void Update () {
        Player.soundsVolume = soundsVolume.value;
	}
}
