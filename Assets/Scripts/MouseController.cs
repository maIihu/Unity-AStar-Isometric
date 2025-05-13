using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;


public class MouseController : MonoBehaviour
{
    private GameObject[] _characters;
    private bool _isAction = false;

    

    private void Start()
    {
        // cai nay loi~ vi gan chi gan Prefab chu khogn lay gameobject tren Hierarchy
        //_characters = MapManager.Instance.characters; 
        _characters = GameObject.FindGameObjectsWithTag("Character");
        
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var hit = GetObjectClicked();
            if (hit.HasValue)
            {
                GameObject gameObjectActive = hit.Value.transform.gameObject;
                //Debug.Log("Click to " + gameObjectActive.name);
                if (gameObjectActive.CompareTag("Character") && !_isAction)
                {
                    CharacterBase characterBaseClone = gameObjectActive.GetComponent<CharacterBase>();
                    characterBaseClone.isMove = true;
                    characterBaseClone.ShowTileCanMove();
                    characterBaseClone.ShowAttackRange();
                    _isAction = true;
                }
                
                if (gameObjectActive.CompareTag("Tile"))
                {
                    foreach (var character in _characters)
                    {
                        CharacterBase characterBase = character.GetComponent<CharacterBase>();
                        if (characterBase.isMove)
                        {
                            characterBase.standingTile.GetComponent<TileNode>().isBlocked = false;
                            characterBase.MoveToTile(gameObjectActive.GetComponent<TileNode>());
                            characterBase.standingTile.GetComponent<TileNode>().isBlocked = true;
                        }
                    }
                }
            }
        }

        if (_isAction)
        {
            _isAction = _characters.Any(x => x.transform.GetComponent<CharacterBase>().isMove);
            // foreach (var character in characters)
            // {
            //     CharacterBase characterBase = character.GetComponent<CharacterBase>();
            //     if (characterBase.isMove)
            //         break;
            //     _isAction = false;
            // }
        }
        
    }
        
    private static RaycastHit2D? GetObjectClicked()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos, Vector2.zero);
        if (hits.Length > 0)
            return hits.OrderByDescending(x => x.collider.transform.position.z).First();
        return null;
    }
}
