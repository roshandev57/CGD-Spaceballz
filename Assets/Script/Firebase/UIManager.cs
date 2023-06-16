using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [SerializeField]
    private GameObject loginPanel;

    [SerializeField]
    private GameObject registrationPanel;

  [SerializeField]
   private GameObject userDataPanel;

   [SerializeField]
    private GameObject scoreboardPanel;



    private void Awake()
    {
        CreateInstance();
    }



    private void CreateInstance()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    public void OpenLoginPanel()
    {
        loginPanel.SetActive(true);
        registrationPanel.SetActive(false);
        userDataPanel.SetActive(false);
        scoreboardPanel.SetActive(false);
    }

    public void OpenRegistrationPanel()
    {
        registrationPanel.SetActive(true);
        loginPanel.SetActive(false);
        userDataPanel.SetActive(false);
       scoreboardPanel.SetActive(false);
    }

   public void OpenUserDataPanel()
    {
       userDataPanel.SetActive(true);
        registrationPanel.SetActive(false);
        loginPanel.SetActive(false);
        scoreboardPanel.SetActive(false);

    }

    public void OpenScoreboardPanel()
    {
        scoreboardPanel.SetActive(true);
        registrationPanel.SetActive(false);
        loginPanel.SetActive(false);
        userDataPanel.SetActive(false);


    }

  
}
