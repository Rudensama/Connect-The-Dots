using System.Collections.Generic;
using UnityEngine;

public class VisualHandler : MonoBehaviour
{
    #region SINGLETON

    public static VisualHandler Instance;

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

    [SerializeField] public GameObject Prefab_Node;
    [SerializeField] public GameObject Prefab_Line;

    private List<GameObject> _nodes = new List<GameObject>();
    private List<GameObject> _lines = new List<GameObject>();
    
    private Vector3 GridToWorld(Vector2Int gridPos)
    {
        return new Vector3(gridPos.x - 2, (gridPos.y - 2) * -1, 0);
    }

    public void SpawnVisuals()
    {
        ClearVisuals();
        
        for (int x = 0; x < 5; x++)
        {
            for (int y = 0; y < 5; y++)
            {
                int id = TileData.Instance.GetOriginalNodeID(new Vector2Int(x, y));
                if (id != 0)
                {
                    Vector3 worldPos = GridToWorld(new Vector2Int(x, y));
                    GameObject node = Instantiate(Prefab_Node, worldPos, Quaternion.identity);
                    node.GetComponent<SpriteRenderer>().color = TileData.Instance.NodeColors[id];
                    _nodes.Add(node);
                }
            }
        }
        
        for (int i = 0; i < 5; i++)
        {
            GameObject lineObj = Instantiate(Prefab_Line, Vector3.zero, Quaternion.identity);
            LineRenderer lr = lineObj.GetComponent<LineRenderer>();
            Color color = TileData.Instance.NodeColors[i + 1];
            lr.startColor = color;
            lr.endColor = color;
            lr.positionCount = 0;
            _lines.Add(lineObj);
        }
    }

    public void ClearVisuals()
    {
        foreach (GameObject node in _nodes) Destroy(node);
        _nodes.Clear();

        foreach (GameObject line in _lines) Destroy(line);
        _lines.Clear();
    }

    public void UpdateVisuals()
    {
        for (int i = 0; i < _lines.Count; i++)
        {
            LineRenderer lr = _lines[i].GetComponent<LineRenderer>();
            List<Vector2Int> points = TileData.Instance.GetLinePoints(i);

            lr.positionCount = points.Count;

            for (int p = 0; p < points.Count; p++)
            {
                lr.SetPosition(p, GridToWorld(points[p]));
            }
        }
    }
}