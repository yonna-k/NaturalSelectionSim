using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StateNameController : MonoBehaviour //for the static variables needed between scenes
{
    public static int population; //static variable allows for access in different scripts (add this somewhere and this script can be deleted)
    
    public void Restart()
    {
        SceneManager.LoadScene("Menu");
    }
}
