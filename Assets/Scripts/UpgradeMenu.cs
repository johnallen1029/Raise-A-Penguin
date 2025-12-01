
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeMenu : MonoBehaviour
{
    public int moneyUpCount = 0; 
    public GameObject[] pages;
    public Button leftArrow;
    public Button rightArrow;


    public GameObject arrowCanvas; 

    private int currentPageIndex = 0; 
    public CameraFollow cameraFollow;
    public MonoBehaviour playerMovement;

    public GameObject obstacleCube;

    public Button fibreButton;

    public GameObject penguinSentry; 

    public Button secondSpotButton;

    public Button fishFeederButton; 

    public GameObject fishFeeder;

    public GameObject secondIsland;

    public Button sentryButton;

    public Button multButton; 

    public PenguinHunger penguinHunger;

    public GameObject drill1;
    public GameObject drill2;

    public GameObject drillTwo;
    public GameObject drillTwoPivot;

    public GameObject drillThree;
    public GameObject drillThreePivot;

    public GameObject quarry; 
    public Button drillButton;

    public GameObject fisherPenguin;
    public Button fisherButton;

    public Button drillUpgradeButton;

    public Button drillSpeedButton; 

    public DrillerInteraction drillerInteraction;

    public DrillerInteraction driller2Interaction;

    public DrillerInteraction driller3Interaction; 

    public Button upFeatherButton;

    public GameObject penguinModel;

    public Button growPenguinButton;

    public CashEmit cashEmit;

    private int drillCount = 0;

    private int drillSpeedCount = 0; 

    public GameObject moneyShrine; 

    int upgradeCost = 20;

    private void Start()
    {
        UpdatePageVisibility();

        leftArrow.onClick.AddListener(GoToPreviousPage);
        rightArrow.onClick.AddListener(GoToNextPage);
    }

    private void UpdatePageVisibility()
    {
        if (pages == null || pages.Length == 0)
        {
            Debug.Log("Pages array not assigned or empty");
            return; 
        }
        for (int i = 0; i < pages.Length; i++)
        {
            if (pages[i] == null)
            {
                Debug.Log("Pages is null");
                continue; 
            }
            pages[i].SetActive(i == currentPageIndex);
        }
        leftArrow.interactable = currentPageIndex > 0;
        rightArrow.interactable = currentPageIndex < pages.Length - 1;
    }

    private void GoToNextPage()
    {
        if (currentPageIndex < pages.Length - 1)
        {
            currentPageIndex++;
            UpdatePageVisibility();
            
        }
    }
    
    private void GoToPreviousPage()
    {
        if (currentPageIndex > 0)
        {
            currentPageIndex--;
            UpdatePageVisibility(); 
        }
    }
    public void UpgradeFeather()
    {
        int cashIncrease = 10;
        if (GameManager.Instance.SpendMoney(upgradeCost) && moneyUpCount < 10)
        {
            Cash.cashValue += cashIncrease;
            Debug.Log("cash value increased");
            TextMeshProUGUI childText = upFeatherButton.transform.Find("topText").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI costText = upFeatherButton.transform.Find("costText").GetComponent<TextMeshProUGUI>(); 
            moneyUpCount++;
            upgradeCost += 50;
            childText.text = $"Money Upgrade ({moneyUpCount} / 10)";
            costText.text = $"${upgradeCost}"; 
            

            if (moneyUpCount >= 10)
            {
                upFeatherButton.interactable = false; 
            }
        }
        else
        {
            Debug.Log("GET A JOB!!!!!");
        }
        Debug.Log("Feather upgraded!");
        // Add logic to upgrade the player's speed
    }

    public void UpgradeHealth()
    {
        Debug.Log("Health upgraded!");
        // Add logic to upgrade the player's health
    }

    public void WinDaGame()
    {
        if (GameManager.Instance.hasEnoughFish(100) && GameManager.Instance.hasEnoughMoney(7000) && GameManager.Instance.hasEnoughPebble(40))
        {
            gameObject.SetActive(false);
            FindFirstObjectByType<GameOverManager>().TriggerWinState();
        }
    }

    public void CloseMenu()
    {
        if (cameraFollow != null)
        {
            Debug.Log("close mehy");
            cameraFollow.ResumeCamera();
            cameraFollow.isPaused = false; 
        }
        if (playerMovement != null)
        {
            playerMovement.enabled = true; 
        }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        arrowCanvas.SetActive(false); 
        gameObject.SetActive(false); // Hide the menu
        
    }
    public void slowHunger()
    {
        int upgradeCost = 200;
        if (GameManager.Instance.SpendMoney(upgradeCost))
        {
            penguinHunger.hungerDrainRate = 1.5f;
            Debug.Log(penguinHunger.hungerDrainRate);
            fibreButton.interactable = false;
        }
        else
        {
            Debug.Log("GET A JOB!!!!!!!");
        }

    }
    
    public void restoreHealth()
    {
        if (GameManager.Instance.SpendFish(30))
        {
            penguinHunger.currentHealth = penguinHunger.maxHealth;
            penguinHunger.healthBar.SetHealth(penguinHunger.currentHealth, penguinHunger.maxHealth);
        }
        else
        {
            Debug.Log("PRINTPRINTRPINTRPINTPRINT");
        }
    }

    public void addFishFeeder()
    {
        int cost = 300;
        if (GameManager.Instance.SpendMoney(cost))
        {
            fishFeeder.SetActive(true);
            fishFeederButton.interactable = false; 
        }
    }

    public void addSecondIsland()
    {
        int cost = 800;
        if (GameManager.Instance.SpendMoney(cost))
        {
            secondIsland.SetActive(true);
            secondSpotButton.interactable = false; 
        }
    }
    public void removeCube()
    {
        int cost = 500;
        if (GameManager.Instance.SpendMoney(cost))
        {
            obstacleCube.SetActive(false);
        }
    }

    public void addShrine()
    {
        int cost = 10; 
        if (GameManager.Instance.SpendPebble(cost))
        {
            moneyShrine.SetActive(true); 

            multButton.interactable = false; 
        }
    }

    public void hireSentry()
    {
        
        if (GameManager.Instance.SpendPebble(15))
        {
            penguinSentry.SetActive(true);
            sentryButton.interactable = false;
        }
    }

    public void buildDrill()
    {
        int cost = 2000;
        if (GameManager.Instance.SpendMoney(cost))
        {
            if (drillCount == 0)
            {
                drill1.SetActive(true);
                drill2.SetActive(true);
                quarry.SetActive(true);
                drillCount++;
                return; 
            }
            if (drillCount == 1)
            {
                drillTwo.SetActive(true);
                drillTwoPivot.SetActive(true);
                drillCount++;
                return; 
            }
            if (drillCount == 2)
            {
                drillThree.SetActive(true);
                drillThreePivot.SetActive(true);
                drillButton.interactable = false; 
            }

            
        }
    }
    
    public void hireFisher()
    {
        if (GameManager.Instance.SpendPebble(5))
        {
            fisherPenguin.SetActive(true);
            fisherButton.interactable = false; 
        }
    }

    public void upgradeDrill()
    {
        if (GameManager.Instance.SpendMoney(3000))
        {
            if (drillSpeedCount == 0)
            {
                drillerInteraction.drillDuration = 1f;
                drillSpeedCount++;
                return;
            }
            if (drillSpeedCount == 1)
            {
                driller2Interaction.drillDuration = 1f;
                drillSpeedCount++;
                return;
            }
            if (drillSpeedCount == 2)
            {
                driller3Interaction.drillDuration = 1f;
                drillSpeedButton.interactable = false; 
            }
            
            
        }
    }

    public void growPenguin()
    {
        if (GameManager.Instance.SpendFish(1))
        {
            penguinModel.transform.localScale = new Vector3(89f, 89f, 89f);
            float currentHunger = penguinHunger.hungerDrainRate;
            penguinHunger.hungerDrainRate = currentHunger + 2f;
            Debug.Log("Current hunger is {penguinHunger.hungerDrainRate}");
            cashEmit.UpdateEmitInterval(1f);  
            growPenguinButton.interactable = false; 
        }
    }

    public void OpenMenu()
    {
        if (!gameObject.activeSelf)
        {
            arrowCanvas.SetActive(true);
            gameObject.SetActive(true); // Ensure the GameObject is active
             
        }
        if (cameraFollow != null)
        {
            cameraFollow.PauseCamera();// Disable camera movement
        }
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true; 
    }
}