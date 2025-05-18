using UnityEngine;
using UnityEngine.UI;

public interface IHuman
{
    public string Name { get; }
    public string Description { get; }
    public Sprite CardSprite { get; }

    public void UseAbility();
}