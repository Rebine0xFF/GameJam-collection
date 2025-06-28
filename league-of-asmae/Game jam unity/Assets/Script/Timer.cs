using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    private float StartTime;

    private float currentTime;
    
    private float maxTime;
    
    public Text timerText;

    public NewBox death;

    public Collider2D Player;
    
    void Start()
    {
        StartTime = Time.time;
        maxTime = 90;
        currentTime = 0;
    }

    void Update()
    {
        currentTime = maxTime - (Time.time - StartTime);
        int min = (int)(currentTime / 60);
        int sec = (int)(currentTime % 60);
        timerText.text = Mathf.Ceil(min) + ":" + Mathf.Ceil(sec);
        if (currentTime <= 0)
        {
            death.GameOverScreeen(Player);
        }
    }

    public void ResetTimer()
    {
        StartTime = Time.time;
    }
}
