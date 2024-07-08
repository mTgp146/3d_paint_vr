using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eraser : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Painted"))
        {
            Destroy(other.gameObject);
        }
    }
}
