using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MoveChicken : MonoBehaviour 
{
    public float movementduration = 2.0f; //the time it takes for the chicken to move to the coordinates generated
    private bool hasarrived = false; //if the chicken has arrived to its new coordinates
    private Animator anim;
    private float waitTime;
    public static int collision;
    public int FoodCollected;
    float randX;
    float randZ;
    private Rigidbody ChickenRigidbody;
    bool stop = false;
    bool redstart = false;
    bool allStopped = false;
    public static bool reproduce = false;
    public static bool done = false;
    private int energychicken = 20; //energy that the chicken has (change in reset too)
    public static float energymutationRate = 0;
    private int amount_of_energy = 20; //store for the amount of energy of the chicken
    public static bool reset = false;
    public static float mutationRate = 0;
    public static bool FastForward = false; //checks if fast forward toggle is on
    public static int ToggleOn = 0;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        this.ChickenRigidbody = gameObject.GetComponent<Rigidbody>();
        mutationRate = PauseResume.mutationRate; //mutations occur as a new chicken is spawned
        if (mutationRate != 0) //speed mutation
        {
            int temp = Random.Range(1, 101); //random number between 1-100
            if (temp <= mutationRate) //if the random number is less than/equal to the mutation rate
            {
                movementduration = 1.0f; //essentially, randomising to see if mutation occurs based on the rate
                this.gameObject.tag = "Speed";
            }
        }
        energymutationRate = PauseResume.energymutationRate; 
        if (energymutationRate != 0) //energy mutation
        {
            int temp = Random.Range(1, 101); //same as the speed mutation
            if (temp <= energymutationRate)
            {
                amount_of_energy = 30;
                this.energychicken = amount_of_energy;
                this.gameObject.tag = "Energy";
            }
        }
        if ((movementduration == 1.0f) && (amount_of_energy == 30)) //checks whether both mutations have been applied
        {
            this.gameObject.tag = "SE";
        }
        FastForward = PauseResume.FastForward;
        if (FastForward == true) //if fast forward toggle is on
        {
            movementduration = movementduration * 0.5f;
        }
    }
   
    public IEnumerator RedStart()
    {
        yield return new WaitForSeconds(15.0f); //amount of time before chickens can go into red area
        redstart = true;
    }

    public void ChangeSpeed()
    {

        ToggleOn = PauseResume.ToggleOn; //checks fast forward toggle (has 3 states - 0 = off, 1 = on, 2 = off (after it was on))
        if (ToggleOn == 1) //when on
        {
            if (this.gameObject.tag != "Speed") //checks if speed mutation applies - if yes then movement duration will be lower than 1
            {
                if (movementduration > 1.0f) //increases the speed
                {
                    movementduration = movementduration * 0.5f;
                    ToggleOn = PauseResume.ToggleOn;
                }
            }
            else
            {
                movementduration = movementduration * 0.5f;
                ToggleOn = PauseResume.ToggleOn;
            }
           

        }
        else if (ToggleOn == 2) //when off
        {
            if (this.gameObject.tag != "Speed")
            {
                if (movementduration < 2.0f) //slows down the speed again
                {
                    movementduration = movementduration * 2.0f;
                    ToggleOn = PauseResume.ToggleOn;
                } 
            }
            else
            {
                movementduration = movementduration * 2.0f;
                ToggleOn = PauseResume.ToggleOn;
            }
                
        }
        ToggleOn = PauseResume.ToggleOn;
    }

    public IEnumerator Wait()
    {
        yield return new WaitForSeconds(1.0f); //check if this time is correct!!
        this.anim.SetTrigger("GoWalk"); //walk animation

    }

    // Update is called once per frame
    void Update()
    {
        if (redstart == false)
        {
            StartCoroutine(RedStart());
        }

        HasArrived();
        AllStopped();
        ToggleOn = PauseResume.ToggleOn;
        if (ToggleOn != 0) //if the fast forward toggle is changed
        {
            ChangeSpeed();
            ToggleOn = PauseResume.ToggleOn;
        }

    }
    private IEnumerator MoveToPoint(Vector3 targetPos)
    {
        CheckRed(); 
        float timer = 0.0f; //timer initialised
        Vector3 startPos = transform.position; //startPos is the initial coordinates
        while (timer < movementduration) //while the timer is less than the movement duration
        {
            timer += Time.deltaTime; //timer incremented by change in time
            float t = timer / movementduration; //timer divided by the movement duration
            t = t * t * t * (t * (6f * t - 15f) + 10f);
            Vector3 directionpos = targetPos - startPos; //to work out position vector for the rotation distance
            transform.position = Vector3.Lerp(startPos, targetPos, t); //makes chicken move to new coordinates
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(directionpos), Time.deltaTime * 5f); //makes chicken rotate in direction of movement
            yield return null;
            

        }
        waitTime = Random.Range(0.0f, 2.0f); //idle for a random amount of time
        anim.SetTrigger("GoIdle");
        yield return new WaitForSeconds(waitTime);
        hasarrived = false;
        this.energychicken = this.energychicken - 1; //decrements energy
        if (this.energychicken <= 0)
        {
            Debug.Log("run out of energy");
            Destroy(this.gameObject); //when energy reaches 0, the chicken is destroyed
            StartCoroutine(Waiting());

        }
    }


    private void HasArrived()
    {

        if (done == true) //when reproduction has finished
        {
            hasarrived = false;
            stop = true;
        }
        
        if (stop == false) //hasn't stopped
        {
            if (!hasarrived) 
            {

                hasarrived = true; //checks every frame whether the chicken has arrived         
                if (redstart == false)
                {
                    //Debug.Log("false");
                    randX = Random.Range(-140.0f, 140.0f); //coordinates generated
                    randZ = Random.Range(-140.0f, 140.0f); //140 w/o red area
                    while (Vector3.Distance(transform.position, new Vector3(randX, 0.0f, randZ)) >= 50.0f || (Vector3.Distance(transform.position, new Vector3(randX, 0.0f, randZ)) <= 10.0f)) //makes sure the distance is within 10-50
                    {
                        randX = Random.Range(-140.0f, 140.0f); //if not, new coordinates are generated
                        randZ = Random.Range(-140.0f, 140.0f);
                    }
                    //anim.speed = 1;
                    anim.SetTrigger("GoWalk");
                    StartCoroutine(MoveToPoint(new Vector3(randX, 0.0f, randZ))); //with the new coordinates
                   
                }
                else if (redstart == true)
                {
                   
                    randX = Random.Range(-170.0f, 170.0f); //coordinates generated
                    randZ = Random.Range(-170.0f, 170.0f); //140 w/o red area
                    while (Vector3.Distance(transform.position, new Vector3(randX, 0.0f, randZ)) >= 50.0f || (Vector3.Distance(transform.position, new Vector3(randX, 0.0f, randZ)) <= 10.0f)) //makes sure the distance is within 10-50
                    {
                        randX = Random.Range(-170.0f, 170.0f); //if not, new coordinates are generated
                        randZ = Random.Range(-170.0f, 170.0f);
                    }
                    
                    anim.SetTrigger("GoWalk");
                    StartCoroutine(MoveToPoint(new Vector3(randX, 0.0f, randZ))); //with the new coordinates
                   
                }
                

            }
        }
    }
    private void OnTriggerEnter(Collider other) //if it collides with food...
    {
        if (other.tag == "Food")
        {
            this.anim.SetTrigger("GoEat"); //eat animation
            this.FoodCollected++; //specific to each chicken
            this.energychicken++;
            StartCoroutine(Wait());
        }
        else if (other.tag == "Predator") //if predator collides with the chicken, it is destroyed
        {
            Destroy(this.gameObject);
            StartCoroutine(Waiting());
        }
        
    }




    private void CheckRed()
    {
        if ((((randZ >= 150) && (randZ <= 170)) || ((randZ <= -150) && (randZ >= -170))) || ((randX >= 150) && (randX <= 170)) || ((randX <= -150) && (randX >= -170))) //coordinates of the red area (150/153)
        {

            this.ChickenRigidbody.velocity = Vector3.zero; //stops
            stop = true;
            this.anim.SetTrigger("GoIdle");
        }
    }



    public void AllStopped() //when all the chickens have stopped, this is called
    {
        allStopped = SpawnChicken.allStopped;
        if (this.gameObject != null)
        {
            if (allStopped == true)
            {
                stop = true;
                hasarrived = true;
                if (this.FoodCollected == 0) //destroyed if no food collected
                {
                    Destroy(this.gameObject);
                    StartCoroutine(Waiting());
                }
                else if (this.FoodCollected >= 2) //reproduces if 2 or more are collected
                {
                    reproduce = true;
                    done = SpawnChicken.done;
                    if (done == false) //done - true when chicken has reproduced
                    {
                        done = SpawnChicken.done;
                    }
                    if (done == true)
                    {
                        reproduce = false;
                    }
                    reset = true; //the reset variable is changed
                    
                }
                reset = true;
                StartCoroutine(Reset());
            }
            
        }

    }

    public IEnumerator Reset()
    {
        if (this.gameObject != null) //resets all the variables
        {
            yield return new WaitForSeconds(0.001f);
            Debug.Log("resetting");
            reset = false;
            allStopped = SpawnChicken.allStopped;
            hasarrived = false;
            stop = false;
            reproduce = false;
            redstart = false;
            done = SpawnChicken.done;
            this.FoodCollected = 0;
            energychicken = amount_of_energy;
            yield return new WaitForSeconds(0.001f); 
            HasArrived();
        }


    }
    private IEnumerator Waiting() //time delay
    {
        yield return new WaitForSeconds(0.001f);
    }
}
    
