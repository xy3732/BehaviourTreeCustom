using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Container 
{
    public Rigidbody2D rigidbody2D;
    public Transform transform;

    public static Container CreateFromGameObject(GameObject gameObject)
    {
        Container container = new Container();

        container.rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        container.transform = gameObject.GetComponent<Transform>();

        return container;
    }
}
