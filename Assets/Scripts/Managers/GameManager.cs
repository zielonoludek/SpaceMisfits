using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameState gameState;
    [SerializeField] private GameScene gameScene;

    //========MANAGERS======//
    [Space]
    [Header("MANAGERS")]

    [SerializeField] InputManager inputManager;
    [SerializeField] ResourceManager resourceManager;
    [SerializeField] TimeManager timeManager;

    [SerializeField] FightManager fightManager;
    [SerializeField] UIManager uiManager;
    [SerializeField] CameraManager cameraManager;
    [SerializeField] ShipDurabilityManager shipDurabilityManager;
    [SerializeField] CrewManager crewManager;
    [SerializeField] RequestManager requestManager;
    [SerializeField] SceneLoader sceneLoader;
    
    //=========GETTERS=========//
    public CameraManager CameraManager { get { return cameraManager; } }
    public ResourceManager ResourceManager { get { return resourceManager; } }
    public UIManager UIManager { get { return uiManager; } }
    public TimeManager TimeManager { get { return timeManager; } }
    public FightManager FightManager { get { return fightManager; } }
    public ShipDurabilityManager ShipDurabilityManager {  get { return shipDurabilityManager; } } 
    public CrewManager CrewManager { get { return crewManager; } }
    public RequestManager RequestManager { get { return requestManager; } }
    public SceneLoader SceneLoader { get { return sceneLoader; } }

    public GameState GameState
    {
        get => gameState;
        set => gameState = value;
    }
    public GameScene GameScene
    {
        get => gameScene;
        set => gameScene = value;
    }

    private void Awake()
    {
        Application.targetFrameRate = 60;
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            Destroy(this);
        }
        else Instance = this;

        DontDestroyOnLoad(this.gameObject);

        LoadManagers();
        sceneLoader.NewSceneLoaded += LoadManagers;
    }
    void LoadManagers()
    {
        cameraManager = FindFirstObjectByType<CameraManager>();
        uiManager = FindFirstObjectByType<UIManager>();
        fightManager = FindFirstObjectByType<FightManager>();
        shipDurabilityManager = FindFirstObjectByType<ShipDurabilityManager>();

        crewManager = gameObject.GetComponentInChildren<CrewManager>();
        requestManager = gameObject.GetComponentInChildren<RequestManager>();

        timeManager = gameObject.GetComponent<TimeManager>();
        resourceManager = gameObject.GetComponent<ResourceManager>();
        inputManager = gameObject.GetComponent<InputManager>();
        sceneLoader = gameObject.GetComponent<SceneLoader>();
    }
}