using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BuildButton : MonoBehaviour {

    public GameObject nextBlockBtn;
    public GameObject previousBlockBtn;
    public GameObject blockSelection;

    private void Awake()
    {
        blockSelection = GameObject.FindGameObjectWithTag("BlockSelection");
        nextBlockBtn = GameObject.FindGameObjectWithTag("NextBlock");
        previousBlockBtn = GameObject.FindGameObjectWithTag("PreviousBlock");
        

    }
    private void OnMouseDown()
    {

        //if (tag.Equals("NextBlock"))
        //    blockSelection.GetComponent<BlockSelection>().ChangeBlock(true);
        //if (tag.Equals("PreviousBlock"))
        //    blockSelection.GetComponent<BlockSelection>().ChangeBlock(false);
       

            
    }
    // Use this for initialization
    void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
        
    }
}
