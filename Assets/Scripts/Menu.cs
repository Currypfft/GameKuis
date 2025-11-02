using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Menu : MonoBehaviour
{

    public void Play()
    {
        SceneManager.LoadScene("Category");
    }

    public void Back()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void Credit()
    {
        

    }

    public void Exit()
    {
        Application.Quit();
        Debug.Log("Quit button pressed. Exiting game...");
    }

  
}
