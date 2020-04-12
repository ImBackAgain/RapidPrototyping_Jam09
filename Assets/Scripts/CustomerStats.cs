using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomerStats : MonoBehaviour
{
    // Customer's rank of the most important thing to them
    // each stat is given a rank from 1 to 5 where 5 being the most important and 1 being the least important
    // For example，if appearance = 5, interior = 4, safety = 3, speed = 2 and size = 1, customer value apearance > interior > safety > speed > size
    public int appearanceRank;
    public int interiorRank;
    public int safetyRank;
    public int speedRank;
    public int sizeRank;

    // Customer preference for ship size, there are 3 kinds: small, regular and large
    public ShipStats.SizeCategory sizePreference;

    // Weight of the stats, represent how important of each stat to the final decision making, used for the math part
    float appearanceWeight;
    float interiorWeight;
    float safetyWeight;
    float speedWeight;
    float sizeWeight;

    // Saved for adding customer character system
    public enum CustomerModifier { None }
    public CustomerModifier modifier = CustomerModifier.None;

    // UI used to display amount of customer patient
    private Text patienceText;
    // Current value for customer patience, an int between 0 and 100, indicate percent of patient left, start value is 100 for all customer
    private float patience;

    // Used to add customer to GameManager class
    private GameManager manager;
    // For use of audio manager
    private AudioManager audioMng = null;

    // Start is called before the first frame update
    void Start()
    {
        // Customer rank values are manualy set in the customer prefabs

        // Locate GameManager and add current customer
        manager = FindObjectOfType<GameManager>();
        manager.SetCustomer(this);

        // Locate patience UI and initialize patience value
        patienceText = GameObject.Find("CustomerPatience").GetComponent<Text>();
        patience = 100.0f;
        patienceText.text = "Customer Patience: " + patience + "%";

        // Initialize weights
        appearanceWeight = convertRankToWeight(appearanceRank);
        interiorWeight = convertRankToWeight(interiorRank);
        safetyWeight = convertRankToWeight(safetyRank);
        speedWeight = convertRankToWeight(speedRank);
        sizeWeight = 1f;

        // Locate audio manager
        audioMng = FindObjectOfType<AudioManager>();
        if (audioMng == null)
            Debug.LogError("\tNo GameObject with the [ AudioManager ] script was found in the current scene!");
    }

    // Convert rank value to weight value, basically higher the rank, higher the weight
    public float convertRankToWeight(int rank)
    {
        switch (rank)
        {
            case 1:
                return 1f;
            case 2:
                return 1.1f;
            case 3:
                return 1.3f;
            case 4:
                return 1.6f;
            case 5:
                return 2f;
            case 6:
                return 2.5f;
            default:
                return 1f;
        }
    }

    // The customer accept the boast and change their weight accordingly
    //1 = appearance, 2 = interior, 3 = safetly, 4 = speed, and 5 = size
    public void TakeBoast(int stat)
    {
        switch (stat)
        {
            case 1:
                appearanceWeight = convertRankToWeight(appearanceRank + 1);
                break;
            case 2:
                interiorWeight = convertRankToWeight(interiorRank + 1);
                break;
            case 3:
                safetyWeight = convertRankToWeight(safetyRank + 1);
                break;
            case 4:
                speedWeight = convertRankToWeight(speedRank + 1);
                break;
            case 5:
                sizeRank += sizeRank;
                break;
        }

        UpdatePatience(-10.0f);
    }

    // Yuanchao's Math code
    // Takes the ship that the player was tring to sale and returns maximum offer the customer can accept
    public float CalculateMaxOffer(ShipStats ship)
    { 
 
        // Math for size
        if (sizePreference != ship.size)
        {
            if (ship.size == ShipStats.SizeCategory.Small)
            {
                if (sizeRank == 1)
                {
                    sizeWeight = 1;
                }
                else if (sizeRank == 2)
                {
                    sizeWeight = 0.98f;
                }
                else if (sizeRank == 3)
                {
                    sizeWeight = 0.96f;
                }
                else if (sizeRank == 4)
                {
                    sizeWeight = 0.93f;
                }
                else if (sizeRank == 5)
                {
                    sizeWeight = 0.90f;
                }
                else if (sizeRank == 6)
                {
                    sizeWeight = 0.85f;
                }
            }
            else if (ship.size == ShipStats.SizeCategory.Regular)
            {
                if (sizeRank == 1)
                {
                    sizeWeight = 1;
                }
                else if (sizeRank == 2)
                {
                    sizeWeight = 0.95f;
                }
                else if (sizeRank == 3)
                {
                    sizeWeight = 0.90f;
                }
                else if (sizeRank == 4)
                {
                    sizeWeight = 0.85f;
                }
                else if (sizeRank == 5)
                {
                    sizeWeight = 0.80f;
                }
                else if (sizeRank == 6)
                {
                    sizeWeight = 0.75f;
                }
            }
            else if (ship.size == ShipStats.SizeCategory.Large)
            {
                if (sizeRank == 1)
                {
                    sizeWeight = 1;
                }
                else if (sizeRank == 2)
                {
                    sizeWeight = 0.90f;
                }
                else if (sizeRank == 3)
                {
                    sizeWeight = 0.70f;
                }
                else if (sizeRank == 4)
                {
                    sizeWeight = 0.40f;
                }
                else if (sizeRank == 5)
                {
                    sizeWeight = 0.20f;
                }
                else if (sizeRank == 6)
                {
                    sizeWeight = 0.10f;
                }
            }

        }

        //patience f(x)=0.003x+0.8
        //each stats has a value
        float appearanceValue, interiorValue, safetyValue, speedValue;
        appearanceValue = ship.appearance / (ship.appearance + ship.interior + ship.safety + ship.speed) * ship.value;
        interiorValue = ship.interior / (ship.appearance + ship.interior + ship.safety + ship.speed) * ship.value;
        safetyValue = ship.safety / (ship.appearance + ship.interior + ship.safety + ship.speed) * ship.value;
        speedValue = ship.speed / (ship.appearance + ship.interior + ship.safety + ship.speed) * ship.value;

        float maximumOffer = (appearanceValue * appearanceWeight) + (interiorValue * interiorWeight) + (safetyValue * safetyWeight) + (speedValue * speedWeight)
            * (0.003f * patience + 0.8f) * sizeWeight;

        return maximumOffer;

    }

    // SUBJECT TO CHANGE!!!!!!!!!!!!!!!!!!!!!!!!!
    // Update patient value of customer
    public void UpdatePatience(float changeOfPatience)
    {
        patience += changeOfPatience;
        patience = Mathf.Clamp(patience, 0.0f, 100.0f);
        patienceText.text = "Customer Patience: " + patience + "%";
        if (patience == 0)
            // Add announcement when customer is out of patient
            CustomerLeave("");
    }
    
    // SUBJECT TO CHANGE!!!!!!!!!!!!!!!!!!!!!!!!!
    // Display announcement when current customer leaves the shop and spawn a new customer
    public void CustomerLeave(string announcement)
    {
        // Add code to display announcement
        StartCoroutine("SpawnNextCustomer");
    }
    // When the current customer is out of the shop (out of patient or already striked a deal), wait for 2 seconds to let player read announcement and spawn a new customer
    private IEnumerator SpawnNextCustomer()
    {
        // Froze all game buttons
        GameObject.Find("Interview").GetComponent<Button>().interactable = false;
        GameObject.Find("Boast").GetComponent<Button>().interactable = false;
        GameObject.Find("Snacks").GetComponent<Button>().interactable = false;
        GameObject.Find("Offer").GetComponent<Button>().interactable = false;

        // Wait for 2 seconds, create new customer and destory current one
        while (true)
        {
            yield return new WaitForSeconds(2.0f);
            manager.SpawnCustomer();
            Destroy(gameObject);
        }
    }
}
