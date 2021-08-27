using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class Game : MonoBehaviour
{

    public Canvas hud_Canvas;
    public Canvas pause_Canvas;

    public static int gridWidth = 10;
    public static int gridHeight = 20;
    public int ScoreOneLine = 40;
    public int ScoreTwoLine = 100;
    public int ScoreThreeLine = 300;
    public int ScoreFourLine = 1400;

    public int currentLevel = 0;
    private int numLinesClean = 0;
    public static float fallSpeed = 1.0f;
    public static bool isPaused = false;

   public AudioClip clearLineSound;

    public Text PointText;
    public Text LevelText;
    public Text LineText;

    public static int currentScore = 0;
    private AudioSource audioSource;
    private int numberOfRowsThisTurn = 0;
    public static Transform[,] grid = new Transform[gridWidth, gridHeight];

    public static bool startingArLevelZero;
    public static int startingLevel;

    private GameObject previewTetr;
    public GameObject nextTetr;
    public GameObject savedTetr;

    private bool gameStarted = false;   

    private int startingHighScore;
    private int startingHighScore2;
    private int startingHighScore3; 

    private Vector2 previewTetrCubPosition = new Vector2(-6.5f, 16);
    private Vector2 saveTetrCubPosition = new Vector2(-6.5f, 10);

    public int maxSwaps = 2;
    public int currentSwaps = 0;


    public void Start()
    {

        currentScore = 0;

        PointText.text = "0";
        
        currentLevel = startingLevel;

        LevelText.text = currentLevel.ToString();

        LineText.text = "0";

        SpawnNextTetrCub();

        audioSource = GetComponent<AudioSource>();

        startingHighScore = PlayerPrefs.GetInt("HighScore");
        startingHighScore2 = PlayerPrefs.GetInt("HighScore2");
        startingHighScore3 = PlayerPrefs.GetInt("HighScore3");
    }

    public void Update()
    {
        UpdateScore();
        UpdateUi();
        UpdateLevel();
        UpdateSpeed();
        UpdateHighScore();
        CheckUserInput();
    }

    public void CheckUserInput()
    {
        if (Input.GetKey(KeyCode.P))
        {
            if (Time.timeScale == 1)
                PauseGame();
            else
                ResumGame();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameObject tempNextTetromino = GameObject.FindGameObjectWithTag("currentActiveTetr");
            SaveTetr(tempNextTetromino.transform);
        }
    }
    void PauseGame()
    {
        Time.timeScale = 0;
        audioSource.Pause();
        isPaused = true;
        hud_Canvas.enabled = false;
        pause_Canvas.enabled = true;
    }
    void ResumGame()
    {
        Time.timeScale = 1;
        audioSource.Play();
        isPaused = false;
        hud_Canvas.enabled = true;
        pause_Canvas.enabled = false;
    }

    public void UpdateLevel()
    {
        if ((startingArLevelZero == true) || (startingArLevelZero == false && numLinesClean / 10 > startingLevel))
        currentLevel = numLinesClean / 10;
                
    }
    public void UpdateSpeed()
    {
        fallSpeed = 1.0f - ((float)currentLevel * 0.1f);
    }

    public void UpdateUi()
    {
        PointText.text = currentScore.ToString();
        LevelText.text = currentLevel.ToString();
        LineText.text = numLinesClean.ToString();
    }

    public void UpdateScore()
    {
        if (numberOfRowsThisTurn > 0)
        {
            if (numberOfRowsThisTurn == 1)
            {
                ClearOneLine();
            }

            else if (numberOfRowsThisTurn == 2)
            {
                ClearTwoLine();
            }
            else if (numberOfRowsThisTurn == 3)
            {
                ClearThreeLine();
            }
            else if (numberOfRowsThisTurn == 4)
            {
                ClearFourLine();
            }
            numberOfRowsThisTurn = 0;

            PlayLineClearSound();
        }
    }

    public void ClearOneLine()
    {
        currentScore += ScoreOneLine + currentLevel * 20;
        numLinesClean++;
    }
    public void ClearTwoLine()
    {
        currentScore += ScoreTwoLine + currentLevel * 25;
        numLinesClean += 2;
    }
    public void ClearThreeLine()
    {
        currentScore += ScoreThreeLine + currentLevel * 30;
        numLinesClean += 3;
    }
    public void ClearFourLine()
    {
        currentScore += ScoreFourLine + currentLevel * 40;
        numLinesClean += 4;
    }

    public void PlayLineClearSound()
    {
        audioSource.PlayOneShot(clearLineSound);
    }

    public void UpdateHighScore()
    {
        if (currentScore > startingHighScore)
        {
            PlayerPrefs.SetInt("HighScore3", startingHighScore2);
            PlayerPrefs.SetInt("HighScore2", startingHighScore);
            PlayerPrefs.SetInt("HighScore", currentScore);            
        }
        else if (currentScore > startingHighScore2)
        {
            PlayerPrefs.SetInt("HighScore3", startingHighScore2);
            PlayerPrefs.SetInt("HighScore2", currentScore);
        } else if (currentScore > startingHighScore3)
        {
            PlayerPrefs.SetInt("HighScore3", currentScore);
        }

        PlayerPrefs.SetInt("lastscore", currentScore);

    }
    public Vector2 Round(Vector2 pos)
    {
        return new Vector2(Mathf.Round(pos.x), Mathf.Round(pos.y));
    }
    bool ChekIsValidPosition(GameObject tetr)
    {
        foreach(Transform mino in tetr.transform)
        {
            Vector2 pos = Round(mino.position);

            if (!ChekIsInsideGrid(pos))
                return false;
            if(GetTransformAtGridPositions(pos) != null && GetTransformAtGridPositions(pos).parent != tetr.transform)
                 return false;
        }

        return true;
    }

    public bool CheckIsAboceGrid(TetroCubController tetroCubController)
    {
        for (int x = 0; x < gridWidth; ++x)
        {
            foreach(Transform mino in tetroCubController.transform)
            {
                Vector2 pos = Round (mino.position);

                if(pos.y >  gridHeight - 1  )
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool IsFullRowAt(int y)
    {
        for(int x = 0; x < gridWidth; x++)
        {
            if(grid[x,y] == null)
            {
                return false;
            }
        }

        numberOfRowsThisTurn++;

        return true;
    }
    public void DeleteCub(int y)
    {
        for(int x = 0; x < gridWidth; x++)
        {
            Destroy(grid[x, y].gameObject);
            grid[x, y] = null;
        }
    }

    public void MoveRowDown(int y)
    {
        for(int x=0;x < gridWidth; x++)
        {
            if (grid[x, y] != null)
            {
                grid[x, y - 1] = grid[x, y];
                grid[x, y] = null;

                grid[x, y - 1].position += new Vector3(0,-1,0); 
            }
        }
    }

    public void MoveAllRowsDown(int y)
    {
        for(int i = y; i < gridHeight; i++)
        {
            MoveRowDown(i);
        }
    }

    public void DeleteRow()
    {
        for (int y = 0 ; y < gridHeight; y++)
        {
            if (IsFullRowAt(y))
            {
                DeleteCub(y);

                MoveAllRowsDown(y + 1);

                --y;
            }
        }
    }


    public void UpdateGrid(TetroCubController tetroCubController)
    {
        for(int y = 0; y < gridHeight; ++y)
        {
            for(int x = 0; x < gridWidth; ++x)
            {
                if (grid[x, y] != null)
                {
                    if(grid[x,y].parent == tetroCubController.transform)
                    {
                        grid[x, y] = null;
                    }
                }
            }
        }

        foreach(Transform mino in tetroCubController.transform)
        {
            Vector2 pos = Round(mino.position);

            if(pos.y < gridHeight)
            {
                grid[(int)pos.x, (int)pos.y] = mino;
            }
        }

    }

    public Transform GetTransformAtGridPositions(Vector2 pos)
    {
        if(pos.y >  gridHeight - 1)
        {
            return null;
        } 
        else
        {
            return grid[(int)pos.x, (int)pos.y];
        }
    }

    public void SpawnNextTetrCub()
    {
        if (!gameStarted)
        {
            gameStarted = true;

            nextTetr = (GameObject)Instantiate(Resources.Load(GetRandomTetrCub(), typeof(GameObject)), new Vector2(5.0f, 20.0f), Quaternion.identity);
            previewTetr = (GameObject)Instantiate(Resources.Load(GetRandomTetrCub(), typeof(GameObject)), previewTetrCubPosition, Quaternion.identity);
            previewTetr.GetComponent<TetroCubController>().enabled = false;
            nextTetr.tag = "currentActiveTetr";
        }
        else
        {
            previewTetr.transform.localPosition = new Vector2(5.0f, 20.0f);
            nextTetr = previewTetr;
            nextTetr.GetComponent<TetroCubController>().enabled = true;
            nextTetr.tag = "currentActiveTetr";
            previewTetr = (GameObject)Instantiate(Resources.Load(GetRandomTetrCub(), typeof(GameObject)), previewTetrCubPosition, Quaternion.identity);
            previewTetr.GetComponent<TetroCubController>().enabled = false;
        }

        currentSwaps = 0;
        
    }

    public void SaveTetr(Transform t)
    {
        currentSwaps++;

        if (currentSwaps > maxSwaps)
            return;

        if (savedTetr != null)
        {
            GameObject tempSavedTetr =  GameObject.FindGameObjectWithTag("currentSavedTetr");

            tempSavedTetr.transform.localPosition = new Vector2(gridWidth / 2, gridHeight);

            if (!ChekIsValidPosition(tempSavedTetr))
            {
                tempSavedTetr.transform.localPosition = saveTetrCubPosition;
                return;
            }

            savedTetr = (GameObject)Instantiate(t.gameObject);
            savedTetr.GetComponent<TetroCubController>().enabled = false;
            savedTetr.transform.localPosition = saveTetrCubPosition;
            savedTetr.tag = "currentSavedTetr";

            nextTetr = (GameObject)Instantiate(tempSavedTetr);
            nextTetr.GetComponent<TetroCubController>().enabled = true;
            nextTetr.transform.localPosition = new Vector2(gridWidth / 2, gridHeight);
            nextTetr.tag = "currentActiveTetr";

            DestroyImmediate(t.gameObject);
            DestroyImmediate(tempSavedTetr);

        } else
        {
            savedTetr = (GameObject)Instantiate(GameObject.FindGameObjectWithTag("currentActiveTetr"));
            savedTetr.GetComponent<TetroCubController>().enabled = false;
            savedTetr.transform.localPosition = saveTetrCubPosition;
            savedTetr.tag = "currentSavedTetr";

            DestroyImmediate (GameObject.FindGameObjectWithTag("currentActiveTetr"));

            SpawnNextTetrCub();
        }
    }

    public bool ChekIsInsideGrid(Vector2 pos)
    {
        return ((int)pos.x >= 0 && (int)pos.x < gridWidth && (int)pos.y >= 0);
    }
   

    public string GetRandomTetrCub()
    {
        int randomTetrCub = Random.Range(1, 8);

        string randomTetrCubName = "Prefabs/TetroKubOrange_T";

        switch (randomTetrCub)
        {
            case 1:
                randomTetrCubName = "Prefabs/TetroKubOrange_T";
                break;
            case 2:
                randomTetrCubName = "Prefabs/TetroKubYellow_Long";
                break;
            case 3:
                randomTetrCubName = "Prefabs/TetroKubPink_Square";
                break;
            case 4:
                randomTetrCubName = "Prefabs/TetroKubBlue_J";
                break;
            case 5:
                randomTetrCubName = "Prefabs/TetroKubRed_L";
                break;
            case 6:
                randomTetrCubName = "Prefabs/TetroKubGreen_S";
                break;
            case 7:
                randomTetrCubName = "Prefabs/TetroKubBiruz_Z";
                break;           
        }

        return randomTetrCubName;
    }

    public void GameOver()
    {
        UpdateHighScore();

        SceneManager.LoadScene("GameOver");
        
    }
}
