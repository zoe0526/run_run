using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Vector3 offset;
    public Transform player;
    public void Awake()
    {
        offset = transform.position - player.transform.position;
    }
    private void LateUpdate()
    {
        //transform.position = new Vector3(player.transform.position.x + offset.x, transform.position.y, transform.position.z);
        transform.position = new Vector3(player.transform.position.x + offset.x, player.transform.position.y + offset.y, transform.position.z);
    }
} 
