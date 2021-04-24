using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private CinemachineVirtualCamera startingCamera;

    private CinemachineVirtualCamera _currentCamera;

    private void Start()
    {
        CameraTriggerArea.PrioritizeNewCamera += PrioritizeNewCamera;

        if (startingCamera != null)
            PrioritizeNewCamera(startingCamera);
    }

    private void PrioritizeNewCamera(CinemachineVirtualCamera newCamera)
    {
        if (_currentCamera != null)
            _currentCamera.Priority = 0;

        _currentCamera = newCamera;
        _currentCamera.Priority = 10;
    }
}
