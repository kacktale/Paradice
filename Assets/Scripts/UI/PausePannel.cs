using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PausePannel : MonoBehaviour
{
    public Player Player;
    public void BackToGame()
    {
        Player.PausePanel.SetActive(false);
        Time.timeScale = 1;
    }
    public void QuitGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
}
