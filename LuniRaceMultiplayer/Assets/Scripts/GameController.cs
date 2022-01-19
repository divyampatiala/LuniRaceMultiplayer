using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
public class GameController : MonoBehaviourPunCallbacks
{
    public GameObject player_prefab;
    public GameObject[] players;
    #region Unity Methods
    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        GameObject go = PhotonNetwork.Instantiate(player_prefab.name, new Vector3(Random.Range(-3, 3), 0.5f, Random.Range(-4,4)), Quaternion.identity);
    }
   
    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            players = GameObject.FindGameObjectsWithTag("Player");
            foreach(GameObject go in players)
            {
                foreach (Player p in PhotonNetwork.PlayerList)
                {
                    if (p.ActorNumber == go.GetComponent<PhotonView>().OwnerActorNr)
                    {
                        go.transform.GetChild(0).transform.Find("PlayerNameText").GetComponent<TextMeshProUGUI>().text =p.NickName;
                    }
                }
            }
        }
    }
    #endregion
    #region UI Callback Methods
    public void OnLeaveRoomClicked()
    {
        PhotonNetwork.LeaveRoom();
    }
    #endregion
    #region PUN Callback Methods
    public override void OnLeftRoom()
    {
        PhotonNetwork.Disconnect();
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        PhotonNetwork.LoadLevel("Menu");
    }
    #endregion
}
