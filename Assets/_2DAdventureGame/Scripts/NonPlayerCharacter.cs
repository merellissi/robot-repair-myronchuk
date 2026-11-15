using UnityEngine;

public class NonPlayerCharacter : MonoBehaviour
{
    public GameObject dialogueBubble;

    [TextArea]
    public string dialogueText = "Привіт! Полагодь усіх роботів!";

    void Start()
    {
        dialogueBubble.SetActive(false);
    }
}