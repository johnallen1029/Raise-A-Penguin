// 11/3/2025 AI-Tag
// This was created with the help of Assistant, a Unity Artificial Intelligence product.

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransitionManager : MonoBehaviour
{
    public Image fadePanel; // Assign the Panel's Image component here
    public float fadeDuration = 1f; // Duration of the fade effect

    private void Start()
    {
        // Ensure the panel is active and starts fully transparent
        if (fadePanel != null)
        {
            fadePanel.gameObject.SetActive(true);
            fadePanel.color = new Color(0, 0, 0, 0);
        }
        else
        {
            Debug.LogWarning("SceneTransitionManager: fadePanel is not assigned.");
        }
    }

    public void StartGame(string sceneName)
    {
        StartCoroutine(FadeOutAndLoadScene(sceneName));
    }

    private IEnumerator FadeOutAndLoadScene(string sceneName)
    {
        // Gradually increase the alpha value of the panel to create a fade-out effect
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            fadePanel.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        // Load the next scene
        SceneManager.LoadScene(sceneName);
    }
}