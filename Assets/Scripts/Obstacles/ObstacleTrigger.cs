using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ObstacleTrigger : MonoBehaviour
{
    [SerializeField] GatitoManager player;
    [SerializeField] ObstacleSpawnScript obsSpawn;

    [SerializeField] float moveSpeed;
    public static int score = 0;
    [SerializeField] TextMeshProUGUI scoreCounter;
    
    bool triggered = false;
    [SerializeField] float despawnDistance;
    bool despawning = false;
    [SerializeField] float fadeDuration;

    private void Start()
    {
        player = FindAnyObjectByType<GatitoManager>().GetComponent<GatitoManager>();
        obsSpawn = FindAnyObjectByType<ObstacleSpawnScript>().GetComponent<ObstacleSpawnScript>();
        scoreCounter = GameObject.Find("ScoreCounter").GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (transform.position.z - player.transform.position.z < -despawnDistance && !despawning)
        {
            obsSpawn.obstacleList.Remove(gameObject);
            
            // We make a loop in case there are many elements with a mesh in the obstacle
            for (int i = 0; i < transform.childCount; i++)
            {
                // For each child we get its mesh (or the mesh of its own child), we take its material and we make it fade out
                Transform obstacleGFX = transform.GetChild(i);
                Material obstacleMat = obstacleGFX.GetComponent<MeshRenderer>().material;
                StartCoroutine(TransformModifier.Instance.FadeOut(transform, obstacleMat, fadeDuration, false));
            }
            despawning = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 3 && !triggered)
        {
            score++;
            scoreCounter.text = score.ToString();

            player.maxForwardSpeed += player.speedRaise;
            triggered = true;
        }
    }
}
