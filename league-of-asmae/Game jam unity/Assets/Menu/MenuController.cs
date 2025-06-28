using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [Header("Levels to Load")]
    public string _newGameLevel;
    private string levelToLoad;
    [SerializeField] private GameObject No_save_file = null;

    public void Yes_New()
    {
        SceneManager.LoadScene(_newGameLevel);
    }

    public void Yes_load()
    {
        if (PlayerPrefs.HasKey("SavedLevel"))
        {
            levelToLoad = PlayerPrefs.GetString("SavedLevel");
            SceneManager.LoadScene(levelToLoad);
        }
        else
        {
            No_save_file.SetActive(true);

        }

    }

    public void ExitButton()
    {
        Application.Quit();
    }

}
