using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ErrorDialog : MonoBehaviour
{

    [SerializeField] GameObject errorDialog;
    [SerializeField] TextMeshProUGUI messageText;

    private static ErrorDialog _instance;

    public static ErrorDialog Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ErrorDialog>();
            }
            return _instance;
        }
    }

    public void ShowMessage(string message)
    {
        errorDialog.SetActive(true);
        messageText.text = message;
    }

    public void Close()
    {
        errorDialog.SetActive(false);   
    }
}
