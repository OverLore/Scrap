using UnityEngine;

public interface IHitDecallabe
{
    [System.Serializable]
    public class ItemDecal
    {
        public IHitDecallabe.HitType type;
        public GameObject impactDecal;
        public float angleMin;
        public float angleMax;
    }

    public enum HitType
    {
        Axe,
        Pickaxe,
        Bullet
    }
    
    public void SetDecal(RaycastHit hit, HitType type);
}