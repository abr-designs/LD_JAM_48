using System;
using Unity.Mathematics;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static Action<int> LevelCompleted;
    
    [SerializeField]
    private LevelController[] levelPrefabs;

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
        if (_currentLevelController != null)
            Destroy(_currentLevelController.gameObject);
        
        LevelCompleted?.Invoke(_currentLevelIndex);
    }

    public void LoadLevelAtIndex(in int index)
    {
        if (_currentLevelController != null)
            Destroy(_currentLevelController.gameObject);

        _currentLevelIndex = index;
        _currentLevelController = Instantiate(levelPrefabs[index],Vector3.zero, quaternion.identity);
    }
    
    private void ResetLevel()
    {
        LoadLevelAtIndex(_currentLevelIndex);
    }

    //====================================================================================================================//
    

}
