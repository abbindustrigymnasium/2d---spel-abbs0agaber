using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactScrips : MonoBehaviour
{
    private float Begin;

    void Start()
    {
        Begin = Time.time;
    }

    void Update()
    {
        if (Time.time - Begin > 1)
        {
            Destroy(gameObject);
        }
    }
}
