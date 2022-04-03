using EchoUtilities;
using System.Collections.Generic;
using UnityEngine;

public class DrawDeck : MonoBehaviour
{
    public List<Note> Notes;

    [SerializeField] public Transform holder;
    [SerializeField] private GameObject prefabEmpathy;
    [SerializeField] private GameObject prefabIntelligence;
    [SerializeField] private GameObject prefabDedication;
    [SerializeField] private GameObject prefabCharisma;
    [SerializeField] private GameObject prefabGreed;
    [SerializeField] private GameObject prefabInterest;

    private HandManager handManager;
    
    void Awake()
    {
        handManager = GameObject.FindGameObjectWithTag("Hand").GetComponent<HandManager>();
    }

    public void StartNewGame()
    {
        int count = holder.childCount - 1;
        for (int i = count; i > 0; i--)
        {
            Destroy(holder.GetChild(i).gameObject);
        }

        Notes = new List<Note>();
        List<Transform> tempOrder = new List<Transform>();
        int id = 0;
        for (int i = 0; i < 8; i++)
        {
            GameObject noteGO = Instantiate(prefabEmpathy, holder, true);
            noteGO.name = "Note - Empathy (" + id + ")";
            id++;
            //Note note = noteGO.GetComponent<Note>();
            noteGO.transform.localPosition = Vector3.zero;
            tempOrder.Add(noteGO.transform);
        }

        for (int i = 0; i < 8; i++)
        {
            GameObject noteGO = Instantiate(prefabIntelligence, holder, true);
            noteGO.name = "Note - Intelligence (" + id + ")";
            id++;
            //Note note = noteGO.GetComponent<Note>();
            noteGO.transform.localPosition = Vector3.zero;
            tempOrder.Add(noteGO.transform);
        }

        for (int i = 0; i < 8; i++)
        {
            GameObject noteGO = Instantiate(prefabCharisma, holder, true);
            noteGO.name = "Note - Charisma (" + id + ")";
            id++;
            //Note note = noteGO.GetComponent<Note>();
            noteGO.transform.localPosition = Vector3.zero;
            tempOrder.Add(noteGO.transform);
        }

        for (int i = 0; i < 8; i++)
        {
            GameObject noteGO = Instantiate(prefabDedication, holder, true);
            noteGO.name = "Note - Dedication (" + id + ")";
            id++;
            //Note note = noteGO.GetComponent<Note>();
            noteGO.transform.localPosition = Vector3.zero;
            tempOrder.Add(noteGO.transform);
        }

        for (int i = tempOrder.Count - 1; i >= 0; i--)
        {
            int index = Random.Range(0, tempOrder.Count);
            Notes.Add(tempOrder[index].GetComponent<Note>());
            tempOrder[index].SetAsLastSibling();
            tempOrder.RemoveAt(index);
        }
    }

    public void DrawCards()
    {
        int alreadyInHand = handManager.Notes.Count;
        int dealAmount = 5 - alreadyInHand;
        for (int i = 0; i < dealAmount; i++)
        {
            Note drawNote = Notes[Notes.Count - 1];
            drawNote.CanMove = true;
            handManager.AddNote(drawNote);
            Notes.Remove(drawNote);
        }
    }


    void Update()
    {
        
    }
}
