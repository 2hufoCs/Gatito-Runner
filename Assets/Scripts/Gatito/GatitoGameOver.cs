using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GatitoGameOver : MonoBehaviour
{
    [SerializeField] GatitoManager player;
    [SerializeField] Transform gatitoFBX;
    
    [SerializeField] GameObject gameOverWindow;
    static int highscore = 0;
    [SerializeField] TextMeshProUGUI highscoreCounter;

    private void Start()
    {
        ObstacleTrigger.score = 0;
        TransformModifier.Instance.pauseGameplay = true;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer == 6)
        {
            GameOver();
        }
    }

    void GameOver()
    {
        player.enabled = false;
        gatitoFBX.gameObject.SetActive(false);
        
        gameOverWindow.SetActive(true);
        highscore = ObstacleTrigger.score > highscore ? ObstacleTrigger.score : highscore;
        highscoreCounter.text = highscore.ToString();
    }
}