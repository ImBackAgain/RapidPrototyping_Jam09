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
        Profit,                 // Get specific profit. Also applies to alll other goals. But you can't lose with this one.
        ProfitByCustomer,       // Limited number of customers.
        ProfitNoFailure,        // Sell something to each customer.
        ProfitLimitedInventory  // You have a drastically limited number of ships.
    }

    [SerializeField] [Tooltip("Nullifies current level win condition")] bool RandomiseCondition = true;
    [SerializeField] ConditionType CurrentLevelWinCondition;

    [Header("For All Goals")]
    public float GoalNetIncome;
    [Tooltip("Picks entire list if greater than equal to prefab list's count")]
    public int TotalShipCount;

    [Space(10)]
    [Header("For Profit By Customer")]
    public int TotalCustomerNumber;


    public static WinCondition instance;
    
    [HideInInspector] public Text goaltext;
    // The number of customers which you failed to make a deal with;
    public static int FailedCustomerNumber = 0;
    // mark the number of active docks, indicating the last 5 ships 
    public static int activedocks = 5;
    [Space(20)]
    public GameObject wincanvas;
    public GameObject losecanvas;
    Canvas mainCanvas;

    // Make instance a singleton
    void Awake()
    {
        if (instance != null)
            Destroy(instance);
        instance = this;

        mainCanvas = GameObject.Find("Canvas").GetComponent<Canvas>();

        if (RandomiseCondition)
        {
            CurrentLevelWinCondition = (ConditionType)UnityEngine.Random.Range(1, 4);

            if (CurrentLevelWinCondition != ConditionType.ProfitLimitedInventory) TotalShipCount = 30;
        }

        GameManager.Invinciblate(CurrentLevelWinCondition == ConditionType.Profit);
        if (CurrentLevelWinCondition == ConditionType.ProfitByCustomer)
        {
            GameManager.CountCustomers(TotalCustomerNumber);
        }

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
        GameManager.instance.LimitInventory(TotalShipCount);

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
                //print("Profit?");
                //print(GameManager.instance.netIncome + " >= " + GoalNetIncome + "?");
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
            case ConditionType.ProfitNoFailure:
                if (GameManager.instance.netIncome >= GoalNetIncome)
                {
                    StartCoroutine("Win");
                    return true;
                }
                if (FailedCustomerNumber > 0)
                {
                    StartCoroutine("Lose");
                    return true;
                }
                break;
            case ConditionType.ProfitLimitedInventory:
                if (GameManager.instance.netIncome >= GoalNetIncome)
                {
                    StartCoroutine("Win");
                    return true;
                }
                else if (activedocks == 0)
                {
                    StartCoroutine("Lose");
                    return true;
                }
                break;
        }
        return false;
    }
    void WinConditionsText(ConditionType Condition)                 //condition texts
    {
        switch (Condition)
        {
            case ConditionType.Profit:
                goaltext.text = "Goal: Earn $" + GoalNetIncome + " profit to continue.";
                break;
            case ConditionType.ProfitByCustomer:
                goaltext.text = "Goal: Earn $" + GoalNetIncome + " profit within " + TotalCustomerNumber + " customers.";
                break;
            case ConditionType.ProfitLimitedInventory:
                goaltext.text = "Goal: Earn $" + GoalNetIncome + " profit with a stock of " + TotalShipCount + " ships.";
                break;
            case ConditionType.ProfitNoFailure:
                goaltext.text = "Goal: Earn $" + GoalNetIncome + " profit without failing any deals.";
                break;
        }
    }
    private IEnumerator Win()
    {
        GameManager.instance.PermaPause();       
        yield return new WaitForSeconds(4.0f);
        wincanvas.SetActive(true);
        mainCanvas.enabled = false;
    }

    private IEnumerator Lose()
    {
        GameManager.instance.PermaPause();
        yield return new WaitForSeconds(3.0f);
        losecanvas.SetActive(true);
        mainCanvas.enabled = false;
    }
}
