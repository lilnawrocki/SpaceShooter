using UnityEngine;
using UnityEngine.UI;

public class LaserBeam : MonoBehaviour
{
    //[SerializeField] private Image laserSprite = null;
    [SerializeField] private float laserLength = 10f;
    [SerializeField] private float laserSpeed = 5f;
    [SerializeField] private float lifeTime = 5f;

    private Vector3 laserScale = new Vector3 (1f, 0.1f, 1f);

    private void Awake()
    {
        Invoke(nameof(DestroyLaser), lifeTime);
    }

    private void Update()
    {
        if (laserScale.y < laserLength)
            laserScale.y += laserSpeed * Time.deltaTime;
        
        transform.localScale = laserScale;
    }

    private void DestroyLaser()
    {
        Destroy(gameObject);
    }
}
