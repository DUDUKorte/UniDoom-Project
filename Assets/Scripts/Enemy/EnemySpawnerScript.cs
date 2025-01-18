using Photon.Pun;
using UnityEngine;

public class EnemySpawnerScript : MonoBehaviour
{

    public GameObject EnemyToSpawn;

    public float timeBetweenSpawns = 2f;
    
    [SerializeField]
    public float queueCount = 0f;
    public float timeAccumulator = 0f;

    private void Awake()
    {
        timeAccumulator = timeBetweenSpawns;
    }
    
    [PunRPC]
    public void AddEnemyToQueue()
    {
        Invoke(nameof(SpawnEnemy), timeAccumulator);
        timeAccumulator += timeBetweenSpawns;
        queueCount++;
    }
    
    [PunRPC]
    public void SpawnEnemy()
    {
        PhotonNetwork.InstantiateRoomObject(EnemyToSpawn.name, transform.position, Quaternion.identity);
        Debug.Log("ENEMY INSTANCIATED CALLED");
        
        queueCount--;
        if (queueCount <= 0)
        {
            timeAccumulator = timeBetweenSpawns;
        }
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Physics.Raycast(transform.position, -transform.up * 100f, out RaycastHit hit);
        Gizmos.DrawLine(transform.position, hit.point);
        Gizmos.DrawWireSphere(hit.point, 0.42f);
    }
}
