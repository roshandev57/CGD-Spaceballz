using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class Instruction : MonoBehaviour
{
    public RectTransform panel1;
    public RectTransform panel2;
    public RectTransform panel3;

    private Vector2 panel1StartPosition;
    private Vector2 panel2StartPosition;
    private Vector2 panel3StartPosition;

    private void Start()
    {
        // Store the initial positions of panels
        panel1StartPosition = panel1.anchoredPosition;
        panel2StartPosition = panel2.anchoredPosition;
        panel3StartPosition = panel3.anchoredPosition;
    }

    public void ShowInstructions()
    {
        // Slide panel 2 to the position of panel 1
        panel2.DOAnchorPos(panel1StartPosition, 0.5f);
    }

    public void ShowControls()
    {
        // Slide panel 3 to the position of panel 1
        panel3.DOAnchorPos(panel1StartPosition, 0.5f);
    }

    public void ReturnToWelcome()
    {
        // Slide panel 2 and panel 3 to their initial positions
        panel2.DOAnchorPos(panel2StartPosition, 0.5f);
        panel3.DOAnchorPos(panel3StartPosition, 0.5f);
    }

    public void MoveToNextScene()
    {
        // Load the next scene
        SceneManager.LoadScene("SelectLevel");
    }
}
