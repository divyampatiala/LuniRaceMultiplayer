using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class PlayerSetup: MonoBehaviour
{
    private void Start()
    {
       if(this.GetComponent<PhotonView>().IsMine==true)
        {
            this.GetComponent<PlayerMovement>().enabled = true;
        }
       else if(this.GetComponent<PhotonView>().IsMine==false)
        {
            this.GetComponent<PlayerMovement>().enabled = false;
        }
    }
}
