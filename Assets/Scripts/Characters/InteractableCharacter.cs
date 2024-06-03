using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableCharacter : InteractableObject
{
    public CharacterData characterData;
    NPCRelationshipState relationship;

    private void Start()
    {
        relationship = RelationshipStats.GetRelationship(characterData);
    }

    public override void Pickup()
    {
        List<DialogueLine> dialogueToHave = characterData.defaultDialogue;
        System.Action onDialogueEnd = null;

        if (RelationshipStats.FirstMeeting(characterData))
        {
            dialogueToHave = characterData.onFirstMeet;
            onDialogueEnd += OnFirstMeeting;
        }

        if (RelationshipStats.IsFirstConversationOfTheDay(characterData))
        {
            onDialogueEnd += OnFirstConversation; 
        }

        DialogueManager.Instance.StartDialogue(dialogueToHave, onDialogueEnd);
    }

    void OnFirstMeeting()
    {
        RelationshipStats.UnlockCharacter(characterData);
        relationship = RelationshipStats.GetRelationship(characterData);
    }

    void OnFirstConversation()
    {
        RelationshipStats.AddFriendPoints(characterData, 20);
        relationship.hasTalkedToday = true;
    }
}