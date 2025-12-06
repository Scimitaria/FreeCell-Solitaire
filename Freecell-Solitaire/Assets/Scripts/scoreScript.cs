using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class Score : MonoBehaviour
{
    public TMP_Text scoreText;
    public int score;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        score = 1000;
        scoreText.text = "01000";
    }

    void UpdateScore()
    {
        scoreText.text = score.ToString("D5");
    }

    public void AddScore(int points)
    {
        score += points;
        UpdateScore();
    }
}