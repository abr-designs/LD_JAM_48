using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AlertUI : MonoBehaviour
{
    private static AlertUI _instance;
    
    [SerializeField]
    private GameObject alertWindowObject;

    [SerializeField]
    private TMP_Text titleText;

    [SerializeField]
    private TMP_Text descriptionText;

    [SerializeField]
    private Button closeButton;
    [SerializeField]
    private TMP_Text buttonText;


    //====================================================================================================================//

    private void Awake()
    {
        _instance = this;
    }

    // Start is called before the first frame update
    private void Start()
    {
        closeButton.onClick.AddListener(() =>
        {
            SetAlertActive(false);
        });
        
        SetAlertActive(false);
    }

    //====================================================================================================================//

    public static void ShowAlert(in string subject, in string body, in string buttonText = "close")
    {
        _instance?.SetupAlert(subject, body, buttonText);
        _instance?.SetAlertActive(true);
    }

    private void SetAlertActive(in bool state)
    {
        alertWindowObject.SetActive(state);
    }

    private void SetupAlert(in string subject, in string body, in string buttonText = "close")
    {
        titleText.text = $"RE: {subject}";

        descriptionText.text = body;

        this.buttonText.text = buttonText;

    }

}
