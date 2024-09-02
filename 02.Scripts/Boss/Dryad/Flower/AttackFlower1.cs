using UnityEngine;

public class AttackFlower1 : MonoBehaviour
{
    public float launchInterval = 5.0f;  // 발사 간격 (초 단위)

    void Start()
    {
        // launchInterval 간격으로 "LaunchMissile" 메서드를 반복 호출
        InvokeRepeating("LaunchMissile", launchInterval, launchInterval);
    }

    // 미사일 발사 메서드
    void LaunchMissile()
    {
        // "Player" 태그를 가진 게임 오브젝트 찾기
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            // ObjectPoolManager를 통해 미사일 오브젝트 가져오기
            GameObject missile = ObjectPoolManager.Instance.GetMissile();
            // 미사일의 위치를 현재 오브젝트의 위치로 설정
            missile.transform.position = transform.position;  
            // 플레이어를 향한 방향 계산
            Vector3 direction = (player.transform.position - transform.position).normalized;
            // 미사일의 속도를 방향과 속도로 설정
            missile.GetComponent<Rigidbody>().velocity = direction * 20;
        }
    }
}
