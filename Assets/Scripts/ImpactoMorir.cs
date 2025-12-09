using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using System.Collections;

public class ImpactoMorir : MonoBehaviour
{
    [Header("Fade y Sonido")]
    [SerializeField] private Image fadeImage;
    [SerializeField] private AudioSource musicaJugador;
    [SerializeField] private float fadeDuration = 2f;

    [Header("Jugador y Movimiento")]
    [SerializeField] private GameObject jugador;
    [SerializeField] private string nombreScriptMovimiento;
    // Ejemplo: "PlayerMovement" (sin comillas en el inspector)

    private MonoBehaviour scriptMovimiento;
    private bool procesoIniciado = false;

    private void Start()
    {
        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(false);
            Color c = fadeImage.color;
            c.a = 0f;
            fadeImage.color = c;
        }

        // Buscar el script por nombre escrito en el inspector
        if (jugador != null && !string.IsNullOrEmpty(nombreScriptMovimiento))
        {
            Type tipo = Type.GetType(nombreScriptMovimiento);

            // Si no encuentra por nombre simple, buscar entre todos los componentes del jugador
            if (tipo == null)
            {
                MonoBehaviour[] todos = jugador.GetComponents<MonoBehaviour>();
                foreach (var comp in todos)
                {
                    if (comp.GetType().Name == nombreScriptMovimiento)
                    {
                        scriptMovimiento = comp;
                        break;
                    }
                }
            }
            else
            {
                scriptMovimiento = jugador.GetComponent(tipo) as MonoBehaviour;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (procesoIniciado) return;
        if (!other.CompareTag("Player")) return;

        procesoIniciado = true;

        // Desactivar el script encontrado
        if (scriptMovimiento != null)
        {
            scriptMovimiento.enabled = false;
        }

        // Detener el rigidbody
        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        StartCoroutine(FadeOutAndReset());
    }

    private IEnumerator FadeOutAndReset()
    {
        if (fadeImage != null)
            fadeImage.gameObject.SetActive(true);

        float tiempo = 0f;
        float volInicial = musicaJugador != null ? musicaJugador.volume : 1f;

        while (tiempo < fadeDuration)
        {
            float t = tiempo / fadeDuration;

            if (musicaJugador != null)
                musicaJugador.volume = Mathf.Lerp(volInicial, 0f, t);

            if (fadeImage != null)
            {
                Color c = fadeImage.color;
                c.a = Mathf.Lerp(0f, 1f, t);
                fadeImage.color = c;
            }

            tiempo += Time.deltaTime;
            yield return null;
        }

        if (musicaJugador != null) musicaJugador.volume = 0f;

        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = 1f;
            fadeImage.color = c;
        }

        Scene escena = SceneManager.GetActiveScene();
        SceneManager.LoadScene(escena.name);
    }
}
