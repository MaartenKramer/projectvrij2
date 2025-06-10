using UnityEngine;
using UnityEngine.SceneManagement;

public class StartingMenu : MonoBehaviour
{
    public GameObject controlsCanvas;
    public GameObject mainMenuCanvas;

    //Load next scene, start the game
    public void StartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        Debug.Log("Loading next scene...");
    }

    public void controlPanel()
    {
        mainMenuCanvas.SetActive(false);
        controlsCanvas.SetActive(true);
    }
    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
    public void mainMenuPanel()
    {
        controlsCanvas.SetActive(false);
        mainMenuCanvas.SetActive(true);
    }
}
