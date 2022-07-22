using UnityEngine;

public interface IHitDecallabe
{
    public enum HitType
    {
        Axe,
        Pickaxe,
        Bullet
    }
    
    public void SetDecal(RaycastHit hit, HitType type);
}