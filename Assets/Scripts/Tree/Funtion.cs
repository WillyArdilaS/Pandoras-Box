using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
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
        public GameObject perilla;
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
    private bool PerillaGirando = false;

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
                Renderer rend = item.objeto.GetComponent<Renderer>();
                QueColor qc = item.objeto.GetComponent<QueColor>();
                if (rend == null || qc == null)
                {
                    continue;
                }
                    

                Material mat = rend.sharedMaterial;

                if (qc.esVerde)
                {
                    mat.EnableKeyword("_EMISSION");
                    Color c = mat.GetColor("_EmissionColor");
                    mat.SetColor("_EmissionColor", c);
                }
                else
                {
                    mat.DisableKeyword("_EMISSION");
                }
            }
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!context.performed || PerillaGirando) return;

        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = cam.ScreenPointToRay(mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            foreach (Controlador controlador in controladores)
            {
                if (hit.transform.gameObject == controlador.objetoControlador)
                {
                    StartCoroutine(RotarPerilla(controlador.perilla));
                    AplicarAcciones(controlador);
                    break;
                }
            }
        }
    }

    IEnumerator RotarPerilla(GameObject perilla)
    {
        PerillaGirando = true;

        Quaternion startRotation = perilla.transform.rotation;
        Quaternion endRotation = startRotation * Quaternion.Euler(0, 90, 0);

        float elapsedTime = 0f;
        float duration = 0.5f;

        while (elapsedTime < duration)
        {
            perilla.transform.rotation = Quaternion.Lerp(startRotation, endRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        perilla.transform.rotation = endRotation;
        PerillaGirando = false;
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
                    QueColor qc = item.objeto.GetComponent<QueColor>();

                    if (rend != null && qc != null)
                    {
                        Material mat = rend.sharedMaterial;
                        bool encender = !qc.esVerde;

                        if (encender)
                            mat.EnableKeyword("_EMISSION");
                        else
                            mat.DisableKeyword("_EMISSION");

                        qc.esVerde = encender;
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

        if (TodosTienenEmisionActiva())
        {
            Debug.Log("Todos los objetos tienen la emisión activada.");
        }
    }

    bool TodosTienenEmisionActiva()
    {
        HashSet<GameObject> revisados = new HashSet<GameObject>();

        foreach (Controlador controlador in controladores)
        {
            foreach (ObjetoConAccion item in controlador.objetosAfectados)
            {
                if (item.accion == TipoAccion.CambiarColor && !revisados.Contains(item.objeto))
                {
                    revisados.Add(item.objeto);
                    QueColor qc = item.objeto.GetComponent<QueColor>();

                    if (qc == null || !qc.esVerde)
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }
}