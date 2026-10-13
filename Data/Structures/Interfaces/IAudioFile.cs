namespace RPGMDataKit.Data.Structures.Interfaces;

public interface IAudioFile
{
    public string Name { get; set; }

    public int Volume { get; set; }

    public int Pitch { get; set; }
    
    public int Pan { get; set; }
}