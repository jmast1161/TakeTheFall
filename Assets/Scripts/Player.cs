using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public enum GameState
{
    WaitingGameplayInput,
    PlayerInAir,
    LevelComplete,
    WaitingLevelCompleteInput,
    WaitingGameOverInput
}

public class Player : MonoBehaviour
{
    private GameState gameState;
    [SerializeField] private Rigidbody2D rigidBody2D;
    [SerializeField] private Canvas levelCompleteCanvasPrefab;
    [SerializeField] private Canvas thanksCanvasPrefab;
    [SerializeField] private Canvas retryCanvasPrefab;
    [SerializeField] private float moveSpeed;
    
    [SerializeField] private AudioSource spikeAudioSource;
    [SerializeField] private AudioSource goalAudioSource;

    
    private Canvas levelCompleteCanvas;
    private Canvas retryCanvas;
    
    private Canvas thanksCanvas;

    private SpriteRenderer _renderer;

    private void Awake() 
    {
        rigidBody2D = GetComponent<Rigidbody2D>();
        _renderer = GetComponent<SpriteRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        GameObject.FindGameObjectWithTag("Music").GetComponent<Music>().PlayMusic();
        gameState = GameState.WaitingGameplayInput;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameState == GameState.WaitingGameplayInput ||
            gameState == GameState.PlayerInAir)
        {
            MovePlayer();
        }
    }

    private void OnCollisionEnter2D(Collision2D other) 
    {
        if (other.gameObject.tag == "Spikes")
        {
            _renderer.color = new Color(_renderer.color.r, _renderer.color.g, _renderer.color.b, 0f);
            spikeAudioSource.Play();
            SpawnRetryCanvas();
        }

        if (other.gameObject.tag == "Goal")
        {
            goalAudioSource.Play();
            rigidBody2D.velocity = new Vector2(0f, 0f);
            var buildIndex = SceneManager.GetActiveScene().buildIndex;
            if(buildIndex == 4)
            {
                SpawnThanksCanvas();
            }
            else
            {
                SpawnLevelCompleteCanvas();
            }
        }

        if(other.gameObject.tag == "Platform")
        {
            gameState = GameState.WaitingGameplayInput;
        }
    }

    private void OnCollisionExit2D(Collision2D other) 
    {
        if(other.gameObject.tag == "Platform")
        {
            gameState = GameState.PlayerInAir;
        }
    }

    public void MovePlayer()
    {
        var moveDirection = Input.GetAxis("Horizontal");
        rigidBody2D.velocity = new Vector2(moveDirection * moveSpeed, rigidBody2D.velocity.y);
    }

    private void SpawnThanksCanvas()
    {
        thanksCanvas = Instantiate(thanksCanvasPrefab, new Vector3(0f, 0f), Quaternion.identity);
        var thanksButton = thanksCanvas.GetComponentInChildren<UnityEngine.UI.Button>();

        thanksButton.onClick.AddListener(() => {
            SceneManager.LoadScene(0);
        });

        gameState = GameState.WaitingLevelCompleteInput;
    }

    private void SpawnLevelCompleteCanvas()
    {
        levelCompleteCanvas = Instantiate(levelCompleteCanvasPrefab, new Vector3(0f, 0f), Quaternion.identity);
        var nextLevelButton = levelCompleteCanvas.GetComponentInChildren<UnityEngine.UI.Button>();

        nextLevelButton.onClick.AddListener(() => {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        });

        gameState = GameState.WaitingLevelCompleteInput;
    }

     private void SpawnRetryCanvas()
    {
        retryCanvas = Instantiate(retryCanvasPrefab, new Vector3(0f, 0f), Quaternion.identity);
        var retryButton = retryCanvas.GetComponentInChildren<UnityEngine.UI.Button>();

        retryButton.onClick.AddListener(() => {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        });

        gameState = GameState.WaitingGameOverInput;
    }
}
