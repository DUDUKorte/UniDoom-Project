using Photon.Pun;
using UnityEngine;

public class EnemyManager : MonoBehaviour, IPunObservable
{
    public float maxSimultaneousEnemies;
    public float timeBetweenSpawns;
    
    private GameObject[] spawners;
    
    [SerializeField] public float _enemiesCount = 0f;
    [SerializeField] public float maxEnemies;
    [SerializeField] public bool canSpawn = false;

    private void Awake()
    {
        canSpawn = false;
        spawners = GameObject.FindGameObjectsWithTag("EnemySpawner");
    }
    
    private void FixedUpdate()
    {
        if (!PhotonNetwork.IsMasterClient)
        { return; }
        
        if (canSpawn)
        {
            Debug.Log("CAN SPAWN FIXED UPDATE CALLED");
            canSpawn = false;
            if (_enemiesCount >= maxEnemies) { return; }
            
            Invoke(nameof(SpawnRandomEnemies), timeBetweenSpawns);
        }
    }

    private void SpawnRandomEnemies()
    {
        Debug.Log("Spawning random enemies CALLED");
        for (int i = 0; i < maxSimultaneousEnemies; i++)
        {
            if (_enemiesCount >= maxEnemies)
            {
                canSpawn = false;
                GameManagerScript.instance.spawnFinished = true;
                return;
            }
            Debug.LogWarning("SPAWN 1 ENEMY RANDOM ENEMIES SIMULTANEOUS");
            
            GameObject currentSpawner = spawners[Random.Range(0, spawners.Length)];
            //currentSpawner.GetComponent<EnemySpawnerScript>().AddEnemyToQueue();
            currentSpawner.GetComponent<PhotonView>().RPC("AddEnemyToQueue", RpcTarget.MasterClient);
            
            _enemiesCount++;
        }
        
        canSpawn = true;
    }
    
    [PunRPC]
    public void ResetEnemies()
    {
        Debug.Log("Reset Enemies CALLED");
        _enemiesCount = 0;
        maxEnemies = GameManagerScript.instance.maxEnemies;
        canSpawn = true;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_enemiesCount);
            stream.SendNext(maxEnemies);
            stream.SendNext(canSpawn);
        }else if (stream.IsReading)
        {
            _enemiesCount = (float) stream.ReceiveNext();
            maxEnemies = (float) stream.ReceiveNext();
            canSpawn = (bool) stream.ReceiveNext();
        }
    }
}
