using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System;

public class ScoreboardDisplay : UIelement
{
    [Header("References")]
    [Tooltip("The text to use when displaying the score")]
    public TextMeshProUGUI displayText = null;

    // Start is called before the first frame update
    public void DisplayScore()
    {
        string sortedListText = "Survived Time:\n";
        if (GameManager.instance.balls != null)
        {
            foreach (Ball ball in GameManager.instance.balls)
            {
                sortedListText += $"Player: {ball.usernameTxt.text}, Survived Time: {TimeSpan.FromSeconds(ball.currentSeconds).ToString(@"hh\:mm\:ss\:fff")}\n";
            }

            if (displayText != null)
            {
                displayText.text = sortedListText;  // Update the UI Text
            }
        }
    }

    public override void UpdateUI()
    {
        // This calls the base update UI function from the UIelement class
        base.UpdateUI();

        // The remaining code is only called for this sub-class of UIelement and not others
        DisplayScore();
    }
}
