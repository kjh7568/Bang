using UnityEngine.UI;

public interface IHuman
{
    public string Name { get; set; }
    public Image CardSprite { get; set; }
    
    public void UseAbility();
}