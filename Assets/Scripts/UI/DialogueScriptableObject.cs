using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Dialogue Object", menuName = "Scriptable Objects/Dialogue Object", order = 0)]
public class DialogueScriptableObject : ScriptableObject
{
    [SerializeField] public string characterName;
    [SerializeField] public Image characterImage;
    [TextArea(3, 10)]
    [SerializeField] public string[] sentences;
}
