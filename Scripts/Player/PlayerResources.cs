using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerResources : MonoBehaviour
{
    [Header("Resources")]
    public int carrotAmount;

    // Start is called before the first frame update
    void Start()
    {
        carrotAmount = 0;
    }

    public void AddCarrots(int amount)
    {
        carrotAmount += amount;
    }

    public void RemoveCarrots(int amount)
    {
        carrotAmount -= amount;
    }

    public int GetCarrotAmount()
    {
        return carrotAmount;
    }
}
