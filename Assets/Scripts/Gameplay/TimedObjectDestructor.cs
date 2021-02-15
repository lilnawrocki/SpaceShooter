using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedObjectDestructor : MonoBehaviour
{
    [SerializeField] private float lifeTimeSec = 0.5f;
    // Start is called before the first frame update

    private void Awake()
    {
        Invoke(nameof(DestroyObject), lifeTimeSec);
    }

    private void DestroyObject()
    {
        Destroy(gameObject);
    }
}
