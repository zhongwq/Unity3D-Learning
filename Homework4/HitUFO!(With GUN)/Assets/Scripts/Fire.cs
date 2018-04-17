using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour {
    public new Camera camera;
    public FirstController sceneController;
    LayerMask layer;
    public GameObject fireFlash;
    bool showFireFlash = false;
    float flashTime = 0;

    void Awake()
    {
        layer = LayerMask.GetMask("UFO", "Terrian");
        fireFlash.SetActive(false);
    }

    // Use this for initialization
    void Start () {
        camera = Camera.main;
        sceneController = GameDirector.getInstance().currentSceneController as FirstController;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButton("Fire1"))
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity ,layer))
            {
                if (hit.transform.gameObject.layer == 8)
                {
                    sceneController.shootUFO(hit.transform.gameObject.GetComponent<UFOCtrl>().ufoController);
                }
                else if (hit.transform.gameObject.layer == 9)
                {
					sceneController.shootGround (hit.point);
                }
            }
            showFireFlash = true;
            fireFlash.SetActive(true);
        }

        if (showFireFlash)
        {
            if (flashTime >= 0.1f)
            {
                showFireFlash = false;
                fireFlash.SetActive(false);
                flashTime = 0;
            }
            flashTime += Time.deltaTime;
        }
	}
}
