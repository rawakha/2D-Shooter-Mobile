using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Method to load the game scene when the Start button is pressed
    public void StartGame()
    {
        // Replace "GameScene" with the name of your actual game scene
        SceneManager.LoadScene(1);
    }

    // Method to quit the game when the Quit button is pressed
    public void QuitGame()
    {
        Debug.Log("Quit!");
        Application.Quit();
    }
}
