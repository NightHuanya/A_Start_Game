using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public Transform Player;
    private void Awake()
    {
        this.enabled = false;
    }
    void Update()
    {
        transform.position = new Vector3(Player.position.x + 4, transform.position.y, Player.transform.position.z - 4);
    }
}
