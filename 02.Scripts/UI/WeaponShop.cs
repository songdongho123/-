using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class WeaponShop : MonoBehaviour
{
    private BackEndDataReceiver backEndDataReceiver;

    public TextMeshProUGUI needGoldText;  // 필요 골드 표시
    public TextMeshProUGUI needMaterialText; // 필요 재료 표시
    public Button enforceButton;
    // public Button advanceButton;
    public Image weaponImage;
    public TextMeshProUGUI buttonText;
    public TextMeshProUGUI levelText;  // 강화 레벨 표시 텍스트

    public TextMeshProUGUI GetMaterial; // 현재 보유중인 재료 1티어
    public TextMeshProUGUI GetMaterial2; // 현재 보유중인 재료 2티어
    public TextMeshProUGUI GetMaterial3; // 현재 보유중인 승급 재료 1티어
    public TextMeshProUGUI GetMaterial4; // 현재 보유중인 승급 재료 2티어
    public TextMeshProUGUI GetGold; // 현재 보유중인 골드

    public TextMeshProUGUI title;

    public CharacterManager characterManager;

    public GameObject resultModal;
    public GameObject enforceBtn;
    public TextMeshProUGUI resultModalText;

    public GameObject grade1Img;
    public GameObject grade2Img;

    public GameObject advanceStoneImg1;
    public GameObject advanceStoneImg2;


    [System.Serializable]
    public class EquipmentData
    {
        public string equipmentName; // 장비 타입
        public Sprite equipmentSprite; // 장비 이미지
        public int gearLevel;  // 현재 강화 레벨
        public int gearGrade; // 현재 승급 레벨
    }

    public List<EquipmentData> equipments; // 장비 데이터 리스트
    public Button[] equipmentButtons; // 장비 선택 버튼 배열

    private int maxLevel = 10; // 최대 강화 레벨
    private EquipmentData selectedEquipment; // 현재 선택된 장비

    private int selectedEqNum;

    // 강화시 필요한 재료 목록
    private int[] materialCosts = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 0 };
    private int[] goldCosts = { 2000, 2000, 200, 2000, 2500, 3000, 3500, 4000, 4500, 5000, 0 };

    void Start()
    {
        backEndDataReceiver = FindObjectOfType<BackEndDataReceiver>();

        equipments[0].gearLevel = backEndDataReceiver.weaponLevel;
        equipments[1].gearLevel = backEndDataReceiver.helmetLevel;
        equipments[2].gearLevel = backEndDataReceiver.armorLevel;
        equipments[3].gearLevel = backEndDataReceiver.gloveLevel;
        equipments[4].gearLevel = backEndDataReceiver.shoesLevel;
        equipments[5].gearLevel = backEndDataReceiver.cloakLevel;

        equipments[0].gearGrade = backEndDataReceiver.weaponAdvanceLevel;
        equipments[1].gearGrade = backEndDataReceiver.helmetAdvanceLevel;
        equipments[2].gearGrade = backEndDataReceiver.armorAdvanceLevel;
        equipments[3].gearGrade = backEndDataReceiver.gloveAdvanceLevel;
        equipments[4].gearGrade = backEndDataReceiver.shoesAdvanceLevel;
        equipments[5].gearGrade = backEndDataReceiver.cloakAdvanceLevel;

        selectedEquipment = equipments[0];

        if (title.text == "장비 강화")
        {
            UpdateEnforceUI(selectedEquipment.gearGrade);
        }
        else if (title.text == "장비 승급")
        {
            UpdateAdvancementUI(selectedEquipment.gearGrade);
        }

        // 버튼에 이벤트 리스너 추가
        for (int i = 0; i < equipmentButtons.Length; i++)
        {
            int index = i;
            equipmentButtons[i].onClick.AddListener(() => SelectEquipment(index));
        }

        enforceButton.onClick.AddListener(EnforceEquipment);

    }

    void Update()
    {
        if (characterManager == null)
        {
            characterManager = FindObjectOfType<CharacterManager>();
        }

        if (backEndDataReceiver == null)
        {
            backEndDataReceiver = FindObjectOfType<BackEndDataReceiver>();
        }
    }

    public void SetTitle(string type)
    {
        switch (type)
        {
            case "강화": title.text = "장비 강화"; UpdateEnforceUI(selectedEquipment.gearGrade); break;
            case "승급": title.text = "장비 승급"; UpdateAdvancementUI(selectedEquipment.gearGrade); break;
            default: break;
        }
    }

    public void SetImg(int itemGrade)
    {
        if (title.text == "장비 강화")
        {
            advanceStoneImg1.SetActive(false);
            advanceStoneImg2.SetActive(false);
            switch (itemGrade)
            {
                case 0: grade1Img.SetActive(true); grade2Img.SetActive(false); break;
                case 1: grade1Img.SetActive(false); grade2Img.SetActive(true); break;
                default: break;
            }
        }
        else if (title.text == "장비 승급")
        {
            grade1Img.SetActive(false);
            grade2Img.SetActive(false);
            switch (itemGrade)
            {
                case 0: advanceStoneImg1.SetActive(true); advanceStoneImg2.SetActive(false); break;
                case 1: advanceStoneImg1.SetActive(false); advanceStoneImg2.SetActive(true); break;
                default: break;
            }
        }
    }

    void SelectEquipment(int index)
    {
        // 선택된 장비 업데이트
        selectedEquipment = equipments[index];
        selectedEqNum = index;
        if (title.text == "장비 강화")
        {
            UpdateEnforceUI(selectedEquipment.gearGrade);
        }
        else if (title.text == "장비 승급")
        {
            UpdateAdvancementUI(selectedEquipment.gearGrade);
        }
    }

    void EnforceEquipment()
    {
        switch (title.text)
        {
            case "장비 강화":
                if (selectedEquipment.gearGrade == 0)
                {
                    Enforce(selectedEquipment.gearGrade, selectedEquipment.gearLevel, backEndDataReceiver.gold, backEndDataReceiver.enforceStone1);
                }
                else if (selectedEquipment.gearGrade == 1)
                {
                    Enforce(selectedEquipment.gearGrade, selectedEquipment.gearLevel, backEndDataReceiver.gold, backEndDataReceiver.enforceStone2);
                }
                else
                {
                    resultModal.SetActive(true);
                    enforceBtn.SetActive(false);
                    resultModalText.text = "3티어로의 강화은 제작되지 않았습니다.";
                    Debug.Log("3티어로의 강화은 제작되지 않았습니다.");
                }
                break;
            case "장비 승급":
                if (selectedEquipment.gearGrade == 0)
                {
                    advancement(selectedEquipment.gearGrade, selectedEquipment.gearLevel, backEndDataReceiver.gold, backEndDataReceiver.advanceStone1);
                }
                else if (selectedEquipment.gearGrade == 1)
                {
                    advancement(selectedEquipment.gearGrade, selectedEquipment.gearLevel, backEndDataReceiver.gold, backEndDataReceiver.advanceStone2);
                }
                else
                {
                    resultModal.SetActive(true);
                    enforceBtn.SetActive(false);
                    resultModalText.text = "3티어로의 강화은 제작되지 않았습니다.";
                    Debug.Log("3티어로의 승급은 제작되지 않았습니다.");
                }
                break;
            default: break;
        }
        if (title.text == "장비 강화")
        {
            UpdateEnforceUI(selectedEquipment.gearGrade);
        }
        else if (title.text == "장비 승급")
        {
            UpdateAdvancementUI(selectedEquipment.gearGrade);
        }

        // if (selectedEquipment.gearLevel <= maxLevel)
        // {
        //     selectedEquipment.gearLevel++;
        //             if ( title.text == "장비 강화" )
        // {
        //     UpdateEnforceUI();
        // }
        // else if ( title.text == "장비 승급")
        // {

        // }
        // }
    }

    void UpdateEnforceUI(int itemGrade)
    {
        SetImg(itemGrade);
        int level = selectedEquipment.gearLevel;
        int enforcePer = (10 - level) * 10;
        string statStr = "";
        string statInt = "";

        switch (selectedEqNum)
        {
            case 0: statStr = "공격력"; statInt = "1"; break;
            case 1: statStr = "방어력"; statInt = "1"; break;
            case 2: statStr = "최대체력"; statInt = "50"; break;
            case 3: statStr = "쿨타임"; statInt = "1%"; break;
            case 4: statStr = "이동속도"; statInt = "1"; break;
            case 5: statStr = "회피율"; statInt = "1%"; break;
            default: break;
        }

        // 선택된 장비의 강화 수치에 따라 UI 변경
        needGoldText.text = $"필요 골드 \n{goldCosts[level]}";
        needMaterialText.text = $"필요 재료 \n{materialCosts[level]}";
        weaponImage.sprite = selectedEquipment.equipmentSprite;
        levelText.text = $"현재 강화 레벨: +{level}\n강화확률 {enforcePer}%\n성공 시, {statStr} +{statInt}";
        GetGold.text = ItemManager.userItemList[0].quantity.ToString() + "골드";
        for (int i = 0; i < ItemManager.userItemList.Count; i++)
        {
            if (itemGrade == 0)
            {
                if (ItemManager.userItemList[i].itemName == "enforceStone1")
                {
                    GetMaterial.text = ItemManager.userItemList[i].quantity.ToString() + "개";
                }
            }
            else if (itemGrade == 1)
            {
                if (ItemManager.userItemList[i].itemName == "enforceStone2")
                {
                    GetMaterial2.text = ItemManager.userItemList[i].quantity.ToString() + "개";
                }
            }
        }
        // GetGold.text = backEndDataReceiver.gold.ToString();
        // GetMaterial.text = backEndDataReceiver.enforceStone1.ToString();
    }

    void UpdateAdvancementUI(int itemGrade)
    {
        SetImg(itemGrade);
        int level = selectedEquipment.gearGrade;
        int enforcePer = 100;
        string statStr = "";
        string statInt = "";

        switch (selectedEqNum)
        {
            case 0: statStr = "공격력"; statInt = "10"; break;
            case 1: statStr = "방어력"; statInt = "10"; break;
            case 2: statStr = "최대체력"; statInt = "300"; break;
            case 3: statStr = "쿨타임"; statInt = "10%"; break;
            case 4: statStr = "이동속도"; statInt = "10"; break;
            case 5: statStr = "회피율"; statInt = "10%"; break;
            default: break;
        }

        // 선택된 장비의 강화 수치에 따라 UI 변경
        needGoldText.text = $"필요 골드 \n16000";
        needMaterialText.text = $"필요 재료 \n1";
        weaponImage.sprite = selectedEquipment.equipmentSprite;
        levelText.text = $"승급 단계: Grade {level}\n성공률 {enforcePer}%\n성공 시, {statStr} +{statInt}";
        GetGold.text = ItemManager.userItemList[0].quantity.ToString() + "골드";
        for (int i = 0; i < ItemManager.userItemList.Count; i++)
        {
            if (itemGrade == 0)
            {
                if (ItemManager.userItemList[i].itemName == "UpgradeMaterial1")
                {
                    GetMaterial3.text = ItemManager.userItemList[i].quantity.ToString() + "개";
                }
            }
            else if (itemGrade == 1)
            {
                if (ItemManager.userItemList[i].itemName == "UpgradeMaterial2")
                {
                    GetMaterial4.text = ItemManager.userItemList[i].quantity.ToString() + "개";
                }
            }
        }
        // GetGold.text = backEndDataReceiver.gold.ToString();
        // GetMaterial.text = backEndDataReceiver.enforceStone1.ToString();
    }

    public bool enforceOfSuccess(int itemGrade)
    {
        // itemGrade가 0에서 1일 떄는 100퍼, ... , 9에서 10강이 10퍼
        // 1 - (itemGrade*0.1)이 강화확률
        int successRate = 10 - itemGrade;

        int randomNum = Random.Range(0, 10); // 0~9까지
        if (randomNum < successRate)
        {
            Debug.Log("강화 성공");
            return true;
        }
        else
        {
            Debug.Log("강화 실패");
            return false;
        }
    }

    public int requirements(int itemGrade)
    {
        switch (itemGrade)
        {
            case 0: return 2000;
            case 1: return 2000;
            case 2: return 2000;
            case 3: return 2000;
            case 4: return 2500;
            case 5: return 3000;
            case 6: return 3500;
            case 7: return 4000;
            case 8: return 4500;
            case 9: return 5000;
            default: return 0;

        }
    }
    public void Enforce(int itemGrade, int itemLv, int gold, int enforceStone)
    {
        if (itemLv >= 10)
        {
            Debug.Log("풀강입니다!");
            return;
        }

        if (requirements(itemLv) <= gold && itemLv + 1 <= enforceStone)
        {
            int receiveGearPk = -1;
            string receiveStatType = "";
            switch (selectedEqNum)
            {
                case 0:
                    receiveGearPk = backEndDataReceiver.myWeaponPk;
                    receiveStatType = "ATK"; break;
                case 1:
                    receiveGearPk = backEndDataReceiver.myHelmetPk;
                    receiveStatType = "DEF"; break;
                case 2:
                    receiveGearPk = backEndDataReceiver.myArmorPk;
                    receiveStatType = "maxHP"; break;
                case 3:
                    receiveGearPk = backEndDataReceiver.myGlovePk;
                    receiveStatType = "reduceCoolTime"; break;
                case 4:
                    receiveGearPk = backEndDataReceiver.myShoesPk;
                    receiveStatType = "speed"; break;
                case 5:
                    receiveGearPk = backEndDataReceiver.myCloakPk;
                    receiveStatType = "avoidanceRate"; break;
                default: break;
            }
            int receiveGearLv = equipments[selectedEqNum].gearLevel;
            int receiveGold = gold;
            int receiveEnforeceStone = enforceStone;
            int receiveStatValue = 0;

            Debug.Log(gold + "골드를 보유하여 " + requirements(itemLv) + "골드를 사용합니다.");
            Debug.Log(enforceStone + "개의 재료를 보유하고 " + (itemLv + 1) + "개의 재료를 사용합니다");
            if (ItemManager.userItemList.Count > 0)
            {
                Obj objGold = ItemManager.userItemList[0];
                objGold.quantity -= requirements(itemLv);
                receiveGold = objGold.quantity; // 사용 후 골드 적용
                ItemManager.userItemList[0] = objGold;

                for (int i = 0; i < ItemManager.userItemList.Count; i++)
                {
                    if (itemGrade == 0)
                    {
                        if (ItemManager.userItemList[i].itemName == "enforceStone1")
                        {
                            Obj objEnforceStone1 = ItemManager.userItemList[i];
                            objEnforceStone1.quantity -= (itemLv + 1);
                            receiveEnforeceStone = objEnforceStone1.quantity; // 사용 후 재료 적용
                            ItemManager.userItemList[i] = objEnforceStone1;
                        }
                    }
                    else if (itemGrade == 1)
                    {
                        if (ItemManager.userItemList[i].itemName == "enforceStone2")
                        {
                            Obj objEnforceStone2 = ItemManager.userItemList[i];
                            objEnforceStone2.quantity -= (itemLv + 1);
                            receiveEnforeceStone = objEnforceStone2.quantity; // 사용 후 재료 적용
                            ItemManager.userItemList[i] = objEnforceStone2;
                        }
                    }
                }
            }

            //ItemManager.userItemList[0].quantity -= requirements(itemGrade);
            //backEndDataReceiver.enforceStone1 -= (itemGrade + 1);

            if (enforceOfSuccess(itemLv))
            {
                //selectedEquipment.gearLevel++;
                equipments[selectedEqNum].gearLevel++;

                switch (selectedEqNum)
                {
                    case 0: // 무기 -> 공격력 1증가
                        receiveGearLv = ++backEndDataReceiver.weaponLevel;
                        receiveStatValue = ++characterManager.ATK;
                        break;
                    case 1: // 헬맷 -> 방어력 1증가
                        receiveGearLv = ++backEndDataReceiver.helmetLevel;
                        receiveStatValue = ++characterManager.DEF;
                        break;
                    case 2: // 아머 -> 체력 50증가
                        receiveGearLv = ++backEndDataReceiver.armorLevel;
                        characterManager.maxHP += 50;
                        receiveStatValue = characterManager.maxHP;
                        break;
                    case 3: // 글러브 -> 쿨감 1증가
                        receiveGearLv = ++backEndDataReceiver.gloveLevel;
                        receiveStatValue = ++characterManager.reduceCoolTime;
                        break;
                    case 4: // 신발 -> 이속 1증가
                        receiveGearLv = ++backEndDataReceiver.shoesLevel;
                        receiveStatValue = ++characterManager.speed;
                        break;
                    case 5: // 망토 -> 회피율 1증가
                        receiveGearLv = ++backEndDataReceiver.cloakLevel;
                        receiveStatValue = ++characterManager.avoidanceRate;
                        break;
                    default: break;
                }

                selectedEquipment = equipments[selectedEqNum];
                backEndDataReceiver.enforceTry(receiveGearPk, receiveGearLv, receiveGold, 3, receiveEnforeceStone, true, receiveStatType, receiveStatValue);
                resultModal.SetActive(true);
                enforceBtn.SetActive(false);
                resultModalText.text = "+" + selectedEquipment.gearLevel + " " + selectedEquipment.equipmentName + " 강화에 성공하셨습니다.";
            }
            else
            {
                backEndDataReceiver.enforceTry(receiveGearPk, receiveGearLv, receiveGold, 3, receiveEnforeceStone, false, receiveStatType, receiveStatValue);
                resultModal.SetActive(true);
                enforceBtn.SetActive(false);
                resultModalText.text = "+" + selectedEquipment.gearLevel + " " + selectedEquipment.equipmentName + " 강화에 실패하셨습니다.";
            }
        }
        else
        {
            if (gold - requirements(itemLv) < 0)
            {
                resultModal.SetActive(true);
                enforceBtn.SetActive(false);
                resultModalText.text = (requirements(itemLv) - gold) + "골드가 모자랍니다.";
                Debug.Log((requirements(itemLv) - gold) + "골드가 모자랍니다.");
            }
            if ((enforceStone - (itemLv + 1)) < 0)
            {
                resultModal.SetActive(true);
                enforceBtn.SetActive(false);
                resultModalText.text = "재료가 " + ((itemLv + 1) - enforceStone) + "개 모자랍니다.";
                Debug.Log("재료가 " + ((itemLv + 1) - enforceStone) + "개 모자랍니다.");
            }
        }
    }

    public void ModalSetFalse()
    {
        resultModal.SetActive(false);
        enforceBtn.SetActive(true);
    }

    // 승급
    public void advancement(int itemGrade, int itemLv, int gold, int advanceStone)
    {
        if (itemLv < 5)
        {
            resultModal.SetActive(true);
            enforceBtn.SetActive(false);
            resultModalText.text = "5강 미만은 승급을 진행할 수 없습니다.";
            Debug.Log("5강 미만은 승급이 안되요잉");
            return;
        }

        if (16000 <= gold && 1 <= advanceStone)
        {
            int receiveGearPk = -1;
            string receiveStatType = "";
            switch (selectedEqNum)
            {
                case 0:
                    receiveGearPk = backEndDataReceiver.myWeaponPk;
                    receiveStatType = "ATK"; break;
                case 1:
                    receiveGearPk = backEndDataReceiver.myHelmetPk;
                    receiveStatType = "DEF"; break;
                case 2:
                    receiveGearPk = backEndDataReceiver.myArmorPk;
                    receiveStatType = "maxHP"; break;
                case 3:
                    receiveGearPk = backEndDataReceiver.myGlovePk;
                    receiveStatType = "reduceCoolTime"; break;
                case 4:
                    receiveGearPk = backEndDataReceiver.myShoesPk;
                    receiveStatType = "speed"; break;
                case 5:
                    receiveGearPk = backEndDataReceiver.myCloakPk;
                    receiveStatType = "avoidanceRate"; break;
                default: break;
            }

            int receiveGearGrade = equipments[selectedEqNum].gearGrade + 1;
            int receiveGearLv = equipments[selectedEqNum].gearLevel;
            int receiveGold = gold;
            int receiveAdvanceStone1 = advanceStone;
            int receiveStatValue = 0;

            int receiveItemPk = 3;

            Debug.Log(gold + "골드를 보유하여 " + 16000 + "골드를 사용합니다.");
            Debug.Log(advanceStone + "개의 재료를 보유하고 " + 1 + "개의 재료를 사용합니다");
            if (ItemManager.userItemList.Count > 0)
            {
                Obj objGold = ItemManager.userItemList[0];
                objGold.quantity -= 16000;
                receiveGold = objGold.quantity; // 사용 후 골드 적용
                ItemManager.userItemList[0] = objGold;

                for (int i = 0; i < ItemManager.userItemList.Count; i++)
                {
                    if (itemGrade == 0)
                    {
                        if (ItemManager.userItemList[i].itemName == "UpgradeMaterial1")
                        {
                            Obj objAdvanceStone1 = ItemManager.userItemList[i];
                            objAdvanceStone1.quantity -= 1;
                            receiveAdvanceStone1 = objAdvanceStone1.quantity; // 사용 후 재료 적용
                            ItemManager.userItemList[i] = objAdvanceStone1;
                        }
                        receiveItemPk = 3;
                    }
                    else if (itemGrade == 1)
                    {
                        if (ItemManager.userItemList[i].itemName == "UpgradeMaterial2")
                        {
                            Obj objAdvanceStone2 = ItemManager.userItemList[i];
                            objAdvanceStone2.quantity -= 1;
                            receiveAdvanceStone1 = objAdvanceStone2.quantity; // 사용 후 재료 적용
                            ItemManager.userItemList[i] = objAdvanceStone2;
                        }
                        receiveItemPk = 4;
                    }
                    else
                    {
                        resultModal.SetActive(true);
                        enforceBtn.SetActive(false);
                        resultModalText.text = equipments[selectedEqNum].gearGrade + "단계는 최대 승급 수치입니다.";
                        Debug.Log(equipments[selectedEqNum].gearGrade + "승급 수치이므로 강화를 진행안해야함.");
                        return;
                    }
                }

            }

            equipments[selectedEqNum].gearGrade++;
            equipments[selectedEqNum].gearLevel -= 5;

            switch (selectedEqNum)
            {
                case 0: // 무기 -> 공격력 100 + 1레벨 당 10 증가
                    backEndDataReceiver.weaponLevel -= 5;
                    receiveGearLv = backEndDataReceiver.weaponLevel;
                    characterManager.ATK += 10;
                    receiveStatValue = characterManager.ATK;
                    break;
                case 1: // 헬맷 -> 방어력 100 + 1레벨 당 10 증가
                    backEndDataReceiver.helmetLevel -= 5;
                    receiveGearLv = backEndDataReceiver.helmetLevel;
                    characterManager.DEF += 10;
                    receiveStatValue = characterManager.DEF;
                    break;
                case 2: // 아머 -> 체력 50 ,,, 처리필요
                    backEndDataReceiver.armorLevel -= 5;
                    receiveGearLv = backEndDataReceiver.armorLevel;
                    characterManager.maxHP += 300;
                    receiveStatValue = characterManager.maxHP;
                    break;
                case 3: // 글러브 -> 쿨감 20 + 1레벨 당 2% 증가
                    backEndDataReceiver.gloveLevel -= 5;
                    receiveGearLv = backEndDataReceiver.gloveLevel;
                    characterManager.reduceCoolTime += 10;
                    receiveStatValue = characterManager.reduceCoolTime;
                    break;
                case 4: // 신발 -> 이속 10 증가 + 1레벨 당 1 증가
                    backEndDataReceiver.shoesLevel -= 5;
                    receiveGearLv = backEndDataReceiver.shoesLevel;
                    characterManager.speed += 10;
                    receiveStatValue = characterManager.speed;
                    break;
                case 5: // 망토 -> 회피율 20 증가 + 1레벨 당 2% 증가
                    backEndDataReceiver.cloakLevel -= 5;
                    receiveGearLv = backEndDataReceiver.cloakLevel;
                    characterManager.avoidanceRate += 10;
                    receiveStatValue = characterManager.avoidanceRate;
                    break;
                default: break;
            }

            selectedEquipment = equipments[selectedEqNum];
            backEndDataReceiver.advancementTry(receiveGearPk, receiveGearGrade, receiveGearLv, receiveGold, receiveItemPk, receiveAdvanceStone1, true, receiveStatType, receiveStatValue);
            resultModal.SetActive(true);
            enforceBtn.SetActive(false);
            resultModalText.text = "승급에 성공했습니다!";
        }
        else
        {
            if (16000 > gold)
            {
                resultModal.SetActive(true);
                enforceBtn.SetActive(false);
                resultModalText.text = 16000 - gold + "골드가 모자랍니다.";
                Debug.Log(16000 - gold + "골드가 모자랍니다.");
            }
            if (1 > advanceStone)
            {
                resultModal.SetActive(true);
                enforceBtn.SetActive(false);
                resultModalText.text = "승급석이 없습니다.";
                Debug.Log("승급석이 없습니다.");
            }
        }
    }
}
