using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

public class GameManagerScript : MonoBehaviourPunCallbacks, IPunObservable
{
    public static GameManagerScript instance;
    [SerializeField] public PhotonView photonView;
    
    [SerializeField] public UnityEvent OnStartGame;
    [SerializeField] public UnityEvent OnResetRound;
    [SerializeField] public UnityEvent OnNextRound;
    
    [SerializeField] public bool gameStarted;
    [SerializeField] public float round = 1f;
    [SerializeField] public float maxEnemies = 10f;
    [SerializeField] public float iEnemiesByRounds = 5f;
    [SerializeField] public float enemyHealth = 50f;
    [SerializeField] public float iEnemyHealthbyRounds = 25f;
    [SerializeField] public float totalEnemiesKilled = 0f;
    [SerializeField] public float timeUntilStartGame = 3f;
    [SerializeField] public float timeUntilNextRound = 12.5f;
    [SerializeField] public float currentTime = 0f;
    [SerializeField] public bool spawnFinished = false;
    
    private void Awake()
    {
        instance = this;
        photonView = GetComponent<PhotonView>();
    }

    private void Start()
    {
        if (!PhotonNetwork.IsMasterClient)
        { return; }
        
        StartCoroutine(nameof(OnStartGameCoroutine));
    }
    
    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
        { return; }
        
        currentTime += Time.deltaTime;
        
        if (!gameStarted)
        { return; }
        
        if (totalEnemiesKilled >= maxEnemies)
        {
            maxEnemies += iEnemiesByRounds;
            enemyHealth += iEnemyHealthbyRounds;
            round++;
            totalEnemiesKilled = 0f;
            spawnFinished = false;
            Debug.Log("ROUND FINISHED");
        
            // Reset enemyManager
            StartCoroutine(nameof(OnNextRoundCoroutine));
            //OnNextRound.Invoke();
        }
    }

    [PunRPC]
    public void OnResetRoundRPC()
    {
        GameObject[] enemiesSpawned = GameObject.FindGameObjectsWithTag("Enemy");
        
        foreach (GameObject currentEnemy in enemiesSpawned)
        {
            PhotonNetwork.Destroy(currentEnemy);
        }
        
        totalEnemiesKilled = 0f;
        OnResetRound.Invoke();
        Debug.Log("ROUND RESET");
        
        StartCoroutine(nameof(OnNextRoundCoroutine));
    }
    
    private IEnumerator OnNextRoundCoroutine()
    {
        spawnFinished = false;
        yield return new WaitForSeconds(timeUntilNextRound);
        OnNextRound.Invoke();
        photonView.RPC("ResetEnemies", RpcTarget.MasterClient);
        Debug.Log("ROUND STARTED");
    }
    private IEnumerator OnStartGameCoroutine()
    {
        spawnFinished = false;
        yield return new WaitForSeconds(timeUntilStartGame);
        gameStarted = true;
        OnStartGame.Invoke();
        photonView.RPC("ResetEnemies", RpcTarget.MasterClient);
        Debug.Log("GAME STARTED");
    }
    
    public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    {
        if (PhotonNetwork.IsMasterClient && !spawnFinished)
        {
            photonView.RPC("OnResetRoundRPC", RpcTarget.MasterClient);
        }
    }
    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // Sync all variables
        if (stream.IsWriting)
        {
            stream.SendNext(gameStarted);
            stream.SendNext(round);
            stream.SendNext(maxEnemies);
            stream.SendNext(iEnemiesByRounds);
            stream.SendNext(enemyHealth);
            stream.SendNext(iEnemyHealthbyRounds);
            stream.SendNext(totalEnemiesKilled);
            stream.SendNext(timeUntilStartGame);
            stream.SendNext(timeUntilNextRound);
            stream.SendNext(currentTime);
            stream.SendNext(spawnFinished);
        }else if (stream.IsReading){
            gameStarted = (bool)stream.ReceiveNext();
            round = (float) stream.ReceiveNext();
            maxEnemies = (float) stream.ReceiveNext();
            iEnemiesByRounds = (float) stream.ReceiveNext();
            enemyHealth = (float) stream.ReceiveNext();
            iEnemyHealthbyRounds = (float) stream.ReceiveNext();
            totalEnemiesKilled = (float) stream.ReceiveNext();
            timeUntilStartGame  = (float) stream.ReceiveNext();
            timeUntilNextRound = (float) stream.ReceiveNext();
            currentTime = (float)stream.ReceiveNext();
            spawnFinished = (bool)stream.ReceiveNext();
        }
    }
}
