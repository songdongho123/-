using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaFlower : MonoBehaviour
{
    public ParticleSystem mp_effect;

    void OnEnable()
    {
        // 오브젝트 활성화 시 IncreaseMana를 시작하고, 이후 5초마다 반복 호출
        mp_effect.Stop();
        InvokeRepeating("IncreaseMana", 0f, 5f);
    }

    void IncreaseMana()
    {
        // DryadStatus의 인스턴스에 접근하여 currentMana를 1 증가
        if (DryadStatus.Instance != null)
        {
            DryadStatus.Instance.currentMana += 2;
        }

        StartCoroutine(effect());
        
    }

    IEnumerator effect()
    {
        mp_effect.Play();
        yield return new WaitForSeconds(1.5f);
        mp_effect.Stop();
    }
}
