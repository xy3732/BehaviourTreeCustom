using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Container 
{
    [HideInInspector] public Rigidbody2D rigidbody2D;
    [HideInInspector] public Transform transform;

    public static Container CreateFromGameObject(GameObject gameObject)
    {
        Container container = new Container();

        container.rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        container.transform = gameObject.GetComponent<Transform>();

        return container;
    }
}
