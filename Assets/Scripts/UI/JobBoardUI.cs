using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JobBoardUI : MonoBehaviour
{
    [SerializeField]
    private LevelController[] controllers;

    [SerializeField]
    private JobEntryUIElement jobEntryUIElementPrefab;

    [SerializeField]
    private MainMenuUI mainMenuUI;
    [SerializeField]
    private LevelManager levelManager;

    [SerializeField] private RectTransform contentTransform;

    [SerializeField] private Image profileImage;
    [SerializeField] private TMP_Text usernameText;
    
    private JobEntryUIElement[] _jobEntryUIElements;

    //====================================================================================================================//
    
    // Start is called before the first frame update
    private void Start()
    {
        SetupElements();

        LevelManager.LevelCompleted += WrapUpJob;
    }

    //====================================================================================================================//

    private void SetupElements()
    {
        _jobEntryUIElements = new JobEntryUIElement[controllers.Length];
        for (var i = 0; i < controllers.Length; i++)
        {
            var levelController = controllers[i];
            var temp = Instantiate(jobEntryUIElementPrefab, contentTransform);
            temp.Init(i, levelController.name, levelController.description, LoadLevel);

            _jobEntryUIElements[i] = temp;
        }
    }

    private void LoadLevel(int index)
    {
        FadeUI.FadeScreen(() =>
        {
            levelManager.LoadLevelAtIndex(index);
            mainMenuUI.SetWindow(MainMenuUI.WINDOW.NONE);
        }, ()=>
        {
            FindObjectOfType<HandController>().Follow = true;
        }, "En Route!!!");
    }

    private void WrapUpJob(int index)
    {
        FadeUI.FadeScreen(() =>
        {
            levelManager.CleanLevel();
            mainMenuUI.SetWindow(MainMenuUI.WINDOW.JOBS);
            _jobEntryUIElements[index].gameObject.SetActive(false);
        },null,  "Job Finished!");
    }

    public void UpdateUserData()
    {
        profileImage.color = GameSettings.PlayerColor;
        usernameText.text = GameSettings.PlayerName;
    }

    //====================================================================================================================//
    
}
