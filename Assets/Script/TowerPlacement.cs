using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class TowerPlacement : MonoBehaviour
{
    [Header("Tower Settings")]
    [SerializeField] private LayerMask groundLayer; // Ground 레이어 설정
    [SerializeField] private LayerMask playerCharacterLayer; // PlayerCharacter 레이어 설정
    [SerializeField] private float maxPlaceDistance = 100f; // 최대 배치 거리
    [SerializeField] private Material validPlacementMaterial; // 배치 가능 시 표시할 재질
    [SerializeField] private Material invalidPlacementMaterial; // 배치 불가능 시 표시할 재질
    [SerializeField] private GameObject placementIndicator; // 배치 위치 표시기

    public List<PlayerCharacterData> PlayerCharacterDataList = new List<PlayerCharacterData>();

    private Camera mainCamera;
    private bool canPlaceTower = false;
    private RaycastHit hitInfo;

    // 타워 선택 및 배치 상태
    private PlayerCharacterType? selectedTowerType = null;
    private bool isPlacementMode = false;
    private PlayerCharacter selectedPlayerCharacter = null;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        HandleKeyboardInput();
        HandleMouseInput();
        HandleMouseOver();
    }

    // 키보드 입력 처리 (1~5번 키로 타워 선택)
    private void HandleKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
        {
            SelectTowerForPlacement(PlayerCharacterType.Melee_1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
        {
            SelectTowerForPlacement(PlayerCharacterType.Melee_2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
        {
            SelectTowerForPlacement(PlayerCharacterType.Range_1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
        {
            SelectTowerForPlacement(PlayerCharacterType.Range_2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5))
        {
            SelectTowerForPlacement(PlayerCharacterType.Range_3);
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            // ESC 키로 선택 취소
            CancelSelection();
        }
    }

    // 마우스 입력 처리
    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0)) // 왼클릭
        {
            if (isPlacementMode && selectedTowerType.HasValue)
            {
                // 배치 모드일 때 타워 배치
                PlaceTower(selectedTowerType.Value);
            }
            else
            {
                // 기존 PlayerCharacter 선택
                HandlePlayerCharacterSelection();
            }
        }
        else if (Input.GetMouseButtonDown(1)) // 우클릭
        {
            // 우클릭으로 선택 취소
            CancelSelection();
        }
    }

    // 타워 선택 (키보드 입력 시)
    private void SelectTowerForPlacement(PlayerCharacterType type)
    {
        selectedTowerType = type;
        isPlacementMode = true;
        selectedPlayerCharacter = null;

        // 선택된 타워 정보를 UI에 표시
        DisplayTowerInfo(type);

        // 업그레이드 버튼 비활성화 (새로 배치할 타워이므로)
        ButtonController.Button.SelectedPlayerCharacter = null;
        ButtonController.Button.SetUpgradeButtonStatus(false);

        Debug.Log($"Selected tower type: {type} for placement");
    }

    // 기존 PlayerCharacter 선택 처리
    private void HandlePlayerCharacterSelection()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, maxPlaceDistance))
        {
            PlayerCharacter playerCharacter = hit.collider.GetComponent<PlayerCharacter>();

            if (playerCharacter != null)
            {
                // 기존 PlayerCharacter 선택
                selectedPlayerCharacter = playerCharacter;
                selectedTowerType = null;
                isPlacementMode = false;

                // 선택된 PlayerCharacter 정보를 UI에 표시
                DisplayPlayerCharacterInfo(playerCharacter);

                // 업그레이드 버튼 설정
                ButtonController.Button.SelectedPlayerCharacter = playerCharacter;
                ButtonController.Button.SetUpgradeButtonStatus(playerCharacter.CanUpgrade());

                Debug.Log($"Selected existing PlayerCharacter: {playerCharacter.name}");
            }
            else
            {
                // 빈 곳을 클릭한 경우 선택 취소
                CancelSelection();
            }
        }
    }

    // 선택 취소
    private void CancelSelection()
    {
        selectedTowerType = null;
        isPlacementMode = false;
        selectedPlayerCharacter = null;
        placementIndicator.SetActive(false);

        ButtonController.Button.SetUpgradeButtonStatus(false);
        Debug.Log("Selection cancelled");
    }

    // 마우스 오버 처리
    private void HandleMouseOver()
    {
        if (!isPlacementMode)
        {
            placementIndicator.SetActive(false);
            return;
        }

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
                if (( playerCharacterLayer & ( 1 << targetCollider.gameObject.layer ) ) != 0)
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

    // 타워 생성 함수
    private void PlaceTower(PlayerCharacterType type)
    {
        if (canPlaceTower == false)
        {
            TextController.Text.SetMessageText(MessageType.PlacementConflict);
            return;
        }

        if ((int)type < 0 || (int)type > (int)PlayerCharacterType.Max)
        {
            Debug.LogError("Tower index out of range!");
            return;
        }

        PlayerCharacter newPlayerCharacter = PoolManager.Pool.GetPlayerCharacter(type);

        if (newPlayerCharacter.HasBuildableGold() == false)
        {
            TextController.Text.SetMessageText(MessageType.NotEnoughGold);
            return;
        }

        newPlayerCharacter.Initialize(hitInfo.point);

        // 배치 완료 후 선택 취소
        CancelSelection();

        Debug.Log($"Tower {type} placed successfully!");
    }

    // 타워 정보 표시 (키보드로 선택한 타워)
    private void DisplayTowerInfo(PlayerCharacterType type)
    {
        // PlayerCharacterData를 가져와서 UI에 표시
        PlayerCharacterData data = PlayerCharacterDataList[(int)type];
        string towerName = type.ToString();

        TextController.Text.SetPlayerInformationText(towerName, data);
    }

    // 기존 PlayerCharacter 정보 표시
    private void DisplayPlayerCharacterInfo(PlayerCharacter playerCharacter)
    {
        PlayerCharacterData data = playerCharacter.GetPlayerCharacterData();
        string name = playerCharacter.name;

        TextController.Text.SetPlayerInformationText(name, data);
    }


    // 공개 메서드: 업그레이드 버튼에서 호출
    public void UpgradeSelectedPlayerCharacter()
    {
        if (selectedPlayerCharacter != null && selectedPlayerCharacter.CanUpgrade())
        {
            bool upgradeSuccess = selectedPlayerCharacter.Upgrade();

            if (upgradeSuccess)
            {
                // 업그레이드 성공 시 UI 정보 갱신
                DisplayPlayerCharacterInfo(selectedPlayerCharacter);

                ButtonController.Button.SetUpgradeButtonStatus(selectedPlayerCharacter.CanUpgrade());
                Debug.Log($"PlayerCharacter {selectedPlayerCharacter.name} upgraded successfully!");
            }
            else
            {
                // 업그레이드 실패 (골드 부족 등)
                TextController.Text.SetMessageText(MessageType.NotEnoughGold);
            }
        }
    }
}