using System;
using UnityEngine;

[System.Flags]
public enum Tags
{
    None = 0,
    Wall = 1 << 0, 
    Ground = 1 << 1,
    Silkable = 1 << 2
}


public class CustomTagger : MonoBehaviour
{
    public Tags tags;
}
