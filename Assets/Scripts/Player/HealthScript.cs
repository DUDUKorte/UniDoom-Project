using Photon.Pun;
using UnityEngine;

public class HealthScript : MonoBehaviour
{
    private float health;

    private void Awake()
    {
        health = GameManagerScript.instance.enemyHealth;
    }
    
    [PunRPC]
    public void TakeDamage(int damage)
    {
        health -= damage;
        
        if (health <= 0)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                GameManagerScript.instance.totalEnemiesKilled++;
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }
}
