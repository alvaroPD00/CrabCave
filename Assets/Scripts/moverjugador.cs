using UnityEngine;

public class moverjugador : MonoBehaviour
{
    [Header("Hitboxes")]
    [SerializeField] private GameObject hitboxDerecha;
    [SerializeField] private GameObject hitboxIzquierda;

    [Header("Referencias")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Movimiento")]
    [SerializeField] private float velocidadCaminar = 5f;
    [SerializeField] private float velocidadCorrer = 10f;

    [Header("Salto")]
    [SerializeField] private float fuerzaSalto = 150.5f;
    private bool enSuelo = true;

    private int direccionMovimiento = 1; // 1 = derecha, -1 = izquierda
    private int direccionAtaque = 1;    // dirección del ataque

    private bool ataqueEnCurso = false;

    private void Update()
    {
        // Movimiento horizontal
        float inputX = 0f;
        if (Input.GetKey(KeyCode.A))
            inputX = -1f;
        else if (Input.GetKey(KeyCode.D))
            inputX = 1f;

        float velocidad = Input.GetKey(KeyCode.LeftShift) ? velocidadCorrer : velocidadCaminar;
        transform.Translate(Vector2.right * inputX * velocidad * Time.deltaTime);

        // Flipping según movimiento solo si no estamos atacando
        if (!ataqueEnCurso)
        {
            if (inputX > 0)
            {
                spriteRenderer.flipX = false;
                direccionMovimiento = 1;
            }
            else if (inputX < 0)
            {
                spriteRenderer.flipX = true;
                direccionMovimiento = -1;
            }
        }

        // Salto
        if (Input.GetKeyDown(KeyCode.Space) && enSuelo)
        {
            rb.AddForce(Vector2.up * fuerzaSalto);
            enSuelo = false;
        }

        // Animaciones de caminar
        animator.SetFloat("Speed", Mathf.Abs(inputX * velocidad));

        // Ataque
        // Ataque con click izquierdo (ataque hacia la izquierda)
        // Ataque con click izquierdo
        if (Input.GetMouseButtonDown(0))
        {
            direccionAtaque = -1;
            spriteRenderer.flipX = true;
            animator.SetTrigger("Attack");
            ataqueEnCurso = true;
        }

        // Ataque con click derecho
        else if (Input.GetMouseButtonDown(1))
        {
            direccionAtaque = 1;
            spriteRenderer.flipX = false;
            animator.SetTrigger("Attack");
            ataqueEnCurso = true;
        }


    }


    // Llamado desde Animation Event en el frame del golpe
    public void ActivarHitboxDesdeEvento()
    {
        if (direccionAtaque == 1)
        {
            hitboxDerecha.SetActive(true);
            hitboxIzquierda.SetActive(false);
        }
        else if (direccionAtaque == -1)
        {
            hitboxIzquierda.SetActive(true);
            hitboxDerecha.SetActive(false);
        }
    }


    // Este método se llama al final de la animación de ataque mediante Animation Event
    public void FinDeAtaque()
    {
        ataqueEnCurso = false;
        // Restaurar flip según dirección de movimiento
        spriteRenderer.flipX = direccionMovimiento < 0;
    }

    public void DesactivarHitboxes()
    {
        hitboxDerecha.SetActive(false);
        hitboxIzquierda.SetActive(false);
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Suelo"))
        {
            enSuelo = true;
        }
    }
}
