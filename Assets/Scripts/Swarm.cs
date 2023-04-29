using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swarm : MonoBehaviour
{
    public GameObject SwarmTarget;

    public float baseMoveSpeed = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        SwarmTarget = GameObject.Find("SwarmTarget");
    }

    // Update is called once per frame
    void Update() { }

    void MoveSwarmTarget(Vector2 direction, int multiplier)
    {
        SwarmTarget.transform.position += new Vector3(direction.x, direction.y, 0);
    }
}
