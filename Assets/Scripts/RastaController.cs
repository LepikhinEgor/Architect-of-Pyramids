using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RastaController : MonoBehaviour {

    #region Variables
    public Material material;
    private Vector2 redOffset;
    private bool rightMotion;
    #endregion

    // Use this for initialization
    void Start()
    {
        redOffset = new Vector2(0, 0);
        if (!SystemInfo.supportsImageEffects)
            enabled = false;
        //material = Resources.Load<Material>("Materials/Fog");
    }

    // Update is called once per frame

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        material.SetFloat("_OffsetX", redOffset.x);
        material.SetFloat("_OffsetY", redOffset.y);
        Graphics.Blit(source, destination, material);
    }

    void Update()
    {
        if (rightMotion)
        {
            if (redOffset.x < 0.1F)
            {
                redOffset.x += Time.deltaTime * 0.07F;
            }
            else
                rightMotion = false;
        }
        else
            if (redOffset.x > 0)
            {
                redOffset.x -= Time.deltaTime * 0.07F;
            }
        else
            rightMotion = true;
    }
}
