using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueEditor;
public class Target : MonoBehaviour
{
    [SerializeField] private NPCConversation conversation;
    public void GetHit()
        {
        if(conversation != null) ConversationManager.Instance.StartConversation(conversation);
        GameManager.instance.TargetHit();
        }
}
