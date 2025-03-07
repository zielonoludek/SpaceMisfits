using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private GameState gameState;

    //========MANAGERS======//
    [Space]
    [Header("MANAGERS")]

    [SerializeField] InputManager inputManager;
    [SerializeField] ResourceManager resourceManager;
    [SerializeField] TimeManager timeManager;
    
    FightManager fightManager;
    [SerializeField] UIManager uiManager;
    CameraManager cameraManager;
    ShipDurabilityManager shipDurabilityManager;
    CrewManager crewManager;
    RequestManager requestManager;
    
    //=========GETTERS=========//
    public CameraManager CameraManager { get { return cameraManager; } }
    public ResourceManager ResourceManager { get { return resourceManager; } }
    public UIManager UIManager { get { return uiManager; } }
    public TimeManager TimeManager { get { return timeManager; } }
    public FightManager FightManager { get { return fightManager; } }
    public ShipDurabilityManager ShipDurabilityManager {  get { return shipDurabilityManager; } } 
    public CrewManager CrewManager { get { return crewManager; } }
    public RequestManager RequestManager { get { return requestManager; } }

    public GameState GameState
    {
        get => gameState;
        set => gameState = value;
    }

    private void Awake()
    {
        Application.targetFrameRate = 60;
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }
    private void Start()
    {
        cameraManager = FindFirstObjectByType<CameraManager>();
        uiManager = FindFirstObjectByType<UIManager>();
        fightManager = FindFirstObjectByType<FightManager>();
        shipDurabilityManager = FindFirstObjectByType<ShipDurabilityManager>();
        crewManager = FindFirstObjectByType<CrewManager>();
        requestManager = FindFirstObjectByType<RequestManager>();
    }
}
