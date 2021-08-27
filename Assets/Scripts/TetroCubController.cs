using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetroCubController : MonoBehaviour
{
    float fall = 0;
    private float fallSpeed;

    public bool allowRotation = true;
    public bool limitRotation = false;

    public int individualScore = 100;
    private float individualScoreTime;

    public AudioClip moveSound;
    public AudioClip rotateSound;
    public AudioClip landeSound;

    private float continuosVerticalSpeed = 0.05f;
    private float continuosHorizontalSpeed = 0.1f;
    private float bottomDownWaitMax = 0.2f;

    private float VerticalTimer = 0;
    private float HorizontalTimer= 0;
    private float bottomDownWaitTimerHorizontal = 0;
    private float bottomDownWaitTimerVertikal = 0;

    private bool movedHorizontal = false;
    private bool movedVertical = false;

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

    }

    public void Update()
    {
        if (!Game.isPaused)
        { 
            ChekUserInpute();
            UpdateIndividualScore();
            UpdateFallSpeed();
        }
    }

    public void UpdateFallSpeed()
    {
        fallSpeed = Game.fallSpeed;
    }

    public void UpdateIndividualScore()
    {
        if (individualScoreTime < 1)
        {
            individualScoreTime += Time.deltaTime;
        }
        else
        {
            individualScoreTime = 0;
            individualScore = Mathf.Max(individualScore - 10,0);
        }
    }

    public void ChekUserInpute()
    {
        if(Input.GetKeyUp(KeyCode.RightArrow)|| Input.GetKeyUp(KeyCode.LeftArrow))
        {
            movedHorizontal = false;       
            HorizontalTimer = 0;           
            bottomDownWaitTimerHorizontal = 0;
        }

        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            movedVertical = false;
            VerticalTimer = 0;
            bottomDownWaitTimerVertikal = 0;
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            moveLRight();
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            moveLeft();
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            rotate();
        }
        if (Input.GetKey(KeyCode.DownArrow)|| Time.time - fall >= fallSpeed)
        {
            moveDown();
        }
    }

    void moveLeft()
    {
        if (movedHorizontal)
        {
            if (bottomDownWaitTimerHorizontal < bottomDownWaitMax)
            {
                bottomDownWaitTimerHorizontal += Time.deltaTime;
                return;
            }
            if (HorizontalTimer < continuosHorizontalSpeed)
            {
                HorizontalTimer += Time.deltaTime;
                return;
            }
        }
        if (!movedHorizontal)
            movedHorizontal = true;

        HorizontalTimer = 0;
        transform.position += new Vector3(-1, 0, 0);

        if (CheckIsValidPosition())
        {
            FindObjectOfType<Game>().UpdateGrid(this);
            PlayMoveSound();
        }
        else
        {
            transform.position += new Vector3(1, 0, 0);
        }
    }
    void moveLRight()
    {
        if (movedHorizontal)
        {
            if (bottomDownWaitTimerHorizontal < bottomDownWaitMax)
            {
                bottomDownWaitTimerHorizontal += Time.deltaTime;
                return;
            }

            if (HorizontalTimer < continuosHorizontalSpeed)
            {
                HorizontalTimer += Time.deltaTime;
                return;
            }
        }

        if (!movedHorizontal)
            movedHorizontal = true;

        HorizontalTimer = 0;

        transform.position += new Vector3(1, 0, 0);

        if (CheckIsValidPosition())
        {
            FindObjectOfType<Game>().UpdateGrid(this);
            PlayMoveSound();
        }
        else
        {
            transform.position += new Vector3(-1, 0, 0);
        }
    }
    void moveDown()
    {
        if (movedVertical)
        {
            if (bottomDownWaitTimerVertikal < bottomDownWaitMax)
            {
                bottomDownWaitTimerVertikal += Time.deltaTime;
                return;
            }
            if (VerticalTimer < continuosVerticalSpeed)
            {
                VerticalTimer += Time.deltaTime;
                return;
            }
        }
        if (!movedVertical)
            movedVertical = true;

        VerticalTimer = 0;

        transform.position += new Vector3(0, -1, 0);

        if (CheckIsValidPosition())
        {
            FindObjectOfType<Game>().UpdateGrid(this);

            if (Input.GetKey(KeyCode.DownArrow))
            {
                PlayMoveSound();
            }

        }
        else
        {
            transform.position += new Vector3(0, 1, 0);

            FindObjectOfType<Game>().DeleteRow();

            if (FindObjectOfType<Game>().CheckIsAboceGrid(this))
            {
                FindObjectOfType<Game>().GameOver();
            }
            PlayLandSound();

            FindObjectOfType<Game>().SpawnNextTetrCub();

            Game.currentScore += individualScore;

            FindObjectOfType<Game>().UpdateHighScore();

            enabled = false;
            tag = "Untagged";
        }

        fall = Time.time;
    }
    void rotate() 
    {


        if (allowRotation)
        {

            if (limitRotation)

            {
                if (transform.rotation.eulerAngles.z >= 90)

                {
                    transform.Rotate(0, 0, -90);
                }
                else
                {
                    transform.Rotate(0, 0, 90);
                }
            }
            else
            {
                transform.Rotate(0, 0, 90);

                if (CheckIsValidPosition())
                {
                    FindObjectOfType<Game>().UpdateGrid(this);
                    PlayRotateSound();
                }
                else
                {
                    if (limitRotation)
                    {

                        if (transform.rotation.eulerAngles.z >= 90)
                        {
                            transform.Rotate(0, 0, -90);
                        }
                        else
                        {
                            transform.Rotate(0, 0, 90);
                        }
                    }
                    else
                    {
                        transform.Rotate(0, 0, -90);
                    }
                }
            }


        }
    }


    void PlayMoveSound()
    {
        audioSource.PlayOneShot(moveSound);
    }
    void PlayRotateSound()
    {
        audioSource.PlayOneShot(rotateSound);
    }
    void PlayLandSound()
    {
        audioSource.PlayOneShot(landeSound);
    }


    public bool CheckIsValidPosition()
    {
        foreach(Transform mino in transform)
        {
            Vector2 pos = FindObjectOfType<Game>().Round(mino.position);

            if(FindObjectOfType<Game>().ChekIsInsideGrid(pos)== false)
            {
                return false;
            }
            if(FindObjectOfType<Game>().GetTransformAtGridPositions(pos) != null && FindObjectOfType <Game>().GetTransformAtGridPositions(pos).parent != transform)
            {
                return false;
            }
        }
        return true;
    }


}
