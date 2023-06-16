using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Firebase.Auth;
using Firebase;
using Firebase.Database;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    public GameObject scoreboardPanel;
    public Transform targetTransform;
    public TextMeshProUGUI usernameText;
    public DatabaseReference DBreference;

    private bool isPanelMoved = false;
    private Vector3 originalPosition;

    private void Start()
    {
        // Get the current authenticated user
        FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;

        if (user != null)
        {
            // Update the username text with the user's display name
            usernameText.text = "Welcome, " + user.DisplayName + "!";
        }
    }

    // Called when the Play button is clicked
    public void OnClickPlay()
    {
        SceneManager.LoadScene("Instruction");
    }

    // Called when the Training button is clicked
    public void OnClickTraining()
    {
        SceneManager.LoadScene("TrainingGrounds");
    }

    // Called when the Leaderboard button is clicked
    public void OnClickLeaderboard()
    {
        if (!isPanelMoved)
        {
            // Store the original position of the scoreboardPanel
            originalPosition = scoreboardPanel.transform.position;

            // Move scoreboardPanel to the target position
            scoreboardPanel.transform.DOMove(targetTransform.position, 0.5f);
            isPanelMoved = true;
        }
    }

    // Called when the Back button is clicked
    public void OnClickBack()
    {
        if (isPanelMoved)
        {
            // Move scoreboardPanel back to the original position
            scoreboardPanel.transform.DOMove(originalPosition, 0.5f);
            isPanelMoved = false;
        }
    }

    // Called when the Quit button is clicked
    public void OnClickQuit()
    {
        // You may need to handle platform-specific behavior here
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
