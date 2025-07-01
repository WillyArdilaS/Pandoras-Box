using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class Funtion : MonoBehaviour
{
    public GameObject[] objetosInicialesVerdes;

    [System.Serializable]
    public class ObjetoConAccion
    {
        public GameObject objeto;
        public TipoAccion accion;
    }

    [System.Serializable]
    public class Controlador
    {
        public GameObject objetoControlador;
        public ObjetoConAccion[] objetosAfectados;
    }

    public enum TipoAccion
    {
        Rotar,
        CambiarColor,
        Escalar,
        Mover
    }

    public Controlador[] controladores;

    private Camera cam;
    private PlayerInput playerInput;

    void Awake()
    {
        cam = Camera.main;
        playerInput = GetComponent<PlayerInput>();
    }

    void Start()
    {
        foreach (Controlador controlador in controladores)
        {
            foreach (ObjetoConAccion item in controlador.objetosAfectados)
            {
                foreach (GameObject verdeObj in objetosInicialesVerdes)
                {
                    if (item.objeto == verdeObj)
                    {
                        Renderer rend = item.objeto.GetComponent<Renderer>();
                        if (rend != null)
                        {
                            rend.material.color = Color.green;
                        }
                    }
                }
            }
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = cam.ScreenPointToRay(mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            foreach (Controlador controlador in controladores)
            {
                if (hit.transform.gameObject == controlador.objetoControlador)
                {
                    AplicarAcciones(controlador);
                    break;
                }
            }
        }
    }

    void AplicarAcciones(Controlador controlador)
    {
        foreach (ObjetoConAccion item in controlador.objetosAfectados)
        {
            switch (item.accion)
            {
                case TipoAccion.Rotar:
                    item.objeto.transform.Rotate(Vector3.up * 90f);
                    break;

                case TipoAccion.CambiarColor:
                    Renderer rend = item.objeto.GetComponent<Renderer>();
                    if (rend != null)
                    {
                        Color actual = rend.material.color;
                        bool nuevoEsVerde = !esVerde(actual);
                        rend.material.color = nuevoEsVerde ? Color.green : Color.white;
                    }
                    break;

                case TipoAccion.Escalar:
                    item.objeto.transform.localScale *= 1.2f;
                    break;

                case TipoAccion.Mover:
                    item.objeto.transform.position += Vector3.right * 1f;
                    break;
            }
        }

        if (TodosEstanEnVerde())
        {
            Debug.Log("Todos los objetos están en verde.");
        }
    }

    bool TodosEstanEnVerde()
    {
        HashSet<GameObject> revisados = new HashSet<GameObject>();

        foreach (Controlador controlador in controladores)
        {
            foreach (ObjetoConAccion item in controlador.objetosAfectados)
            {
                if (item.accion == TipoAccion.CambiarColor && !revisados.Contains(item.objeto))
                {
                    revisados.Add(item.objeto);
                    Renderer rend = item.objeto.GetComponent<Renderer>();
                    if (rend == null || !esVerde(rend.material.color))
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }

    bool esVerde(Color color)
    {
        return color == Color.green;
    }
}
