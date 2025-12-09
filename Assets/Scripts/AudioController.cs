using UnityEngine;

public class AudioController : MonoBehaviour
{
    [Header("Audio de zona")]
    [SerializeField] private AudioSource audioZona;

    [Header("Audio principal del jugador")]
    [SerializeField] private AudioSource audioJugador;

    [Header("Crossfade")]
    [SerializeField] private float duracionFade = 1.5f;

    private bool jugadorDentro = false;
    private Coroutine rutinaFade = null;

    private void Start()
    {
        if (audioZona != null)
        {
            audioZona.volume = 0f;
            audioZona.loop = true;
            audioZona.Stop();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (!gameObject.activeInHierarchy) return;

        jugadorDentro = true;

        if (rutinaFade != null) StopCoroutine(rutinaFade);
        rutinaFade = StartCoroutine(Crossfade(audioJugador, audioZona, true));
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (!gameObject.activeInHierarchy) return;

        jugadorDentro = false;

        if (rutinaFade != null) StopCoroutine(rutinaFade);
        rutinaFade = StartCoroutine(Crossfade(audioJugador, audioZona, false));
    }

    private void OnDisable()
    {
        // Si la zona se apaga mientras el jugador estaba dentro,
        // debemos restaurar manualmente sin usar coroutines.
        if (jugadorDentro)
        {
            jugadorDentro = false;

            // Restablecer musica del jugador
            if (audioJugador != null)
            {
                audioJugador.volume = 1f;
            }

            // Apagar sonido de la zona
            if (audioZona != null)
            {
                audioZona.volume = 0f;
                audioZona.Stop();
            }
        }

        // Cancelar coroutine si estaba corriendo
        if (rutinaFade != null)
        {
            StopCoroutine(rutinaFade);
            rutinaFade = null;
        }
    }

    private System.Collections.IEnumerator Crossfade(AudioSource musicaJugador, AudioSource musicaZona, bool entrando)
    {
        float tiempo = 0f;

        if (entrando)
        {
            if (!musicaZona.isPlaying) musicaZona.Play();
        }

        float volInicialJugador = musicaJugador.volume;
        float volInicialZona = musicaZona.volume;

        float volObjetivoJugador = entrando ? 0f : 1f;
        float volObjetivoZona = entrando ? 1f : 0f;

        while (tiempo < duracionFade)
        {
            // Si se desactiva la zona durante el crossfade, cortamos
            if (!gameObject.activeInHierarchy)
            {
                yield break;
            }

            tiempo += Time.deltaTime;
            float t = tiempo / duracionFade;

            musicaJugador.volume = Mathf.Lerp(volInicialJugador, volObjetivoJugador, t);
            musicaZona.volume = Mathf.Lerp(volInicialZona, volObjetivoZona, t);

            yield return null;
        }

        musicaJugador.volume = volObjetivoJugador;
        musicaZona.volume = volObjetivoZona;

        if (!entrando)
        {
            musicaZona.Stop();
        }
    }
}
