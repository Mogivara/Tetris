using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public Text levelText;
    public Text highScoreText;
    public Text highScoreText2;
    public Text highScoreText3;

    public Text lastScore;

    public void Start()
    {
        if(levelText!= null) 
            levelText.text = "0";
        if (highScoreText != null)
            highScoreText.text = PlayerPrefs.GetInt("HighScore").ToString();
        if (highScoreText2 != null)
            highScoreText2.text = PlayerPrefs.GetInt("HighScore2").ToString();
        if (highScoreText3 != null)
            highScoreText3.text = PlayerPrefs.GetInt("HighScore3").ToString();
        if (lastScore != null)
            lastScore.text = PlayerPrefs.GetInt("lastscore").ToString();
    }

    public void Play()
    {
        if (Game.startingLevel == 0)
            Game.startingArLevelZero = true;
        else
            Game.startingArLevelZero = false;

        SceneManager.LoadScene("StartGame");
    }

   

    public void ChangeValue(float value)
    {
        Game.startingLevel = (int)value;
        levelText.text = value.ToString();
    }

    public void LaunchMenu()
    {
        SceneManager.LoadScene("Main");
    }

}
