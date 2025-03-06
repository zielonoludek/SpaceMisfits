using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    //========MANAGERS======//
    [Space]
    [Header("MANAGERS")]

    [SerializeField] CameraManager cameraManager;
    [SerializeField] InputManager inputManager;
    [SerializeField] ResourceManager resourceManager;
    [SerializeField] UIManager uiManager;
    [SerializeField] TimeManager timeManager;
    [SerializeField] FightManager fightManager;
    [SerializeField] ShipDurabilityManager shipDurability;
    [SerializeField] CrewManager crewManager;
    [SerializeField] RequestManager requestManager;

    //========SCRIPTABLES======//
    [Space]
    [Header("SCRIPTABLES")]

    [SerializeField] GameManager Empty;

    
    //=========GETTERS=========//
    public CameraManager CameraManager { get { return cameraManager; } }
    public ResourceManager ResourceManager { get { return resourceManager; } }
    public UIManager UIManager { get { return uiManager; } }
    public TimeManager TimeManager { get { return timeManager; } }
    public FightManager FightManager { get { return fightManager; } }
    public ShipDurabilityManager ShipDurabilityManager { get { return shipDurability; } }
    public CrewManager CrewManager { get { return crewManager; } }
    public RequestManager RequestManager { get { return requestManager; } }


    private void Awake()
    {
        Application.targetFrameRate = 60;
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }
}
