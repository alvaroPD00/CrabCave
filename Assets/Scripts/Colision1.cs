using UnityEngine;

public class Colision1 : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        print("algo toca el piso");
        Destroy(collision.gameObject);
    }
}
