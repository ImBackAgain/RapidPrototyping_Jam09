﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WinCondition : MonoBehaviour
{
    // Net income goal in each level
    public float GoalNetIncome;
    [HideInInspector] public Text goaltext;
    // The number of customers in one level
    public int TotalCustomerNumber;
    // The number of customers which you failed to make a deal with;
    public static int FailedCustomerNumber = 0;
    // mark the number of active docks, indicating the last 5 ships 
    public static int activedocks = 5;
    private int CurrentLevelWinCondition;
    void Start()
    {
        goaltext = GameObject.Find("GoalPanelText").GetComponent<Text>();
        CurrentLevelWinCondition = Random.Range(0, 4);//draw a goal from the pool
        WinConditionsText(CurrentLevelWinCondition); //display the goal
    }

    // Update is called once per frame
    void LateUpdate()
    {
        WinConditions(CurrentLevelWinCondition);
        if (Input.GetKeyDown(KeyCode.W)) StartCoroutine(Win());
    }

    void WinConditions(int ConditionIndex)                    //condition logics
    {
        switch (ConditionIndex)
        {
            case 0:                                        //win condition 0
                if (GameManager.instance.netIncome >= 5000)
                {
                    GameManager.instance.
                    StartCoroutine("Win");
                }
                break;
            case 1:                                        //win condition 1
                if (GameManager.instance.VisitedCustomerNumber > TotalCustomerNumber)
                {
                    if (GameManager.instance.netIncome >= GoalNetIncome)
                        StartCoroutine("Win");
                    else
                        StartCoroutine("Lose");
                }
                break;
            case 2:                                        //win condition 2
                if (FailedCustomerNumber > 0)
                    StartCoroutine("Lose");
                if (GameManager.instance.VisitedCustomerNumber > TotalCustomerNumber)
                    StartCoroutine("Win");
                break;
            default:
                if (activedocks == 0)
                    StartCoroutine("Win");
                break;
        }
    }
    void WinConditionsText(int ConditionIndex)                 //condition texts
    {
        switch (ConditionIndex)
        {
            case 0:                                        //win condition 0 text
                goaltext.text = "Goal: Earn $5000 Profit";
                break;
            case 1:                                        //win condition 1 text
                goaltext.text = "Goal: Earn $" + GoalNetIncome + " Profit within " + TotalCustomerNumber + " Customers";
                break;
            case 2:                                        //win condition 2 text
                goaltext.text = "Goal: Do Not Fail a Single Deal in " + TotalCustomerNumber + " Customers";
                break;
            default:                                       //other conditions text
                goaltext.text = "Goal: Empty Your Inventory (Sell 30 Ships)";
                break;
        }
    }
    private IEnumerator Win()
    {
        GameManager.PermaPause();
        yield return new WaitForSeconds(4.0f);
        SceneManager.LoadScene("WinScene");
    }

    private IEnumerator Lose()
    {
        GameManager.PermaPause();
        yield return new WaitForSeconds(3.0f);
        SceneManager.LoadScene("LoseScene");
    }
}
