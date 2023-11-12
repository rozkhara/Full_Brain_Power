using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FillerBehaviour : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        GameManager.Instance.PlaneController.collisionHappened = true;
    }
}
