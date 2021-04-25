using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class JobEntryUIElement : MonoBehaviour
{
    public int Index { get; private set; }

    [SerializeField]
    private Image profileImage;
    [SerializeField]
    private TMP_Text jobTitleText;
    [SerializeField]
    private TMP_Text descriptionText;
    [SerializeField]
    private Button yesButton;

    //====================================================================================================================//

    public void Init(in int index, in string title, in string description, Action<int> buttonPressedCallback)
    {
        Index = index;
        
        profileImage.color = Color.HSVToRGB(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0.7f, 1f));

        jobTitleText.text = title;
        descriptionText.text = description;
        
        yesButton.onClick.AddListener(() =>
        {
            //TODO Need to load the scene with the selected index
            buttonPressedCallback?.Invoke(Index);
        });
    }
    
}
