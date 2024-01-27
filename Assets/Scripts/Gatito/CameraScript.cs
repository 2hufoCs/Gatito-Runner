using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField] GatitoManager player;
    [SerializeField] ObstacleSpawnScript obsSpawn;
    public Vector3 baseOffset;
    public Vector3 baseRotation;

    public Vector3 animOffset;

    private void Start()
    {
        transform.LookAt(player.transform);
    }

    private void LateUpdate()
    {
        if (!TransformModifier.Instance.pauseGameplay)
        {
            transform.position = new Vector3(baseOffset.x, baseOffset.y, player.transform.position.z + baseOffset.z);
        }
    }
   
    public IEnumerator TowardsGatito()
    {
        yield return new WaitForSeconds(1);
        TransformModifier.Instance.pauseGameplay = true;
        player.rb.velocity = Vector3.zero;

        Vector3 baseCamPos = transform.position;
        Vector3 endCamPos = player.transform.position + animOffset;
        float animDuration = 1;

        StartCoroutine(TransformModifier.Instance.MoveTransform(transform, baseCamPos, endCamPos, animDuration));
        StartCoroutine(TransformModifier.Instance.StareAt(transform, player.transform, animDuration));

        yield return new WaitForSeconds(animDuration);
        StartCoroutine(player.StretchGatito());
    }

    public IEnumerator AwayFromGatito()
    {
        Vector3 baseCamPos = transform.position;
        Vector3 endCamPos = baseOffset + new Vector3(0, 0, player.transform.position.z);
        float animDuration = 1;

        Transform stareTransform = new GameObject().transform;     
        stareTransform.position = new Vector3(0, player.transform.position.y, player.transform.position.z);

        StartCoroutine(TransformModifier.Instance.MoveTransform(transform, baseCamPos, endCamPos, animDuration));
        StartCoroutine(TransformModifier.Instance.StareAt(transform, stareTransform, animDuration));

        yield return new WaitForSeconds(animDuration);
        TransformModifier.Instance.pauseGameplay = false;
    }
}
