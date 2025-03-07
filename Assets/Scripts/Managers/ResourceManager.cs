using UnityEngine;
using System;

public class ResourceManager : MonoBehaviour
{
    private int shipHealth;
    private int booty;
    private int food;
    private int notoriety;
    private int crewMood;
    private int sightLevel;
    private int speedValue;

    public event Action<int> OnShipHealthChanged;
    public event Action<int> OnBootyChanged;
    public event Action<int> OnNotorietyChanged;
    public event Action<int> OnCrewMoodChanged;
    public event Action<int> OnSightChanged;
    public event Action<int> OnSpeedChanged;
    public event Action<int> OnFoodChanged;

 
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

    public int CrewMood
    {
        get => CrewMood;
        set
        {
            if (CrewMood != value)
            {
                crewMood = Mathf.Max(0, value);
                OnCrewMoodChanged?.Invoke(CrewMood);
            }
        }
    }

    //GameManager.Instance.ResourceManager.Booty += 100; (trigger booty event in other scripts)
    //GameManager.Instance.ResourceManager.Notoriety -= 5;

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

    //GameManager.Instance.ResourceManager.IncreaseSight(); //This line adds sight level in other scripts
    //GameManager.Instance.ResourceManager.DecreaseSight(); //This line removes sight level in other scripts
}