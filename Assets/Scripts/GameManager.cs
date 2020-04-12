using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    // List of prefabs used, manualy added
    private GameObject[] shipPrefabs, customerPrefabs;
    // List of data for ships
    private List<ShipStats> ships;
    // Initial value for previous customer, set to -1 since the first current customer is at 0
    private int previousCustomerIndex = -1;
    // Customer have 5 preference and are ranked from 1 to 5 where 5 is the most important
    // When customer is first interviewed, they will start by saying the most important thing to them and time customer is interviewed again they will say the next most important thing
    // currentInterviewRank represent current rank of most important thing to customer, start value will be 5
    private int currentInterviewRank;
    // Current Customer that the player is talking to
    private CustomerStats currentCustomer;

    // SUBJECT TO CHANGE!!!!!!!!!!!!!!!!!!!!!!!!!
    // The one and only GameManager instance, not currently used
    public static GameManager instance;
    // A ship being selected, not currently used
    private ShipStats currentShip;
    // Final price decided between customer and player, not currently used
    private float finalBuyingPrice;
    // UI components, not currently used since methods are called under onclick behavior under buttons directly
    //public Button inter;
    //public Button boa;
    //public Button sna;
    //public Button offer;

    //Useful UI components, drag into the place from inspector
    public GameObject BoastPanel;
    public Text incomeText;
    public Text speechBubble;


    // Total income earned by this shop
    private float income;
    // Total value of all the ships in stock (currently will increase whenever a ship is spawned, but need to be decreased once a ship is sold)
    private float totalShipValue;
    // For use of AudioManager
    private AudioManager audioMng = null;

    // Customer syntax
    // Start of conversation
    private string[] greetings = { "Hi there!", "Whatta ya got?", "What're ya sellin'?", "Is this the right place?", "Can I get some service, please?" };
    // Responce after player use boast
    private string[] boastResponse = { "You don't say!", "I hadn't considered that...", "I'll take your word for it.", "Impressive!", "Wow!" };
    // Responce after player give snack
    private string[] snackResponse = { "Thank you!", "Thanks!", "For me? Thanks!", "Talk about customer service!", "You have my attention" };
    // Interview responce for value appearance
    private string[] appearanceResponse = { "I guess it would have to be the looks?", "Style is everything!", "I want something that looks cool", "Something that'll turn heads", "One that looks as good as I do" };
    // Interview responce for value interior
    private string[] interiorResponse = { "Something that looks good from the inside", "A luxury interior!", "Comfortable seats for long trips", "CUP HOLDERS", "Lots of flashing buttons!" };
    // Interview responce for value safety
    private string[] safteyResponse = { "Something that'll keep my family safe", "Got anything that can blow up a small planet?", "Guns. Lots of them. Don't ask.", "State of the art defense system", "Airbags. Wait, do you need airbags in space?" };
    // Interview responce for value speed
    private string[] speedResponse = { "GOTTA GO FAST", "The fastest ya got", "Speed is key!", "I want to break some speed records", "Something quick would be nice" };
    // Interview responce for want smaller ship
    private string[] sizeResponseSmall = { "Something that doesn't take up too much space", "The smaller the better", "I don't need anything too big", "A smaller one will do", "Itsy bitsy teeny weeny spacey shipy" };
    // Interview responce for want regular ship
    private string[] sizeResponseRegular = { "Something not too big or too small", "Something sized juuuuuust right", "Average sized would be fine", "Got anything regular sized?", "I'm not looking for anything crazy for size" };
    // Interview responce for want large ship
    private string[] sizeResponseLarge = { "Biggest ya got!", "I need something to fit the whole family", "BIG SHIP PLEASE", "I would prefer something on the large side", "Something big enough to fit an asteroid. No reason." };
    // Responce when price offered too cheap
    private string[] purchaseResponseCheap = { "You're practically giving it away!", "What a steal!", "How do you stay in business with such low prices?!", "Haha, sucker!", "Way less than I was expecting!" };
    // Responce when price offered just about right
    private string[] purchaseResponseAverage = { "You got yourself a deal", "Sounds reasonable", "Sure, sounds fair", "A fair price", "I can do that" };
    // Responce when price offered too high
    private string[] purchaseResponseExpensive = { "I can't afford that", "No way, pal", "That's way too expensive", "For that hunk of junk?! No way!", "You're out of your mind!" };
    


    void Start()
    {
        // Find the dialogue UI
        speechBubble.text = greetings[Random.Range(0, greetings.Length)];

        // Set initail value for variables
        income = 0.0f;
        totalShipValue = 0.0f;
        ships = new List<ShipStats>();

        // Spawn 5 ships and 1 customer when game starts
        SpawnShips();
        SpawnCustomer();
        BoastPanel.SetActive(false);

        // Locate AudioManager
        audioMng = FindObjectOfType<AudioManager>();
        if (audioMng == null)
            Debug.LogError("\tNo GameObject with the [ AudioManager ] script was found in the current scene!");
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Behavior for interview button
    public void Interview()
    {
        // Display text for customer responce of current interview
        if (currentCustomer.appearanceRank == currentInterviewRank)
            speechBubble.text = appearanceResponse[Random.Range(0, appearanceResponse.Length)];
        else if (currentCustomer.interiorRank == currentInterviewRank)
            speechBubble.text = interiorResponse[Random.Range(0, interiorResponse.Length)];
        else if (currentCustomer.safetyRank == currentInterviewRank)
            speechBubble.text = safteyResponse[Random.Range(0, safteyResponse.Length)];
        else if (currentCustomer.speedRank == currentInterviewRank)
            speechBubble.text = speedResponse[Random.Range(0, speedResponse.Length)];
        else if (currentCustomer.sizeRank == currentInterviewRank)
        {
            switch (currentCustomer.sizePreference)
            {
                case ShipStats.SizeCategory.Large:
                    speechBubble.text = sizeResponseLarge[Random.Range(0, sizeResponseLarge.Length)];
                    break;
                case ShipStats.SizeCategory.Regular:
                    speechBubble.text = sizeResponseRegular[Random.Range(0, sizeResponseRegular.Length)];
                    break;
                case ShipStats.SizeCategory.Small:
                    speechBubble.text = sizeResponseSmall[Random.Range(0, sizeResponseSmall.Length)];
                    break;
            }
        }

        // Customer lost patience everytime they were interviewed
        currentCustomer.UpdatePatience(-10.0f);

        // Set current interview to the next
        currentInterviewRank--;

        // Each customer can only be interviewed 3 times
        if (currentInterviewRank == 2)
            GameObject.Find("Interview").GetComponent<Button>().interactable = false;
    }

    // SUBJECT TO CHANGE!!!!!!!!!!!!!!!!!!!!!!!!!
    // Currently linked to boast button in UI, but have no behavior besides active the panel
    public void ActivateBoastPanel()
    {
        BoastPanel.SetActive(true);
    }

    // SUBJECT TO CHANGE!!!!!!!!!!!!!!!!!!!!!!!!!
    // Not linked to Boast button, take a stat number input as the type of boast to customer, change customer's weight of indicated type
    // For int stat, 1 = appearance, 2 = interior, 3 = safetly, 4 = speed, and 5 = size
    public void Boast(int stat)
    {
        speechBubble.text = boastResponse[Random.Range(0, boastResponse.Length)];
        currentCustomer.TakeBoast(stat);
        // Can only boast to each customer once
        BoastPanel.SetActive(false);
        GameObject.Find("Boast").GetComponent<Button>().interactable = false;
    }

    // SUBJECT TO CHANGE!!!!!!!!!!!!!!!!!!!!!!!!!
    // Behavior for offer snack for customer, currently works as debug method for spawn new customer, need to change to add patience when offer snack
    public void Snacks()
    {
        speechBubble.text = snackResponse[Random.Range(0, snackResponse.Length)];
        // Need to change to positive numbers
        currentCustomer.UpdatePatience(-100.0f);
        // Each customer can only be offered once
        GameObject.Find("Snacks").GetComponent<Button>().interactable = false;
    }

    // SUBJECT TO CHANGE!!!!!!!!!!!!!!!!!!!!!!!!!
    // Math stuff for making offer, not currently linked to the offer buttom, takes the price input by player and ship selected by player, determines customer behavior
    void Offer(float playerInputPrice, ShipStats ship)
    {
        float maximumOffer = currentCustomer.CalculateMaxOffer(ship);
        // When customer accept the offer
        if (playerInputPrice <= maximumOffer)
        {
            AddIncome(playerInputPrice);
            if (playerInputPrice / maximumOffer < 0.85f)
            {
                speechBubble.text = purchaseResponseCheap[Random.Range(0, purchaseResponseCheap.Length)];
                // Add announcement here when customer is satisfied with the price and deal is made
                currentCustomer.CustomerLeave("");
            }
            else
            {
                speechBubble.text = purchaseResponseAverage[Random.Range(0, purchaseResponseAverage.Length)];
                // Add announcement here when customer is satisfied with the price and deal is made
                currentCustomer.CustomerLeave("");
            }
        }
        // When customer can't accept the offer made, customer becomes inpatient
        else
        {
            if (playerInputPrice >= maximumOffer && playerInputPrice < maximumOffer * 1.2f)
                currentCustomer.UpdatePatience(-10.0f);
            else if (playerInputPrice >= maximumOffer * 1.2f && playerInputPrice < maximumOffer * 1.5f)
                currentCustomer.UpdatePatience(-20.0f);
            else if (playerInputPrice >= maximumOffer * 1.5f && playerInputPrice < maximumOffer * 2f)
                currentCustomer.UpdatePatience(-40.0f);
            else if (playerInputPrice >= maximumOffer * 2f && playerInputPrice < maximumOffer * 3f)
                currentCustomer.UpdatePatience(-70.0f);
            else if (playerInputPrice >= maximumOffer * 3f)
                currentCustomer.UpdatePatience(-100.0f);
            speechBubble.text = purchaseResponseExpensive[Random.Range(0, purchaseResponseExpensive.Length)];
        }
    }

    // Spawn a new ship that haven't been stocked yet
    private void SpawnShips()
    {
        // Find spawn location for the ship
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("ShipSpawnPoint");
        HashSet<int> exclude = new HashSet<int>();

        foreach (GameObject spawn in spawnPoints)
        {
            IEnumerable<int> range = Enumerable.Range(0, shipPrefabs.Length).Where(i => !exclude.Contains(i));

            int randomIndex = Random.Range(0, shipPrefabs.Length - exclude.Count);
            int shipIndex = range.ElementAt(randomIndex);

            GameObject spawnedShip = Instantiate(shipPrefabs[shipIndex], spawn.transform.position, Quaternion.identity);
            exclude.Add(shipIndex);

            totalShipValue += spawnedShip.GetComponent<ShipStats>().value;
        }

        AddIncome(0.0f);
    }

    // Spawn a new random customer that haven't been to the shop yet
    public void SpawnCustomer()
    {
        // When new customer is spawed, reset interviewRank back to maximum (5)
        currentInterviewRank = 5;
        // New customer is spawned at this position
        Vector3 spawnPoint = GameObject.FindGameObjectWithTag("CustomerSpawnPoint").transform.position;

        // If this is the first customer being created, create a customer
        if (previousCustomerIndex == -1)
        {
            int randomIndex = Random.Range(0, customerPrefabs.Length);
            Instantiate(customerPrefabs[randomIndex], spawnPoint, Quaternion.identity);
        }
        // If this is not the first customer, save the index of previous customer to a list of used customer, only spawn from customer prefabs that haven't been used
        else
        {
            HashSet<int> exclude = new HashSet<int>() { previousCustomerIndex };
            IEnumerable<int> range = Enumerable.Range(0, shipPrefabs.Length).Where(i => !exclude.Contains(i));

            int randomIndex = Random.Range(0, customerPrefabs.Length - exclude.Count);
            int customerIndex = range.ElementAt(randomIndex);

            Instantiate(customerPrefabs[customerIndex], spawnPoint, Quaternion.identity);
            previousCustomerIndex = customerIndex;
        }

        //Refresh buttons
        GameObject.Find("Interview").GetComponent<Button>().interactable = true;
        GameObject.Find("Boast").GetComponent<Button>().interactable = true;
        GameObject.Find("Snacks").GetComponent<Button>().interactable = true;
        GameObject.Find("Offer").GetComponent<Button>().interactable = true;

        //audioMng.PlayAudio("Customer Arrives");
    }

    // Add a certain amount to the current total income of the shop
    public void AddIncome(float amount)
    {
        income += amount;
        incomeText.text = "Amount Earned: " + income + " / " + totalShipValue;
        int randomSound = Random.Range(1, 4);
        //audioMng.PlayAudio("Spaceship Sold " + randomSound);
    }

    private void ShowToolTip(string toolTip)
    {

    }

    public void SetCustomer(CustomerStats customer)
    {
        currentCustomer = customer;
    }
}
