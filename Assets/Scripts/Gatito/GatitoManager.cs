using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;
using UnityEngine.TextCore.Text;

public class GatitoManager : MonoBehaviour
{
    [SerializeField] Transform gatitoFBX;
    [SerializeField] BoxCollider col;
    [SerializeField] CameraScript cam;
    [SerializeField] PlatformSpawner platformSpawner;
    
    [SerializeField] ObstacleSpawnScript obsSpawn;
    [SerializeField] GameObject emptyObstacle;

    [SerializeField] LayerMask groundLayer;
    [SerializeField] float bottomLimit;
    [SerializeField] float rightLimit;
    [SerializeField] float leftLimit;

    public Rigidbody rb;
    public float length = 1;
    public float maxLength;
    [SerializeField] float lengthRaise;

    [SerializeField] float forwardSpeed;
    public float maxForwardSpeed;
    public float speedRaise;

    [SerializeField] float sideSpeed;
    [SerializeField] float minSideDistance;
    [SerializeField] float moveDiffMultiplier;
    [SerializeField] float accel;

    float touchOffset = 0;
    float moveX;
    [SerializeField] float frictionX;
    float previousVel = 0;
    public bool isHorizontal = true;

    [SerializeField] Image image;

    void Update()
    {
        if (TransformModifier.Instance.pauseGameplay)
        {
            return;
        }

        if (Input.touchCount > 0)
        {
            // How far from the player the camera is
            float camDistance = transform.position.z - cam.transform.position.z;
            Touch touch = Input.GetTouch(0);

            if (touch.phase == UnityEngine.TouchPhase.Began)
            {
                // If the player began touching screen, even if it's not aligned with the player, they will not move
                // You could say it's the point where holding your finger won't make them move
                touchOffset = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, camDistance)).x;
                Debug.Log("BEGAN TOUCHING SCREEN");
                return;
            }
            float target = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, camDistance)).x;

            // Substracting the offset to the target so the position of the player will be relative to the location of the starting touch
            float deltaX = target - touchOffset;
            moveX = Mathf.Clamp(deltaX, platformSpawner.leftEdge, platformSpawner.rightEdge);

            // deltaX is the target location, what we want is the vector from the player to the target location (the x component), which is:
            moveX = deltaX;
            Debug.Log("velocity this frame: " + rb.velocity + ", moveX: " + moveX + ", touchOffset: " + touchOffset);
        }

        else
        {
            moveX = 0;
        }
    }

    public void FixedUpdate()
    {
        if (TransformModifier.Instance.pauseGameplay)
        {
            return;
        }

        float moveZ = forwardSpeed - rb.velocity.z;
        rb.AddForce(moveZ * Vector3.forward);

        if (Mathf.Abs(moveX) > minSideDistance)
        {
            float wantedSpeed = moveX * sideSpeed;
            float moveDiff = wantedSpeed - rb.velocity.x;
            float force = Mathf.Pow(Mathf.Abs(moveDiff), accel) * Mathf.Sign(moveDiff);


            rb.AddForce(force * Vector3.right);
        }

        float speedZ = rb.velocity.z;
        speedZ = Mathf.Clamp(speedZ, 0, maxForwardSpeed);
        rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, speedZ);
        /*
        else if (Mathf.Abs(rb.velocity.x) > 0.01f)
        {    
            // The player stopped touching screen but they still has some velocity left
            float force = Mathf.Min(Mathf.Abs(rb.velocity.x), frictionX);
            force *= Mathf.Sign(rb.velocity.x);
            rb.AddForce(-force * Vector3.right, ForceMode.Impulse);
        }
        */
    }    

    private void LateUpdate()
    {
        // After rotating the player will have a huge drop in velocity for some reason, when that happens just set the velocity to the value during the previous frame
        if (previousVel - rb.velocity.z > 3)
        {
            Debug.Log("Prevented velocity drop");
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, previousVel);
        }


        previousVel = rb.velocity.z;
    }

    public IEnumerator StretchGatito()
    {
        yield return new WaitForSeconds(.25f);
        length += lengthRaise;

        // Raising scale of the gatito
        Vector3 baseScale = gatitoFBX.localScale;
        Vector3 targetScale = isHorizontal ? new Vector3(1, 1, length) : new Vector3(1, length, 1); 
        float stretchAnimDuration = .5f;
        StartCoroutine(TransformModifier.Instance.ScaleTransform(gatitoFBX, baseScale, targetScale, stretchAnimDuration));

        // Raising the length of the gatito also raises the size of their collider and the position of its center
        col.size = isHorizontal ? new Vector3(col.size.x, col.size.y, col.size.z + lengthRaise) : new Vector3(col.size.x, col.size.y + lengthRaise, col.size.z);
        col.center = isHorizontal ? Vector3.zero : new Vector3(col.center.x, (Mathf.Max(col.size.x, col.size.y) - .6f) / 2, col.center.z);

        Debug.Log($"length: {length}, anchor size: {targetScale}, collider size: {col.size}");
        yield return new WaitForSeconds(.75f);
        StartCoroutine(cam.AwayFromGatito());
    }


    public void ClampPosition(Vector3 direction)
    {
        float halfLength = gatitoFBX.localScale.x / 2;
        float newPosition;

        // When the player is horizontal and wants to rotate to vertical
        if (direction == Vector3.down)
        {
            newPosition = bottomLimit + halfLength;
            transform.position = new Vector3(transform.position.x, newPosition, transform.position.z);
            return;
        }

        if (direction == Vector3.up)
        {
            Debug.Log("What the hell, you just called the ClampPosition function with a \"Vector3.up\" parameter");
            return;
        }

        // When the player is vertical and wants to rotate to horizontal, check if there's an obstacle left or right to them
        halfLength = gatitoFBX.localScale.y / 2;
        bool rightHit = Physics.Raycast(transform.position, Vector3.right, halfLength, groundLayer);
        bool leftHit = Physics.Raycast(transform.position, Vector3.left, halfLength, groundLayer);

        if (!rightHit && !leftHit)
        {
            transform.position = new Vector3(transform.position.x, 1, transform.position.z);
            return;
        }

        // Moves the player so they don't hit walls when they rotate back to horizontal
        float limit = rightHit ? rightLimit : leftLimit;
        newPosition = limit + (rightHit ? -halfLength : halfLength);
        transform.position = new Vector3(newPosition, 1, transform.position.z);
    }
}