using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
//using UnityEditor.AssetImporters;
using UnityEngine;
using UnityEngine.UI;

public class NewBox : MonoBehaviour
{
    public GameObject GameOver;
    public GameObject GameWin;
    private int box;
    
    public Text BoxText;

    public Text TimerText;

    private bool b;

    void Start()
    {
        box = 4;
        b = true;
    }

    void Update()
    {
        BoxText.text = Mathf.Ceil(box).ToString();
        if (box == 0)
        {
            GameWin.SetActive(true);
        }
    }
    
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Box") && b)
        {
            b =  false;
            other.transform.position = new Vector2(-7.69f, -0.07f);
            other.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            box--;
            StartCoroutine(Wait());
        }
        else if (other.gameObject.CompareTag("Player"))
        {
            GameOverScreeen(other);
        }
    }
    
    IEnumerator Wait()
    {
        yield return new WaitForSeconds(2);
        b = true;
    }
    
    public void GameOverScreeen(Collider2D Player)
    {
        GameOver.SetActive(true);
        BoxText.gameObject.SetActive(false);
        var timer = TimerText.GetComponent<Timer>();
        timer.ResetTimer();
        TimerText.gameObject.SetActive(false);
        Player.transform.position = new Vector2(-2.34f, -0.5f);
        Player.gameObject.SetActive(false);
        StartCoroutine(ExampleCoroutine(Player));
    }
    
    IEnumerator ExampleCoroutine(Collider2D Player)
    {
        yield return new WaitForSeconds(3);
        Player.gameObject.SetActive(true);
        GameOver.SetActive(false);
        GameWin.SetActive(false);
        BoxText.gameObject.SetActive(true);
        TimerText.gameObject.SetActive(true);
        TimerText.GetComponent<Timer>().ResetTimer();
        ResetBox(5);
    }

    public void ResetBox(int i)
    {
        box = i;
    }
}
