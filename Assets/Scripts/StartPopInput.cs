using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class StartPopInput : MonoBehaviour
{
    private int userpopulation;

    [SerializeField]
    private TMP_InputField input; //what the user inputs
    
    
    [SerializeField]
    private TextMeshProUGUI text; //the text above the input field

    public void GetInput(string population)
    {
        CheckInput(int.Parse(population)); //changes input string to integer
        StateNameController.population = int.Parse(population);
    }


    public void CheckInput(int population)
    {
        if ((population <= 20) && (population >= 1)) //checking whether the number is valid
        {
            text.text = "Valid response! Loading simulation..."; //changes text
            input.text = ""; //input text cleared
            StartCoroutine(SceneWithLoadDelay(1)); //delays the loading of the scene (linked to IEnumerator)
        }
        else if (population > 20) //data out of range
        {
            text.text = "Please enter a number that is 20 or below!";
            input.text = "";
        }
        else if (population < 1)
        {
            text.text = "Please enter a number above 1!";
        }
        else
        {
            text.text = "Invalid response. Please try again!";
            input.text = "";
        }
        
    }
    IEnumerator SceneWithLoadDelay(int numsecs) //creates a delay when the right input is entered
    {
        yield return new WaitForSeconds(numsecs);
        PlaySim();
    }
    public void PlaySim()
    {
        
        SceneManager.LoadScene("Simulation"); //loads the scene with the name "simulation"
        
    }
}
