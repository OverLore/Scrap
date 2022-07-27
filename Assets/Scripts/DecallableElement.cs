using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecallableElement : MonoBehaviour, IHitDecallabe
{
    [Header("Decal")]
    [SerializeField] private List<IHitDecallabe.ItemDecal> decals;
    [SerializeField] private float projectorOffset;

    GameObject GetDecal(IHitDecallabe.HitType type)
    {
        foreach (IHitDecallabe.ItemDecal decal in decals)
        {
            if (decal.type == type)
                return decal.impactDecal;
        }

        return null;
    }

    float GetRotation(IHitDecallabe.HitType type)
    {
        foreach (IHitDecallabe.ItemDecal decal in decals)
        {
            if (decal.type == type)
                return Random.Range(decal.angleMin, decal.angleMax);
        }

        return 0;
    }

    public void SetDecal(RaycastHit hit, IHitDecallabe.HitType type)
    {
        GameObject decal = GetDecal(type);

        if (decal == null)
            return;

        GameObject go = Instantiate(GetDecal(type));

        go.transform.position = hit.point + hit.normal * projectorOffset;
        go.transform.LookAt(hit.point);
        go.transform.Rotate(Vector3.forward, GetRotation(type));

        Destroy(go, 10f);
    }
}
