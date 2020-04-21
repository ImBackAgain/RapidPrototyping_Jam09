using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmphasizeInstruction : MonoBehaviour
{
    public GameObject boast;
    public GameObject emphasizeInstruction, emphasizepointer;
    void Start()
    {
        emphasizeInstruction.SetActive(false);
        emphasizepointer.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (boast.GetComponent<Button>().interactable == true)
        {
            emphasizeInstruction.SetActive(true);
            emphasizepointer.SetActive(true);
        }
    }
    public void triggered()
    {
        emphasizeInstruction.SetActive(false);
        emphasizepointer.SetActive(false);
        Destroy(this);
    }
}
