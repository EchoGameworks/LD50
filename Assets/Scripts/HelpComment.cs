using EchoUtilities;
using UnityEngine;

public class HelpComment : MonoBehaviour
{
    [SerializeField] GameObject comment;

    private float delayTimer;

    [SerializeField] bool showOnStart = false;

    private void Start()
    {
        delayTimer = -1f;
#if UNITY_EDITOR
        showOnStart = false;
#endif
        comment.SetActive(showOnStart);
    }

    private void Update()
    {
        if(delayTimer > 0)
        {
            delayTimer -= Time.deltaTime;
        }
        else if (delayTimer != -1f)
        {
            comment.SetActive(true);
            delayTimer = -1f;
        }
    }

    public void PointerEnter()
    {
        //print("pointer enter");
        delayTimer = 0.3f;
    }

    public void PointerExit()
    {
        //print("pointer exit");
        comment.SetActive(false);
        delayTimer = -1f;
    }
}
