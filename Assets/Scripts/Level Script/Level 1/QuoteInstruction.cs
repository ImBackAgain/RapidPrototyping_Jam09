using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuoteInstruction : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject step3text, step3pointer;
    public GameObject quoteInstruction, quoteInstructionPointer;
    public GameObject offerpanel;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        quoteInstruction.SetActive(true);
        quoteInstructionPointer.SetActive(true);
        if (offerpanel.activeSelf == false)
        {
            quoteInstruction.SetActive(false);
            quoteInstructionPointer.SetActive(false);
        }
    }

    public void quote()
    {
        step3text.SetActive(false);
        step3pointer.SetActive(false);        
    }
}
