using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    public GameObject target;
    public Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        if (target == null)
        {
            target = GameObject.Find("Player");
        }
    }

    void LateUpdate()
    {
        transform.position = target.transform.position + offset;
    }
}
