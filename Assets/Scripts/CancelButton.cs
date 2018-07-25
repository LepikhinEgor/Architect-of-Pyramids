using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CancelButton : MonoBehaviour
{
    Sprite toMenuSprite;
    Sprite removeBlock;
    public GameObject blockSelection;

    // Use this for initialization
    void Start()
    {
        GetComponent<AudioSource>().volume = Player.soundsVolume;
        blockSelection = GameObject.FindGameObjectWithTag("BlockSelection");
        toMenuSprite = Resources.Load<Sprite>("Sprites/RetryButton2");
        removeBlock = Resources.Load<Sprite>("Sprites/cancelButton");
        if (Player.isChoosingPlatform)
            GetComponent<Image>().sprite = removeBlock;
        else
            GetComponent<Image>().sprite = toMenuSprite;
    }


    public void CancelButtonAction()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");


        if (Player.isChoosingPlatform)
        {
            GetComponent<AudioSource>().Play();
            Player.isChoosingPlatform = false;
            if (Player.selectedPlatfomID != -1)
            {

                Destroy(Player.selectedPlatform.transform.Find("YellowLightPrefab(Clone)").gameObject);
                Player.selectedPlatform.transform.Find("Select").gameObject.SetActive(true);
            }
            GameObject sample = GameObject.FindGameObjectWithTag("Sample");
            sample.SetActive(false);
            Player.selectedPlatfomID = -1;
            Player.score = 0;
            GameObject canvas = GameObject.FindGameObjectWithTag("MainCanvas");
            Destroy(canvas.transform.Find("SampleScorePrefab(Clone)").gameObject);
            if (Player.selectedPlatform != null)
                Destroy(canvas.transform.Find("BlockScorePrefab(Clone)").gameObject);
            player.GetComponent<Player>().ShowSelectBlockUI();
            if (Player.pointer != null)
                Destroy(Player.pointer);
            GetComponent<Image>().sprite = toMenuSprite;
        }
        else
        {
            Player.score = 0;
            SceneManager.LoadSceneAsync("MainMenu");
        }
    }
}
