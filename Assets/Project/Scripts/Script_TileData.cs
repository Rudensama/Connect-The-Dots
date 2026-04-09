using System.Collections.Generic;
using UnityEngine;

public class TileData : MonoBehaviour
{
    #region SINGLETON

    public static TileData Instance;

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

    public List<Color> NodeColors = new List<Color>()
    {
        Color.clear,
        Color.red,
        Color.blue,
        Color.green,
        Color.yellow,
        Color.magenta,
    };

    #endregion

    #region PRIVATE VARIABLES

    private int _currentLevelIndex = 0;

    public int[,] _tileIDTable = new int[5, 5];

    private List<Vector2Int>[] _linePoints = new List<Vector2Int>[5];

    private int[,] _level_1 = new int[5, 5]
    {
        { 1, 0, 0, 0, 2 },
        { 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0 },
        { 1, 0, 0, 0, 2 }
    };

    private int[,] _level_2 = new int[5, 5]
    {
        { 4, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0 },
        { 0, 0, 3, 0, 0 },
        { 2, 3, 1, 0, 4 },
        { 1, 0, 0, 0, 2 }
    };

    private int[,] _level_3 = new int[5, 5]
    {
        { 0, 4, 2, 3, 0 },
        { 0, 0, 0, 1, 0 },
        { 0, 0, 1, 0, 0 },
        { 4, 0, 0, 5, 0 },
        { 2, 0, 5, 3, 0 }
    };

    private int[,] _level_4 = new int[5, 5]
    {
        { 0, 0, 0, 1, 3 },
        { 1, 0, 0, 0, 0 },
        { 0, 0, 4, 0, 0 },
        { 0, 0, 0, 2, 0 },
        { 3, 2, 4, 0, 0 }
    };

    private int[,] _level_5 = new int[5, 5]
    {
        { 0, 0, 0, 1, 3 },
        { 0, 4, 2, 3, 0 },
        { 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 2 },
        { 0, 0, 1, 0, 4 }
    };

    #endregion

    #region PUBLIC METHODS

    public int GetTileID(Vector2Int pos)
    {
        return _tileIDTable[pos.x, pos.y];
    }

    public void SetTileID(Vector2Int pos, int id)
    {
        _tileIDTable[pos.x, pos.y] = id;
    }
    
    public void ClearTileID(Vector2Int pos)
    {
        _tileIDTable[pos.x, pos.y] = GetCurrentLevelData()[pos.x, pos.y];
    }
    
    public bool IsOriginalNode(Vector2Int pos)
    {
        return GetCurrentLevelData()[pos.x, pos.y] != 0;
    }
    
    public int GetOriginalNodeID(Vector2Int pos)
    {
        return GetCurrentLevelData()[pos.x, pos.y];
    }

    public List<Vector2Int> GetLinePoints(int index)
    {
        return _linePoints[index];
    }
    
    public bool IsValidLinePoint(Vector2Int point, int index)
    {
        return !_linePoints[index].Contains(point);
    }
    
    public int GetLinePointOrder(Vector2Int point, int index)
    {
        return _linePoints[index].IndexOf(point);
    }
    
    public int GetLineIndexAtPoint(Vector2Int point)
    {
        for (int i = 0; i < 5; i++)
        {
            if (_linePoints[i].Contains(point))
                return i;
        }
        return -1;
    }
    
    public void AddLinePoint(Vector2Int newPoint, int index)
    {
        _linePoints[index].Add(newPoint);
        SetTileID(newPoint, index + 1);
    }
    
    public void TrimLineFromOrder(int fromOrder, int index)
    {
        List<Vector2Int> points = _linePoints[index];
        for (int i = points.Count - 1; i > fromOrder; i--)
        {
            ClearTileID(points[i]);
            points.RemoveAt(i);
        }
    }
    
    public void ClearLinePoints(int index)
    {
        foreach (Vector2Int point in _linePoints[index])
            ClearTileID(point);
        _linePoints[index].Clear();
    }

    public int GetCurrentLevelIndex()
    {
        return _currentLevelIndex;
    }

    public void SetCurrentLevelIndex(int newIndex)
    {
        _currentLevelIndex = newIndex;
    }

    public int[,] GetCurrentLevelData()
    {
        List<int[,]> levels = new List<int[,]>() { _level_1, _level_2, _level_3, _level_4, _level_5 };
        return levels[_currentLevelIndex];
    }

    public void UpdateTileData()
    {
        int[,] levelData = GetCurrentLevelData();
        
        for (int x = 0; x < 5; x++)
            for (int y = 0; y < 5; y++)
                _tileIDTable[x, y] = levelData[x, y];
        
        for (int i = 0; i < 5; i++)
            _linePoints[i] = new List<Vector2Int>();
    }
    
    public bool CheckWinGame()
    {
        int[,] levelData = GetCurrentLevelData();

        for (int colorID = 1; colorID <= 5; colorID++)
        {
            List<Vector2Int> nodes = new List<Vector2Int>();

            for (int x = 0; x < 5; x++)
                for (int y = 0; y < 5; y++)
                    if (levelData[x, y] == colorID)
                        nodes.Add(new Vector2Int(x, y));
            
            if (nodes.Count == 0) continue;
            
            int lineIndex = colorID - 1;
            foreach (Vector2Int node in nodes)
            {
                if (!_linePoints[lineIndex].Contains(node))
                    return false;
            }
        }

        return true;
    }

    #endregion
}