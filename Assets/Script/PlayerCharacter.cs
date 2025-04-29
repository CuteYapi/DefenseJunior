using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerCharacter : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField] private float detectionRadius = 5f;
    [SerializeField] private LayerMask targetLayer;  // Inspector���� ������ ����� ���̾ ����
    [SerializeField] private Color gizmoColor = Color.red;  // Gizmo ����

    [Header("Attack Settings")]
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private float attackFXDuration = 1f; // ����Ʈ ���� �ð�
    [SerializeField] private int attackFXType; // ����Ʈ ����

    [Header("Manager")]
    [SerializeField] private PoolManager pool;

    private GameObject currentTarget;
    private float nextAttackTime;

    private void Update()
    {
        // ���� Ÿ���� ���ų�, �׾��ų�, ������ ��� ��� ���ο� Ÿ�� ã��
        if (currentTarget == null ||
            !IsTargetAlive(currentTarget) ||
            !IsTargetInRange(currentTarget))
        {
            FindNewTarget();
        }

        // Ÿ���� �ְ� ���� ������ ��� ����
        if (currentTarget != null && Time.time >= nextAttackTime)
        {
            AttackTarget();
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    // Ÿ���� ���� ���� ���� �ִ��� Ȯ���ϴ� �Լ�
    private bool IsTargetInRange(GameObject target)
    {
        if (target == null) return false;

        float distance = Vector3.Distance(transform.position, target.transform.position);
        return distance <= detectionRadius;
    }

    private void FindNewTarget()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius, targetLayer);
        float closestDistance = float.MaxValue;
        currentTarget = null;

        foreach (Collider collider in colliders)
        {
            GameObject target = collider.gameObject;

            // �ڱ� �ڽ��� ����
            if (target == gameObject)
            {
                continue;
            }

            // Ÿ���� ����ִ��� Ȯ��
            if (!IsTargetAlive(target))
            {
                continue;
            }

            // ���� ����� Ÿ�� ����
            float distance = Vector3.Distance(transform.position, target.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                currentTarget = target;
            }
        }
    }

    private bool IsTargetAlive(GameObject target)
    {
        // Ÿ���� Enemy ������Ʈ�� Ȯ��
        var enemy = target.GetComponent<Enemy>();
        return enemy != null && enemy.IsAlive();
    }

    private void AttackTarget()
    {
        if (currentTarget == null) return;

        var enemy = currentTarget.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.Damaged(attackDamage);

            // ���� ����Ʈ ����
            SpawnAttackFX(currentTarget.transform.position);

            // ���� �� Ÿ���� �׾����� Ȯ��
            if (!enemy.IsAlive())
            {
                currentTarget = null;
            }
        }
    }

    // ���� ����Ʈ�� �����ϴ� �Լ�
    private void SpawnAttackFX(Vector3 position)
    {
        // ����Ʈ �������� => � ����Ʈ�� �������� �����ϱ�
        ParticleSystem targetFx = null;
        switch (attackFXType)
        {
            case 1:
                {
                    targetFx = pool.GetFx();
                }
                break;
            case 2:
                {
                    targetFx = pool.GetFx_1();
                }
                break;
            case 3:
                {
                    targetFx = pool.GetFx_2();
                }
                break;
            default:
                Debug.LogError("�ùٸ��� �ʴ� ����Ʈ Ÿ���� �ԷµǾ����ϴ�!");
                goto case 1;
        }

        // 1. ����Ʈ�� ��� ��ġ�� ����Ʈ�� �ű�� ����ϱ�
        targetFx.transform.position = position;
        targetFx.gameObject.SetActive(true);
        targetFx.Play();

        // 2. ����Ʈ ����� ������ �����ϱ�
        StartCoroutine(SetOffFx(targetFx));
    }

    // ���� �ð� �� ����Ʈ �����ϱ�
    private IEnumerator SetOffFx(ParticleSystem Fx)
    {
        yield return new WaitForSeconds(attackFXDuration);
        Fx.Stop();
        Fx.gameObject.SetActive(false);
    }


    // ����׿����� ���� ������ �����Ϳ��� �ð�ȭ
    private void OnDrawGizmos()
    {
        // �׻� ������ ǥ�� (�������� �ʾƵ�)
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // ���� �� �� ���̰� �ϱ� ���� ������ ���� �������� ���� �׸�
        Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, 1f);
        DrawCircle(transform.position, detectionRadius, 32);

        // ���� Ÿ���� ������ Ÿ������ ���� �׸�
        if (currentTarget != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, currentTarget.transform.position);
        }
    }

    // ���� ������ �� ��Ȯ�ϰ� ǥ���ϱ� ���� ���� �޼���
    private void DrawCircle(Vector3 center, float radius, int segments)
    {
        float angle = 0f;
        Vector3 lastPoint = center + new Vector3(radius, 0, 0);
        Vector3 newPoint;

        for (int i = 0; i <= segments; i++)
        {
            angle += 360f / segments;
            newPoint = center + new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad) * radius, 0, Mathf.Sin(angle * Mathf.Deg2Rad) * radius);
            Gizmos.DrawLine(lastPoint, newPoint);
            lastPoint = newPoint;
        }
    }
}