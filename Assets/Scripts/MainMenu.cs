using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject GameMenu;
    [SerializeField]
    private GameObject OptionsMenu;
    [SerializeField]
    private GameObject AboutMenu; 


    public void About()
    {
        if (AboutMenu.activeSelf)
        {
            GameMenu.SetActive(true);
            AboutMenu.SetActive(false);
        }
        else
        {
            GameMenu.SetActive(false);
            AboutMenu.SetActive(true);
        }
    }

    public void Options()
    {
        if (OptionsMenu.activeSelf)
        {
            GameMenu.SetActive(true);
            OptionsMenu.SetActive(false);
        }
        else
        {
            GameMenu.SetActive(false);
            OptionsMenu.SetActive(true);
        }
    }

    public void Quit()
    {
        Application.Quit();
    }
}
