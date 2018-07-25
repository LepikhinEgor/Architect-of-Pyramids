using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogController : MonoBehaviour {

#region Variables
    public Material material;
    public Block block;
    private Vector4 illumUv;
    private float fogPower = 0;
    private float finalFogPower = 0.915F;
    public bool isDissipating;
#endregion

    // Use this for initialization
    void Start () {
        if (!SystemInfo.supportsImageEffects)
            enabled = false;
        material = Resources.Load<Material>("Materials/Fog");
	}

    // Update is called once per frame

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        material.SetFloat("_IlluminationRadius", Player.illumRadius);
        material.SetVector("_IlluminationUV", illumUv);
        material.SetFloat("_FogPower", fogPower);
        Graphics.Blit(source, destination, material);
    }

    void Update () {
        if (fogPower < finalFogPower && !isDissipating)
            fogPower += finalFogPower/2 * Time.deltaTime;
        if (isDissipating)
            if (fogPower > 0)
                fogPower -= finalFogPower / 2 * Time.deltaTime;
            else
                Destroy(this);

        illumUv.x = Camera.main.WorldToScreenPoint(block.transform.position).x / Screen.width;
        illumUv.y = Camera.main.WorldToScreenPoint(block.transform.position).y / Screen.height;
    }
}
