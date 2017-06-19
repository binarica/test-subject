using System;

[Serializable]
public class Power {

    //The intensities are represented by an index [0..2] which goes from low to high you can add a block or unblock boolean later on in here 
    //and in Intensity class but you would need an array of intensities for each power, this way they all share the same...
    private static Intensity[] intensities = { new Intensity("low", 120.0f), new Intensity("medium", 0.0f),
                                               new Intensity("high", -120.0f) };
    
    public string name;
    public int currentIntensityIndex;
    public bool blocked;
    public bool itemInUse;

    public Power(string name, int currentIntensityIndex, bool blocked, bool itemInUse)
    {
        this.name = name;
        this.currentIntensityIndex = currentIntensityIndex;
        this.blocked = blocked;
        this.itemInUse = itemInUse;
    }

    public static string GetIntensities()
    {
        string intensitiesString = "";

        for (int i = 0; i < intensities.Length; i++)
        {
            intensitiesString += intensities[i].getName() + " " + "\n";
        }

        return intensitiesString;
    }

    public string GetName()
    {
        return name;
    }

    public int GetCurrentIntensityIndex()
    {
        return currentIntensityIndex;
    }
    
    public string GetCurrentIntensityName()
    {
        return intensities[currentIntensityIndex].getName();
    }

    public float GetCurrentIntensitySelectedAngle()
    {
        return intensities[currentIntensityIndex].getSelectedAngle();
    }


    public float GetCurrentIntensityAngle(int intensityIndex)
    {
        return intensities[intensityIndex].getSelectedAngle();
    }

    private bool IsIntensityInRange(int index)
    {
        return index >= 0 && index < intensities.Length;
    }

   public void SetCurrentIntensity(int currentIntensityIndex)
    {
        this.currentIntensityIndex = currentIntensityIndex;
    }

    public bool IsBlocked()
    {
        return blocked;
    }

    public void SetBlockStatus(bool curStatus)
    {
        blocked = curStatus;
    }

    public bool IsItemInUse()
    {
        return itemInUse;
    }

    public void SetItemInUseStatus(bool curStatus)
    {
        itemInUse = curStatus;
    }

    public int IntensitiesLength()
    {
        return intensities.Length;
    }
}
