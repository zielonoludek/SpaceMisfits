using UnityEngine;
using System;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }

    private int shipHealth;
    private int booty;
    private int food;
    private int notoriety;
    private int crewMorale;
    private int sightLevel;
    private int speedValue;

    public event Action<int> OnShipHealthChanged;
    public event Action<int> OnBootyChanged;
    public event Action<int> OnNotorietyChanged;
    public event Action<int> OnCrewMoraleChanged;
    public event Action<int> OnSightChanged;
    public event Action<int> OnSpeedChanged;
    public event Action<int> OnFoodChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public int ShipHealth
    {
        get => shipHealth;
        set
        {
            if (shipHealth != value)
            {
                shipHealth = Mathf.Max(0, value);
                OnShipHealthChanged?.Invoke(shipHealth);
            }
        }
    }

    public int Booty
    {
        get => booty;
        set
        {
            if (booty != value)
            {
                booty = Mathf.Max(0, value);
                OnBootyChanged?.Invoke(booty);
            }
        }
    }

    public int Notoriety
    {
        get => notoriety;
        set
        {
            if (notoriety != value)
            {
                notoriety = Mathf.Max(0, value);
                OnNotorietyChanged?.Invoke(notoriety);
            }
        }
    }

    public int CrewMorale
    {
        get => crewMorale;
        set
        {
            if (crewMorale != value)
            {
                crewMorale = Mathf.Max(0, value);
                OnCrewMoraleChanged?.Invoke(crewMorale);
            }
        }
    }

    //ResourceManager.Instance.Booty += 100; (trigger booty event in other scripts)
    //ResourceManager.Instance.Notoriety -= 5;

    public int Food
    {
        get => food;
        set
        {
            if (food != value)
            {
                food = Mathf.Max(0, value);
                OnFoodChanged?.Invoke(food);
            }
        }
    }


    public int Sight
    {
        get => sightLevel;
        set
        {
            sightLevel = Mathf.Clamp(value, 0, 3);
            OnSightChanged?.Invoke(sightLevel);
        }
    }

    public void IncreaseSight(int amount = 1)
    {
        Sight += amount;
    }

    public void DecreaseSight(int amount = 1)
    {
        Sight -= amount;
    }

    public int GetCurrentSight()
    {
        return Sight;
    }

    public int Speed
    {
        get => speedValue;
        set
        {
            speedValue = Mathf.Clamp(value, 0, 3);
            OnSpeedChanged?.Invoke(speedValue);
        }
    }

    public void IncreaseSpeed(int amount = 1)
    {
        Speed += amount;
    }

    public void DecreaseSpeed(int amount = 1)
    {
        Speed -= amount;
    }

    public int GetCurrentSpeed()
    {
        return Speed;
    }

    //ResourceManager.Instance.IncreaseSight(); //This line adds sight level in other scripts
    //ResourceManager.Instance.DecreaseSight(); //This line removes sight level in other scripts
}