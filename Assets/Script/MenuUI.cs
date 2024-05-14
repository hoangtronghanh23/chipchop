using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class MenuUI : MonoBehaviour
{
    public GameObject menuUI;           //  Menu game object to show the menu in screen
            //  Health of the player

    bool isPause = false;                       //  Control if the player has paused the game

    private void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPause)
            {
                Pause();
            }
            else
            {
                Continue();
            }
        }

    }

    public void Play()
    {
        // Playe the game, reset the same scene
       
        ResetGame();
    }

    public void ResetGame()
    {
        //  Get the active scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    private void Continue()
    {
        //  Unpause the game and hide the menu
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1;
        menuUI.SetActive(false);
        isPause = false;
    }

    private void Pause()
    {
        //  Pause the game and show the menu
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0;
        menuUI.SetActive(true);
        isPause = true;
    }

    public void ExitGame()
    {
        //  Quit the aplicacion
        Application.Quit();
    }
}
