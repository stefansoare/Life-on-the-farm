using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamCtrl : MonoBehaviour
{
    public float zOff = 5f;
    public float smooth = 2f;
    Transform pPos;

    void Start()
    {
        pPos = FindObjectOfType<PlayerController>().transform;
    }

    void Update()
    {
        Follow();
    }

    void Follow()
    {
        Vector3 targetPos = new Vector3(pPos.position.x, transform.position.y, pPos.position.z - zOff);
        transform.position = Vector3.Lerp(transform.position, targetPos, smooth * Time.deltaTime);
    }
}