using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SnackInstruction : MonoBehaviour
{
    public GameObject actions;
    public GameObject snack, snackpointer, snackinformationtext, snackinformationpointer;
    // Start is called before the first frame update
    void Start()
    {
        snackinformationtext.SetActive(false);
        snackinformationpointer.SetActive(false);
        snack.GetComponent<Image>().color = new Color32(12, 191, 236, 255);
    }

    // Update is called once per frame
    void Update()
    {
        if (int.Parse(actions.GetComponent<Text>().text) == 2)
        {
            snack.transform.position = new Vector3(374.7f, 326.3f, 3.2414f);
            snackpointer.transform.position = new Vector3(263f, 310f, 3.2414f);
            snackinformationtext.SetActive(true);
            snackinformationpointer.SetActive(true);
        }        //12.191.236
    }
    public void triggered()
    {
        snackinformationtext.SetActive(false);
        snackinformationpointer.SetActive(false);
        snack.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        Destroy(this);
    }

}
