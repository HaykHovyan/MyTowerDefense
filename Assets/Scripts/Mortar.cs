using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mortar : MonoBehaviour
{
    [SerializeField]
    Transform mortar;
    [SerializeField]
    Shell Shell;
    [SerializeField]
    LayerMask enemyMask;
    [SerializeField, Range(1, 10f)]
    float targetRange = 2f;
    [SerializeField, Range(0.5f, 5f)]
    float shellBlastRadius = 1f;
    [SerializeField, Range(1, 200)]
    float shellDamage = 30f;

    TargetPoint target;
    float launchSpeed;
    float launchProgress = 0;
    float shotsPerSecond = 1f;

    void Awake()
    {
        float x = targetRange + 0.250001f;
        float y = -mortar.position.y;
        launchSpeed = Mathf.Sqrt(9.81f * (y + Mathf.Sqrt(x * x + y * y)));
    }

    void Update()
    {
        launchProgress += shotsPerSecond * Time.deltaTime;
        if (launchProgress >= 1)
        {
            if (AcquireTarget())
            {
                Launch(target);
            }
            launchProgress = 0;
        }
    }

    void Launch(TargetPoint target)
    {
        if (target == null)
        {
            return;
        }
        Vector2 direction;
        Vector3 launchPoint = mortar.position;
        Vector3 targetPoint = target.Position;
        targetPoint.y = 0;

        direction.x = targetPoint.x - launchPoint.x;
        direction.y = targetPoint.z - launchPoint.z;
        float x = direction.magnitude;
        float y = -launchPoint.y;
        direction /= x;

        float g = 9.81f;
        float s = launchSpeed;
        float s2 = s * s;

        float r = s2 * s2 - g*(g*x*x + 2f*y*s2);
        float tanTheta = (s2 + Mathf.Sqrt(r)) / (g * x);
        float theta = Mathf.Atan(tanTheta);
        float cosTheta = Mathf.Cos(theta);
        float sinTheta = Mathf.Sin(theta);

        mortar.localRotation = Quaternion.LookRotation(new Vector3(direction.x, tanTheta, direction.y));

        Shell shell = Instantiate(Shell);
        shell.Initialize(launchPoint, targetPoint, new Vector3(s * cosTheta * direction.x, s * sinTheta, s * cosTheta * direction.y), shellBlastRadius, shellDamage);

        /*
        Vector3 prev = launchPoint;
        Vector3 next = launchPoint;
        for (int i = 0; i < 10; i++)
        {
            float t = i / 10f;
            float dx = s * cosTheta * t;
            float dy = s * sinTheta * t - 0.5f * g * t * t;
            next = launchPoint + new Vector3(direction.x * dx, dy, direction.y * dx);
            Debug.DrawLine(prev, next, Color.blue);
            prev = next;
        }
        */
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
}
