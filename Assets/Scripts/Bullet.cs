using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private LayerMask collidables;

    private Vector3 lastPosition;
    private bool shoot;

    private void Awake()
    {
        Destroy(gameObject, 10f);
    }

    public void Shot(Vector3 direction)
    {
        shoot = true;
        lastPosition = transform.position;

        GetComponent<Rigidbody>().AddForce(direction * 16f, ForceMode.Impulse);
    }

    void Update()
    {
        if (!shoot)
            return;

        if (Physics.Raycast(lastPosition, transform.localPosition - lastPosition,
                out RaycastHit hit, Vector3.Distance(transform.localPosition, lastPosition), 
                collidables))
        {
            Debug.Log(hit.transform.gameObject, hit.transform.gameObject);

            IHitDecallabe decallable = hit.transform.gameObject.GetComponent<IHitDecallabe>();

            if (decallable != null)
            {
                decallable.SetDecal(hit, IHitDecallabe.HitType.Bullet);
            }

            Destroy(gameObject);
        }

        lastPosition = transform.position;
    }
}
