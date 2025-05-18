using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IJob
{
    public string Name { get; }
    
    public string Description { get; }
    
    public Sprite CardSprite { get; }
}
