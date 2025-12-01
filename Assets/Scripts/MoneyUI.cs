using UnityEngine;
using TMPro;
using UnityEngine.Rendering;

public class MoneyUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI MoneyText;
    [SerializeField] private TextMeshProUGUI FishText;

    [SerializeField] private TextMeshProUGUI PebbleText;

    
    private void OnEnable()
    {
        StartCoroutine(WaitForGameManagerInitialization());

    }
    private System.Collections.IEnumerator WaitForGameManagerInitialization()
    {
        // Wait until GameManager.Instance is not null
        while (GameManager.Instance == null)
        {
            yield return null; // Wait for the next frame
        }

        // Subscribe to the event and update the UI
        GameManager.Instance.OnMoneyChanged += UpdateMoneyText;
        GameManager.Instance.OnFishChanged += UpdateFishText;
        GameManager.Instance.OnPebbleChanged += UpdatePebbleText; 
        UpdateMoneyText();
        UpdateFishText();
        UpdatePebbleText(); 
    }
    private void OnDisable()
{
    if (GameManager.Instance != null)
    {
        GameManager.Instance.OnMoneyChanged -= UpdateMoneyText;
            GameManager.Instance.OnFishChanged -= UpdateFishText;
            GameManager.Instance.OnPebbleChanged -= UpdatePebbleText; 
    }
}

    private void UpdateMoneyText()
    {
        MoneyText.text = $"Money: ${GameManager.Instance.PlayerMoney}";
    }

    private void UpdateFishText()
    {
        FishText.text = $"Fish: {GameManager.Instance.PlayerFish}";
    }
    
    private void UpdatePebbleText()
    {
        if (GameManager.Instance.PlayerPebble > 0)
        {
            PebbleText.gameObject.SetActive(true);
             PebbleText.text = $"Pebble: {GameManager.Instance.PlayerPebble}";
        }
        
        
    }
    
}
