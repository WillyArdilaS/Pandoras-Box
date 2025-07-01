using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public AudioClip sonidoClick;
    private AudioSource audioSource;
    public GameObject instrucciones;
    bool activate = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void abrirInstrucciones()
    {
        activate = !activate;
        instrucciones.SetActive(activate);
        audioSource.PlayOneShot(sonidoClick);
    }

    public void alJuego()
    {
        audioSource.PlayOneShot(sonidoClick);
        SceneManager.LoadScene("Game");
    }


}
