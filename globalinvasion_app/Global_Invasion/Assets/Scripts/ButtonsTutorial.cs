using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class ButtonsTutorial : MonoBehaviour {
    public StrategistTutorial sm;
    public int id;

    // Use this for initialization
    void Start () {
        sm = StrategistTutorial.FindObjectOfType<StrategistTutorial>();

    }
	
    public void clickEvent()
    {
 
        if (this.id == 5)
        {
            sm.buildBridge(0);
        }
        else
        {
            sm.manageButton(this.id);
        }

    }

    public void tutorialButton()
    {
        SceneManager.LoadScene("Scenes/InstructionScene");
    }

    public void gameButton()
    {
        SceneManager.LoadScene("Scenes/Start");
    }

    // Update is called once per frame
    void Update () {
		
	}
}
