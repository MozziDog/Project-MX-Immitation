using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Visual;

[CreateAssetMenu()]
public class CharacterPrefabDatabase : SerializedScriptableObject
{
    public Dictionary<string, CharacterVisual> CharacterViews;
}
