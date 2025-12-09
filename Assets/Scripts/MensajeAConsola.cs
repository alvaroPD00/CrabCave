using UnityEngine;

public class MensajeAConsola : MonoBehaviour
{
   public int vida = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        print("Valor vida: " + vida);
    }
}
