using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWave : MonoBehaviour
{
    public static EnemyWave EW;
    public List<GameObject> enemies = new List<GameObject>();
    public int enemyCount = 0;

    private void Awake()
    {
        EW = GetComponent<EnemyWave>();

        enemyCount = transform.childCount;
        if (enemyCount > 0)
        {
            for (int i = 0; i < enemyCount; i++)
            {
                enemies.Add(transform.GetChild(i).gameObject);
            }
        }

        
    }

    private void Start()
    {
        if (GameManager.GM != null)
        {
            GameManager.GM.currentWave = gameObject;
            GameManager.GM.waveNumber++;
        }
    }

    private void OnDestroy()
    {
        if (GameManager.GM != null)
        {
            if (GameManager.GM.waveNumber == GameManager.GM.numberOfWaves)
            {
                GameManager.GM.Victory();
                
            }
        }
    }
}
