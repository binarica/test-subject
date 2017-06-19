using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerManager : MonoBehaviour {

    public static PowerManager Instance;

    int currentPowerIndex = 0;

    bool intensityChanged = false;
    int intensityIndex = -1;

    bool resetting = false;

    [Header("Power Settings")]
    [Tooltip("Don't change the names tweak everything else")]
    [SerializeField]
    Power[] powers = { new Power("gravity", 1, false, false), new Power("temperature", 1, false, false) };

    Power[] originalPowers;

    // all the game objects that will be affected by changing the power's intensities
    List<IGravitable> affectedByGravity = new List<IGravitable>();
    List<ITemperaturable> affectedByTemperature = new List<ITemperaturable>();
    List<Interactive> allInteractiveObjects = new List<Interactive>();
    
    PlayerController player;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    // Use this for initialization
    void Start () {
        originalPowers = new Power[powers.Length];
        currentPowerIndex = GetFstAvailablePower();
        CopyPowers(powers, originalPowers);
        UIPowerManager.Instance.Init(powers, currentPowerIndex);
        LoadAllObjects();

        // related to "Resetting the player"
        if(player == null)
        {
            SetReferenceToPlayer(null);
        }
        if(player != null)
        {
            SetPlayerItems();
            SetPlayerStatus();
        }
    }


    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown(KeyCode.R))
        {
            resetting = true;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ChangeItemStatus();
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetPowerStatus(0, true);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetPowerStatus(1, true);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SetPowerStatus(0, false);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SetPowerStatus(1, false);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ChangePower(false);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ChangePower(true);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            ChangeIntensity(false);
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            ChangeIntensity(true);
        }
    }

    // i do the updates on each attribute in LateUpdate to avoid conflicts between UI and this script
    private void LateUpdate()
    {
        if (intensityChanged)
        {
            AffectObjects();
            intensityChanged = false;
        }
        // i wait until the animation finished to reset things otherwise you miss some references due to how Reset works
        if (resetting && UIPowerManager.Instance.CanMove())
        {
            ResetToOriginalState();
        }
    }

    private void LoadAllObjects()
    {
        allInteractiveObjects.Clear();
        affectedByGravity.Clear();
        affectedByTemperature.Clear();
        Interactive[] allInteractiveArray = GameObject.FindObjectsOfType<Interactive>();
        foreach (Interactive io in allInteractiveArray)
        {
            //Debug.Log("Got one interactive");
            allInteractiveObjects.Add(io);

            IGravitable affectedByGravityObject = io.GetComponent<IGravitable>();
            if (affectedByGravityObject != null)
            {
                //Debug.Log("Got one affected by Gravity");
                affectedByGravity.Add(affectedByGravityObject);
            }

            ITemperaturable affectedByTemperatureObject = io.GetComponent<ITemperaturable>();
            if (affectedByTemperatureObject != null)
            {
                //Debug.Log("Got one affected by Temperature");
                affectedByTemperature.Add(affectedByTemperatureObject);
            }
        }
    }

    private int GetFstAvailablePower()
    {
        for(int i = 0; i < powers.Length; i++)
        {
            if (!powers[i].IsBlocked())
            {
                return i;
            }
        }
        return 0;
    }

    public int GetNextPower(bool next)
    {
        int tempCurrentPowerIndex = currentPowerIndex;
        if (next)
        {
            while(tempCurrentPowerIndex + 1 < powers.Length)
            {
                tempCurrentPowerIndex++;
                if (!powers[tempCurrentPowerIndex].IsBlocked())
                {
                    Debug.Log(tempCurrentPowerIndex);
                    return tempCurrentPowerIndex;
                }
            }
        }
        else
        {
            while (tempCurrentPowerIndex - 1 >= 0)
            {
                tempCurrentPowerIndex--;
                if (!powers[tempCurrentPowerIndex].IsBlocked())
                {
                    Debug.Log(tempCurrentPowerIndex);
                    return tempCurrentPowerIndex;
                }
            }
        }
        
        return -1;
    }

    public void SetPowerIndex(int currentPowerIndex)
    {
        this.currentPowerIndex = currentPowerIndex;
    }

    public void SetCurrentIntensity(int currentPowerIndex, int currentIntensityIndex)
    {
        powers[currentPowerIndex].SetCurrentIntensity(currentIntensityIndex);
    }

    public bool GetCurrentPowerItemStatus(int currentPowerIndex)
    {
        return powers[currentPowerIndex].IsItemInUse();
    }

    public int GetNextIntensity(int currentPowerIndex, bool next)
    {
        int tempCurrentIntensity = -1;
        if (next)
        {
            if (powers[currentPowerIndex].IntensitiesLength() > powers[currentPowerIndex].GetCurrentIntensityIndex() + 1)
            {
                tempCurrentIntensity = powers[currentPowerIndex].GetCurrentIntensityIndex() + 1;
            }
            else
            {
                tempCurrentIntensity = 0;
            }
        }
        if (!next)
        {
            if (0 <= powers[currentPowerIndex].GetCurrentIntensityIndex() - 1)
            {
                tempCurrentIntensity = powers[currentPowerIndex].GetCurrentIntensityIndex() - 1;
            }
            else
            {
                tempCurrentIntensity = powers[currentPowerIndex].IntensitiesLength() - 1;
            }
        }
        return tempCurrentIntensity;
    }

    public int GetCurrentIntensity(int currentPowerIndex)
    {
        return powers[currentPowerIndex].GetCurrentIntensityIndex();
    }

    public float GetCurrentIntensityAngle(int currentPowerIndex, int tempIntensityIndex)
    {
        return powers[currentPowerIndex].GetCurrentIntensityAngle(tempIntensityIndex);
    }

    public void IntensityChanged(int intensityIndex)
    {
        this.intensityIndex = intensityIndex;
        intensityChanged = true;
    }

    public void ResetToOriginalState()
    {
        CopyPowers(originalPowers, powers);
        currentPowerIndex = GetFstAvailablePower();
        UIPowerManager.Instance.Init(powers, currentPowerIndex);
        resetting = false;
    }

    private void CopyPowers(Power[] from, Power[] to)
    {
        for(int i = 0; i < from.Length; i++)
        {
            to[i] = new Power(from[i].GetName(), from[i].GetCurrentIntensityIndex(), from[i].IsBlocked(), from[i].IsItemInUse());
        }
    }

    public bool Resetting()
    {
        return resetting;
    }

    public void ChangeItemStatus()
    {
        powers[currentPowerIndex].SetItemInUseStatus(!powers[currentPowerIndex].IsItemInUse());
        UIPowerManager.Instance.SetCurrentItemStatus(powers[currentPowerIndex].IsItemInUse());
        if(player != null)
        {
            player.SetItemInUse(currentPowerIndex, powers[currentPowerIndex].IsItemInUse());
            player.CheckStatus();
        }
    }

    public void SetPowerStatus(int powerIndex, bool status)
    {
        powers[powerIndex].SetBlockStatus(status);
        UIPowerManager.Instance.SetPowerStatus(powerIndex, powers[powerIndex].IsBlocked());
    }

    public bool IsCurrentPowerInactive()
    {
        return powers[currentPowerIndex].IsBlocked();
    }

    public void ChangePower(bool next)
    {
        UIPowerManager.Instance.ChangePower(next);
    }

    public void ChangeIntensity(bool next)
    {
        UIPowerManager.Instance.ChangeIntensity(next);
    }

    private void AffectObjects()
    {
        switch (currentPowerIndex)
        {
            case 0:
                switch (GetCurrentIntensity(currentPowerIndex))
                {
                    case 0:
                        foreach (IGravitable affectedByGravityObject in affectedByGravity)
                        {
                            affectedByGravityObject.LowGravity();
                        }
                        break;
                    case 1:
                        foreach (IGravitable affectedByGravityObject in affectedByGravity)
                        {
                            affectedByGravityObject.NormalGravity();
                        }
                        break;
                    case 2:
                        foreach (IGravitable affectedByGravityObject in affectedByGravity)
                        {
                            affectedByGravityObject.InversedGravity();
                        }
                        break;
                    default:
                        Debug.Log("Something went wrong with Gravity Intensity!");
                        break;
                }
                break;
            case 1:
                switch (GetCurrentIntensity(currentPowerIndex))
                {
                    case 0:
                        foreach (ITemperaturable affectedByTemperatureObject in affectedByTemperature)
                        {
                            affectedByTemperatureObject.Cold();
                        }
                        break;
                    case 1:
                        foreach (ITemperaturable affectedByTemperatureObject in affectedByTemperature)
                        {
                            affectedByTemperatureObject.NormalTemperature();
                        }
                        break;
                    case 2:
                        foreach (ITemperaturable affectedByTemperatureObject in affectedByTemperature)
                        {
                            affectedByTemperatureObject.Hot();
                        }
                        break;
                    default:
                        Debug.Log("Something went wrong with Temperature Intensity!");
                        break;
                }
                break;
            default:
                Debug.Log("Something went wrong with Power!");
                break;
        }
    }

    public void SetReferenceToPlayer(PlayerController player)
    {
        if(player == null)
        {
            this.player = FindObjectOfType<PlayerController>();
        }
        else
        {
            this.player = player;
        }
    }

    // functions related to resetting the player start

    public void ResetPlayer(GameObject player)
    {
        PlayerSpawned(player);
    }

    public void SetPlayerItems()
    {
        for(int i = 0; i < powers.Length; i++)
        {
            player.SetItemInUse(i, powers[i].IsItemInUse());
        }
    }

    private void SetPlayerStatus()
    {
        for (int i = 0; i < powers.Length; i++)
        {
            player.SetCurrentStatus(i, powers[i].GetCurrentIntensityIndex());
        }
    }

    // functions related to resetting the player end

    // functions to handle player dead and resurrection start

    private void PlayerSpawned(GameObject player)
    {
        // Player Died is for testing purposes should be removed TODO
        // PlayerDied(this.player.gameObject);
        SetReferenceToPlayer(player.GetComponent<PlayerController>());
        SetPlayerItems();
        SetPlayerStatus();
        AddToLists(player);
    }

    public void PlayerDied(GameObject player)
    {
        player.GetComponent<PlayerController>().Die();
    }

    // functions to handle player dead and resurrection end

    // eliminating objects from lists
    private void AddToLists(GameObject GO)
    {
        Interactive interactiveGO = GO.GetComponent<Interactive>();
        IGravitable gravitableGO = GO.GetComponent<IGravitable>();
        ITemperaturable temperaturableGO = GO.GetComponent<ITemperaturable>();

        if (interactiveGO != null)
        {
            if (!allInteractiveObjects.Contains(interactiveGO))
            {
                allInteractiveObjects.Add(interactiveGO);
            }
        }

        if (gravitableGO != null)
        {
            if (!affectedByGravity.Contains(gravitableGO))
            {                
                // the line below is due to the object not being affected instantly
                BeAffectedByGravity(gravitableGO);
                affectedByGravity.Add(gravitableGO);
            }
        }

        if (temperaturableGO != null)
        {
            if (!affectedByTemperature.Contains(temperaturableGO))
            {
                // the line below is due to the object not being affected instantly
                BeAffectedByTemperature(temperaturableGO);
                affectedByTemperature.Add(temperaturableGO);
            }
        }
    }

    public void DeleteFromLists(GameObject GO)
    {
        Interactive interactiveGO = GO.GetComponent<Interactive>();
        IGravitable gravitableGO = GO.GetComponent<IGravitable>();
        ITemperaturable temperaturableGO = GO.GetComponent<ITemperaturable>();

        if (interactiveGO != null)
        {
            allInteractiveObjects.Remove(interactiveGO);
        }

        if (gravitableGO != null)
        {
            affectedByGravity.Remove(gravitableGO);
            gravitableGO.NormalGravity();
        }

        if (temperaturableGO != null)
        {
            affectedByTemperature.Remove(temperaturableGO);
            temperaturableGO.NormalTemperature();
        }
    }

    // to affect instantiated game objects at run time
    private void BeAffectedByGravity(IGravitable gravitableGO)
    {
        switch (powers[0].GetCurrentIntensityIndex())
        {
            case 0:
                gravitableGO.LowGravity();
                break;
            case 1:
                gravitableGO.NormalGravity();
                break;
            case 2:
                gravitableGO.InversedGravity();
                break;
            default:
                Debug.Log("Something went wrong with Gravity Intensity!");
                break;
        }
    }

    // to affect instantiated game objects at run time
    private void BeAffectedByTemperature(ITemperaturable temperaturableGO)
    {
        switch (powers[1].GetCurrentIntensityIndex())
        {
            case 0:
                temperaturableGO.Cold();
                break;
            case 1:
                temperaturableGO.NormalTemperature();
                break;
            case 2:
                temperaturableGO.Hot();
                break;
            default:
                Debug.Log("Something went wrong with Gravity Intensity!");
                break;
        }
    }

    // for other interactive objects spawned at runtime
    public void AffectInteractive(GameObject interactiveGO)
    {
        if(interactiveGO.GetComponent<Interactive>() != null)
        {
            IGravitable gravitableGO = interactiveGO.GetComponent<IGravitable>();
            if(gravitableGO != null)
            {
                BeAffectedByGravity(gravitableGO);
            }
            ITemperaturable temperaturableGO = interactiveGO.GetComponent<ITemperaturable>();
            {
                BeAffectedByTemperature(temperaturableGO);
            }
        }
    }
}
