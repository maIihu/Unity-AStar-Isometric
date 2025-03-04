using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class TileNode : MonoBehaviour
{
    public int G;
    public int H;
    public int F { get { return G + H; } }

    public TileNode previousNode;
    public Vector3Int gridLocationInt;
    public Vector3 gridLocation;
    
    public bool isBlocked;
    
    
}
