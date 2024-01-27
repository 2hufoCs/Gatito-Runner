using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script spawns platforms during runtime, instead of having to place a ton of them beforehand.
/// </summary>
public class PlatformSpawner : MonoBehaviour
{
    [SerializeField] GatitoManager player;
    [SerializeField] GameObject platform;
    [SerializeField] Transform preSpawnedPlatform;

    [SerializeField] int nbOfPlatforms;
    int currentPosZ; 
    
    [SerializeField] float distanceTraveled = 0;

    [SerializeField] int startQuantity;
    [SerializeField] float startInterval;

    public float leftEdge;
    public float rightEdge;

    private void Start()
    {
        currentPosZ = (int)preSpawnedPlatform.position.z;
        StartCoroutine(SpawnStartingPlatforms(startQuantity, startInterval));

        leftEdge = transform.Find("LeftWall").GetComponent<BoxCollider>().center.x + 1;
        rightEdge = transform.Find("RightWall").GetComponent<BoxCollider>().center.x - 1;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        distanceTraveled += player.rb.velocity.magnitude * Time.deltaTime;
        if (distanceTraveled >= nbOfPlatforms)
        {
            SpawnPlatforms();
            distanceTraveled = 0;
        }
    }

    IEnumerator SpawnStartingPlatforms(int quantity, float interval)
    {
        float timer = 0;
        while (timer < interval)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        if (quantity > 0)
        {
            SpawnPlatforms();
            quantity -= 1;
            StartCoroutine(SpawnStartingPlatforms(quantity, interval));
        }
        
    }

    void SpawnPlatforms()
    {
        GameObject platformList = new();
        platformList.name = "PackOfObstacles";
        platformList.transform.parent = transform;

        for (int i = currentPosZ; i <= currentPosZ + nbOfPlatforms; i++)
        {
            GameObject newPlatform = Instantiate(platform, new Vector3(0, 0, i), Quaternion.identity);
            newPlatform.transform.parent = platformList.transform;
        }
        currentPosZ += nbOfPlatforms;
    }
}
