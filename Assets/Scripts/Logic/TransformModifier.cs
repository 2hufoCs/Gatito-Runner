using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TransformModifier : MonoBehaviour
{
    public static TransformModifier Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.Log("Ayo, actually destroyed an instance of the transformModif singleton ?!");
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public IEnumerator MoveTransform(Transform objTransform, Vector3 startPos, Vector3 endPos, float duration)
    {
        float timer = 0;
        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            float interpolateValue = timer / duration;
            objTransform.position = new Vector3(Mathf.Lerp(startPos.x, endPos.x, interpolateValue), 
                                                Mathf.Lerp(startPos.y, endPos.y, interpolateValue), 
                                                Mathf.Lerp(startPos.z, endPos.z, interpolateValue));
            yield return null;
        }
        objTransform.position = endPos;
    }

    public IEnumerator RotateTransform(Transform objTransform, Vector3 startRot, Vector3 endRot, float duration)
    {
        float timer = 0;
        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            float interpolateValue = timer / duration;
            objTransform.localRotation = Quaternion.Euler(new Vector3(Mathf.Lerp(startRot.x, endRot.x, interpolateValue), 
                                                                      Mathf.Lerp(startRot.y, endRot.y, interpolateValue),   
                                                                      Mathf.Lerp(startRot.z, endRot.z, interpolateValue)));
            yield return null;
        }
        objTransform.localRotation = Quaternion.Euler(endRot);
    }

    public IEnumerator ScaleTransform(Transform objTransform, Vector3 startScale, Vector3 endScale, float duration)
    {
        float timer = 0;
        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            float interpolateValue = timer / duration;
            objTransform.localScale = new Vector3(Mathf.Lerp(startScale.x, endScale.x, interpolateValue),
                                                  Mathf.Lerp(startScale.y, endScale.y, interpolateValue),
                                                  Mathf.Lerp(startScale.z, endScale.z, interpolateValue));
            yield return null;
        }
        objTransform.localScale = endScale;
    }

    public IEnumerator StareAt(Transform agent, Transform patient, float duration)
    {
        float timer = 0;
        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            agent.LookAt(patient);
            yield return null;
        }
    }

    public IEnumerator FadeOut(Transform objTransform, Material mat, float duration, bool destroy)
    {
        Color objColor = mat.color;

        float timer = 0;
        while (timer < duration)
        {
            float fadeAmount = objColor.a - timer / duration;
            Color newColor = new Color(objColor.r, objColor.g, objColor.b, fadeAmount);
            mat.color = newColor;

            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        objColor = new Color(objColor.r, objColor.g, objColor.b, 0);
        mat.color = objColor;
        if (destroy)
        {
            Destroy(objTransform.gameObject);
        }
    }

    public bool pauseGameplay = false;

    public IEnumerator PauseEverything(float seconds)
    {
        // We set the timescale to an extremely low value, so stuff involving time will most likely pause
        Time.timeScale = 0.00001f;
        yield return null;

        float timer = 0;
        while (timer < seconds)
        {
            // Do nothing in case of a lag spike
            if (Time.unscaledDeltaTime > 0.1f)
            {
                yield return null;
            }

            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        Time.timeScale = 1;
    }
}
