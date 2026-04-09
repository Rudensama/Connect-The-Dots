using TMPro;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    #region SINGLETON

    public static GameHandler Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #endregion

    #region PUBLIC VARIABLES
    
    public GameObject titleText;
    public GameObject levelText;
    public GameObject completeText;
    
    public GameObject Board;
    public GameObject Cover;
    
    public GameObject buttonNext;
    public GameObject buttonPlay;
    public GameObject buttonClear;

    #endregion

    #region PRIVATE VARIABLES

    private bool _drawing = false;
    private int _currentColorID = 0;
    private Vector2Int _currentPos = new Vector2Int(-1, -1);

    #endregion

    #region PUBLIC METHODS

    public bool IsDrawing() => _drawing;
    public int GetCurrentColorID() => _currentColorID;
    public Vector2Int GetCurrentPos() => _currentPos;

    public void PlayButtonPressed()
    {
        StatePlay();
    }

    public void ClearButtonPressed()
    {
        TileData.Instance.UpdateTileData();
        VisualHandler.Instance.SpawnVisuals();
    }
    
    public void NextButtonPressed()
    {
        _drawing = false;
        _currentColorID = 0;
        LoadNextLevel();
    }

    #endregion

    #region PRIVATE METHODS

    private void Start()
    {
        StateBegin();
    }

    private void Update()
    {
        HandleInput();
    }
    
    private Vector2Int GetMouseGridPos()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        int x = Mathf.RoundToInt(mousePos.x) + 2;
        int y = (Mathf.RoundToInt(mousePos.y) - 2) * -1;
        return new Vector2Int(x, y);
    }

    private bool IsValidGridPos(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x <= 4 && pos.y >= 0 && pos.y <= 4;
    }

    private void HandleInput()
    {
        Vector2Int mousePos = GetMouseGridPos();

        if (!IsValidGridPos(mousePos)) return;

        if (Input.GetMouseButtonDown(0))
        {
            HandleMouseDown(mousePos);
        }

        if (Input.GetMouseButtonUp(0))
        {
            _drawing = false;
            _currentColorID = 0;
        }

        if (Input.GetMouseButton(0) && _drawing && mousePos != _currentPos)
        {
            _currentPos = mousePos;
            HandleMouseDrag(mousePos);
        }
    }

    private void HandleMouseDown(Vector2Int clickPos)
    {
        int tileID = TileData.Instance.GetTileID(clickPos);
        
        if (tileID == 0)
        {
            _drawing = false;
            return;
        }
        
        if (TileData.Instance.IsOriginalNode(clickPos))
        {
            int colorID = TileData.Instance.GetOriginalNodeID(clickPos);
            TileData.Instance.ClearLinePoints(colorID - 1);
            _currentColorID = colorID;
            _currentPos = clickPos;
            _drawing = true;
            AddPointAndUpdate(clickPos);
            return;
        }
        
        int lineIndex = TileData.Instance.GetLineIndexAtPoint(clickPos);
        if (lineIndex >= 0)
        {
            int order = TileData.Instance.GetLinePointOrder(clickPos, lineIndex);
            TileData.Instance.TrimLineFromOrder(order, lineIndex);
            _currentColorID = lineIndex + 1;
            _currentPos = clickPos;
            _drawing = true;
            VisualHandler.Instance.UpdateVisuals();
        }
    }

    private void HandleMouseDrag(Vector2Int newPos)
    {
        if (_currentColorID == 0) return;

        int lineIndex = _currentColorID - 1;
        
        int otherLineIndex = TileData.Instance.GetLineIndexAtPoint(newPos);
        if (otherLineIndex >= 0 && otherLineIndex != lineIndex)
        {
            int order = TileData.Instance.GetLinePointOrder(newPos, otherLineIndex);
            TileData.Instance.TrimLineFromOrder(order, otherLineIndex);
        }
        
        if (!TileData.Instance.IsValidLinePoint(newPos, lineIndex))
        {
            int order = TileData.Instance.GetLinePointOrder(newPos, lineIndex);
            TileData.Instance.TrimLineFromOrder(order, lineIndex);
        }

        AddPointAndUpdate(newPos);
        
        if (TileData.Instance.CheckWinGame())
        {
            OnWin();
        }
    }

    private void AddPointAndUpdate(Vector2Int pos)
    {
        TileData.Instance.AddLinePoint(pos, _currentColorID - 1);
        VisualHandler.Instance.UpdateVisuals();
    }

    private void OnWin()
    {
        StateComplete();
    }

    private void LoadNextLevel()
    {
        int nextLevel = TileData.Instance.GetCurrentLevelIndex() + 1;
        if (nextLevel < 5)
        {
            TileData.Instance.SetCurrentLevelIndex(nextLevel);
            TileData.Instance.UpdateTileData();
            VisualHandler.Instance.SpawnVisuals();
            UpdateLevelText();
            
            levelText.SetActive(true);
            completeText.SetActive(false);
            
            Cover.SetActive(false);
            
            buttonNext.SetActive(false);
            buttonClear.SetActive(true);
        }
        else
        {
            StateBegin();
        }
    }

    private void UpdateLevelText()
    {
        TMP_Text levelTMP = levelText.GetComponent<TMP_Text>();
        levelTMP.text = $"LEVEL {TileData.Instance.GetCurrentLevelIndex() + 1}";
    }

    private void StateBegin()
    {
        titleText.SetActive(true);
        levelText.SetActive(false);
        completeText.SetActive(false);
        
        Board.SetActive(false);
        Cover.SetActive(false);
        
        buttonPlay.SetActive(true);
        buttonNext.SetActive(false);
        buttonClear.SetActive(false);
        
        VisualHandler.Instance.ClearVisuals();
    }

    private void StatePlay()
    {
        titleText.SetActive(false);
        levelText.SetActive(true);
        completeText.SetActive(false);
        
        Board.SetActive(true);
        Cover.SetActive(false);
        
        buttonPlay.SetActive(false);
        buttonNext.SetActive(false);
        buttonClear.SetActive(true);

        UpdateLevelText();
        
        TileData.Instance.SetCurrentLevelIndex(0);
        TileData.Instance.UpdateTileData();
        VisualHandler.Instance.SpawnVisuals();
    }

    private void StateComplete()
    {
        titleText.SetActive(false);
        levelText.SetActive(false);
        completeText.SetActive(true);
        
        Board.SetActive(true);
        Cover.SetActive(true);
        
        buttonPlay.SetActive(false);
        buttonNext.SetActive(true);
        buttonClear.SetActive(false);
    }
    #endregion
}