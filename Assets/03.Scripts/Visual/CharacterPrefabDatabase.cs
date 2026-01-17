using System.Collections.Generic;
using UnityEngine;
using Visual;

[CreateAssetMenu()]
public class CharacterPrefabDatabase : ScriptableObject
{
    public SerializableDictionary<string, CharacterVisual> CharacterViews;
}
