using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourTreeRunner : MonoBehaviour
{
    public BehaviourTree tree;

    void Start()
    {
        // 실행되면 트리 복사 해서 사용.
        tree = tree.Clone();
        tree.Bind();
    }

    void Update()
    {
        tree.Update();
    }
}
