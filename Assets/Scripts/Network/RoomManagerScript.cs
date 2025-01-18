using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class RoomManagerScript : MonoBehaviourPunCallbacks
{

    public static RoomManagerScript instance;
    
    public GameObject player;
    public GameObject masterClientInstancesPrefab;

    [Space]
    public Transform spawnPoint;
    
    [Space]
    public GameObject roomCam;

    [Space] 
    public GameObject nickNameUI;
    public GameObject connectingUI;

    [SerializeField]
    private RoomOptions roomOptions;
    private string nickName = "unnamed";
    
    void Awake()
    {
        
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    private void StartConnection()
    {
        Debug.Log("Connecting...");
        
        roomOptions = new RoomOptions() { MaxPlayers = 4 };
        roomOptions.IsVisible = true;
        roomOptions.IsOpen = true;
        roomOptions.CleanupCacheOnLeave = false;
        roomOptions.PlayerTtl = -1;
        PhotonNetwork.ConnectUsingSettings();
    }
    
    void Update()
    {
        //Debug.Log("Estado da conexão: " + PhotonNetwork.NetworkClientState);
        //Debug.Log("Lobby atual: " + PhotonNetwork.CurrentLobby);
        //Debug.Log("Connected and Ready: " + PhotonNetwork.IsConnectedAndReady);
    }
    
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("Connected to Server!!!");
        
        if(PhotonNetwork.NetworkClientState == ClientState.ConnectedToMasterServer)
        {
            PhotonNetwork.JoinLobby();
        }
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        Debug.Log("Joined Lobby");

        if (PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.JoinOrCreateRoom("test", roomOptions, TypedLobby.Default);
        }
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        
        Debug.LogFormat("Joined in Room!!!!!");
        Debug.Log(player);
        Debug.Log(PhotonNetwork.IsConnectedAndReady);
        Debug.Log(PhotonNetwork.CurrentRoom);
        Debug.Log(PhotonNetwork.InRoom);

        if (player != null)
        {
            GameObject _player = PhotonNetwork.Instantiate(player.name, spawnPoint.position, Quaternion.identity);
            if (_player != null)
            {
                _player.GetComponent<PlayerSetup>().IsLocalPlayer();
                roomCam.SetActive(false);
                Debug.Log("PLAYER SPAWN AND IS LOCAL!!!!");
                
                if (PhotonNetwork.IsMasterClient)
                {
                    //Debug.Log("MASTER CLIENT MANAGER!!!!!");
                    PhotonNetwork.InstantiateRoomObject(masterClientInstancesPrefab.name, new Vector3(0,0,0), Quaternion.identity);
                }
                
                _player.GetComponent<PhotonView>().RPC("SetNickName", RpcTarget.AllBuffered, nickName);
            }
        }
    }

    public void RespawnPlayer()
    {
        Debug.Log(player);
        if (this.player != null)
        {
            GameObject _player = PhotonNetwork.Instantiate(this.player.name, spawnPoint.position, Quaternion.identity);
            if (_player != null)
            {
                _player.GetComponent<PlayerSetup>().IsLocalPlayer();
            }
            else
            {
                RespawnPlayer();
            }
        }
        else
        {
            RespawnPlayer();
        }
    }

    public void ChangeNickName(string newNickName)
    {
        this.nickName = newNickName;
    }

    public void OnStartButtonPressed()
    {
        StartConnection();
     
        nickNameUI.SetActive(false);
        connectingUI.SetActive(true);
    }
    
}
