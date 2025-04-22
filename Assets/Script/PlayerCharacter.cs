using UnityEngine;
using System.Collections.Generic;

public class PlayerCharacter : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField] private float detectionRadius = 5f;
    [SerializeField] private LayerMask targetLayer;  // Inspector에서 공격할 대상의 레이어를 설정
    [SerializeField] private Color gizmoColor = Color.red;  // Gizmo 색상

    [Header("Attack Settings")]
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private GameObject attackFXPrefab; // 공격 이펙트 프리팹
    [SerializeField] private float attackFXDuration = 1f; // 이펙트 지속 시간

    private GameObject currentTarget;
    private float nextAttackTime;

    private void Update()
    {
        // 현재 타겟이 없거나, 죽었거나, 범위를 벗어난 경우 새로운 타겟 찾기
        if (currentTarget == null ||
            !IsTargetAlive(currentTarget) ||
            !IsTargetInRange(currentTarget))
        {
            FindNewTarget();
        }

        // 타겟이 있고 공격 가능한 경우 공격
        if (currentTarget != null && Time.time >= nextAttackTime)
        {
            AttackTarget();
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    // 타겟이 공격 범위 내에 있는지 확인하는 함수
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

            // 자기 자신은 제외
            if (target == gameObject)
            {
                continue;
            }

            // 타겟이 살아있는지 확인
            if (!IsTargetAlive(target))
            {
                continue;
            }

            // 가장 가까운 타겟 선택
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
        // 타겟의 Enemy 컴포넌트를 확인
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

            // 공격 이펙트 생성
            SpawnAttackFX(currentTarget.transform.position);

            // 공격 후 타겟이 죽었는지 확인
            if (!enemy.IsAlive())
            {
                currentTarget = null;
            }
        }
    }

    // 공격 이펙트를 생성하는 함수
    private void SpawnAttackFX(Vector3 position)
    {
        // 이펙트 프리팹이 설정되었는지 확인
        if (attackFXPrefab != null)
        {
            // 이펙트 프리팹 인스턴스화
            GameObject fxInstance = Instantiate(attackFXPrefab, position, Quaternion.identity);

            // 일정 시간 후 이펙트 제거
            Destroy(fxInstance, attackFXDuration);
        }
    }

    // 디버그용으로 감지 범위를 에디터에서 시각화
    private void OnDrawGizmos()
    {
        // 항상 범위를 표시 (선택하지 않아도)
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // 선을 더 잘 보이게 하기 위해 불투명도 높은 색상으로 선을 그림
        Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, 1f);
        DrawCircle(transform.position, detectionRadius, 32);

        // 현재 타겟이 있으면 타겟으로 선을 그림
        if (currentTarget != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, currentTarget.transform.position);
        }
    }

    // 원형 범위를 더 명확하게 표시하기 위한 헬퍼 메서드
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