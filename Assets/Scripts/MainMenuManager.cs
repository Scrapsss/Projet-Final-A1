using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Paramètres du Menu Principal")]
    [Tooltip("Nom de la scène de jeu à charger depuis le menu principal.")]
    public string sceneDuJeu = "TestLD";

    void Update()
    {
        
        if (SceneManager.GetActiveScene().name != "MainMenu" && Input.GetKeyDown(KeyCode.Escape))
        {
            RetourMenu();
        }
    }

    
    public void Jouer()
    {
        SceneManager.LoadScene(sceneDuJeu);
    }

   
    public void OuvrirOptions()
    {
        Debug.Log("Options à implémenter");
    }

   
    public void Quitter()
    {
        Debug.Log("Quitter le jeu");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

   
    public void RetourMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
