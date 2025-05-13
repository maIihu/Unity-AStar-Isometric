using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    [SerializeField] private GameObject tilePrefab;
    public GameObject tileContainer;
    public GameObject[] characters;
    
    private Dictionary<Vector2Int, TileNode> _map;

    private static MapManager _instance;
    public static MapManager Instance { get { return _instance; } }

    private Dictionary<Vector3, bool> _locationList;
    
    private void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
            _instance = this;
    }

    public void Start()
    {
        _locationList = new Dictionary<Vector3, bool>();
        _locationList.Add(new Vector3(4, 2, 1), false);
        _locationList.Add(new Vector3(5.5f, 1.75f, 1), false);
        
        BuildMap();
        
        SpawnCharacter();
    }

    private void BuildMap()
    {
        _map = new Dictionary<Vector2Int, TileNode>();
        Tilemap tileMap = GetComponentInChildren<Tilemap>();
        BoundsInt bound = tileMap.cellBounds;
        
        for (var z = bound.max.z; z >= bound.min.z; z--)
        {
            for (var x = bound.min.x; x <= bound.max.x; x++)
            {
                for (var y = bound.min.y; y <= bound.max.y; y++)
                {
                    Vector3Int tileLocation = new Vector3Int(x, y, z);
                    Vector2Int tileKey = new Vector2Int(x, y);
                    
                    if (tileMap.HasTile(tileLocation) && !_map.ContainsKey(tileKey))
                    {
                        Vector3 pos = tileMap.GetCellCenterWorld(tileLocation);
                        GameObject tileClone = Instantiate(tilePrefab, pos, Quaternion.identity, tileContainer.transform);
                        tileClone.name = "Tile " + pos;
                        
                        tileClone.GetComponent<SpriteRenderer>().sortingOrder =
                            tileMap.GetComponent<TilemapRenderer>().sortingOrder;

                        TileNode tileNodeClone = tileClone.GetComponent<TileNode>();
                        tileNodeClone.gridLocationInt = tileLocation;
                        tileNodeClone.gridLocation = pos;
                        
                        _map.Add(tileKey, tileNodeClone);
                    }
                }
            }
        }
    }

    private void SpawnCharacter()
    {
        Tilemap tileMap = GetComponentInChildren<Tilemap>(); 

        foreach (var character in characters)
        {
            var location = _locationList.First(i => !i.Value);
            GameObject characterInstance = Instantiate(character, location.Key, Quaternion.identity);

            _locationList[location.Key] = true;

            Vector3Int cellPosition = tileMap.WorldToCell(location.Key);
            Vector2Int tileKey = new Vector2Int(cellPosition.x, cellPosition.y);
            
            if (_map.ContainsKey(tileKey))
            {
                _map[tileKey].isBlocked = true;
            }
        }
    }

    
    public List<TileNode> GetNeighbourTiles(TileNode currentTile, List<TileNode> availableTiles)
    {
        Dictionary<Vector2Int, TileNode> tilesToCheck = new Dictionary<Vector2Int, TileNode>();
        if (availableTiles.Count > 0)
        {
            foreach (var tile in availableTiles)
            {
                tilesToCheck.Add(new Vector2Int(tile.gridLocationInt.x, tile.gridLocationInt.y), tile);
            }
        }
        else
            tilesToCheck = _map;

        List<TileNode> neighbourTiles = new List<TileNode>();

        Vector2Int[] directions = { Vector2Int.down, Vector2Int.up, Vector2Int.left, Vector2Int.right };

        foreach (var direction in directions)
        {
            Vector2Int locationToCheck = (Vector2Int)currentTile.gridLocationInt + direction;
            if (_map.ContainsKey(locationToCheck))
            {
                if (tilesToCheck.ContainsKey(locationToCheck) && !tilesToCheck[locationToCheck].isBlocked)
                {
                    if (Mathf.Abs(currentTile.transform.position.z -
                                  tilesToCheck[locationToCheck].transform.position.z) <= 1)
                        neighbourTiles.Add(tilesToCheck[locationToCheck]);
                }
            }
        }

        return neighbourTiles;
    }

    public List<TileNode> GetStraightTiles(TileNode currentTile, int attackRange)
    {
        List<TileNode> availableTiles = new List<TileNode>();
        for (int i = 1; i <= attackRange; i++)
        {
            Vector2Int locationToCheck = (Vector2Int)currentTile.gridLocationInt + new Vector2Int(i, 0);
            if (_map.ContainsKey(locationToCheck))
            {
                if (!_map[locationToCheck].isBlocked && Mathf.Abs(currentTile.transform.position.z -
                                                                  _map[locationToCheck].transform.position.z) <= 1)
                    availableTiles.Add(_map[locationToCheck]);
                else
                    break;
            }
        }
        
        for (int i = 1; i <= attackRange; i++)
        {
            Vector2Int locationToCheck = (Vector2Int)currentTile.gridLocationInt + new Vector2Int(-i, 0);
            if (_map.ContainsKey(locationToCheck))
            {
                if (!_map[locationToCheck].isBlocked && Mathf.Abs(currentTile.transform.position.z -
                                                                  _map[locationToCheck].transform.position.z) <= 1)
                    availableTiles.Add(_map[locationToCheck]);
                else
                    break;
            }
        }
        
        for (int i = 1; i <= attackRange; i++)
        {
            Vector2Int locationToCheck = (Vector2Int)currentTile.gridLocationInt + new Vector2Int(0, -i);
            if (_map.ContainsKey(locationToCheck))
            {
                if (!_map[locationToCheck].isBlocked && Mathf.Abs(currentTile.transform.position.z -
                                                                  _map[locationToCheck].transform.position.z) <= 1)
                    availableTiles.Add(_map[locationToCheck]);
                else
                    break;
            }
        }
        
        for (int i = 1; i <= attackRange; i++)
        {
            Vector2Int locationToCheck = (Vector2Int)currentTile.gridLocationInt + new Vector2Int(0, i);
            if (_map.ContainsKey(locationToCheck))
            {
                if (!_map[locationToCheck].isBlocked && Mathf.Abs(currentTile.transform.position.z -
                                                                  _map[locationToCheck].transform.position.z) <= 1)
                    availableTiles.Add(_map[locationToCheck]);
                else
                    break;
            }
        }

        return availableTiles;
    }
}
