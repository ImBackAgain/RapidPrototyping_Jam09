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
    //float sizeWeight;

    // Saved for adding customer character system
    public enum CustomerModifier { None, Scholar, Punk, Afffluent, Wandererer }
    public CustomerModifier modifier = CustomerModifier.None;

    // UI used to display amount of customer patient
    private Text patienceText;
    // Current value for customer patience, an int between 0 and 100, indicate percent of patient left, start value is 100 for all customer
    public float patience;

    // Used to add customer to GameManager class (fix: use GameManager singleton directly)
    //private GameManager manager;
    // For use of audio manager
    private AudioManager audioMng = null;

    // Start is called before the first frame update
    void Start()
    {
        // Customer rank values are manualy set in the customer prefabs
        Image icon = GameObject.Find("CustoModIcon").GetComponent<Image>();

        int random = Random.Range(-5, 5);

        if (random < 0) random = 0;
        //modifier = (CustomerModifier)random;

        if (modifier != CustomerModifier.None)
        {
            icon.sprite = Resources.Load<Sprite>(modifier.ToString());
            icon.enabled = true;
        }
        else
        {
            icon.enabled = false;
        }
        // Locate GameManager and add current customer (fix: use GameManager singleton directly)
        //manager = FindObjectOfType<GameManager>();
        GameManager.instance.SetCustomer(this);

        // Locate patience UI and initialize patience value
        patienceText = GameObject.Find("CustomerPatience").GetComponent<Text>();
        patience = 100.0f;
        patienceText.text = patience.ToString();

        // Initialize weights
        appearanceWeight = ConvertRankToWeight(appearanceRank);
        interiorWeight = ConvertRankToWeight(interiorRank);
        safetyWeight = ConvertRankToWeight(safetyRank);
        speedWeight = ConvertRankToWeight(speedRank);
        //sizeWeight = 1f;

        // Locate audio manager
        audioMng = FindObjectOfType<AudioManager>();
        if (audioMng == null)
            Debug.LogError("\tNo GameObject with the [ AudioManager ] script was found in the current scene!");

        // Greet new customer
        GameManager.instance.Greeting();
    }

    // Convert rank value to weight value, basically higher the rank, higher the weight
    public float ConvertRankToWeight(int rank)
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

    /// <summary>
    /// Updates the internal stats in response to boasting.
    /// </summary>
    /// <param name="stat">int repesenting stat, from 1 to 5 (apppearance, interior, safety, speeed or size in that order)</param>
    /// <returns>Whether or not the customer favours this stat. Used for response.</returns>
    public bool TakeBoast(int stat)
    {
        UpdatePatience(-10.0f);

        int statWeight = 0;

        if (modifier == CustomerModifier.Scholar)
        {
            float difff;
            switch (stat)
            {
                case 1:
                    statWeight = appearanceRank;
                    difff = ConvertRankToWeight(appearanceRank + 1) - appearanceWeight;
                    appearanceWeight += difff / 2;
                    break;
                case 2:
                    statWeight = interiorRank;
                    difff = ConvertRankToWeight(interiorRank + 1) - interiorWeight;
                    interiorWeight += difff / 2;
                    break;
                case 3:
                    statWeight = safetyRank;
                    difff = ConvertRankToWeight(safetyRank + 1) - safetyWeight;
                    safetyWeight += difff / 2;
                    break;
                case 4:
                    statWeight = speedRank;
                    difff = ConvertRankToWeight(speedRank + 1) - speedWeight;
                    speedWeight += difff / 2;
                    break;
                case 5:
                    statWeight = sizeRank;
                    sizeRank += 1;
                    break;
            }
        }
        else
        { 
            switch (stat)
            {
                case 1:
                    statWeight = appearanceRank;
                    appearanceWeight = ConvertRankToWeight(appearanceRank + 1);
                    break;
                case 2:
                    statWeight = interiorRank;
                    interiorWeight = ConvertRankToWeight(interiorRank + 1);
                    break;
                case 3:
                    statWeight = safetyRank;
                    safetyWeight = ConvertRankToWeight(safetyRank + 1);
                    break;
                case 4:
                    statWeight = speedRank;
                    speedWeight = ConvertRankToWeight(speedRank + 1);
                    break;
                case 5:
                    statWeight = sizeRank;
                    sizeRank += 1;
                    break;
            }
        }
        return statWeight > 3;
    }

    // Yuanchao's Math code
    // Takes the ship that the player was tring to sell, calculate maximum offer and generate customer behavior
    public float MaxBuyingPrice(ShipStats ship)
    {
        float sizeWeight = 1;
        float shipValue = ship.value;
        // Math for size
        if (sizePreference != ship.size)
        {
            switch(ship.size)
            {
                case ShipStats.SizeCategory.Small:
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
                    break;

                case ShipStats.SizeCategory.Regular:
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
                    break;

                case ShipStats.SizeCategory.Large:
            
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
                    break;
            }
        }

        //patience f(x)=0.003x+0.8
        //each stats has a value
        float appearanceValue, interiorValue, safetyValue, speedValue;

        float totalValue = (ship.appearance + ship.interior + ship.safety + ship.speed);

        appearanceValue = ship.appearance / totalValue;
        interiorValue   = ship.interior   / totalValue;
        safetyValue     = ship.safety     / totalValue;
        speedValue      = ship.speed      / totalValue;
        // These are alll in (0, 1). 

        float maximumOffer = 0;

        float bonusMult = 1;

        switch (ship.modifier)
        {
            case ShipModifier.Wartorn:
                safetyValue += 0.2f;

                if (modifier == CustomerModifier.Punk) bonusMult *= 1.5f;
                if (modifier == CustomerModifier.Wandererer) bonusMult *= 0.8f;

                break;

            case ShipModifier.Antique:
                interiorValue += 0.1f;
                appearanceValue += 0.2f;
                safetyValue -= 0.1f;
                if (safetyValue < 0) safetyValue = 0;
                break;

            case ShipModifier.Homely:
                safetyValue += 0.1f;
                interiorValue += 0.3f;
                speedValue -= 0.2f;
                if (speedValue < 0) speedValue = 0;

                if (modifier == CustomerModifier.Wandererer) bonusMult *= 1.6f; //Large mult because his pooornesss multiplier willl come in
                break;
        }

        maximumOffer += (appearanceValue    * appearanceWeight);    // So these are each within
        maximumOffer += (interiorValue      * interiorWeight);      // the range (0, 2.5), since
        maximumOffer += (safetyValue        * safetyWeight);        // 2.5 is the highest posssible weight value
        maximumOffer += (speedValue         * speedWeight);
        //The above is within the range (0, 2.5)

        maximumOffer *= (0.003f * patience + 0.8f) * sizeWeight;
                     // (0.8, 1.1)                 * (0, 1)
        maximumOffer *= ship.value;

        maximumOffer *= bonusMult;

        if (modifier == CustomerModifier.Afffluent) maximumOffer *= 1.2f;
        if (modifier == CustomerModifier.Wandererer) maximumOffer *= 0.9f;

        //Thus, maximumOfffer is in the range (0, 2.75v), where v is the value of the shup.

        // Round the highest price to a multiple of 50
        maximumOffer = Mathf.Round(maximumOffer / 10.0f) * 10;

        return maximumOffer;
    }

    // Update patient value of customer
    public void UpdatePatience(float changeOfPatience)
    {
        switch (modifier)
        {
            case CustomerModifier.Punk:
                if (changeOfPatience < 0) changeOfPatience *= 1.2f;
                break;

            case CustomerModifier.Scholar:
                if (changeOfPatience < 0) changeOfPatience *= 0.8f;
                break;
        }

        patience += changeOfPatience;
        patience = Mathf.Clamp(patience, 0.0f, 100.0f);
        patienceText.text = patience.ToString();
        if (patience == 0)
        {
            GameManager.instance.NoSaleResponse();
            OutOfActions("Out of Patience");
        }

    }

    // Display announcement when current customer leaves the shop and spawn a new customer
    public void OutOfActions(string announcement)
    {        
        GameManager.instance.UpdateFeedback(announcement);
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
        //while (true)
        //{
            yield return new WaitForSeconds(2.0f);
            // only remove sold ship
            if (GameManager.instance.currentSoldShipParent != null)
            {
                GameManager.instance.SpawnOneShip(GameManager.instance.currentSoldShipParent);
                GameManager.instance.currentSoldShipParent = null;
            }
            // remove the current ship selection
            GameManager.instance.SpawnCustomer();
            Destroy(gameObject);
            GameManager.instance.RemoveShipSelection();
            GameManager.instance.InitUIComponets();
        //}
    }
}