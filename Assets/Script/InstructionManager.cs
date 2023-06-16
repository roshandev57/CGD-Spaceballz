using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class InstructionManager : MonoBehaviour
{
    public TextMeshProUGUI instructionText;
    public GameObject continueButton;
    public string[] instructions;
    public string nextScene;

    private int currentPage = 0;

    void Start()
    {
        instructionText.text = instructions[currentPage];
    }

    public void NextPage()
    {
        if (currentPage < instructions.Length - 1)
        {
            currentPage++;
            instructionText.text = instructions[currentPage];
        }
        else
        {
            SceneManager.LoadScene(nextScene);
        }
    }
}
