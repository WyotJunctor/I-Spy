using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlanetoidList", menuName = "ScriptableObjects/Create PlanetoidList", order = 1)]
public class PlanetoidScriptableObjectList : ScriptableObject
{
    public List<GameObject> planetoids;
}