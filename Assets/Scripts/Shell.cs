using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{
    [SerializeField]
    LayerMask enemyMask;
    Vector3 launchPoint, targetPoint, launchVelocity;
    float age, blastRadius, damage;

    void Update()
    {
        age += Time.deltaTime;
        Vector3 p = launchPoint + launchVelocity * age;
        p.y -= 0.5f * 9.81f * age * age;
        Debug.Log(p);
        transform.position = p;

        Vector3 d = launchVelocity;
        d.y -= 9.81f * age;
        transform.localRotation = Quaternion.LookRotation(d);

        if (transform.position.y < 0)
        {
            Explode();
        }
    }

    public void Initialize(Vector3 launchPoint, Vector3 targetPoint, Vector3 launchVelocity, float blastRadius, float damage)
    {
        this.launchPoint = launchPoint;
        this.targetPoint = targetPoint;
        this.launchVelocity = launchVelocity;
        this.blastRadius = blastRadius;
        this.damage = damage;
    }

    void Explode()
    {
        Collider[] targets = Physics.OverlapSphere(transform.position, blastRadius, enemyMask);
        if (targets.Length > 0)
        {
            for (int i = 0; i < targets.Length; i++)
            {
                targets[i].GetComponent<TargetPoint>().Enemy.ApplyDamage(damage);
            }
        }
        Destroy(this.gameObject);
    }
}
