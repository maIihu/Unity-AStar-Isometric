using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class CharacterBase : MonoBehaviour
{
    [HideInInspector] public bool isMove;

    public TileNode standingTile;

    private Pathfinding _pathFinding;
    private TileInRange _tileInRange;

    private List<TileNode> _path;
    private List<TileNode> _tilesCanMove;
    
    private GameObject _tileContainer;

    [SerializeField] private int moveRange; 
    [SerializeField] private int attackRange;
    
    private void Start()
    {
        _tileContainer = MapManager.Instance.tileContainer;
        _pathFinding = new Pathfinding();
        _tileInRange = new TileInRange();

        _path = new List<TileNode>();
        _tilesCanMove = new List<TileNode>();
        
        for (int i = 0; i < _tileContainer.transform.childCount; i++)
        {
            GameObject tileGameObject = _tileContainer.transform.GetChild(i).gameObject;
            if ((Vector2)transform.position == (Vector2)tileGameObject.GetComponent<TileNode>().gridLocation)
            {
                standingTile = _tileContainer.transform.GetChild(i).gameObject.GetComponent<TileNode>();
            }
        }
        this.gameObject.GetComponent<SpriteRenderer>().sortingOrder =
            standingTile.GetComponent<SpriteRenderer>().sortingOrder;
        
        this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, standingTile.transform.position.z + 10);
    }

    private void Update()
    {
        if (_path.Count > 0)
        {
            foreach (var tile in _path)
            {
                tile.GetComponent<SpriteRenderer>().color = Color.yellow;
            }
            
            MoveAlongPath();
            foreach (var tile in _tilesCanMove)
            {
                tile.GetComponent<SpriteRenderer>().color = Color.white;
            }
            _tilesCanMove.Clear();
        }
    }

    public void ShowTileCanMove()
    {
        if(_tilesCanMove.Count <= 0)
            GetInRangeTiles();
    }
    public void MoveToTile(TileNode tileToMove)
    {
        Debug.Log("Ham nay chay");
        if (_tilesCanMove.Count > 0)
            _path = _pathFinding.FindPath(this.standingTile, tileToMove, _tilesCanMove);
        
        if (_path.Count > 0)
        {
            standingTile = tileToMove;
        }
    } 
    
    private void GetInRangeTiles()
    {
        _tilesCanMove = _tileInRange.GetTilesInRange(this.standingTile, moveRange, new List<TileNode>());
        foreach (var tile in _tilesCanMove)
        {
            tile.GetComponent<SpriteRenderer>().color = Color.blue;
        }
    }
    
    private void MoveAlongPath()
    {
        var step = 2f * Time.deltaTime;

        float zIndex = _path[0].transform.position.z;
        this.transform.position = Vector2.MoveTowards(this.transform.position, _path[0].transform.position, step);
        this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, zIndex);

        if(Vector2.Distance(this.transform.position, _path[0].transform.position) < 0.00001f)
        {
            this.transform.position = new Vector3(_path[0].transform.position.x, _path[0].transform.position.y, _path[0].transform.position.z);
            _path[0].gameObject.GetComponent<SpriteRenderer>().color = Color.white;
            _path.RemoveAt(0);
        }
        isMove = false;
    }
    
}
