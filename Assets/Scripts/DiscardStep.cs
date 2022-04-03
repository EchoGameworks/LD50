using EchoUtilities;
using UnityEngine;
using UnityEngine.EventSystems;

public class DiscardStep : MonoBehaviour
{
    [SerializeField] DayDrop parentDay;
    [SerializeField] DrawDeck deck;

    [SerializeField] private GameObject bgActive;
    [SerializeField] private GameObject bgInactive;

    public bool IsActive;
    public void MoveToDrawDeck() {
        for (int i = 0; i < parentDay.UsedNotes.Count; i++) { 
            deck.Notes.Insert(0, parentDay.UsedNotes[i]);
        }
        //deck.Notes.AddRange(parentDay.UsedNotes);
        foreach (Note note in parentDay.UsedNotes) {
            LeanTween.move(note.gameObject, deck.holder.transform, 0.3f).setEaseInOutCubic();
        }
        LeanTween.delayedCall(0.35f, () =>
        {
            //for (int i = 0; i < parentDay.UsedNotes.Count; i++)
            //{
            //    parentDay.UsedNotes[i].transform.SetParent(deck.holder.transform, false);
            //    parentDay.UsedNotes.RemoveAt(i);
            //}

            for (int i = 0; i < parentDay.UsedNoteHolder.transform.childCount; i++)
            {
                Transform dummyT = parentDay.UsedNoteHolder.transform.GetChild(i);
                if (dummyT.childCount > 0)
                {
                    Note removedNote = dummyT.GetChild(0).GetComponent<Note>();
                    removedNote.transform.SetParent(deck.holder.transform, false);
                    parentDay.UsedNotes.Remove(removedNote);
                }

            }
            GameManager.instance.MoveToNextWeek();
        });

    }

    public void PointerEnter(BaseEventData data)
    {
        if (!IsActive) return;
        LeanTween.scale(gameObject, Vector3.one * 1.3f, 0.2f).setEaseInOutCirc();
        //EchoLogger.Log(this, "Pointer Enter");
    }

    public void PointerExit(BaseEventData data)
    {
        LeanTween.scale(gameObject, Vector3.one, 0.2f).setEaseInOutCirc();
    }

    public void Activate()
    {
        IsActive = true;
        bgActive.SetActive(true);
        LeanTween.scale(bgActive, Vector3.one * 1.3f, 0.2f).setEaseInOutCubic().setLoopPingPong(1);
        bgInactive.SetActive(false);
    }

    public void Deactivate()
    {
        IsActive = false;
        bgActive.SetActive(false);
        bgInactive.SetActive(true);
        //LeanTween.scale(bgInactive, Vector3.one * 1.3f, 0.2f).setEaseInOutCubic().setLoopPingPong(1);
    }
}
