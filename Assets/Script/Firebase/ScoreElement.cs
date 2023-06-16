using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class ScoreElement : MonoBehaviour
{

    public TMP_Text usernameText;
    
    public TMP_Text scoreText;

    public TMP_Text scoreText1;

    public TMP_Text scoreText2;

    public void NewScoreElement(string _username,  int _score, int _score1, int _score2)
    {
        usernameText.text = _username;
        scoreText.text = _score.ToString();
        scoreText1.text = _score1.ToString();
        scoreText2.text = _score2.ToString();

    }

}