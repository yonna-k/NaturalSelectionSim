using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SpawnChicken : MonoBehaviour
{
    public GameObject PreyPrefab; //sets the chicken as the prefab (thing that will be cloned)
    public float xPos;
    public float zPos;
    public Vector3 pos;
    List<GameObject> chickenclones = new List<GameObject>(); // list for the chicken prefabs
    public int populationnumber;
    public static bool allStopped = false; //checks if all chickens have stopped
    int counter = 0;
    Vector3 prev_velocity = new Vector3(0, 0, 0);
    Vector3 cur_velocity = new Vector3(0, 0, 0);
    int i = 0;
    int j = 0;
    int k = 0;
    int foodcount = 0;
    public static bool reproduce = false;
    public static bool done = false;
    private Rigidbody PrefabRB; //rigidbody component of chicken clone
    private Animator chickenanim; //animation component of chicken clone
    int foodcollected = 0; //amount of food collected
    public static bool reset = false;
    

    private void Start()
    {
        populationnumber = (StateNameController.population); //from the start screen, population is passed
        RandomFood(populationnumber);
        StartCoroutine(Velocity()); //checks if chickens have stopped
    }
    public IEnumerator Delay()
    {
        yield return new WaitForSeconds(2.1f); 
        StartCoroutine(Velocity());
    }
    
    void Update()
    {
       
        reproduce = MoveChicken.reproduce; //checks if there are any chickens reproducing
        if (reproduce == true) //if all reproduction has finished
        {
            k = 0;
            while (k < chickenclones.Count)
            {
                if (chickenclones[k] != null)
                {
                    foodcollected = chickenclones[k].GetComponent<MoveChicken>().FoodCollected; //for every chicken - checks food collected
                    if (foodcollected >= 2) 
                    {
                        foodcount++;
                    }
                }
                k++;

            }
            Reproduce(foodcount); //spawns chickens in the next generation (born)
            reproduce = false;
            j = 0;
            k = 0;
            foodcount = 0;
        }
        reset = MoveChicken.reset;
        if (reset == true)
        {
            allStopped = false;
            done = false;
            reset = MoveChicken.reset;
            StartCoroutine(Delay());
            
        }
        CheckActive();
        CheckEnding();
    }

    void RandomFood(int populationnumber) //spawns chickens with initial population
    {

        for (int i = 0; i < populationnumber; i++)
        {
            xPos = Random.Range(-140.0f, 140.0f); //sets random coordinates for the chicken to spawn at (y coordinate will always be 0)
            zPos = Random.Range(-140.0f, 140.0f);
            pos = new Vector3(xPos, 0.0f, zPos);
            if (Vector3.Distance(transform.position, pos) <= 3.0f) //makes sure space between chickens is more than 0.5
            {
                xPos = Random.Range(-140.0f, 140.0f);
                zPos = Random.Range(-140.0f, 140.0f);
                pos = new Vector3(xPos, 0.0f, zPos);
            }
            GameObject ChickenClone = Instantiate(PreyPrefab, pos, Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0))); //spawns in the chicken at the random coordinates generated
            chickenclones.Add(ChickenClone); //adds to list
        }



    }
    public IEnumerator Velocity()
    {
        while (allStopped == false) //while all chickens have not stopped
        {
            i = 0;
            counter = 0;
            while (i < chickenclones.Count) //for all chickens, checks their position now and after some time - if it is the same it has stopped
            {
                if (chickenclones[i] != null)
                {
                    bool passed = false;
                    prev_velocity = chickenclones[i].transform.position;
                    do
                    {
                        prev_velocity = chickenclones[i].transform.position;
                        yield return new WaitForSeconds(2.1f); //2.1f - max time chicken is idle is 2, so 2.1 to avoid false positive
                        try
                        {
                            cur_velocity = chickenclones[i].transform.position;
                            if ((Vector3.Distance(cur_velocity, prev_velocity) >= 0.00) && (Vector3.Distance(cur_velocity, prev_velocity) < 0.1f)) //bit risky? - 0.01 instead??
                            {
                                counter++; //counter keeps track of the number of chickens that have stopped
                                passed = true;
                            }
                        }
                        catch
                        {
                            
                            CheckActive();
                            i = 0;
                            counter = 0;
                            allStopped = false;

                        }

                    }
                    while (passed == false);
                }
                i++;
                CheckActive();
                
            }
            if (counter == chickenclones.Count) //if the number of chickens stopped is the same as the total number of chickens (i.e. when all have stopped)
            {
                allStopped = true;
                Debug.Log("all have stopped");
            }
            else
            {
                counter = 0;
                allStopped = false; //sends this to move chicken script
                i = 0;
            }
            
        }
        

    }

    void Reproduce(int spawn) //reproduction of next generation chickens (if food >= 2)
    {
        
        while (j < spawn)
        {
            int random = Random.Range(0, 4); //random number 0 - 3 (determines where the chicken spawns)
            if (random == 0)
            {
                float randomz = Random.Range(150.0f, 170.0f);
                float randomx = Random.Range(-170.0f, 170.0f);
                Vector3 position = new Vector3(randomx, 0.0f, randomz);
                GameObject ChickenClone = Instantiate(PreyPrefab, position, Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0))); //spawns in the chicken at the random coordinates generated
                chickenclones.Add(ChickenClone); //adds to list
                PrefabRB = ChickenClone.GetComponent<Rigidbody>();
                chickenanim = ChickenClone.GetComponent<Animator>();
                PrefabRB.velocity = Vector3.zero;
                chickenanim.SetTrigger("GoIdle");
                j++;
            }
            else if (random == 1)
            {
                float randomz = Random.Range(-150.0f, -170.0f);
                float randomx = Random.Range(-170.0f, 170.0f);
                Vector3 position = new Vector3(randomx, 0.0f, randomz);
                GameObject ChickenClone = Instantiate(PreyPrefab, position, Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0))); //spawns in the chicken at the random coordinates generated
                chickenclones.Add(ChickenClone); //adds to list
                PrefabRB = ChickenClone.GetComponent<Rigidbody>();
                chickenanim = ChickenClone.GetComponent<Animator>();
                PrefabRB.velocity = Vector3.zero;
                chickenanim.SetTrigger("GoIdle");
                j++;
            }
            else if (random == 2)
            {
                float randomz = Random.Range(-170.0f, 170.0f);
                float randomx = Random.Range(-150.0f, -170.0f);
                Vector3 position = new Vector3(randomx, 0.0f, randomz);
                GameObject ChickenClone = Instantiate(PreyPrefab, position, Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0))); //spawns in the chicken at the random coordinates generated
                chickenclones.Add(ChickenClone); //adds to list
                PrefabRB = ChickenClone.GetComponent<Rigidbody>();
                chickenanim = ChickenClone.GetComponent<Animator>();
                PrefabRB.velocity = Vector3.zero;
                chickenanim.SetTrigger("GoIdle");
                j++;
            }
            else if (random == 3)
            {
                float randomz = Random.Range(-170.0f, 170.0f);
                float randomx = Random.Range(150.0f, 170.0f);
                Vector3 position = new Vector3(randomx, 0.0f, randomz);
                GameObject ChickenClone = Instantiate(PreyPrefab, position, Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0))); //spawns in the chicken at the random coordinates generated
                chickenclones.Add(ChickenClone); //adds to list
                PrefabRB = ChickenClone.GetComponent<Rigidbody>();
                chickenanim = ChickenClone.GetComponent<Animator>();
                PrefabRB.velocity = Vector3.zero;
                chickenanim.SetTrigger("GoIdle");
                j++;
            }
            done = true; //indicates that reproduction has finished
            
        }
        
        
    }

    void CheckActive() //checks if any chickens that have been destroyed are still null in the list
    {
        int a = 0;
        while (a < chickenclones.Count)
        {
            if (chickenclones[a] == null)
            {
                chickenclones.RemoveAt(a);
            }
            a++;
        }

    }

    public void CheckEnding() //if all chickens have died
    {
        if (chickenclones.Count == 0)
        {
            SceneManager.LoadScene("EndScreen");
        }
    }
}



