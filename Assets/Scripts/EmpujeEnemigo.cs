using UnityEngine;
[RequireComponent(typeof(Collider2D))]
public class EmpujeEnemigo : MonoBehaviour
{
    [Header("Configuración de Empuje")]
    [SerializeField] private float fuerzaEmpuje = 5f;    // Magnitud del empuje
    [SerializeField] private bool usarDireccionVertical = true; // Opcional: empuje también vertical

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Verificamos que el objeto con el que colisionó tenga la tag "Player"
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody2D rbJugador = collision.gameObject.GetComponent<Rigidbody2D>();
            if (rbJugador == null) return;

            // Obtenemos el punto de contacto promedio entre ambos colliders
            Vector2 puntoContacto = collision.GetContact(0).point;

            // Dirección desde el enemigo hacia el jugador
            Vector2 direccion = (collision.transform.position - transform.position).normalized;

            // Si no queremos empuje vertical (solo horizontal), forzamos Y a 0
            if (!usarDireccionVertical)
                direccion.y = 0f;

            // Aplicamos una fuerza de impulso en dirección opuesta
            rbJugador.AddForce(direccion * fuerzaEmpuje, ForceMode2D.Impulse);
        }
    }
}
