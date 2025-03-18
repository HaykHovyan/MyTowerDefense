using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [SerializeField]
    Transform turret;
    [SerializeField]
    Transform lazerBeam;
    [SerializeField]
    LayerMask enemyMask;
    [SerializeField, Range(1, 10f)]
    float targetRange = 2f;
    [SerializeField, Range(1, 100f)]
    float damagePerSecond = 30f;

    TargetPoint target;
    Vector3 lazerBeamScale;

    private void Awake()
    {
        lazerBeamScale = lazerBeam.localScale;
    }

    private void Update()
    {
        if (AcquireTarget())
        {
            Shoot();
        }
        else
        {
            turret.localRotation = Quaternion.Euler(0, 0, 0);
            lazerBeam.localScale = Vector3.zero;
        }
    }

    void Shoot()
    {
        Vector3 targetPoint = target.Position + Vector3.up * 0.5f;
        turret.LookAt(targetPoint);
        lazerBeam.localRotation = turret.localRotation;
        float distanceToTarget = Vector3.Distance(turret.position, targetPoint);
        lazerBeamScale.z = distanceToTarget;
        lazerBeam.localScale = lazerBeamScale;
        lazerBeam.localPosition = turret.localPosition + 0.5f * distanceToTarget * lazerBeam.forward;
        target.Enemy.ApplyDamage(damagePerSecond * Time.deltaTime);
    }

    bool AcquireTarget()
    {
        Collider[] targets = Physics.OverlapSphere(transform.position, targetRange, enemyMask);
        float minDistance = float.MaxValue;
        for (int i = 0; i < targets.Length; i++)
        {
            if (targets[i].GetComponent<TargetPoint>() == null)
                continue;
            float distance = Vector3.Distance(transform.position, targets[i].transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                target = targets[i].GetComponent<TargetPoint>();
            }
        }
        if (target != null) return true;
        target = null;
        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector3 position = transform.position;
        Gizmos.DrawWireSphere(position, targetRange);

        if (target != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(position, target.transform.position);
        }
    }
}
