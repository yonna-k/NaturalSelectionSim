using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PauseResume : MonoBehaviour
{
    public GameObject PauseScreen;
    public GameObject PauseButton;
    public GameObject SettingsButton;
    public GameObject SettingsScreen;
    public GameObject GraphScreen;
    public Toggle MutationToggle;
    public GameObject SpeedMutation; //inputfield
    public GameObject EnergyMutation;//inputfield
    public Slider slider;
    public Slider predatorslider;
    public float amountFood;
    public static float PredNumber;
    public static bool GamePaused;
    public static bool reset;
    public static int gennumber = 1;
    public int predatornumber;
    public static bool sliderchange = false;
    public static float mutationRate = 0;
    public static float energymutationRate = 0;
    private int num_speedmutation = 0;
    private int num_energymutation = 0;
    private int num_bothmutation = 0;

    [SerializeField]
    private TextMeshProUGUI textgeneration;

    [SerializeField]
    private TextMeshProUGUI text; //text to show value of slider

    [SerializeField]
    private TextMeshProUGUI predatortext; //text to show value of slider

    [SerializeField]
    private TextMeshProUGUI inputfielddisplay_text;

    [SerializeField]
    private TMP_InputField input_field;

    [SerializeField]
    private TextMeshProUGUI inputfielddisplay2_text;

    [SerializeField]
    private TMP_InputField input_field2;

    [SerializeField]
    private TextMeshProUGUI speed_num;

    [SerializeField]
    private TextMeshProUGUI energy_num;

    public Toggle SpeedToggle;
    [SerializeField]
    private TextMeshProUGUI speed_text;
    public static int ToggleOn = 0;
    public static bool FastForward = false;

    // Start is called before the first frame update
    void Start()
    {
        PauseScreen.SetActive(false); //initially you can see the buttons but not their corresponsding screens
        PauseButton.SetActive(true);
        SettingsButton.SetActive(true);
        SettingsScreen.SetActive(false);
        GraphScreen.SetActive(false);
        SpeedMutation.SetActive(false);
        EnergyMutation.SetActive(false);
        GamePaused = false; //the game is not paused
        slider.minValue = 10;
        slider.maxValue = 100; //food slider
        predatorslider.minValue = 0;
        predatorslider.maxValue = 10;
    }

    // Update is called once per frame
    void Update()
    {
        if (GamePaused) //controls the pausing of the game
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
        reset = MoveChicken.reset;
        
        if (reset == true)
        {
            reset = false;
            StartCoroutine(Waiting());
        }

       if (SpeedToggle.isOn) //if the fast forward toggle is on
       {
           FastForward = true;
       }
       else
       {
           FastForward = false;
       }

        predatornumber = GameObject.FindGameObjectsWithTag("Predator").Length; //checks amount of predators on the plane
        sliderchange = SpawnPredator.ChangeSlider;
        if (sliderchange == true) //checks against value of slider and changes it accordingly
        {
            predatorslider.value = predatornumber;
            predatortext.text = predatornumber.ToString();
            sliderchange = false;
        }
        sliderchange = SpawnPredator.ChangeSlider;

        num_speedmutation = GameObject.FindGameObjectsWithTag("Speed").Length; //checks how many speed and energy mutations have occured
        num_energymutation = GameObject.FindGameObjectsWithTag("Energy").Length;
        num_bothmutation = GameObject.FindGameObjectsWithTag("SE").Length;
        speed_num.text = (num_speedmutation + num_bothmutation).ToString(); //displays them
        energy_num.text = (num_energymutation + num_bothmutation).ToString();
    }
    public IEnumerator Waiting()
    {
        yield return new WaitForSeconds(0.001f);
        reset = MoveChicken.reset;
        gennumber++;
        textgeneration.text = "GENERATION " + gennumber; //increments the gen number in the simulation

    }

    public void PauseGame() //if the pause button is clicked...
    {
        GamePaused = true;
        PauseScreen.SetActive(true);
        PauseButton.SetActive(false);
        SettingsScreen.SetActive(false);
        SettingsButton.SetActive(true);
        textgeneration.enabled = false;
        GraphScreen.SetActive(false);
    }

    public void ResumeGame() //if the resume button is clicked...
    {
        GamePaused = false;
        PauseScreen.SetActive(false);
        PauseButton.SetActive(true);
        SettingsScreen.SetActive(false);
        SettingsButton.SetActive(true);
        textgeneration.enabled = true;
        GraphScreen.SetActive(false);
    }

    public void SettingsPage() //if the settings button is clicked...
    {
        GamePaused = true;
        PauseScreen.SetActive(false);
        PauseButton.SetActive(true);
        SettingsButton.SetActive(false);
        SettingsScreen.SetActive(true);
        textgeneration.enabled = false;
        GraphScreen.SetActive(false);
    }

    public void MainScreen() //if the back button is clicked (on settings page)...
    {
        GamePaused = false;
        PauseScreen.SetActive(false);
        PauseButton.SetActive(true);
        SettingsScreen.SetActive(false);
        SettingsButton.SetActive(true);
        textgeneration.enabled = true;
        GraphScreen.SetActive(false);
    }

    public void GraphPage() //if graph button is pressed
    {
        GamePaused = true;
        PauseScreen.SetActive(false);
        PauseButton.SetActive(true);
        SettingsButton.SetActive(false);
        SettingsScreen.SetActive(false);
        GraphScreen.SetActive(true);
        textgeneration.enabled = false;
    }

    public void ChangeToggle() //if mutation toggle is pressed
    {

        if (MutationToggle.isOn) //displays input fields
        {
            SpeedMutation.SetActive(true);
            EnergyMutation.SetActive(true);
        }
        else
        {
            SpeedMutation.SetActive(false);
            EnergyMutation.SetActive(false);
        }
        
    }

    public IEnumerator ChangeSpeed() //affects the toggle (0 = off, 1 = on, 2 = off after it has been on)
    {
        if (SpeedToggle.isOn)
        {
            ToggleOn = 1;
            speed_text.text = "x2";
            yield return new WaitForSeconds(0.0001f);
            ToggleOn = 0;
        }
        else if (!SpeedToggle.isOn)
        {
            ToggleOn = 2;
            speed_text.text = "x1";
            yield return new WaitForSeconds(0.0001f);
            ToggleOn = 0;
        }
        ToggleOn = 0;
    }

    public void ChangeSpeed1()
    {
        StartCoroutine(ChangeSpeed());
    }

    public void GetMutationRate() //gets input of speed mutation
    {
        inputfielddisplay_text.text = "0-100%";
        Debug.Log(input_field.text);
        try
        {
            int temp = int.Parse(input_field.text);
            if ((temp < 0) || (temp > 100))
            {
                inputfielddisplay_text.text = "try again!"; //error handling when putting in mutation rates
                input_field.text = "";
            }
            else if ((temp >= 0) && (temp <= 100))
            {
                inputfielddisplay_text.text = "accepted - change will start in next gen";
                mutationRate = temp;
            }
        }
        catch
        {
            inputfielddisplay_text.text = "whole numbers only!";
            input_field.text = "";
        }
       
    }

    public void GetEnergyMutationRate() //gets input of energy mutation
    {
        inputfielddisplay2_text.text = "0-100%";
        Debug.Log(input_field2.text);
        try
        {
            int temp = int.Parse(input_field2.text);
            if ((temp < 0) || (temp > 100))
            {
                inputfielddisplay2_text.text = "try again!"; //error handling when putting in mutation rates
                input_field2.text = "";
            }
            else if ((temp >= 0) && (temp <= 100))
            {
                inputfielddisplay2_text.text = "accepted - change will start in next gen";
                energymutationRate = temp;
            }
        }
        catch
        {
            inputfielddisplay2_text.text = "whole numbers only!";
            input_field2.text = "";
        }
        
    }

    public void ChangeFood(float newFood) //for the slider : changes the amount of food
    {
        amountFood = newFood;
        text.text = newFood.ToString();
    }

    public void Predator(float PredatorNumber) //for the slider : changes the amount of predators
    {
        PredNumber = PredatorNumber;
        predatortext.text = PredNumber.ToString();
    }

   
}
