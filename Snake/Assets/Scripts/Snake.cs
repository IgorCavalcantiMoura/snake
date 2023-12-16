using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Snake : MonoBehaviour
{
    private Vector2 direction = Vector2.right;
    private bool up, down, left, right;

    private List<Transform> segments = new List<Transform>();

    [SerializeField] private Transform segmentPrefab, segmentParent;

    [SerializeField] private int initialSize;

    GameManager gM;

  
    void Start()
    {

        gM = FindObjectOfType<GameManager>();
        Reset();
        right = true;

        gM.hscore = PlayerPrefs.GetInt("Hscore");
        gM.hscoreText.text = "New Best Score" + gM.hscore.ToString();
    }

    
    void Update()
    {
        InputController();
    }

    void FixedUpdate()
    {
        MovementAndSegments();
    }

    private void InputController()
    {
        if(Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) && !down) 
        {
            direction = Vector2.up;
            up = true;
            down = false;
            left = false;
            right = false;
        }

        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow) && !up)
        {
            direction = Vector2.down;
            up = false;
            down = true;
            left = false;
            right = false;

        }

        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow) && !right)
        {
            direction = Vector2.left;
            up = false;
            down = false;
            left = true;
            right = false;
        }

        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow) && !left)
        {
            direction = Vector2.right;
            up = false;
            down = false;
            left = false;
            right = true;
        }
    }

    private void MovementAndSegments()
    {
        for(int i = segments.Count - 1; i > 0; i--)
        {
            segments[i].position = segments[i - 1].position;
        }

        this.transform.position = new Vector2(Mathf.Round(this.transform.position.x) + direction.x, Mathf.Round(this.transform.position.y) + direction.y);
    }

    private void Reset()
    {
        for( int i = 1; i < segments.Count; i++)
        {
            Destroy(segments[i].gameObject);
        }

        segments.Clear();
        segments.Add(this.transform);

        for (int i = 1; i < initialSize; i++)
        {
            Transform tempSegment = Instantiate(segmentPrefab);
            tempSegment.transform.parent = segmentParent;
            segments.Add(tempSegment);
        }

        this.transform.position = Vector2.zero;
    }

    private void Grow()
    {
        Transform tempSegment = Instantiate(segmentPrefab, segments[segments.Count - 1].position, Quaternion.identity);
        tempSegment.transform.parent = segmentParent;
        segments.Add(tempSegment);
        gM.SetScore(5);

    }

    public void BtnStartGame()
    {
        gM.gameOverPanel.SetActive(false);
        gM.startPanel.SetActive(false);

        Time.timeScale = 1;
        transform.position = Vector2.zero;
        direction = Vector2.zero;

        for(int i = 1; i < segments.Count; i++)
        {
            Destroy(segments[i].gameObject);
        }

        segments.Clear();
        segments.Add(transform);

        gM.score = 0;
        gM.scoreText.text = "Score: 0";

        gM.hscore = PlayerPrefs.GetInt("Hscore");
        gM.hscoreText.text = "New Best Score" + gM.hscore.ToString();
    }

    private void OnTriggerEnter2D(Collider2D target)
    {
        if (target.gameObject.CompareTag("Food"))
        {
            Grow();
        }

        else if(target.gameObject.CompareTag("Segment") || target.gameObject.CompareTag("Wall"))
        {
            gM.GameOver();
        }
    }
}
