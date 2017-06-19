public class Intensity {

    private string name;
    private float selectedAngle;

    public Intensity(string name, float selectedAngle)
    {
        this.name = name;
        this.selectedAngle = selectedAngle;
    }

    public string getName()
    {
        return name;
    }

    public float getSelectedAngle()
    {
        return selectedAngle;
    }
}
