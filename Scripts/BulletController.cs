using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    private bool targetHit;
    public int damage;

    public float bounciness;
    public bool useGravity;

    public int maxLifetime;
    public bool explodeOnTouch = true;

    int collisions;
    PhysicMaterial physicMaterial;

    private void Start()
    {
        Setup();
    }

    private void Update()
    {
        if (maxLifetime <= 0) Destroy(this.gameObject);
    }

    private void Setup()
    {
        physicMaterial = new PhysicMaterial();
        physicMaterial.bounciness = bounciness;
        physicMaterial.frictionCombine = PhysicMaterialCombine.Minimum;
        physicMaterial.bounceCombine = PhysicMaterialCombine.Maximum;

        GetComponent<SphereCollider>().material = physicMaterial;

        rb.useGravity = useGravity;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(this.gameObject);
    }


}
