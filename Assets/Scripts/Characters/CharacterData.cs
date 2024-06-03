using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Character/Character")]
public class CharacterData : ScriptableObject
{
    public GameTimestamp birthday;
    public List<ItemData> likes;
    public List<ItemData> dislikes; 

    public List<DialogueLine> onFirstMeet;
    public List<DialogueLine> defaultDialogue; 
}