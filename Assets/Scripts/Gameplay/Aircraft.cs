using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aircraft : MonoBehaviour
{
    [SerializeField] private int health = 10;
    [SerializeField] private Transform[] missileSpawnPos = new Transform[2];
    [SerializeField] private GameObject missilePrefab = null;
    [SerializeField] private GameObject explosionPrefab = null;

    private bool collided = false;

    public int GetHealth()
    {
        return health;
    }

    public void FireMissile()
    {
        if (GameManager.GM != null)
        {
            if (GameManager.GM.numberOfMissiles > 0)
            {
                if (missilePrefab != null)
                {
                    if (missileSpawnPos.Length >= 2)
                    {
                        if (missileSpawnPos[0] != null)
                        {
                            Instantiate(missilePrefab, missileSpawnPos[0].position, Quaternion.identity);
                        }

                        if (missileSpawnPos[1] != null)
                        {
                            Instantiate(missilePrefab, missileSpawnPos[1].position, Quaternion.identity);
                        }
                    }
                    
                }

                GameManager.GM.UpdateMissiles();
            }
        }      
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collided)
        {
            collided = false;
            return;
        }

        if (!collided)
        {
            if (collision.CompareTag("Enemy"))
            {
                if (explosionPrefab != null)
                {
                    Instantiate(explosionPrefab, transform.position, Quaternion.identity);
                }
                Destroy(gameObject);
            }

            if (collision.CompareTag("Missile"))
            {
                health--;

                if (GameManager.GM != null)
                {
                    GameManager.GM.UpdateHealth(); 
                }
                    

                if (health == 0)
                {
                    if (explosionPrefab != null)
                    {
                        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
                    }
                        
                    Destroy(gameObject);
                    //GameOver
                }

            }
        }
        
    }

    private void OnDestroy()
    {
        if (GameManager.GM != null)
            GameManager.GM.GameOver();
    }
}
