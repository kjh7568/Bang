
public class PlayerInGameStat
{
    public int Hp { get; set; }
    public int BulletRange { get; set; }
    
    public ICard[] HandCards { get; set; }
    public enum Job { Sceriffo, Vice, Fuorilegge, Rinnegato }
    
    public IHuman MyHuman { get; set; }

    public bool IsBang { get; set; }
    public bool IsVolcanic { get; set; }
    public bool IsBarrel { get; set; }
    public bool IsMustang { get; set; }
    public bool IsJail { get; set; }
    public bool IsDynamite { get; set; }
}
