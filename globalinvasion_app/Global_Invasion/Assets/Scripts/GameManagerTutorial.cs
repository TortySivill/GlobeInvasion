using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerTutorial : MonoBehaviour
{

    public void LoadInstructionScene()
    {
        SceneManager.LoadScene("InstructionScene");
    }

    public void LoadCommanderTutorial()
    {
        SceneManager.LoadScene("CommanderTutorial");
    }

    public void LoadStrategistTutorial()
    {
        SceneManager.LoadScene("StrategistTutorial");
    }
    
    public void LoadSkipScene()
    {
        SceneManager.LoadScene("SkipScene");

    }

    public void LoadHome()
    {
        SceneManager.LoadScene("TutorialStart");
    }




}
