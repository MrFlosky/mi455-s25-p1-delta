using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class TimerSystem : MonoBehaviour
{
    public TextMeshProUGUI timerText;

    [Header("Time Values")]
    [Range(0, 60)]
    public int seconds;
    [Range(0, 60)]
    public int minutes;
    [Range(0, 60)]
    public int hours;

    public Color startColor = Color.white;
    public Color flashColor = Color.red;

    public bool showMilliseconds;

    private float currentSeconds;
    private int timerDefault;
    //public PlayerController playerController;

    private bool gameEnded = false;
    private bool isFlashing = false;
    private Coroutine flashCoroutine;

    private string timerType = "";
    private Action functionAtEnd = null;

    // Start is called before the first frame update
    void Start()
    {
        /*
        timerDefault = 0;
        timerDefault += (seconds + (minutes * 60) + (hours * 60 * 60));
        currentSeconds = timerDefault;
        timerText.color = startColor;
        */
    }

    public void SetTimer(int seconds, int minutes, int hours, string timerType, Action functionAtEnd = null)
    {
        // set the timer
        timerDefault = 0;
        timerDefault += (seconds + (minutes * 60) + (hours * 60 * 60));
        currentSeconds = timerDefault;
        
        // set the color
        timerText.color = startColor;
        
        // add the timer type
        if (timerType == "")
            this.timerType = timerType;
        else
            this.timerType = timerType + ": ";
        
        this.functionAtEnd = functionAtEnd;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isFlashing)
        {
            flashCoroutine = StartCoroutine(FlashText());
        }
        //return;

        if ((currentSeconds -= Time.deltaTime) <= 0)
        {
            TimeUp();
            //EndGame();
        }
        else
        {
            if (showMilliseconds)
            {
                timerText.text = timerType + TimeSpan.FromSeconds(currentSeconds).ToString(@"hh\:mm\:ss\:fff");
            }
            else
            {
                timerText.text = timerType + TimeSpan.FromSeconds(currentSeconds).ToString(@"hh\:mm\:ss");
            }

            if (currentSeconds <= 10f && !isFlashing)
            {
                flashCoroutine = StartCoroutine(FlashText());
            }
            else if (currentSeconds > 10f && isFlashing)
            {
                StopFlashing();
            }
        }
    }

    private void TimeUp()
    {
        if (showMilliseconds)
            timerText.text = timerType + "00:00:00:000";
        else
            timerText.text = timerType + "00:00:00";

        StopFlashing();  // Stop flashing when time is up
        timerText.color = startColor;  // Reset to default color

        if (functionAtEnd == null) return;
        
        functionAtEnd?.Invoke();
        functionAtEnd = null;
    }

    private void EndGame()
    {
        gameEnded = true;

        if (GameManager.instance != null)
        {
            //GameManager.instance.GameOver();
        }

        Time.timeScale = 0f;  // Freeze the game
        StopFlashing();
    }

    private void StopFlashing()
    {
        if (isFlashing)
        {
            StopCoroutine(flashCoroutine);
            isFlashing = false;
            timerText.color = startColor;
        }
    }

    IEnumerator FlashText()
    {
        isFlashing = true;
        while (currentSeconds > 0)
        {
            timerText.color = flashColor;
            yield return new WaitForSeconds(0.5f);
            timerText.color = startColor;
            yield return new WaitForSeconds(0.5f);
        }
        isFlashing = false;
    }
}
