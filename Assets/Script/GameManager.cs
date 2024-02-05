using System;
using System.Runtime.ConstrainedExecution;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;



public class GameManager : MonoBehaviour, IPointerClickHandler
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private FruitObjectSetting setting;
    [SerializeField] private GameArea gameArea;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float InputPosition;
    [SerializeField] private SpriteRenderer fruitShower;

    [SerializeField] private TextMeshProUGUI textScore;
    [SerializeField] private TextMeshProUGUI bestScore;
    [SerializeField] private int selectedIndex = 0;
    

    private int Score
    {
        get => _score;
        set
        {
            _score = value;
            if (value == 0)
                return;

            textScore.SetText(value.ToString());
            if(_score > _bestScore)
            {
                _bestScore = _score;
                bestScore.SetText(_bestScore.ToString());
                PlayerPrefs.SetInt("best", _bestScore);
            }
        }

    }

    private int _score;
    private int _bestScore;


    private bool IsClick;
    private readonly Vector2Int _fruitRange = new Vector2Int(0, 4);

    private void Awake()
    {
        Instance = this;
        _bestScore = PlayerPrefs.GetInt("best");
        bestScore.SetText(_bestScore.ToString());
    }
    private void Start()
    {
        selectedIndex = Random.Range(_fruitRange.x, _fruitRange.y);
        ChangeSpriteFruitObject(selectedIndex);
    }
    private void ChangeSpriteFruitObject(int index)
    {
        fruitShower.sprite = setting.GetSprite(index);
    }
    private float GetInputHorizontalPosition()
    {
        
        var limit = gameArea.GetBorderPositionHorizontal();
        var result = Mathf.Clamp(InputPosition, limit.x, limit.y);
        return InputPosition;
    }
    private void OnClicked()
    {
        var spawnPosition = new Vector3(GetInputHorizontalPosition(), spawnPoint.position.y, spawnPoint.position.z);
        SpawnFruit(selectedIndex, spawnPosition);
        selectedIndex = Random.Range(_fruitRange.x, _fruitRange.y);
        ChangeSpriteFruitObject(selectedIndex);
    }
    
    private void SpawnFruit(int index, Vector3 position)
    {
        var prefab = setting.SpawnObject;
        var fruitObject = Instantiate(prefab, position, quaternion.identity);
        var sprite = setting.GetSprite(index);
        var scale = setting.GetScale(index);
        fruitObject.GetComponent<FruitObject>().Prepare(sprite, index, scale);
    }
    public void Merge(FruitObject first, FruitObject second)
    {
        var type = first.type + 1;
        var spawnPosition = (first.transform.position + second.transform.position) * 0.5f;
        Destroy(first.gameObject);
        Destroy(second.gameObject);
        SpawnFruit(type, spawnPosition);

        Score += type * 2;
    }
    
    private void FixedUpdate()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        if (IsClick)
        {
            OnClicked();
            
            IsClick = false;        
        }
    }
    public void GameOver()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Debug.Log("Game Over");
    }
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        //Debug.Log("1");
        InputPosition = (pointerEventData.position.x - (Screen.width/2))/216 ;
        IsClick = true;
    }

}