using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneDirector : MonoBehaviour
{
    public void OnQuitButtonPressed()
    {
        Application.Quit();
    }

    public void OnStartButtonPressed()
    {
        SceneManager.LoadScene(1);
    }
}
