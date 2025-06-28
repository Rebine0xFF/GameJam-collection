using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.SceneManagement;

public class LoreManager : MonoBehaviour
{
    public Text textElement; 
    public Image imageElement;
    public Sprite[] loreImages; 
    public string[] loreTexts; 
    public string sceneToLoad = "GameScene"; 

    private int currentStep = 0; 

    void Start()
    {
        
        ShowStep(currentStep);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) 
        {
            currentStep++; 

            if (currentStep < loreTexts.Length) 
            {
                ShowStep(currentStep); 
            }
            else
            {
                
                textElement.text = "le jeu commence maintenant !!!!!";
                imageElement.enabled = false;

                
                Invoke("LoadGameScene", 2f);
            }
        }
    }

    
    void ShowStep(int stepIndex)
    {
        textElement.text = loreTexts[stepIndex]; 
        imageElement.sprite = loreImages[stepIndex];
        imageElement.enabled = true; 
    }

    // change de scene
    void LoadGameScene()
    {
        SceneManager.LoadScene(sceneToLoad); 
    }
}
