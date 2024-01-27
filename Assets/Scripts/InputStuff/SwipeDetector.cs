using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeDetector : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;
    [SerializeField] private GatitoManager player;
    [SerializeField] Transform gatitoFBX;
    [SerializeField] BoxCollider gatitoCol;

    [SerializeField] private float minimumDistance;
    [SerializeField] private float maximumTime;

    [SerializeField, Range(0, 1)] private float directionThreshold;

    Vector2 startPosition;
    float startTime;
    Vector2 endPosition;
    float endTime;

    private void OnEnable()
    {
        inputManager.OnStartTouch += SwipeStart;
        inputManager.OnEndTouch += SwipeEnd;
    }

    private void OnDisable()
    {
        inputManager.OnStartTouch -= SwipeStart;
        inputManager.OnEndTouch -= SwipeEnd;
    }

    void SwipeStart(Vector2 position, float time)
    {
        startPosition = position;
        startTime = time;
    }

    void SwipeEnd(Vector2 position, float time)
    {
        endPosition = position;
        endTime = time;
        DetectSwipe();
    }

    void DetectSwipe() 
    {
        if (TransformModifier.Instance.pauseGameplay)
        {
            return;
        }

        if (Vector2.Distance(startPosition, endPosition) >= minimumDistance && endTime - startTime <= maximumTime)
        {
            Vector3 direction = (endPosition - startPosition).normalized;
            FindSwipeDirection(direction);
        }
    }

    void FindSwipeDirection(Vector3 direction)
    {
        if (Vector3.Dot(Vector3.up, direction) > directionThreshold && player.isHorizontal)
        {
            Debug.Log("JUST SWIPED UP");
            //Vector3 velocityToConserve = player.rb.velocity;

            player.ClampPosition(Vector2.down);
            gatitoFBX.localScale = new Vector3(1, gatitoFBX.localScale.z, gatitoFBX.localScale.y);
            
            gatitoCol.size = new Vector3(gatitoCol.size.x, gatitoCol.size.z, gatitoCol.size.y);
            gatitoCol.center = new Vector3(gatitoCol.center.x, (Mathf.Max(gatitoCol.size.x, gatitoCol.size.y) - .6f) / 2, gatitoCol.center.z);

            player.isHorizontal = false;
            //player.rb.velocity = velocityToConserve;
            
        }
        else if (Vector3.Dot(Vector3.down, direction) > directionThreshold && !player.isHorizontal)
        {
            Debug.Log("JUST SWIPED DOWN");

            //Vector3 velocityToConserve = player.rb.velocity;

            player.ClampPosition(Vector2.right);
            gatitoFBX.localScale = new Vector3(1, gatitoFBX.localScale.z, gatitoFBX.localScale.y);

            gatitoCol.size = new Vector3(gatitoCol.size.x, gatitoCol.size.z, gatitoCol.size.y);
            gatitoCol.center = Vector3.zero;

            player.isHorizontal = true;
            //player.rb.velocity = velocityToConserve;
        }
    }
}
