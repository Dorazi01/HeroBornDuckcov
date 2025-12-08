using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneDirector : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnQuitButtonPressed()
    {
        Application.Quit();
    }

    public void OnStartButtonPressed()
    {
        SceneManager.LoadScene(1);
    }
}
