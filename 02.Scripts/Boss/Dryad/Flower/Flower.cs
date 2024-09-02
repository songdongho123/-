using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Flower : MonoBehaviourPunCallbacks
{
    public GameObject flower1; // 첫 번째 꽃 오브젝트
    public GameObject flower2; // 두 번째 꽃 오브젝트
    public GameObject flower3; // 세 번째 꽃 오브젝트
    public ParticleSystem flower_effect; // 꽃 이펙트

    private Renderer seedRenderer; // 씨앗의 렌더러
    private bool isBlossom = false; 
    void Start()
    {
        seedRenderer = GetComponent<Renderer>(); // 씨앗의 렌더러 컴포넌트를 가져옴
        flower_effect.Stop(); // 꽃 이펙트 초기화
    }

    void Update()
    {
        float currentAngle = TimeController.Instance.GetCurrentLightAngleX(); // 현재 조명 각도 가져오기

        if (PhotonNetwork.IsMasterClient)
        {
            // 현재 각도가 50에서 90 사이이고 꽃이 아직 피지 않았다면 꽃을 피우기
            if (currentAngle >= 50 && currentAngle <= 90 && !isBlossom)
            {
                GrowRandomFlower();
                isBlossom = true;
            }
            // 현재 각도가 50보다 작거나 90보다 크다면 플래그 초기화
            else if (currentAngle < 50 || currentAngle > 90)
            {
                isBlossom = false;
            }
        }
    }

    // 랜덤하게 하나의 꽃 피우기
    void GrowRandomFlower()
    {
        // 모든 꽃을 비활성화
        flower1.SetActive(false);
        flower2.SetActive(false);
        flower3.SetActive(false);

        // 랜덤하게 하나의 꽃을 활성화
        // 0, 1, 2 중 하나 랜덤 선택
        int randomIndex = Random.Range(0, 3);
        switch(randomIndex)
        {
            case 0:
                flower1.SetActive(true);
                break;
            case 1:
                flower2.SetActive(true);
                break;
            case 2:
                flower3.SetActive(true);
                break;
        }

        // 씨앗의 렌더러를 비활성화 해 씨앗 숨김
        if (seedRenderer != null)
            seedRenderer.enabled = false;

        // RPC 통신을 통해 꽃 생성 동기화
        photonView.RPC("GrowRandomFlowerRPC", RpcTarget.Others, randomIndex);
    }

    // RPC로 호출될 꽃 생성 메서드
    [PunRPC]
    void GrowRandomFlowerRPC(int randomIndex)
    {
        // 모든 꽃을 비활성화
        flower1.SetActive(false);
        flower2.SetActive(false);
        flower3.SetActive(false);

        // 랜덤하게 하나의 꽃을 활성화
        // 0, 1, 2 중 하나 랜덤 선택
        switch(randomIndex)
        {
            case 0:
                flower1.SetActive(true);
                break;
            case 1:
                flower2.SetActive(true);
                break;
            case 2:
                flower3.SetActive(true);
                break;
        }

        // 씨앗의 렌더러를 비활성화 해 씨앗 숨김
        if (seedRenderer != null)
            seedRenderer.enabled = false;
    }

    // 꽃 피는 이펙트를 재생하는 코루틴
    IEnumerator Effect()
    {
        flower_effect.Play(); // 꽃 피는 이펙트 재생
        yield return new WaitForSeconds(2); // 2초 대기
        flower_effect.Stop(); // 꽃 피는 이펙트 중지
    }
}
