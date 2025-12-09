using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class final : MonoBehaviour
{
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 2f;

    private void Start()
    {
        

        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        

        StartCoroutine(FadeToWhiteAndReload());
    }

    private IEnumerator FadeToWhiteAndReload()
    {
        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);
            Color color = fadeImage.color;
            float elapsedTime = 0f;

            while (elapsedTime < fadeDuration)
            {
                color.a = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
                fadeImage.color = color;
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            color.a = 1f;
            fadeImage.color = color;
        }

        // Recargar la escena actual
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}
