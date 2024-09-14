using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnFood : MonoBehaviour
{
    public GameObject FoodPrefab; //sets the apple as the prefab (thing that will be cloned)
    public float xPos;
    public float zPos;
    public Vector3 pos;
    public float timer = 5.0f; //timer to space out random spawning
    public float amountFood = 10; 
    int i = 0;
    int j = 0;
    List<GameObject> foodclones = new List<GameObject>(); //list to store the food prefabs
    private GameObject newGo;
    private float newFood;
    private Vector3 prevPos = new Vector3(0, 0, 0);
    public static bool gamePaused;
    public static bool reset = false;

    IEnumerator Delay(float amountFood)
    {
        if (reset == true)
        {
            reset = MoveChicken.reset;
        }
        
        while (i < amountFood)
        {
            xPos = Random.Range(-140.0f, 140.0f); //sets random coordinates for the food to spawn at (y coordinate will always be 0)
            zPos = Random.Range(-140.0f, 140.0f);
            pos = new Vector3(xPos, 0.0f, zPos);
            
            while (Vector3.Distance(prevPos, pos) <= 5.0f) //makes sure space between food is more than 5
            {
                xPos = Random.Range(-140.0f, 140.0f);
                zPos = Random.Range(-140.0f, 140.0f);
                pos = new Vector3(xPos, 0.0f, zPos);
            }
            yield return new WaitForSeconds(0.001f); //adds a time delay - 0.01f
            newGo = Instantiate(FoodPrefab, pos, Quaternion.identity); //creates new food in the variable newGo
            foodclones.Add(newGo); //this is then added to the list
            prevPos = pos;
            i++;
        }
        int k = 0;
        while (k < foodclones.Count)
        {
            if (foodclones[k] == null)
            {
                foodclones.RemoveAt(k);
            }
            k++;
        }
        k = 0;
    }

    void Start()
    {
        StartCoroutine(Delay(amountFood));
    }

    void Update()
    {
        reset = MoveChicken.reset;
        newFood = GameObject.Find("Canvas").GetComponent<PauseResume>().amountFood; //gets value of slider from PauseResume
        gamePaused = PauseResume.GamePaused;
        if (gamePaused == false)
        {
            if (newFood != amountFood) //if the new value differs from the old value...
            {
                if (newFood > amountFood) //to spawn more food...
                {
                    i = 0;
                    amountFood = newFood - amountFood; //calculates the remainder
                    StartCoroutine(Delay(amountFood)); //instantiates the remainder
                    amountFood = newFood; //makes the values the same again
                    i = 0;
                }
                if (newFood < amountFood) //to delete existing food...
                {
                    j = 0;
                    float difference = amountFood - newFood; //calculates the difference between the old and new values
                    while (j < difference)
                    {
                        UnityEngine.Object.Destroy(foodclones[j], 1.0f); //destroys food prefabs
                        foodclones.RemoveAt(j); //removes from the list
                        Debug.Log(foodclones.Count);
                        j++;
                    }
                    amountFood = newFood; //makes values the same again
                    j = 0;
                }
                amountFood = newFood;
            }
            if (reset == true)
            {
                float resetfood = foodclones.Count - GameObject.FindGameObjectsWithTag("Food").Length;
                i = 0;
                StartCoroutine(Delay(resetfood));
                
            }
        } 
    }  
    }
    

  
