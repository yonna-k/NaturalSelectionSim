using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePredator : MonoBehaviour
{
    private Animator anim;
    private Rigidbody PredatorRigidbody;
    private bool hasarrived = false;
    float randX;
    float randZ;
    private float movementduration = 2.0f;
    private float waitTime;
    private int energypredator = 15; //30 for chickens (15)
    public int FoodCollected = 0;
    public static int ToggleOn = 0;
    public static bool FastForward = false;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        this.PredatorRigidbody = gameObject.GetComponent<Rigidbody>();
        FastForward = PauseResume.FastForward;
        if (FastForward == true) //if fast forward toggle is on
        {
            movementduration = movementduration * 0.5f;
        }
    }

    private IEnumerator MoveToPoint(Vector3 targetPos)
    {
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
    }
    public IEnumerator Wait()
    {
        yield return new WaitForSeconds(1.0f); 
        this.anim.SetTrigger("GoWalk");
    }

    private IEnumerator Waiting()
    {
        yield return new WaitForSeconds(0.001f);
    }

    public void ChangeSpeed() //checks fast forward toggle (has 3 states - 0 = off, 1 = on, 2 = off (after it was on))
    {
        ToggleOn = PauseResume.ToggleOn;
        if (ToggleOn == 1) //when on
        {
            if (movementduration > 1.0f) //checks if speed mutation applies - if yes then movement duration will be lower than 1
            {
                movementduration = movementduration * 0.5f;  //increases the speed
                ToggleOn = PauseResume.ToggleOn;
            }
        }
        else if (ToggleOn == 2) //when off
        {
            if (movementduration < 2.0f)
            {
                movementduration = movementduration * 2.0f; //increases speed again
                ToggleOn = PauseResume.ToggleOn;
            }
            
        }
        ToggleOn = PauseResume.ToggleOn;
    }

    // Update is called once per frame
    void Update()
    {
        HasArrived();
        ToggleOn = PauseResume.ToggleOn;
        if (ToggleOn != 0)
        {
            ChangeSpeed();
        }
    }

    private void HasArrived()
    {
        if (!hasarrived)
        {

            hasarrived = true; //checks every frame whether the chicken has arrived         
            randX = Random.Range(-140.0f, 140.0f); //coordinates generated
            randZ = Random.Range(-140.0f, 140.0f); //140 w/o red area
            while (Vector3.Distance(transform.position, new Vector3(randX, 0.0f, randZ)) >= 50.0f || (Vector3.Distance(transform.position, new Vector3(randX, 0.0f, randZ)) <= 10.0f)) //makes sure the distance is within 10-50
            {
                randX = Random.Range(-140.0f, 140.0f); //if not, new coordinates are generated
                randZ = Random.Range(-140.0f, 140.0f);
            }
            anim.SetTrigger("GoWalk");
            StartCoroutine(MoveToPoint(new Vector3(randX, 0.0f, randZ))); //with the new coordinates
            this.energypredator = this.energypredator - 1;
            if (this.energypredator <= 0)
            {
                Debug.Log("run out of energy");
                Destroy(this.gameObject);
                StartCoroutine(Waiting());

            }
        }
    }
    private void OnTriggerEnter(Collider other) //if it collides
    {
        if ((other.tag == "Chicken") || (other.tag == "Speed") || (other.tag == "Energy") ||(other.tag == "SE"))
        {
            this.anim.SetTrigger("GoEat");
            this.FoodCollected++;
            energypredator = energypredator + 5; //adds 5 to energy every time a chicken is eaten
            StartCoroutine(Wait());
        }

    }
}
