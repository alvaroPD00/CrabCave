using UnityEngine;

public class muroFragil : MonoBehaviour
{
    [Header("Tag que destruye el muro")]
    [SerializeField] private string tagInteractivo = "hitbox";


    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag(tagInteractivo))
        {
            Destroy(gameObject);
        }
    }

}
