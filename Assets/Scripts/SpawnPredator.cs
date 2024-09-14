using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPredator : MonoBehaviour
{
    public GameObject PredatorPrefab;
    public float xPos;
    public float zPos;
    public Vector3 pos;
    List<GameObject> predatorclones = new List<GameObject>();
    float pred_num = 0;
    float new_pred_num = 0;
    public static bool gamePaused;
    int j = 0;
    public static bool ChangeSlider = false;

    public void PredatorSpawner(float populationnumber)
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
            GameObject ChickenClone = Instantiate(PredatorPrefab, pos, Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0))); //spawns in the chicken at the random coordinates generated
            predatorclones.Add(ChickenClone); //adds to list
        }

    }
    public IEnumerator Delay()
    {
        yield return new WaitForSeconds(0.01f);
        ChangeSlider = false;
    }

    void Update()
    {
        CheckActive();
        new_pred_num = PauseResume.PredNumber; //gets value of slider from PauseResume
        gamePaused = PauseResume.GamePaused;
        if (gamePaused == false)
        {
            if (new_pred_num != pred_num) //if the new value differs from the old value...
            {
                if (new_pred_num > pred_num) //to spawn more food...
                {
                    pred_num = new_pred_num - pred_num; //calculates the remainder
                    PredatorSpawner(pred_num); //instantiates the remainder
                    pred_num = new_pred_num; //makes the values the same again
                    
                }
                else if (new_pred_num < pred_num) //to delete existing food...
                {
                    j = 0;
                    float difference = pred_num - new_pred_num; //calculates the difference between the old and new values
                    while ((j < difference) && (predatorclones.Count != 0))
                    {
                        if (predatorclones[j] != null)
                        {
                            UnityEngine.Object.Destroy(predatorclones[j], 0.1f); //destroys food prefabs

                        }
                        CheckActive();
                        j++;
                    }
                    pred_num = new_pred_num; //makes values the same again
                    pred_num = GameObject.FindGameObjectsWithTag("Predator").Length;
                    j = 0;
                }
                pred_num = new_pred_num;

            }
            int k = 0;
            while (k < predatorclones.Count) //checks if null objects in list
            {
                if (predatorclones[k] == null)
                {
                    predatorclones.RemoveAt(k);
                    ChangeSlider = true;
                }
                k++;
            }
            k = 0;
            StartCoroutine(Delay());
        }
    }

    void CheckActive()
    {
        int k = 0;
        while (k < predatorclones.Count)
        {
            if (predatorclones[k] == null)
            {
                predatorclones.RemoveAt(k);
                ChangeSlider = true;
            }
            k++;
        }
        k = 0;
    }
}


