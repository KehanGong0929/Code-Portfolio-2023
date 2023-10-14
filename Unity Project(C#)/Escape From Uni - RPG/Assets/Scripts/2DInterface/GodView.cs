using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GodView : MonoBehaviour
{
    public GameObject cmCam;

    // Start is called before the first frame update
    void Start()
    {
        //cmCam = GameObject.Find("CMMap");
    }

    // Update is called once per frame
    void Update()
    {

        StartCoroutine(MapKeyDetection());


    }

    IEnumerator MapKeyDetection()
    {
        if (Input.GetKeyDown("c"))
        {

            if (cmCam.activeSelf == false)
            {
                cmCam.SetActive(true);
            }
            else
            {
                cmCam.SetActive(false);
            }

        }

        yield return new WaitForSeconds(1.5f);

    }
}
