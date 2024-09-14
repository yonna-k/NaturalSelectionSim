using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodCollision : MonoBehaviour //this is attached to food prefab
{
    public static int collision = 0;
    
   
    private void OnTriggerEnter(Collider other) //if collision occurs
    {
        if ((other.tag == "Chicken") || (other.tag == "Speed") || (other.tag == "Energy") || (other.tag == "SE"))
        {
            FoodCollect foodCollect = other.GetComponent<FoodCollect>(); //foodcollected incremented and object destroyed
            if (foodCollect != null)
            {
                foodCollect.FoodCollected();
                Destroy(gameObject);
            }
        }
        
    }

}
