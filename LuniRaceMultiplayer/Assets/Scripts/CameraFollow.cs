using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class CameraFollow : MonoBehaviour
{
    public GameObject[] player_gameobjects_array;
    public GameObject player_gameObject;
    public float follow_speed;
    private void Start()
    {
        player_gameobjects_array = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject go in player_gameobjects_array)
        {
            if(go.GetComponent<PhotonView>().IsMine==true)
            {
                player_gameObject = go;
            }
        }
    }
    private void LateUpdate()
    {
        this.transform.position = Vector3.MoveTowards(this.transform.position, player_gameObject.transform.position+new Vector3(0,2,-5), follow_speed);
    }
}
