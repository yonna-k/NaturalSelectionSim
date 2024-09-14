using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodCollect : MonoBehaviour
{
    public int NumOfFood;

    public void FoodCollected()
    {
        NumOfFood++;
    }
}
