using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTIRange : MonoBehaviour
{
    Container container;
    void Start()
    {
        container = GetComponentInParent<BehaviourTreeRunner>().container;
    }

    private void OnTriggerEnter(Collider other)
    {
        container.isInrange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        container.isInrange = false;
    }
}
