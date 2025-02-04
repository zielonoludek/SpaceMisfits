using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    //========MANAGERS======//
    [Space]
    [Header("MANAGERS")]

    [SerializeField] CameraManager cameraManager;
    [SerializeField] InputManager inputManager ;

    //========SCRIPTABLES======//
    [Space]
    [Header("SCRIPTABLES")]

    [SerializeField] GameManager Empty;


    //=========GETTERS=========//
    public CameraManager CameraManager { get { return cameraManager; } }


    private void Awake()
    {
        Application.targetFrameRate = 60;
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }
}
