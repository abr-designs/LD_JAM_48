using System;
using Unity.Mathematics;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static Action<int> LevelCompleted;
    
    [SerializeField]
    private LevelController[] levelPrefabs;

    [SerializeField] private string finalTitle;
    
    [SerializeField, TextArea]
    private string finalBody;

    private LevelController _currentLevelController;
    private int _currentLevelIndex;

    private new Transform transform;

    //Unity Functions
    //====================================================================================================================//
    
    // Start is called before the first frame update
    private void Start()
    {
        HandController.OnLevelCompleted += LoadMenu;

        transform = gameObject.transform;

        //LoadLevelAtIndex(_currentLevelIndex);
    }
    
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
            ResetLevel();
        
    }

    //====================================================================================================================//

    /*private void LoadNextLevel()
    {
        if (_currentLevelIndex + 1 > levelPrefabs.Length)
            throw new NotImplementedException("Need to add end game functionality");

        LoadLevelAtIndex(_currentLevelIndex + 1);
    }*/

    private void LoadMenu()
    {
        LevelCompleted?.Invoke(_currentLevelIndex);

        GameSettings.LevelsCompleted++;

        if (GameSettings.LevelsCompleted >= levelPrefabs.Length)
        {
            var response = finalBody.Replace("#NAME", GameSettings.PlayerName);
            AlertUI.ShowAlert(finalTitle, response);
        }
        else
        {
            var levelData = levelPrefabs[_currentLevelIndex];
            var response = levelData.response.Replace("#NAME", GameSettings.PlayerName);
            AlertUI.ShowAlert(levelData.name, response);
        }
        
    }

    public void CleanLevel()
    {
        if (_currentLevelController != null)
            Destroy(_currentLevelController.gameObject);
    }

    public void LoadLevelAtIndex(in int index)
    {
        CleanLevel();

        _currentLevelIndex = index;
        _currentLevelController = Instantiate(levelPrefabs[index],Vector3.zero, quaternion.identity);
    }
    
    private void ResetLevel()
    {
        LoadLevelAtIndex(_currentLevelIndex);
    }

    //====================================================================================================================//
    

}
