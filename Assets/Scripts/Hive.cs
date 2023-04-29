using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStorable
{
    void Store(int storeAmount);
}

public class Hive : MonoBehaviour, IStorable
{
    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
    }

    // Update is called once per frame
    void Update() { }

    public void Store(int storeAmount)
    {
        Debug.Log("Storing nectar in hive");
        gameManager.CollectNectar(storeAmount);
    }
}
