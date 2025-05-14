using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerPlacement : MonoBehaviour
{
    [Header("Tower Settings")]
    [SerializeField] private LayerMask groundLayer; // Ground 레이어 설정
    [SerializeField] private LayerMask playerCharacterLayer; // Ground 레이어 설정
    [SerializeField] private float maxPlaceDistance = 100f; // 최대 배치 거리
    [SerializeField] private Material validPlacementMaterial; // 배치 가능 시 표시할 재질
    [SerializeField] private Material invalidPlacementMaterial; // 배치 불가능 시 표시할 재질
    [SerializeField] private GameObject placementIndicator; // 배치 위치 표시기

    private Camera mainCamera;
    private bool canPlaceTower = false;
    private RaycastHit hitInfo;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        HandleMouseOver();
        HandleTowerPlacement();
    }

    // 마우스 오버 처리
    private void HandleMouseOver()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hitInfo, maxPlaceDistance, groundLayer))
        {
            // Ground 레이어에 마우스 오버 시
            canPlaceTower = true;
            placementIndicator.SetActive(true);
            placementIndicator.transform.position = new Vector3(hitInfo.point.x, hitInfo.point.y + 0.05f, hitInfo.point.z);

            // Ground에 이미 타워가 있는지 확인
            Collider[] targetColliderArray = Physics.OverlapSphere(hitInfo.point, 0.5f);
            bool isOccupied = false;

            foreach (Collider targetCollider in targetColliderArray)
            {
                if ((playerCharacterLayer & ( 1 << targetCollider.gameObject.layer ) ) != 0)
                {
                    isOccupied = true;
                    break;
                }
            }

            // 배치 가능 여부에 따라 표시기 색상 변경
            if (isOccupied)
            {
                canPlaceTower = false;
                placementIndicator.GetComponent<Renderer>().material = invalidPlacementMaterial;
            }
            else
            {
                placementIndicator.GetComponent<Renderer>().material = validPlacementMaterial;
            }
        }
        else
        {
            // Ground 레이어가 아닌 곳에 마우스 오버 시
            canPlaceTower = false;
            placementIndicator.SetActive(false);
        }
    }

    // 타워 배치 처리
    private void HandleTowerPlacement()
    {
        if (!canPlaceTower) return;

        // 1~4번 키를 눌러 해당 타워 배치
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
        {
            PlaceTower(PlayerCharacterType.Melee_1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
        {
            PlaceTower(PlayerCharacterType.Melee_2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
        {
            PlaceTower(PlayerCharacterType.Range_1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
        {
            PlaceTower(PlayerCharacterType.Range_2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5))
        {
            PlaceTower(PlayerCharacterType.Range_3);
        }
    }

    // 타워 생성 함수
    private void PlaceTower(PlayerCharacterType type)
    {
        if ((int)type < 0 || (int)type > (int)PlayerCharacterType.Max)
        {
            Debug.LogError("Tower index out of range!");
            return;
        }

        PlayerCharacter newPlayerCharacter = PoolManager.Pool.GetPlayerCharacter(type);
        newPlayerCharacter.Initialize(hitInfo.point);
    }
}