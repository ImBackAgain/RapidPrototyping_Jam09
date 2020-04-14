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
    //  Parent of the ship being selected
    private Transform currentShipParent;
    // Parent of the ship being sold
    [HideInInspector] public Transform currentSoldShipParent;

    // The one and only GameManager instance
    public static GameManager instance;
    // A ship being selected
    private ShipStats currentShip;
    // The number of customers visited the shop
    [HideInInspector] public int VisitedCustomerNumber = 0;
    // Final price decided between customer and player, not currently used
    private float finalBuyingPrice;

    //Useful UI components, drag into the place from inspector
    public GameObject BoastPanel;
    public GameObject OfferPanel;
    [HideInInspector] public Text incomeText;
    [HideInInspector] public Text netIncomeText;
    [HideInInspector] public Text speechBubble;
    [HideInInspector] public Text feedbackText;
    [HideInInspector] public GameObject feedbackPanel;
    [HideInInspector] public GameObject shipPromptPanel;

    // Pannel Controller object
    private StatsPannelController statsPannelController;
    // UI used to display available dealer actions remaining
    private Text actionsText;
    // Current count of actions left, an int between 0 and 5, starting at 5 with each new customer, when it's 0, put customer out of action
    private int dealerActions;
    private int maxActions = 5;

    // Total income earned by this shop
    private float income;
    // Net income = Income - shipValue
    [HideInInspector] public float netIncome;
    // Total value of all the ships in stock (currently will increase whenever a ship is spawned, but need to be decreased once a ship is sold)
    //private float totalShipValue;
    // For use of AudioManager
    private AudioManager audioMng = null;

    // state of the ship states panel
    private bool isShipStatsPanelInit = false;

    // state of the popup window
    private bool isPopUp = false;
    // store buttons state for popupwindow func, don't use tme for other purposes
    private bool interviewBtnState;
    private bool snackBtnState;
    private bool boastBtnState;
    private bool offerBtnState;

    //Ship pool; Exclude one ship per spwan
    private HashSet<int> exclude = new HashSet<int>();

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
    // Response when leaving without a sale
    private string[] leaveNoSaleResponse = { "I have places to be", "Have your people call my people", "I'm bored, I'm busy, I'm done here", "Whatever, I don't like your selection...", "Guess I'm not flying home in a new ride" };


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
        // Locate AudioManager
        audioMng = FindObjectOfType<AudioManager>();
        if (audioMng == null)
            Debug.LogError("\tNo GameObject with the [ AudioManager ] script was found in the current scene!");

        // Find the dialogue UI
        speechBubble = GameObject.Find("SpeechText").GetComponent<Text>();
        feedbackText = GameObject.Find("FeedbackLine").GetComponent<Text>();
        incomeText = GameObject.Find("TotalIncome").GetComponent<Text>();
        netIncomeText = GameObject.Find("NetIncome").GetComponent<Text>();
        actionsText = GameObject.Find("DealerActions").GetComponent<Text>();
        feedbackPanel = GameObject.Find("FeedbackPanel");
        shipPromptPanel = GameObject.Find("ShipPromptPanel");

        // Locate stats pannel controller
        statsPannelController = FindObjectOfType<StatsPannelController>();

        // Set initial value for text
        //speechBubble.text = greetings[Random.Range(0, greetings.Length)];
        actionsText.text = dealerActions.ToString();
        feedbackText.text = "";
        feedbackPanel.SetActive(false);

        // Set initail value for variables
        income = 0.0f;
        netIncome = 0.0f;
        isPopUp = false;
        //totalShipValue = 0.0f;
        dealerActions = maxActions;
        ships = new List<ShipStats>();

        SpawnShips();
        SpawnCustomer();

        BoastPanel.SetActive(false);
        InitUIComponets();
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

        DealerActionCountdown();
    }

    // Currently linked to boast button in UI, activate the boast panel
    public void ActivateBoastPanel()
    {
        BoastPanel.SetActive(true);
    }

    // Linked to Boast button, take a stat number input according to button clicked as the type of boast to customer, change customer's weight of indicated type
    // For int stat, 1 = appearance, 2 = interior, 3 = safetly, 4 = speed, and 5 = size
    public void Boast(int stat)
    {
        speechBubble.text = boastResponse[Random.Range(0, boastResponse.Length)];
        currentCustomer.TakeBoast(stat);
        // Can only boast to each customer once (perhaps not)
        BoastPanel.SetActive(false);
        //GameObject.Find("Boast").GetComponent<Button>().interactable = false;

        DealerActionCountdown();
    }

    // Behavior for offer snack for customer, currently works as debug method for spawn new customer, need to change to add patience when offer snack
    public void Snacks()
    {
        speechBubble.text = snackResponse[Random.Range(0, snackResponse.Length)];
        currentCustomer.UpdatePatience(20.0f);
        // Each customer can only be offered once
        GameObject.Find("Snacks").GetComponent<Button>().interactable = false;

        DealerActionCountdown();
    }

    // Activate offer input panel
    public void Offer()
    {
        // THIS IS A PLACEHOLDER AND DOES NOT ALLOW FOR PLAYER CHOICE
        OfferPanel.SetActive(true);
        ShowPopUpWindow();
    }

    public void CancelPrice()
    {
        GameObject.Find("InputPrice").GetComponent<InputField>().text = "0";
        OfferPanel.SetActive(false);
        HidePopUpWindow();
    }

    // Take input price and calculate customer behavior
    public void ConfirmPrice()
    {
        float amount = float.Parse(GameObject.Find("InputPrice").GetComponent<InputField>().text);
        if (amount <= 0)
        {
            return;//Error input
        }
        else
        {
            ShipStats ship = currentShip;

            float maximumOffer = currentCustomer.takeOffer(amount, currentShip);
            //When customer accept the offer
            if (amount <= maximumOffer)
            {
                currentSoldShipParent = currentShipParent;
                AddIncome(amount, ship.value);
                if (amount / maximumOffer < 0.85f)
                {
                    speechBubble.text = purchaseResponseCheap[Random.Range(0, purchaseResponseCheap.Length)];
                    currentCustomer.OutOfActions("Perfect Price: $" + maximumOffer);
                }
                else
                {
                    speechBubble.text = purchaseResponseAverage[Random.Range(0, purchaseResponseAverage.Length)];
                    currentCustomer.OutOfActions("Perfect Price: $" + amount);
                }

                // If it has been accepted, just decrement the dealer action count for the visual of the thing
                dealerActions--;
                actionsText.text = dealerActions.ToString();
            }
            //When customer can't accept the offer made, customer becomes inpatient
            else
            {
                speechBubble.text = purchaseResponseExpensive[Random.Range(0, purchaseResponseExpensive.Length)];
                if (amount >= maximumOffer && amount < maximumOffer * 1.2f)
                    currentCustomer.UpdatePatience(-10.0f);
                else if (amount >= maximumOffer * 1.2f && amount < maximumOffer * 1.5f)
                    currentCustomer.UpdatePatience(-20.0f);
                else if (amount >= maximumOffer * 1.5f && amount < maximumOffer * 2f)
                    currentCustomer.UpdatePatience(-40.0f);
                else if (amount >= maximumOffer * 2f && amount < maximumOffer * 3f)
                    currentCustomer.UpdatePatience(-70.0f);
                else if (amount >= maximumOffer * 3f)
                    currentCustomer.UpdatePatience(-100.0f);

                // If it has not been accepted, check as usual to see if the dealer is out of actions
                DealerActionCountdown();
            }

        }

        // Make sure offer panel is inaccesible after offer is made
        OfferPanel.SetActive(false);
        HidePopUpWindow();
    }

    // Spawn new ship that wasn't currently in stock
    private void SpawnShips()
    {
        // Find spawn location for the ship
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("ShipSpawnPoint");        

        foreach (GameObject spawn in spawnPoints)
        {
            IEnumerable<int> range = Enumerable.Range(0, shipPrefabs.Length).Where(i => !exclude.Contains(i));

            int randomIndex = Random.Range(0, shipPrefabs.Length - exclude.Count);
            int shipIndex = range.ElementAt(randomIndex);

            GameObject spawnedShip = Instantiate(shipPrefabs[shipIndex], spawn.transform.position, Quaternion.identity);
            exclude.Add(shipIndex);

            spawnedShip.transform.localScale = new Vector3(spawnedShip.transform.localScale.x * 45f, spawnedShip.transform.localScale.y * 45f, 1f);
            spawnedShip.transform.SetParent(spawn.transform.parent);

            //totalShipValue += spawnedShip.GetComponent<ShipStats>().value;
        }

        AddIncome(0.0f, 0.0f);

        //GameObject[] docks = GameObject.FindGameObjectsWithTag("ShipDock");
        //foreach (GameObject dock in docks)
        //{
        //    dock.GetComponent<DockHandler>().TurnOnDefault();
        //}
    }

    // Yuanchao's code of spawn only one ship
    public void SpawnOneShip(Transform SpawnPointParent)
    {        
        Destroy(SpawnPointParent.GetChild(2).gameObject);
        if (exclude.Count == shipPrefabs.Length)
        {
            SpawnPointParent.gameObject.SetActive(false);
            WinCondition.activedocks = WinCondition.activedocks - 1;
        }
        else
        {
            Transform spawn = SpawnPointParent.GetChild(0);
            IEnumerable<int> range = Enumerable.Range(0, shipPrefabs.Length).Where(i => !exclude.Contains(i));

            int randomIndex = Random.Range(0, shipPrefabs.Length - exclude.Count);
            int shipIndex = range.ElementAt(randomIndex);

            GameObject spawnedShip = Instantiate(shipPrefabs[shipIndex], spawn.position, Quaternion.identity);
            exclude.Add(shipIndex);

            spawnedShip.transform.localScale = new Vector3(spawnedShip.transform.localScale.x * 45f, spawnedShip.transform.localScale.y * 45f, 1f);
            spawnedShip.transform.SetParent(spawn.parent);
        }
        

        //totalShipValue += spawnedShip.GetComponent<ShipStats>().value;
        //totalIncomeText.text = "/ " + totalShipValue;
    }


    // Spawn random customer that is different from the current one
    public void SpawnCustomer()
    {

        // When new customer is spawed, reset interviewRank back to maximum (5)
        currentInterviewRank = 5;
        VisitedCustomerNumber = VisitedCustomerNumber + 1;
        // New customer is spawned at this position
        Vector3 spawnPoint = GameObject.FindGameObjectWithTag("CustomerSpawnPoint").transform.position;
        Transform container = GameObject.Find("CustomerContainer").transform;

        // If this is the first customer being created, create a customer
        if (previousCustomerIndex == -1)
        {
            int randomIndex = Random.Range(0, customerPrefabs.Length);
            GameObject spawnedCustomer = Instantiate(customerPrefabs[randomIndex], spawnPoint, Quaternion.identity);
            spawnedCustomer.transform.localScale = new Vector3(spawnedCustomer.transform.localScale.x * 45f, spawnedCustomer.transform.localScale.y * 45f, 1f);
            spawnedCustomer.transform.SetParent(container);
        }
        // If this is not the first customer, save the index of previous customer to a list of used customer, only spawn from customer prefabs that haven't been used
        else
        {
            HashSet<int> exclude = new HashSet<int>() { previousCustomerIndex };
            IEnumerable<int> range = Enumerable.Range(0, shipPrefabs.Length).Where(i => !exclude.Contains(i));

            int randomIndex = Random.Range(0, customerPrefabs.Length - exclude.Count);
            int customerIndex = range.ElementAt(randomIndex);

            GameObject spawnedCustomer = Instantiate(customerPrefabs[customerIndex], spawnPoint, Quaternion.identity);
            spawnedCustomer.transform.localScale = new Vector3(spawnedCustomer.transform.localScale.x * 45f, spawnedCustomer.transform.localScale.y * 45f, 1f);
            spawnedCustomer.transform.SetParent(container);
            previousCustomerIndex = customerIndex;
        }

        //Refresh buttons
        GameObject.Find("Interview").GetComponent<Button>().interactable = true;
        GameObject.Find("Boast").GetComponent<Button>().interactable = true;
        GameObject.Find("Snacks").GetComponent<Button>().interactable = true;
        GameObject.Find("Offer").GetComponent<Button>().interactable = true;

        // Reset dealer actions
        dealerActions = maxActions;

        //Reset content for textbox
        actionsText.text = dealerActions.ToString();
        feedbackText.text = "";
        feedbackPanel.SetActive(false);

        // Play audio effect accordingly
        audioMng.PlayAudio("Customer Arrives");
    }

    // Add a certain amount to the current total income of the shop
    public void AddIncome(float price, float value)
    {
        income += price;
        netIncome += price - value;
        incomeText.text = income.ToString();
        netIncomeText.text = netIncome.ToString();
        //incomeText.text = "Amount Earned: " + income + " / " + totalShipValue;
        int randomSound = Random.Range(1, 4);
        audioMng.PlayAudio("Spaceship Sold " + randomSound);
    }

    // Select a ship
    public void GetCurrentShip(ShipStats selectedShip, Transform parent)
    {
        if (isPopUp) return;
        statsPannelController.UpdateStats(selectedShip.model, selectedShip.size.ToString(), selectedShip.appearance, selectedShip.interior, selectedShip.safety, selectedShip.speed, Mathf.FloorToInt(selectedShip.value));
        currentShip = selectedShip;
        currentShipParent = parent;
        ActivateCurrentShipDock(parent.Find("Dock"));

        BoastPanel.SetActive(false);
        if (!isShipStatsPanelInit)
        {
            ActivateUIComponetsOnShipSelect();
        }
    }

    //
    private void ActivateCurrentShipDock(Transform currentDock)
    {
        GameObject[] docks = GameObject.FindGameObjectsWithTag("ShipDock");
        foreach (GameObject dock in docks)
        {
            dock.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        }
        currentDock.GetComponent<Image>().color = new Color32(22, 103, 222, 255);
    }

    // If dealer action is 0, put customer out of action
    private void DealerActionCountdown()
    {
        dealerActions--;
        actionsText.text = dealerActions.ToString();
        if (dealerActions == 0)
        {
            StartCoroutine("WaitForTextBeforeEndOfCustomer");
        }
    }

    private IEnumerator WaitForTextBeforeEndOfCustomer()
    {
        // The player has 2 seconds to read whatever text was recently displayed, then another customer will be spawned after another 2 seconds
        GameObject.Find("Interview").GetComponent<Button>().interactable = false;          //block buttons
        GameObject.Find("Boast").GetComponent<Button>().interactable = false;
        GameObject.Find("Snacks").GetComponent<Button>().interactable = false;
        GameObject.Find("Offer").GetComponent<Button>().interactable = false;

        bool stillWaiting = true;

        while (stillWaiting)
        {
            yield return new WaitForSeconds(2.0f);
            NoSaleResponce();
            currentCustomer.OutOfActions("Out of Actions");
            stillWaiting = false;
        }
    }

    // Set current customer to a certain custumoer
    public void SetCustomer(CustomerStats customer)
    {
        currentCustomer = customer;
    }

    // Change display on the feedback text
    public void UpdateFeedback(string announcement)
    {
        feedbackText.text = announcement;
        feedbackPanel.SetActive(true);
    }

    // Greeting new customer
    public void Greeting()
    {
        speechBubble.text = greetings[Random.Range(0, greetings.Length)];
    }

    // Leave no sale responce
    public void NoSaleResponce()
    {
        WinCondition.FailedCustomerNumber = WinCondition.FailedCustomerNumber + 1;
        speechBubble.text = leaveNoSaleResponse[Random.Range(0, leaveNoSaleResponse.Length)];
    }

    public void RemoveShipSelection()
    {
        currentShipParent = null;
        currentShip = null;
        GameObject[] docks = GameObject.FindGameObjectsWithTag("ShipDock");
        foreach (GameObject dock in docks)
        {
            dock.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        }
        statsPannelController.UpdateStats("Model", "Size", 0, 0, 0, 0, 0);
    }

    public void InitUIComponets()
    {
        GameObject.Find("Boast").GetComponent<Button>().interactable = false;
        GameObject.Find("Offer").GetComponent<Button>().interactable = false;
        //show prompt
        shipPromptPanel.SetActive(true);
        // no data on the panel
        isShipStatsPanelInit = false;
    }

    public void ActivateUIComponetsOnShipSelect()
    {
        
        GameObject.Find("Boast").GetComponent<Button>().interactable = true;
        GameObject.Find("Offer").GetComponent<Button>().interactable = true;
        //hide prompt
        shipPromptPanel.SetActive(false);
        // panel has been init
        isShipStatsPanelInit = true;
    }

    public void ShowPopUpWindow()
    {
        // storing buttons interactable state
        interviewBtnState = GameObject.Find("Interview").GetComponent<Button>().interactable;
        boastBtnState = GameObject.Find("Boast").GetComponent<Button>().interactable;
        snackBtnState = GameObject.Find("Snacks").GetComponent<Button>().interactable;
        offerBtnState = GameObject.Find("Offer").GetComponent<Button>().interactable;

        GameObject.Find("Interview").GetComponent<Button>().interactable = false;
        GameObject.Find("Boast").GetComponent<Button>().interactable = false;
        GameObject.Find("Snacks").GetComponent<Button>().interactable = false;
        GameObject.Find("Offer").GetComponent<Button>().interactable = false;

        isPopUp = true;
    }


    public void HidePopUpWindow()
    {
        GameObject.Find("Interview").GetComponent<Button>().interactable = interviewBtnState;
        GameObject.Find("Boast").GetComponent<Button>().interactable = boastBtnState;
        GameObject.Find("Snacks").GetComponent<Button>().interactable = snackBtnState;
        GameObject.Find("Offer").GetComponent<Button>().interactable = offerBtnState;

        isPopUp = false;
    }
}