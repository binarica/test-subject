using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPowerManager : MonoBehaviour {

    public static UIPowerManager Instance;
    

    [SerializeField]
    float animationSpeedMultiplier = 5.0f;

    // for testing intensities
    int currentIntensity = 1;

    // this states are to avoid weird glitches due to stoping an animation in the middle of it
    int currentPowerIndex = 0;
    bool canMove = true;
    bool canChangeIntensity = true;

    // in order = gravity fst, temperature snd
    // this are the buttons related to gravity and temperature
    [SerializeField]
    Transform[] buttons = new Transform[2];

    [SerializeField]
    GameObject buttonMask;

    // this are the two placeholders for the original and disappeared position
    [SerializeField]
    Transform[] buttonsPlaceholders = new Transform[2];

    // this are the intensities holders in order
    [SerializeField]
    Transform[] intensitiesHolders = new Transform[2];

    // this are the intensities in order
    [SerializeField]
    Transform[] intensities = new Transform[2];

    // this are the intensities blockers
    [SerializeField]
    GameObject[] blockers = new GameObject[2];

    // this is the selector lever
    [SerializeField]
    Transform selectorLever;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        
    }

    // this function initializes the states for all the powers and their respective intensities
    public void Init(Power[] powers, int currentPowerIndex)
    {
        this.currentPowerIndex = currentPowerIndex;
        PositionSelector(currentPowerIndex);
        for(int i = 0; i < powers.Length; i++)
        {
            SetIntensityAngle(i, powers[i].GetCurrentIntensitySelectedAngle());
            SetPowerStatus(i, powers[i].IsBlocked());
        }
        currentIntensity = PowerManager.Instance.GetCurrentIntensity(currentPowerIndex);
        SetCurrentPowerButton(currentPowerIndex);
        SetCurrentItemStatus(powers[currentPowerIndex].IsItemInUse());
    }

    public bool CanMove()
    {
        return canMove;
    }

    public bool CanChangeIntensity()
    {
        return canChangeIntensity;
    }

    // animates the change of power relying on its index
    //public void ChangePower(int newPowerIndex)
    public void ChangePower(bool next)
    {
        if (canMove && !PowerManager.Instance.Resetting())
        {
            int newPowerIndex = PowerManager.Instance.GetNextPower(next);
            if(newPowerIndex != -1)
            {
                canMove = false;
                StopCoroutine("CorChangeLeverPosition");
                StartCoroutine("CorChangeLeverPosition", newPowerIndex);
                StopCoroutine("CorChangeButton");
                StartCoroutine("CorChangeButton", newPowerIndex);
            }
        }
    }

    public void SetCurrentItemStatus(bool active)
    {
        buttonMask.SetActive(active);
    }

    public void SetPowerStatus(int powerIndex, bool status)
    {
        blockers[powerIndex].SetActive(status);
        TryResetLever();
    }

    // animates the change of intensity to the next or previous based upon a bool
    public void ChangeIntensity(bool next)
    {
        if (canMove && canChangeIntensity && !PowerManager.Instance.Resetting())
        {
            int tempCurrentIntensity = PowerManager.Instance.GetNextIntensity(currentPowerIndex, next);
            if(tempCurrentIntensity != -1)
            {
                StopCoroutine("CorChangeIntensity");
                StartCoroutine("CorChangeIntensity", tempCurrentIntensity);
            }
        }
    }

    // this gets the next intensity it makes the selection rotative so you can go from inversed gravity to low same from cold to hot and viceversa

    // Init related functions start
    private void PositionSelector(int currentPowerIndex)
    {
        selectorLever.transform.position = new Vector3(intensities[currentPowerIndex].position.x, selectorLever.position.y);
    }
    
    private void SetIntensityAngle(int powerIndex, float intensityAngle)
    {
        intensities[powerIndex].eulerAngles = Vector3.forward * intensityAngle;
    }

    private void SetCurrentPowerButton(int currentPowerIndex)
    {
        buttons[currentPowerIndex].position = buttonsPlaceholders[0].position;
        buttons[currentPowerIndex].gameObject.SetActive(true);
    }

    // Init related functions end

    // to keep UIPowerManager and PowerManager synchronized start
    private void SetCurrentPowerIndex(int currentPowerIndex)
    {
        this.currentPowerIndex = currentPowerIndex;
        PowerManager.Instance.SetPowerIndex(currentPowerIndex);
    }

    private void SetCurrentIntensity(int currentPowerIndex, int currentIntensityIndex)
    {
        currentIntensity = currentIntensityIndex;
        PowerManager.Instance.SetCurrentIntensity(currentPowerIndex, currentIntensityIndex);
    }

    // tries to reset the lever to the right fst and to the left after if the fst one failed...
    private void TryResetLever()
    {
        if (PowerManager.Instance.IsCurrentPowerInactive())
        {
            ChangePower(true);
            if (PowerManager.Instance.IsCurrentPowerInactive())
            {
                ChangePower(false);
            }
        }
    }

    // to keep UIPowerManager and PowerManager synchronized end

    // start of coroutines

    // coroutine set to handle animation of power change start
    IEnumerator CorChangeButton(int newPowerIndex)
    {
        // start tiem stamp
        float timeStamp = 0.0f;

        // get final positions
        Vector3 finishedPosition = buttonsPlaceholders[0].transform.localPosition;
        Vector3 disappearedPosition = buttonsPlaceholders[1].transform.localPosition;

        // set starting position for new power and activate it
        buttons[newPowerIndex].transform.localPosition = disappearedPosition;
        buttons[newPowerIndex].gameObject.SetActive(true);

        // get current positions
        Vector3 currentPositionActive = buttons[currentPowerIndex].transform.localPosition;
        Vector3 currentPositionInactive = buttons[newPowerIndex].transform.localPosition;

        // animate the lerp of the starting states to the finished ones
        while ((buttons[currentPowerIndex].transform.localPosition != disappearedPosition) || (buttons[newPowerIndex].transform.localPosition != finishedPosition))
        {
            timeStamp += Time.deltaTime * animationSpeedMultiplier;
            buttons[currentPowerIndex].transform.localPosition = Vector3.Lerp(currentPositionActive, disappearedPosition, timeStamp);
            buttons[newPowerIndex].transform.localPosition = Vector3.Lerp(currentPositionInactive, finishedPosition, timeStamp);
            yield return null;
        }

        // deactivate previous state button
        buttons[currentPowerIndex].gameObject.SetActive(false);
        Debug.Log("Finished changing buttons");

        // set new state to UIPowerManager otherwise it will break things up...
        // i know this isn't the best practice but here I synchronize things up with the proper index for each thing
        SetCurrentPowerIndex(newPowerIndex);
        SetCurrentItemStatus(PowerManager.Instance.GetCurrentPowerItemStatus(newPowerIndex));
        currentIntensity = PowerManager.Instance.GetCurrentIntensity(newPowerIndex);
        canMove = true;
    }

    IEnumerator CorChangeLeverPosition(int newPowerIndex)
    {
        float timeStamp = 0.0f;

        // set initial and final state
        Vector2 selectorStartingPos = selectorLever.transform.localPosition;
        selectorStartingPos.y = 0;
        Vector2 finishedPosition = new Vector2(intensitiesHolders[newPowerIndex].localPosition.x, 0);

        // animate until change is accomplished
        while ((Vector2)selectorLever.transform.localPosition != finishedPosition)
        {
            timeStamp += Time.deltaTime * animationSpeedMultiplier;
            selectorLever.transform.localPosition = Vector2.Lerp(selectorStartingPos, finishedPosition, timeStamp);
            yield return null;
        }
    }
    // coroutine set to handle animation of power change end

    // coroutine to handle intensity change animation start

    IEnumerator CorChangeIntensity(int tempCurrentIntensity)
    {
        canChangeIntensity = false;

        float newIntensityAngle = PowerManager.Instance.GetCurrentIntensityAngle(currentPowerIndex, tempCurrentIntensity);

        float fromAngle = intensities[currentPowerIndex].rotation.eulerAngles.z;
        float endPoint = newIntensityAngle < 0 ? 360.0f + newIntensityAngle : newIntensityAngle;
        float difference = fromAngle > endPoint ? fromAngle - endPoint : endPoint - fromAngle;

        float timeStamp = 0.0f;
        float threshold = 0.01f;

        while (difference > threshold)
        {
            timeStamp += Time.deltaTime * animationSpeedMultiplier;
            intensities[currentPowerIndex].eulerAngles = new Vector3(0.0f, 0.0f, Mathf.LerpAngle(fromAngle, newIntensityAngle, timeStamp));
            float curAngle = intensities[currentPowerIndex].rotation.eulerAngles.z;
            difference = curAngle > endPoint ? curAngle - endPoint : endPoint - curAngle;
            //The yield has to be inside this loop otherwise it won't go back to the update and won't show the "animation"
            yield return null;
        }

        SetCurrentIntensity(currentPowerIndex, tempCurrentIntensity);
        PowerManager.Instance.IntensityChanged(tempCurrentIntensity);
        canChangeIntensity = true;
    }

    // coroutine to handle intensity change animation end

    // end of coroutine
    
}
