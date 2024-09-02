using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RaidPlayTime : MonoBehaviour
{
    public float sceneStartTime;

    public float playTime; // 씬에서 진행되고 있는 시간

    public int getTime;

    CharacterManager characterManager;

    void Start()
    {
        // 씬이 시작될 때 현재 시간 기록
        sceneStartTime = Time.time;
        characterManager = FindObjectOfType<CharacterManager>();
    }

    void Update()
    {
        if (characterManager.currentHP > 0)
        {
            playTime = Time.time - sceneStartTime;

            getTime = (int)playTime;
        }
    }
}
