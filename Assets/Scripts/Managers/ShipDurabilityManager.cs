using UnityEngine;
using System;
using System.Collections.Generic;

public class ShipDurabilityManager : MonoBehaviour
{
    public static ShipDurabilityManager Instance { get; private set; }

    public event Action<string, int> OnShipPartDamaged;
    public event Action<string, int> OnShipPartRepaired;

    private Dictionary<string, int> shipParts = new Dictionary<string, int>();

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

        shipParts["Hull"] = 100;
        shipParts["Sails"] = 100;
        shipParts["Cannon"] = 100;

    }

    public int this[string partName]
    {
        get => shipParts.ContainsKey(partName) ? shipParts[partName] : 0;
        set
        {
            if (shipParts.ContainsKey(partName))
            {
                shipParts[partName] = Mathf.Clamp(value, 0, 100);
            }
        }
    }

    public void DamagePart(string partName, int damage)
    {
        if (shipParts.ContainsKey(partName))
        {
            this[partName] -= damage;
            OnShipPartDamaged?.Invoke(partName, shipParts[partName]);
        }
    }

    public void RepairPart(string partName, int repairAmount)
    {
        if (shipParts.ContainsKey(partName))
        {
            this[partName] += repairAmount;
            OnShipPartRepaired?.Invoke(partName, shipParts[partName]);
        }
    }

    public int GetDurability(string partName)
    {
        return shipParts.ContainsKey(partName) ? shipParts[partName] : 0;
    }

    public void DamageHull()
    {
        DamagePart("Hull", 10);
    }

    public void RepairHull()
    {
        RepairPart("Hull", 10);
    }

    public void DamageSails()
    {
        DamagePart("Sails", 10);
    }

    public void RepairSails()
    {
        RepairPart("Sails", 10);
    }

    public void DamageCannon()
    {
        DamagePart("Cannon", 10);
    }

    public void RepairCannon()
    {
        RepairPart("Cannon", 10);
    }

    //Use the below in other scripts to apply damage or repair to ship parts
    //ShipDurabilityManager.Instance["Hull"] -= 20;
    //ShipDurabilityManager.Instance["Sails"] += 15;
    //ShipDurabilityManager.Instance["Cannon"] -= 10;

}
