using EchoUtilities;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class HandManager : MonoBehaviour, IDrop
{
    public List<Note> Notes;

    [SerializeField]
    private HorizontalLayoutGroup layout;
    public void AddNote(Note note)
    {
        //EchoLogger.Log(this, "AddingNote");
        if (Notes.Contains(note)) Notes.Remove(note);
        Notes.Add(note);
        note.SetPreDropScript(this);
        note.transform.SetParent(layout.transform);
        layout.SetLayoutHorizontal();
    }

    public void StartNewGame()
    {
        Notes = new List<Note>();
        for (int i = layout.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(layout.transform.GetChild(i).gameObject);
        }
    }

    public void AddNoteAt(Note note, Note at)
    {
        int indx = 0;
        if (Notes.Contains(note))
        {
            Notes.Remove(note);
            indx = Notes.IndexOf(at) + 1;
        }
        else
        {
            indx = Notes.IndexOf(at) + 1;
        }
        //EchoLogger.Log(this, "AddingNoteAt " + indx);
        Notes.Insert(indx, note);
        note.SetPreDropScript(this);
        note.transform.SetParent(layout.transform);
        note.transform.SetSiblingIndex(indx);
        layout.SetLayoutHorizontal();
    }

    public void RemoveNote(Note note)
    {
        Notes.Remove(note);
        layout.SetLayoutHorizontal();
    }

    public void RefreshHand()
    {
        layout.SetLayoutHorizontal();
    }

    public void SetCurrentNote(Note note)
    {
        
    }

    public void RemoveOldNote(Note note)
    {
        RemoveNote(note);
    }

    public GameObject getGameObject()
    {
        return gameObject;
    }
}
