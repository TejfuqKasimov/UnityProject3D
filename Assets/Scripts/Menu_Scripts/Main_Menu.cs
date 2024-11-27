using UnityEngine;
using UnityEngine.SceneManagement;

public class Main_Manu : MonoBehaviour
{
    public void Play_Game()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void ExitGame()
    {
        Debug.Log("Gay");
        Application.Quit();
    }
}
