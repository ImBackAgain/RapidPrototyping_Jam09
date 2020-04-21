using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectShipInstruction : MonoBehaviour
{
    public GameObject[] dock = new GameObject[5];
    public GameObject step2text, step2pointer;
    public GameObject step3text, step3pointer;
    Color[] initialcolor = new Color[5];
    // Start is called before the first frame update
    void Start()
    {
        for (int t = 0; t <= 4; t++)
        {
            initialcolor[t] = dock[t].GetComponent<Image>().color;
            dock[t].SetActive(false);
            dock[t].transform.parent.GetChild(2).gameObject.SetActive(false);
        }
        step2text.SetActive(false);
        step2pointer.SetActive(false);
        step3text.SetActive(false);
        step3pointer.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (step2text.activeSelf == true && step2pointer.activeSelf == true)
        {
            for (int t = 0; t <= 4; t++)
            {
                dock[t].SetActive(true);
                dock[t].transform.parent.GetChild(2).gameObject.SetActive(true);
            }

        }
        for (int t = 0; t <= 4; t++)
        {
            if (dock[t].GetComponent<Image>().color != initialcolor[t])
            {
                step2pointer.SetActive(false);
                step2text.SetActive(false);
                step3text.SetActive(true);
                step3pointer.SetActive(true);
                Destroy(this);
            }
        }
    }
}
