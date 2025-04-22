using UnityEngine;

public class MoveToPosition : MonoBehaviour
{
    // Inspector���� �Ҵ��� ��ǥ ��ġ�� Transform
    [SerializeField] private Transform targetPosition;

    // Inspector���� ���� ������ �̵� �ӵ�
    [SerializeField] private float moveSpeed = 5f;

    // �̵��� �Ϸ�Ǿ����� üũ�ϴ� �÷���
    private bool isMoving = false;

    // Start�� ���� ���� �� ȣ��˴ϴ�
    void Start()
    {
        // targetPosition�� �����Ǿ� �ִ��� Ȯ��
        if (targetPosition != null)
        {
            // �̵� ����
            isMoving = true;
        }
        else
        {
            Debug.LogWarning("Target Position�� �������� �ʾҽ��ϴ�!");
        }
    }

    // Update�� �� �����Ӹ��� ȣ��˴ϴ�
    void Update()
    {
        if (isMoving && targetPosition != null)
        {
            // ���� ��ġ���� ��ǥ ��ġ�� �̵�
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition.position,
                moveSpeed * Time.deltaTime
            );

            // ��ǥ ��ġ�� �����ߴ��� Ȯ��
            if (Vector3.Distance(transform.position, targetPosition.position) < 0.001f)
            {
                // ��Ȯ�� ��ġ�� ��ġ
                transform.position = targetPosition.position;
                isMoving = false;

                // ���������� �˸�
                Debug.Log($"{gameObject.name}�� ��ǥ ��ġ�� �����߽��ϴ�.");
            }
        }
    }

    // ��Ÿ�� �߿� ��ǥ ��ġ�� �����ϱ� ���� �޼���
    public void SetTargetPosition(Transform newTarget)
    {
        targetPosition = newTarget;
        if (targetPosition != null)
        {
            isMoving = true;
        }
    }

    // �̵� �ӵ��� ��Ÿ�� �߿� �����ϱ� ���� �޼���
    public void SetMoveSpeed(float newSpeed)
    {
        moveSpeed = newSpeed;
    }
}