using UnityEngine;

public class MoveToPosition : MonoBehaviour
{
    // Inspector에서 할당할 목표 위치의 Transform
    [SerializeField] private Transform targetPosition;

    // Inspector에서 수정 가능한 이동 속도
    [SerializeField] private float moveSpeed = 5f;

    // 이동이 완료되었는지 체크하는 플래그
    private bool isMoving = false;

    // Start는 최초 생성 시 호출됩니다
    void Start()
    {
        // targetPosition이 설정되어 있는지 확인
        if (targetPosition != null)
        {
            // 이동 시작
            isMoving = true;
        }
        else
        {
            Debug.LogWarning("Target Position이 설정되지 않았습니다!");
        }
    }

    // Update는 매 프레임마다 호출됩니다
    void Update()
    {
        if (isMoving && targetPosition != null)
        {
            // 현재 위치에서 목표 위치로 이동
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition.position,
                moveSpeed * Time.deltaTime
            );

            // 목표 위치에 도달했는지 확인
            if (Vector3.Distance(transform.position, targetPosition.position) < 0.001f)
            {
                // 정확한 위치에 배치
                transform.position = targetPosition.position;
                isMoving = false;

                // 도착했음을 알림
                Debug.Log($"{gameObject.name}가 목표 위치에 도착했습니다.");
            }
        }
    }

    // 런타임 중에 목표 위치를 변경하기 위한 메서드
    public void SetTargetPosition(Transform newTarget)
    {
        targetPosition = newTarget;
        if (targetPosition != null)
        {
            isMoving = true;
        }
    }

    // 이동 속도를 런타임 중에 변경하기 위한 메서드
    public void SetMoveSpeed(float newSpeed)
    {
        moveSpeed = newSpeed;
    }
}