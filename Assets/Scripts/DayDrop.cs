using EchoUtilities;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DayDrop : MonoBehaviour, IDrop
{
    public Note CurrentNote;
    public List<Note> UsedNotes;

    [SerializeField]
    public RectTransform UsedNoteHolder;
    private VerticalLayoutGroup layoutUsedNotes;

    [SerializeField]
    public RectTransform CurrentNoteHolder;

    [SerializeField]
    private GameObject paper;

    private HandManager handManager;

    [SerializeField] public GameObject NextButton;
    private void Awake()
    {
        handManager = GameObject.FindGameObjectWithTag("Hand").GetComponent<HandManager>();
        layoutUsedNotes = UsedNoteHolder.GetComponent<VerticalLayoutGroup>();
    }

    public void StartNewGame()
    {
        for(int i = layoutUsedNotes.transform.childCount - 1; i >= 0; i--)
        {
            Transform dummyT = layoutUsedNotes.transform.GetChild(i);
            if(dummyT.childCount > 0)
            {
                Destroy(dummyT.GetChild(0).gameObject);
            }            
        }

        for (int i = CurrentNoteHolder.transform.childCount - 1; i >= 0; i--)
        {
            Transform dummyT = CurrentNoteHolder.transform.GetChild(i);
            if (dummyT.childCount > 0)
            {
                Destroy(dummyT.GetChild(0).gameObject);
            }
        }

        UsedNotes = new List<Note>();
        CurrentNote = null;
    }

    public void PointerEnter(BaseEventData data)
    {
        //EchoLogger.Log(this, "Pointer Enter");
        if (GameManager.instance.CurrentNote != null)
        {
            GameManager.instance.SetCurrentDrop(this);
            LeanTween.scale(paper, Vector3.one * 1.2f, 0.2f).setEaseInOutBack().setLoopCount(1);
        }

    }

    public void PointerExit(BaseEventData data)
    {
        //EchoLogger.Log(this, "Pointer Exit");
        GameManager.instance.SetCurrentDrop(null);
        LeanTween.scale(paper, Vector3.one, 0.2f).setEaseInOutBack().setLoopCount(1);
    }

    public void SetCurrentNote(Note note)
    {
        Note previousNote = CurrentNote;
        if (previousNote != null)
        {
            //Remove old Current Note
            handManager.AddNote(previousNote);
        }

        //Set New Current Note
        CurrentNote = note;
        //handManager.RemoveNote(CurrentNote);
        CurrentNote.transform.SetParent(CurrentNoteHolder.transform, true);
        CurrentNote.SetPreDropScript(this);
        LeanTween.moveLocal(CurrentNote.gameObject, new Vector3(10f,-18f,0f), 0.2f).setEaseInOutCubic();
        GameManager.instance.UpdatePlan();
    }

    public void RemoveOldNote(Note note)
    {
        CurrentNote = null;
    }

    public GameObject getGameObject()
    {
        return gameObject;        
    }

    public void MoveActiveToUsed()
    {
        if (CurrentNote != null)
        {
            UsedNotes.Add(CurrentNote);
            CurrentNote.CanMove = false;
            Vector3 targetPos = UsedNoteHolder.GetChild(UsedNotes.Count - 1).position;
            LeanTween.move(CurrentNote.gameObject, targetPos, 0.3f).setEaseInOutCirc()
                .setOnComplete(() =>
                {
                    CurrentNote.transform.SetParent(UsedNoteHolder.GetChild(UsedNotes.Count - 1), true);
                    CurrentNote = null;
                });            
        }
    }
}
