using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingObstacle : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    
    [SerializeField] float baseRollSpeed;
    [SerializeField] float difficultyCoef;
    public float rollSpeed;

    private void Start()
    {
        // Roll speed depends both on some predefined value and on the score of the player
        rollSpeed = baseRollSpeed + ObstacleTrigger.score * difficultyCoef;

        // We randomize the direction
        int rdmNum = Random.Range(0, 2);
        float xVel = rdmNum == 0 ? rollSpeed : -rollSpeed;
        rb.velocity = new Vector3(xVel, 0, 0);
    }

    private void OnCollisionEnter(Collision other)
    {
        // Reverse velocity when hitting wall
        if (other.gameObject.name == "RightWall")
        {
            rb.velocity = new Vector3(-rollSpeed, 0, 0);
        }

        else if (other.gameObject.name == "LeftWall")
        {
            rb.velocity = new Vector3(rollSpeed, 0, 0);
        }
    }
}
