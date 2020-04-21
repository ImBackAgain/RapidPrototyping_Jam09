using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InterviewInstruction : MonoBehaviour
{
    public GameObject instruction, instructionPointer1, instructionPointer2;
    public GameObject Step1Pointer, Step1text, Step2Pointer, Step2text;
        
    void Start()
    {
        instruction.SetActive(false);
        instructionPointer1.SetActive(false);
        instructionPointer2.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void showinstruction()
    {
        instruction.SetActive(true);
        instructionPointer1.SetActive(true);
        instructionPointer2.SetActive(true);
        Step1Pointer.SetActive(false);
        Step1text.SetActive(false);
        Step2Pointer.SetActive(true);
        Step2text.SetActive(true);
        Destroy(this);
    }
}
