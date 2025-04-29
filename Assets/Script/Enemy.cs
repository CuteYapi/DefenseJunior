using UnityEngine;

public class Enemy : MonoBehaviour
{
    // HP 값
    [SerializeField] private float hp;
    [SerializeField] private float maxHp;

    // Inspector에서 할당할 목표 위치의 Transform
    [SerializeField] private Transform targetPosition;

    // Inspector에서 수정 가능한 이동 속도
    [SerializeField] private float moveSpeed = 5f;

    // 이동이 완료되었는지 체크하는 플래그
    private bool isMoving = false;

    // 살아있는지 체크하는 플래그
    private bool isAlive = true;

    // Start는 최초 생성 시 호출됩니다
    void Start()
    {
        // targetPosition이 설정되어 있는지 확인
        if (targetPosition != null)
        {
            // 이동 시작
            StatusReset();
        }
        else
        {
            Debug.LogWarning("Target Position이 설정되지 않았습니다!");
        }
    }

    // Update는 매 프레임마다 호출됩니다
    void Update()
    {
        // 살아있을 때만 이동
        if (isAlive && isMoving && targetPosition != null)
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

    public void StatusReset()
    {
        hp = maxHp;

        isAlive = true;
        isMoving = true;

        Debug.Log($"{gameObject.name}이(가) 초기화됐습니다.");

        // 게임 오브젝트 활성화
        gameObject.SetActive(true);
    }

    // 데미지를 받는 함수
    public void Damaged(float damage)
    {
        // 이미 죽었다면 데미지를 받지 않음
        if (!isAlive) return;

        // HP 감소
        hp -= damage; // hp = hp - damaged;

        Debug.Log($"{gameObject.name}이(가) {damage}의 데미지를 받았습니다. 남은 HP: {hp}");

        // HP가 0 이하가 되면 Dead 함수 호출
        if (hp <= 0)
        {
            hp = 0;
            Dead();
        }
    }

    // 사망 처리 함수
    private void Dead()
    {
        isAlive = false;
        isMoving = false;

        Debug.Log($"{gameObject.name}이(가) 사망했습니다.");

        // 게임 오브젝트 비활성화
        gameObject.SetActive(false);
    }

    // 런타임 중에 목표 위치를 변경하기 위한 메서드
    public void SetTargetPosition(Transform newTarget)
    {
        targetPosition = newTarget;
        if (targetPosition != null && isAlive)
        {
            isMoving = true;
        }
    }

    // 이동 속도를 런타임 중에 변경하기 위한 메서드
    public void SetMoveSpeed(float newSpeed)
    {
        moveSpeed = newSpeed;
    }

    // HP를 반환하는 메서드
    public float GetHp()
    {
        return hp;
    }

    // 생존 여부를 반환하는 메서드
    public bool IsAlive()
    {
        return isAlive;
    }
}