using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.VisualScripting;
using UnityEngine;

public class ObstacleSpawnScript : MonoBehaviour
{
    [SerializeField] GatitoManager player;
    [SerializeField] CameraScript cam;
    [SerializeField] PlatformSpawner platformSpawner;

    [SerializeField] GameObject obstacle;
    [SerializeField, Range(0, 100)] int wallSpawnPrc;
    [SerializeField] GameObject rollObstacle;
    [SerializeField, Range(0, 100)] int rollSpawnPrc;
    [SerializeField] BlockWall blockWall;

    public float obstacleLimit;
    public int obstaclesSpawned = 0;

    public List<GameObject> obstacleList = new();
    [SerializeField] float obstacleGap;
    [SerializeField] float spawnDistance;
    float distance = 0;

    // Update is called once per frame
    void Update()
    {
        if (TransformModifier.Instance.pauseGameplay)
        {
            return;
        }

        if (obstaclesSpawned == obstacleLimit)
        {
            if (obstacleList.Count == 0)
            {
                // If all obstacles are spawned then we launch the animation to raise the length of gatito
                StartCoroutine(cam.TowardsGatito());

                obstaclesSpawned = 0;
                // If the player length is to its maximum then we assign the threshold to infinity to stop calling the stretch function
                obstacleLimit = player.length >= player.maxLength ? Mathf.Infinity : obstacleLimit;
            }
        }
        else
        {
            distance += player.rb.velocity.magnitude * Time.deltaTime;
        }

        if (distance >= obstacleGap)
        {
            distance = 0;
            // That's an empty object, we'll spawn the actual obstacle later
            GameObject newObstacle = Instantiate(obstacle, new Vector3Int(0, 0, (int)(player.transform.position.z + spawnDistance)), Quaternion.identity);
            newObstacle.transform.parent = transform;

            // The type of the obstacle to spawn is mostly random, it depends on the value attributed to the "prc" floats
            int rdmNum = Random.Range(0, 100);
            if (rdmNum < rollSpawnPrc)
            {
                // The x position is just somewhere in the range of the platform
                float xPos = Random.Range(platformSpawner.leftEdge, platformSpawner.rightEdge);

                GameObject newBall = Instantiate(rollObstacle, new Vector3(xPos, 1, newObstacle.transform.position.z), Quaternion.identity);
                newBall.transform.parent = newObstacle.transform;
            }
            else
            {
                blockWall.SpawnObstacle(newObstacle);
            }

            obstacleList.Add(newObstacle);
            obstaclesSpawned++;
        }
    }
}
