﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Effect", menuName = "Effect")]
public class Effect : ScriptableObject
{
    public new string name;
    public string effectType;
    public float stackValue;
    public Sprite effectImage;
    public Color effectColor;
}
