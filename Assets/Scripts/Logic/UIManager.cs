using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] GatitoManager player;
    [SerializeField] CameraScript cam;

    [SerializeField] GameObject startWindow;
    [SerializeField] TextMeshProUGUI startCountdown;

    IEnumerator Countdown(float seconds)
    {
        float countdown = seconds;
        StartCoroutine(TransformModifier.Instance.PauseEverything(countdown));

        while (countdown > 0)
        {
            countdown -= Time.unscaledDeltaTime;
            yield return null;
        }

        TransformModifier.Instance.pauseGameplay = false;
    }

    public void PlayGame()
    {
        startWindow.SetActive(false);
        StartCoroutine(TransformModifier.Instance.MoveTransform(cam.transform, cam.transform.position, cam.baseOffset, 2));
        StartCoroutine(TransformModifier.Instance.StareAt(cam.transform, player.transform, 2));

        StartCoroutine(Countdown(2));
    }

    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
