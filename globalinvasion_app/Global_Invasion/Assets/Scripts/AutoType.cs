using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class AutoType : MonoBehaviour
{
    public float letterPause = 0.0000001f;

    Text instructions;

    string message;

    // Use this for initialization
    void Start()
    {

        instructions = GetComponent<Text>();
        message = instructions.text;
        instructions.text = "";
        StartCoroutine(TypeText());
    }

    IEnumerator TypeText()
    {
        foreach (char letter in message.ToCharArray())
        {
            instructions.text += letter;

            yield return new WaitForSeconds(letterPause);
        }
        yield return new WaitForSeconds(5.0f);
        SceneManager.LoadScene("SkipScene");
    }
}