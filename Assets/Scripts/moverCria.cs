using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class moverCriaContainer : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Collider2D miCollider;               // collider del bot (BoxCollider2D)
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;

    [Header("Movimiento")]
    [SerializeField] private float velocidadCaminar = 3f;

    [Header("Container (asignar en el inspector)")]
    [Tooltip("BoxCollider2D que define el recinto donde el bot se mueve")]
    [SerializeField] private BoxCollider2D containerCollider;

    [Header("Deteccion lateral")]
    [SerializeField] private float margenDeteccion = 0.02f;       // margen para considerar 'tocando'
    [SerializeField] private float flipCooldown = 0.12f;         // tiempo minimo entre flips
    [SerializeField] private bool debugLogs = false;

    [Header("Objetos de control (opcional)")]
    [SerializeField] private GameObject objetoActivo;       // mientras exista, patrulla
    [SerializeField] private GameObject objetivoFinal;      // cuando objetoActivo desaparece va hacia aqui

    private int direccion = 1; // 1 = derecha, -1 = izquierda
    private float ultimoFlip = -999f;

    private void Reset()
    {
        rb = GetComponent<Rigidbody2D>();
        miCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    private void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (miCollider == null) miCollider = GetComponent<Collider2D>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (animator == null) animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        bool modoCaminar = objetoActivo != null;

        if (modoCaminar)
        {
            PatrolWithinContainer();
        }
        else if (objetivoFinal != null)
        {
            MoveTowardTarget();
        }

        // Para la animacion, usamos la velocidad objetivo en X (no depender de rb.velocity que puede ser 0 con MovePosition)
        float speedForAnim = modoCaminar ? Mathf.Abs(direccion * velocidadCaminar) : Mathf.Abs((objetivoFinal.transform.position.x - transform.position.x) >= 0 ? velocidadCaminar : -velocidadCaminar);
        if (animator != null) animator.SetFloat("Speed", speedForAnim);
    }

    private void PatrolWithinContainer()
    {
        if (containerCollider == null)
        {
            // fallback: movimiento simple sin container
            Vector2 nuevaPosFallback = rb.position + Vector2.right * direccion * velocidadCaminar * Time.fixedDeltaTime;
            rb.MovePosition(nuevaPosFallback);
            spriteRenderer.flipX = direccion < 0;
            if (debugLogs) Debug.Log("[patrol] sin container: pos=" + rb.position + " dir=" + direccion);
            return;
        }

        // mover
        Vector2 nuevaPos = rb.position + Vector2.right * direccion * velocidadCaminar * Time.fixedDeltaTime;
        rb.MovePosition(nuevaPos);
        spriteRenderer.flipX = direccion < 0;

        // revisar colisiones laterales por bounds (deterministico)
        Bounds botBounds = miCollider.bounds;
        Bounds contBounds = containerCollider.bounds;

        // si el borde derecho del bot supera (o esta cerca) del borde derecho del container -> contacto derecho
        bool tocandoDerecha = (botBounds.max.x >= contBounds.max.x - margenDeteccion);
        bool tocandoIzquierda = (botBounds.min.x <= contBounds.min.x + margenDeteccion);

        if (tocandoDerecha && direccion > 0)
        {
            TryFlipAndReposition(-1, contBounds, botBounds, true);
        }
        else if (tocandoIzquierda && direccion < 0)
        {
            TryFlipAndReposition(1, contBounds, botBounds, false);
        }

        if (debugLogs) Debug.Log("[patrol] pos=" + rb.position + " botMaxX=" + botBounds.max.x + " contMaxX=" + contBounds.max.x + " tocDer=" + tocandoDerecha + " tocIzq=" + tocandoIzquierda + " dir=" + direccion);
    }

    private void TryFlipAndReposition(int nuevaDireccion, Bounds contBounds, Bounds botBounds, bool fueDerecha)
    {
        if (Time.time - ultimoFlip < flipCooldown)
        {
            if (debugLogs) Debug.Log("[flip] cooldown activo, ignorando flip");
            return;
        }

        // calcular posicion segura dentro del container
        float botHalfWidth = botBounds.extents.x;
        float targetX;
        if (fueDerecha) // se tocó la derecha -> poner al bot justo dentro del borde derecho
        {
            targetX = contBounds.max.x - botHalfWidth - margenDeteccion;
        }
        else // tocó izquierda -> poner dentro borde izquierdo
        {
            targetX = contBounds.min.x + botHalfWidth + margenDeteccion;
        }

        // mover rigidbody a la posicion segura (solo eje X)
        Vector2 posActual = rb.position;
        Vector2 posAjustada = new Vector2(targetX, posActual.y);
        rb.MovePosition(posAjustada);

        // aplicar flip
        direccion = nuevaDireccion;
        spriteRenderer.flipX = direccion < 0;
        ultimoFlip = Time.time;

        if (debugLogs) Debug.Log($"[flip] invertido a {direccion}. reposicionado a x={targetX}");
    }

    private void MoveTowardTarget()
    {
        Vector2 dir = (objetivoFinal.transform.position - transform.position).normalized;
        Vector2 movimiento = new Vector2(dir.x, 0f) * velocidadCaminar * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movimiento);
        spriteRenderer.flipX = dir.x < 0;
        if (debugLogs) Debug.Log("[moveTo] moviendo hacia objetivo: " + objetivoFinal.name);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == objetivoFinal)
        {
            if (debugLogs) Debug.Log("[final] colision con el objetivo final. destruyendo...");
            Destroy(gameObject); // destruye el objeto que tiene este script
        }
    }


}
