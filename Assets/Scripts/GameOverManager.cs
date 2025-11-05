using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using NUnit.Framework;
public class GameOverManager : MonoBehaviour
{
    private bool isGameOver = false;

    [SerializeField] Canvas gameOverCanvas;
    [SerializeField] Canvas winCanvas; 

    public void Start()
    {
        Time.timeScale = 1f;
        isGameOver = false;
    }

    public void TriggerGameOver()
    {
        isGameOver = true;

        Time.timeScale = 0f;

        gameOverCanvas.gameObject.SetActive(true);


    }
    public void TriggerWinState()
    {
        isGameOver = true;
        Time.timeScale = 0f;
        winCanvas.gameObject.SetActive(true); 
    }
    private void Update()
    {
        if (isGameOver && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Time.timeScale = 1f;
            RestartGame();
        }
    }
    private void RestartGame()
    {
        Time.timeScale = 1f;

        GameManager.Instance.ResetStats(); 


        SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
    }
}
