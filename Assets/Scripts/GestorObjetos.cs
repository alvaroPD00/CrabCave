using System.Collections.Generic;
using UnityEngine;

public class GestorObjetos : MonoBehaviour
{
    [Header("Objeto que debe desaparecer")]
    [SerializeField] private GameObject objeto1;

    [Header("Objetos que se DESACTIVAN cuando objeto1 desaparece")]
    [SerializeField] private List<GameObject> objetosParaDesactivar = new List<GameObject>();

    [Header("Objetos que se ACTIVAN cuando objeto1 desaparece")]
    [SerializeField] private List<GameObject> objetosParaActivar = new List<GameObject>();

    private bool cambioRealizado = false;

    private void Update()
    {
        if (cambioRealizado) return;

        if (objeto1 == null)
        {
            // Desactivar todos los del primer pool
            foreach (var obj in objetosParaDesactivar)
            {
                if (obj != null) obj.SetActive(false);
            }

            // Activar todos los del segundo pool
            foreach (var obj in objetosParaActivar)
            {
                if (obj != null) obj.SetActive(true);
            }

            cambioRealizado = true;
            Debug.Log("[GestorObjetos] objeto1 desapareció  desactivar pool A, activar pool B");
        }
    }
}
