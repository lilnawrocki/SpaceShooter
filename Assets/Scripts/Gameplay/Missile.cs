using UnityEngine;

public class Missile : MonoBehaviour
{
    [SerializeField] private Transform followPoint = null;
    [SerializeField] private GameObject explosionPrefab = null;
    [SerializeField] private float flightSpeed = 10f;
    [SerializeField] private float lifeTimeSec = 10f;
    [SerializeField] private bool enemyMissile = false;

    private void Awake()
    {
        Invoke(nameof(DestroyMissile), lifeTimeSec);
        if (GameManager.GM != null && !enemyMissile)
        {
            if (GameManager.GM.numberOfMissiles > 0)
                GameManager.GM.numberOfMissiles--;
        }
    }

    private void Update()
    {
        if (followPoint != null)
        {
            transform.position = Vector3.MoveTowards(transform.position,
                followPoint.position, flightSpeed * Time.deltaTime);
        }
    }

    private void DestroyMissile()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (GameManager.GM != null && !enemyMissile)
        {
            GameManager.GM.numberOfMissiles++;
            GameManager.GM.UpdateMissiles();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            if (explosionPrefab != null)
            {
                Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Missile"))
        {
            if (explosionPrefab != null)
            {
                Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Player"))
        {
            if (explosionPrefab != null)
            {
                Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
        }
    }
}
