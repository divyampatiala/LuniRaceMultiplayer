using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
public class NetworkManager : MonoBehaviourPunCallbacks
{
    [Header("Home Panel")]
    public GameObject home_panel;
    public InputField player_name_input_field;
    [Header("Game Options Panel")]
    public GameObject game_options_panel;
    [Header("Play With Friends Panel")]
    public GameObject play_with_friends_panel;
    [Header("Create Custom Room Panel")]
    public GameObject create_custom_room_panel;
    public InputField room_name_input_field;
    public InputField max_players_input_field;
    public InputField room_password_input_field;
    public InputField confirm_password_input_field;
    [Header("Join Custom Room Panel")]
    public GameObject join_custom_room_panel;
    public GameObject room_info_prefab;
    public GameObject room_info_prefab_holder;
    private Dictionary<string, RoomInfo> cached_room_list;
    private Dictionary<string, GameObject> room_list_game_object_dictionary;
    #region Unity Methods
    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        ActivatePanel(home_panel.name);
        cached_room_list = new Dictionary<string, RoomInfo>();
        room_list_game_object_dictionary = new Dictionary<string, GameObject>();
    }
    #endregion
    #region UI Callback Methods
    public void OnLoginButtonClicked()
    {
        string player_name = player_name_input_field.text;
        if(!string.IsNullOrEmpty(player_name))
        {
            PhotonNetwork.LocalPlayer.NickName = player_name;
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            Debug.Log("Invalid Player Name");
        }
    }
    public void OnRandomMultiplayerButtonClicked()
    {
        ExitGames.Client.Photon.Hashtable expectedcustomproperties = new ExitGames.Client.Photon.Hashtable() {{ "RoomType", "Random" } };
        PhotonNetwork.JoinRandomRoom(expectedcustomproperties, 0);
    }
    public void OnPlayWithFriendsButtonClicked()
    {
        ActivatePanel(play_with_friends_panel.name);
    }
    public void OnCreateRoomPanelClicked()
    {
        ActivatePanel(create_custom_room_panel.name);
    }
    public void OnJoinRoomPanelClicked()
    {
        if(!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }
        ActivatePanel(join_custom_room_panel.name);
    }
    public void OnCreateCustomRoomButtonClicked()
    {
        string room_name = room_name_input_field.text;
        string max_players = max_players_input_field.text;
        string room_password = room_password_input_field.text;
        string confirm_password = confirm_password_input_field.text;
        if(!string.IsNullOrEmpty(room_name) && !string.IsNullOrEmpty(max_players) && !string.IsNullOrEmpty(room_password) && !string.IsNullOrEmpty(confirm_password))
        {
            if(room_password==confirm_password)
            {
                RoomOptions room_options = new RoomOptions();
                room_options.IsOpen = true;
                room_options.IsVisible = true;
                room_options.MaxPlayers = (byte)int.Parse(max_players);
                string[] roomPropertiesInLobby = {"RoomPassword", "RoomType" };
                ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable() {{ "RoomPassword", room_password }, { "RoomType", "Custom" } };
                room_options.CustomRoomPropertiesForLobby = roomPropertiesInLobby;
                room_options.CustomRoomProperties = customProperties;
                PhotonNetwork.CreateRoom(room_name, room_options);
            }
        }
    }
    public void OnBackButtonClicked()
    {
        if(PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            PhotonNetwork.Disconnect();
        }
    }
    #endregion
    #region PUN Callback Methods
    public override void OnConnected()
    {
        Debug.Log("Connected To Internet");
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + "has coneected to Photon");
        ActivatePanel(game_options_panel.name);
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log(message);
        CreateRoom();
    }
    public override void OnCreatedRoom()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " has created the room " + PhotonNetwork.CurrentRoom.Name);
    }
    public override void OnJoinedRoom()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " has joined the room " + PhotonNetwork.CurrentRoom.Name);
        PhotonNetwork.LoadLevel("Game");
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (var go in room_list_game_object_dictionary.Values)
        {
            Destroy(go);
        }
        room_list_game_object_dictionary.Clear();
        foreach (RoomInfo room_info in roomList)
        {
            if (room_info.CustomProperties.ContainsKey("RoomPassword"))
            {
                if (!room_info.IsVisible || !room_info.IsOpen || room_info.RemovedFromList)
                {
                    if (cached_room_list.ContainsKey(room_info.Name))
                    {
                        cached_room_list.Remove(room_info.Name);
                    }
                }
                else
                {
                    if (cached_room_list.ContainsKey(room_info.Name))
                    {
                        cached_room_list[room_info.Name] = room_info;
                    }
                    else
                    {
                        cached_room_list.Add(room_info.Name, room_info);
                    }
                }
            }

        }
        foreach (RoomInfo roomInfo in cached_room_list.Values)
        {
            Debug.Log(roomInfo);
            GameObject go = Instantiate(room_info_prefab);
            go.transform.SetParent(room_info_prefab_holder.transform);
            go.transform.localScale = Vector3.one;
            go.transform.Find("RoomNameHolderImage").transform.GetChild(0).GetComponent<Text>().text = roomInfo.Name;
            go.transform.Find("PlayerAmountHolderImage").transform.GetChild(0).GetComponent<Text>().text = roomInfo.PlayerCount+"/"+roomInfo.MaxPlayers;
            go.transform.Find("JoinButton").GetComponent<Button>().onClick.AddListener(() => OnJoinCustomRoomButtonClicked(roomInfo.Name, go.transform.Find("EnterPasswordInputField").GetComponent<InputField>().text, roomInfo));
            room_list_game_object_dictionary.Add(roomInfo.Name, go);
        }
    }
    public override void OnLeftLobby()
    {
        ClearRoomListView();
    }
    public override void OnLeftRoom()
    {
        PhotonNetwork.Disconnect();
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        ActivatePanel(home_panel.name);
    }
    #endregion
    #region Private Methods
    private void ActivatePanel(string panel_name_to_be_updated)
    {
        home_panel.SetActive(panel_name_to_be_updated.Equals(home_panel.name));
        game_options_panel.SetActive(panel_name_to_be_updated.Equals(game_options_panel.name));
        play_with_friends_panel.SetActive(panel_name_to_be_updated.Equals(play_with_friends_panel.name));
        create_custom_room_panel.SetActive(panel_name_to_be_updated.Equals(create_custom_room_panel.name));
        join_custom_room_panel.SetActive(panel_name_to_be_updated.Equals(join_custom_room_panel.name));
    }
    private void CreateRoom()
    {
        string room_name ="Room"+Random.Range(0,9999);
        string max_players = "20";
        RoomOptions room_options = new RoomOptions();
        room_options.IsOpen = true;
        room_options.IsVisible = true;
        room_options.MaxPlayers = (byte)int.Parse(max_players);
        string[] roomPropertiesInLobby = {"RoomType"};
        ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable() { { "RoomType", "Random" } };
        room_options.CustomRoomPropertiesForLobby = roomPropertiesInLobby;
        room_options.CustomRoomProperties = customProperties;
        PhotonNetwork.CreateRoom(room_name, room_options);
    }
    public void OnJoinCustomRoomButtonClicked(string roomName, string password, RoomInfo ri)
    {
        if (ri.CustomProperties.ContainsKey("RoomPassword"))
        {

            object keyPassword;
            if (ri.CustomProperties.TryGetValue("RoomPassword", out keyPassword))
            {
                if (password == keyPassword.ToString())
                {
                    Debug.Log("Correct Password");
                   
                    if (PhotonNetwork.InLobby)
                    {
                        PhotonNetwork.LeaveLobby();
                    }

                    PhotonNetwork.JoinRoom(roomName);
                }
                else
                {
                    Debug.Log("Incorrect Password");
                }
            }

        }
    }
    private void ClearRoomListView()
    {
        foreach(var go in room_list_game_object_dictionary.Values)
        {
            Destroy(go);
        }
        room_list_game_object_dictionary.Clear();
    }
    #endregion
}
