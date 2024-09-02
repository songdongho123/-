using System.Collections;
using UnityEngine;
using Photon.Pun;

public class SeedPlantingController : MonoBehaviourPunCallbacks
{
    public GameObject seedPrefab; // 스폰할 씨앗의 프리팹
    public Vector3 spawnAreaCenter; // 스폰 범위의 중심
    public float spawnRadius; // 스폰할 원의 반경
    public int numberOfSeeds; // 스폰할 씨앗 수

    private float plantingTimer = 7.0f; // 씨앗 심기 타이머 초기값

    void Update()
    {
        // 현재 조명의 X축 각도가 200 이상인 경우
        if (TimeController.Instance.GetCurrentLightAngleX() >= 200)
        {
            // 타이머 감소
            plantingTimer -= Time.deltaTime;
            // 타이머가 0 이하가 되면 씨앗을 스폰하고 타이머 재설정
            if (plantingTimer <= 0)
            {
                SpawnSeedsInCircle();
                plantingTimer = 5.0f; // 타이머 재설정
                Debug.Log("씨앗 생성");
            }
        }
    }

    // 원형 범위 내에 씨앗을 스폰하는 메서드
    void SpawnSeedsInCircle()
    {
        // 현재 클라이언트가 마스터 클라이언트인지 확인
        if (PhotonNetwork.IsMasterClient)
        {
            // 지정된 수의 씨앗을 스폰
            for (int i = 0; i < numberOfSeeds; i++)
            {
                // 랜덤 위치를 계산하여 씨앗을 스폰
                Vector3 spawnPosition = GetRandomPositionInCircle(spawnAreaCenter, spawnRadius);
                PhotonNetwork.Instantiate("seed", spawnPosition, seedPrefab.transform.rotation); // Resources 폴더의 "seed" 프리팹을 사용
            }
        }
    }

    // 주어진 중심과 반경 내에서 랜덤 위치를 반환하는 메서드
    Vector3 GetRandomPositionInCircle(Vector3 center, float radius)
    {
        // 0에서 2π 사이의 랜덤 각도 생성
        float angle = Random.Range(0, Mathf.PI * 2);
        // 0에서 주어진 반경 사이의 랜덤 거리 생성
        float distance = Random.Range(0, radius);
        // 중심점에서 랜덤 각도와 거리로 변환된 위치 반환
        return center + new Vector3(Mathf.Cos(angle) * distance, 0, Mathf.Sin(angle) * distance);
    }
}
