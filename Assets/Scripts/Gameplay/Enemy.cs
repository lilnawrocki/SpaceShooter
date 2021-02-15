using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyType enemytype = 0;
    [SerializeField] private int points = 10;
    [SerializeField] private int health = 1;
    [SerializeField] private float flightSpeed = 5f;
    public bool canShoot = false;
    public float shootEverySec = 2f;
    [SerializeField] private GameObject explosionPrefab = null;
    [SerializeField] private GameObject target = null;
    [SerializeField] private Transform followPoint = null;
    [SerializeField] private Transform[] missileSpawnPosition = new Transform[2];
    [SerializeField] private GameObject missilePrefab = null; 

    private bool collided = false;
    private Vector3 neighborPosition;
    private float distanceToTarget = 0f;

    [System.Serializable]
    public enum EnemyType
    {
        Green = 0,
        Blue = 1,
        Red = 2,
        Purple = 3,
        Golden = 4
    }

    private void Start()
    {
        RaycastHit2D[] hit = Physics2D.RaycastAll(transform.position, Vector2.down, 10f);
        
        if (hit.Length > 1)
        {
            
            if (hit[1].collider != null)
            {
                target = hit[1].collider.gameObject;
                if (target != null)
                    distanceToTarget = transform.position.y - target.transform.position.y;
                
            }
        }
        
    }

    private void Update()
    {
        if (canShoot && target == null)
        {
            if (!IsInvoking(nameof(shootMissiles)))
                InvokeRepeating(nameof(shootMissiles), 0.5f, shootEverySec);
        }
        else
        {
            if (IsInvoking(nameof(shootMissiles)))
                CancelInvoke(nameof(shootMissiles));
        }
        moveEnemy();
    }

    private void moveEnemy()
    {
        if (target == null)
        {
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position);

            if (screenPosition.y > GameManager.GM.enemyDistance)
            {
                if (followPoint != null)
                {
                    transform.position = Vector3.MoveTowards(transform.position,
                            followPoint.position, flightSpeed * Time.deltaTime);
                }             
            }
            else
            {
                return;
            }
        }
        else
        {
            if (transform.position.y > target.transform.position.y + distanceToTarget)
            {
                transform.position = Vector3.MoveTowards(transform.position,
                    target.transform.position, flightSpeed * Time.deltaTime);
            }
                     
        }
    }

    private void shootMissiles()
    {
        
        if (missilePrefab != null)
        {
            if (missileSpawnPosition[0] != null)
            {
                Instantiate(missilePrefab, missileSpawnPosition[0].position, Quaternion.identity);
            }

            if (missileSpawnPosition[1] != null)
            {
                Instantiate(missilePrefab, missileSpawnPosition[1].position, Quaternion.identity);
            }
        }
            
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Prevents from double collision
        if (collided)
        {
            collided = false;
            return;
        }

        if (!collided)
        {
            collided = true;

            if (collision.CompareTag("Missile"))
            {
                health--;
                if (health == 0)
                {
                    if (explosionPrefab != null)
                    {
                        Instantiate(explosionPrefab, transform.position, Quaternion.identity);                       
                    }
                    
                    if (GameManager.GM != null)
                    {
                        GameManager.GM.score += points;
                        GameManager.GM.UpdateScore();
                    }
                    Destroy(gameObject);
                }
                
                    
            }

                   
        }
        
    }

    private void OnDestroy()
    {
        if (EnemyWave.EW != null)
        {
            if (EnemyWave.EW.enemies.Contains(gameObject))
            {
                EnemyWave.EW.enemies.Remove(gameObject);
                
                EnemyWave.EW.enemyCount--;

                if (EnemyWave.EW.enemyCount == 0)
                {
                    if (GameManager.GM != null)
                    {
                        GameManager.GM.DestroyWave();
                    }
                }
            }
        }

        switch (enemytype)
        {         
            case EnemyType.Green:
                {
                    if (GameManager.GM != null)
                    {
                        GameManager.GM.enemyCounter.GreenCount++;
                    }
                    break;
                }
            case EnemyType.Blue:
                {
                    if (GameManager.GM != null)
                    {
                        GameManager.GM.enemyCounter.BlueCount++;
                    }
                    break;
                }
            case EnemyType.Red:
                {
                    if (GameManager.GM != null)
                    {
                        GameManager.GM.enemyCounter.RedCount++;
                    }
                    break;
                }
            case EnemyType.Purple:
                {
                    if (GameManager.GM != null)
                    {
                        GameManager.GM.enemyCounter.PurpleCount++;
                    }
                    break;
                }
            case EnemyType.Golden:
                {
                    if (GameManager.GM != null)
                    {
                        GameManager.GM.enemyCounter.GoldenCount++;
                    }
                    break;
                }
        }
    }
}
