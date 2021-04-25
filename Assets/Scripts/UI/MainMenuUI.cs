using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MainMenuUI : MonoBehaviour
{
    public enum WINDOW
    {
        NONE,
        MAIN,
        JOBS
    }
    //Properties
    //====================================================================================================================//

    [SerializeField] 
    private JobBoardUI jobBoardUI;
    
    [SerializeField]
    private Image cursor;
    [SerializeField]
    private Sprite openHandSprite, closedHandSprite;
    [SerializeField]
    private Image handProfileImage;
    
    [FormerlySerializedAs("InputField")] [SerializeField, Header("User Info")]
    private TMP_InputField inputField;
    [SerializeField]
    private Slider hueSlider;
    [SerializeField]
    private Slider saturationSlider;
    [SerializeField]
    private Slider valueSlider;

    [SerializeField, Header("User Settings")]
    private Slider volumeSlider;

    [SerializeField]
    private Button clickMovementButton;
    [SerializeField]
    private Button autoMovementButton;

    [SerializeField]
    private Button startButton;
    
    private new RectTransform transform;

    [SerializeField, Header("Windows")] 
    private GameObject canvasObject;
    [SerializeField]
    private GameObject menuObject;
    [SerializeField]
    private GameObject jobsObject;

    //====================================================================================================================//

    private void OnEnable()
    {
        cursor.sprite = openHandSprite;
    }

    // Start is called before the first frame update
    private void Start()
    {
        transform = gameObject.transform as RectTransform;
        
        SetupUI();

        SetWindow(WINDOW.MAIN);
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            cursor.sprite = closedHandSprite;
            AudioController.PlaySound(AudioController.SFX.HAND);
        }
        if (Input.GetKeyUp(KeyCode.Mouse0))
            cursor.sprite = openHandSprite;
        
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform, Input.mousePosition, null, out var localPoint);
        cursor.transform.localPosition = localPoint;
    }

    //====================================================================================================================//

    private void SetupUI()
    {
        //--------------------------------------------------------------------------------------------------------//
        
        hueSlider.value = Random.Range(0f, 1f);
        saturationSlider.value = Random.Range(0f, 1f);
        valueSlider.value = 1f;
        
        hueSlider.onValueChanged.AddListener(UpdateColors);
        saturationSlider.onValueChanged.AddListener(UpdateColors);
        valueSlider.onValueChanged.AddListener(UpdateColors);
        UpdateColors(0f);

        //--------------------------------------------------------------------------------------------------------//

        inputField.onValueChanged.AddListener(UpdateName);

        //--------------------------------------------------------------------------------------------------------//
        
        volumeSlider.onValueChanged.AddListener(UpdateVolume);
        volumeSlider.value = 0.65f;

        //--------------------------------------------------------------------------------------------------------//
        clickMovementButton.onClick.AddListener(() =>
        {
            UpdateMoveType(HandController.MOVE_TYPE.CLICK);
        });
        autoMovementButton.onClick.AddListener(() =>
        {
            UpdateMoveType(HandController.MOVE_TYPE.AUTO_MOVE);
        });

        UpdateMoveType(GameSettings.MoveType);

        //--------------------------------------------------------------------------------------------------------//
        
        startButton.onClick.AddListener(() =>
        {
            SetWindow(WINDOW.JOBS);
        });

        //--------------------------------------------------------------------------------------------------------//
        
    }

    //====================================================================================================================//
    
    private void UpdateColors(float _)
    {
        GameSettings.PlayerColor = Color.HSVToRGB(hueSlider.value, saturationSlider.value, valueSlider.value);

        cursor.color = GameSettings.PlayerColor;
        handProfileImage.color = GameSettings.PlayerColor;
    }
    
    private static void UpdateName(string playerName) => GameSettings.PlayerName = playerName;

    private static void UpdateVolume(float volume)
    {
        GameSettings.Volume = volume;
        AudioController.Volume(volume);
    }

    private void UpdateMoveType(HandController.MOVE_TYPE moveType)
    {
        switch (moveType)
        {
            case HandController.MOVE_TYPE.CLICK:
                clickMovementButton.interactable = false;
                autoMovementButton.interactable = true;
                break;
            case HandController.MOVE_TYPE.AUTO_MOVE:
                clickMovementButton.interactable = true;
                autoMovementButton.interactable = false;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(moveType), moveType, null);
        }

        GameSettings.MoveType = moveType;
    }

    //====================================================================================================================//

    public void SetWindow(in WINDOW window)
    {
        switch (window)
        {
            case WINDOW.NONE:
                canvasObject.SetActive(false);
                menuObject.SetActive(false);
                jobsObject.SetActive(false);
                break;
            case WINDOW.MAIN:
                canvasObject.SetActive(true);
                menuObject.SetActive(true);
                jobsObject.SetActive(false);
                break;
            case WINDOW.JOBS:
                canvasObject.SetActive(true);
                menuObject.SetActive(false);
                jobsObject.SetActive(true);
                
                jobBoardUI.UpdateUserData();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(window), window, null);
        }
    }

    //====================================================================================================================//
    
}
