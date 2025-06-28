using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScenes : MonoBehaviour
{
    public string platformer;

    public void LoadScenes()
    {
        SceneManager.LoadScene(platformer);

    }
}
