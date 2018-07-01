using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicVolumeController : MonoBehaviour {
    AudioSource menuSoundAudio;
    Slider musicVolume;
	// Use this for initialization
	void Start () {
        musicVolume = GetComponent<Slider>();
        menuSoundAudio = GameObject.FindGameObjectWithTag("MenuSound").GetComponent<AudioSource>();

    }
	
	// Update is called once per frame
	void Update () {
        Player.musicVolume = musicVolume.value;
        menuSoundAudio.volume = musicVolume.value;

    }
}
