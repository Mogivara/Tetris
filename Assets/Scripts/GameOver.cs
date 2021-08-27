using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public void GoMenu()
    {
        SceneManager.LoadScene("Main");
    }

    public void GoNewGame()
    {
        SceneManager.LoadScene("StartGame");
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
