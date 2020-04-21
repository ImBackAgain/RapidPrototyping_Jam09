using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WinCondition : MonoBehaviour
{
    enum ConditionType
    {
        Profit,             //Get specific profit before finishing inventory.
        ProfitByCustomer,   //Get specific profit within specific number of customers.
        NoFailure,          //Selll something to each customer.
        SelllOut            //Can't lose. Finish inventory to win.
    }

    [SerializeField] ConditionType CurrentLevelWinCondition;

    [Header("For \"Profit\" and \"ProfitByCustomer\"")]
    public float GoalNetIncome;

    public static WinCondition instance;
    
    [HideInInspector] public Text goaltext;
    [Space(10)]
    // The number of customers in one level
    public int TotalCustomerNumber;
    // The number of customers which you failed to make a deal with;
    public static int FailedCustomerNumber = 0;
    // mark the number of active docks, indicating the last 5 ships 
    public static int activedocks = 5;
    public GameObject wincanvas;

    // Make instance a singleton
    void Awake()
    {
        if (instance != null)
            Destroy(instance);
        instance = this;
    }

    void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    void Start()
    {
        FailedCustomerNumber = 0;
        activedocks = 5;
        goaltext = GameObject.Find("GoalPanelText").GetComponent<Text>();
        //CurrentLevelWinCondition = Random.Range(0, 4);//draw a goal from the pool
        WinConditionsText(CurrentLevelWinCondition); //display the goal
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //CheckWinCondition();
        if (Input.GetKeyDown(KeyCode.W)) StartCoroutine(Win());
    }

    public bool CheckWinCondition()        //condition logics
    {
        //print("checking win condition");
        switch (CurrentLevelWinCondition)
        {
            case ConditionType.Profit:
                print("Profit?");
                print(GameManager.instance.netIncome + " >= " + GoalNetIncome + "?");
                if (GameManager.instance.netIncome >= GoalNetIncome)
                {
                    StartCoroutine("Win");
                    return true;
                }
                break;
            case ConditionType.ProfitByCustomer:
                if (GameManager.instance.netIncome >= GoalNetIncome)
                {
                    StartCoroutine("Win");
                    return true;
                }
                else if (GameManager.instance.VisitedCustomerNumber >= TotalCustomerNumber)
                {
                    StartCoroutine("Lose");
                    return true;
                }
                break;
            case ConditionType.NoFailure:
                if (FailedCustomerNumber > 0)
                {
                    StartCoroutine("Lose");
                    return true;
                }
                if (GameManager.instance.VisitedCustomerNumber >= TotalCustomerNumber)
                {
                    StartCoroutine("Win");
                    return true;
                }
                break;
            default:
                if (activedocks == 0)
                {
                    StartCoroutine("Win");
                    return true;
                }
                break;
        }
        return false;
    }
    void WinConditionsText(ConditionType Condition)                 //condition texts
    {
        int ConditionIndex = (int)Condition;
        switch (ConditionIndex)
        {
            case 0:                                        //win condition 0 text
                goaltext.text = "Goal: Earn $" + GoalNetIncome + " Profit";
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
        GameManager.instance.PermaPause();       
        yield return new WaitForSeconds(4.0f);
        wincanvas.SetActive(true);       
    }

    private IEnumerator Lose()
    {
        GameManager.instance.PermaPause();
        yield return new WaitForSeconds(3.0f);
        SceneManager.LoadScene("LoseScene");
    }
}
