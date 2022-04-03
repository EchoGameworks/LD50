using EchoUtilities;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Note : MonoBehaviour, IDrop
{
    public enum Capabilities { Empathy, Dedication, Charisma, Intelligence, Greed, Interest }

    public Capabilities Capability;

    [SerializeField]
    private int baseValue = 1;
    [ReadOnly]
    public int modifiedValue;

    [SerializeField]
    private Canvas canvas;
    [SerializeField]
    private GameObject holder;

    [SerializeField]
    private Image paper;
    [SerializeField]
    private TextMeshProUGUI valueText;


    private Transform topLayer;
    [SerializeField] private Vector3 preDropPosition;
    [SerializeField] private Transform preDropParent;
    private IDrop preDropScript;
    [SerializeField] GameObject preDropScriptGO;

    private float dragToEnterBufferTimer = 0f;

    public bool CanMove;
 

    private void Start()
    {
        canvas = GameObject.FindGameObjectWithTag("Board Canvas").GetComponent<Canvas>();
        topLayer = GameObject.FindGameObjectWithTag("Top").transform;
        if(Capability == Capabilities.Intelligence) modifiedValue = baseValue;
    }

    private void Update()
    {
        if(dragToEnterBufferTimer > 0f) dragToEnterBufferTimer -= Time.deltaTime;
    }
    public void DragStart(BaseEventData data)
    {
        if (GameManager.instance.CurrentGameState != GameManager.GameState.Plan) return;
        if (!CanMove) return;
        //EchoLogger.Log(this, "Drag Start");
        preDropParent = this.transform.parent;
        preDropPosition = this.transform.position;
        this.transform.SetParent(topLayer);
        paper.raycastTarget = false;
        GameManager.instance.SetCurrentNote(this);
        LeanTween.moveLocalY(holder, 0f, 0.2f).setEaseInOutCirc();
        LeanTween.scale(holder, Vector3.one, 0.2f).setEaseInOutCirc();
    }

    public void DragHandler(BaseEventData data)
    {
        if (GameManager.instance.CurrentGameState != GameManager.GameState.Plan) return;
        if (!CanMove) return;
        PointerEventData pointerData = (PointerEventData)data;

        Vector2 position;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)canvas.transform,
            pointerData.position,
            canvas.worldCamera,
            out position);

        transform.position = canvas.transform.TransformPoint(position);
    }

    public void DragEnd(BaseEventData data)
    {
        //EchoLogger.Log(this, "Drag End");
        if (GameManager.instance.CurrentGameState != GameManager.GameState.Plan) return;
        if (!CanMove) return;
        if (GameManager.instance.CurrentDrop != null)
        {
            //go to new location
            if (preDropScript != null)
            {
                preDropScript.RemoveOldNote(this);
                preDropParent = null;
                preDropPosition = Vector3.positiveInfinity;
            }
            GameManager.instance.CurrentDrop.SetCurrentNote(this);
            GameManager.instance.UpdatePlan();
            LeanTween.scale(holder, Vector3.one, 0.2f).setEaseInOutCirc();
        }
        else
        {
            //Go back to where it was
            //EchoLogger.Log(this, "Dropping back to old");
            this.transform.SetParent(preDropParent, true);
            LeanTween.move(this.gameObject, preDropPosition, 0.3f).setEaseInOutCubic();
            LeanTween.scale(holder, Vector3.one, 0.2f).setEaseInOutCirc();
            HandManager maybeHand = preDropScriptGO.GetComponent<HandManager>();
            if (maybeHand != null) maybeHand.RefreshHand();
        }
        dragToEnterBufferTimer = 1f;
        GameManager.instance.SetCurrentNote(null);
        paper.raycastTarget = true;
    }

    public void PointerEnter(BaseEventData data)
    {
        if (dragToEnterBufferTimer > 0f) return;
        if (GameManager.instance.CurrentGameState != GameManager.GameState.Plan) return;
        if (!CanMove) return;

        LeanTween.moveLocalY(holder, 10f, 0.2f).setEaseInOutCirc();
        LeanTween.scale(holder, Vector3.one * 1.3f, 0.2f).setEaseInOutCirc();
        if (GameManager.instance.CurrentNote != null)
        {
            GameManager.instance.SetCurrentDrop(this);            
        }
        //EchoLogger.Log(this, "Pointer Enter");
    }

    public void PointerExit(BaseEventData data)
    {
        LeanTween.moveLocalY(holder, 0f, 0.2f).setEaseInOutCirc();
        LeanTween.scale(holder, Vector3.one, 0.2f).setEaseInOutCirc();
        GameManager.instance.SetCurrentDrop(null);
        //EchoLogger.Log(this, "Pointer Exit");
    }

    public void SetCurrentNote(Note note)
    {
        //Transfer to hand for management
        HandManager handManager = this.transform.parent.parent.GetComponent<HandManager>();
        if (handManager != null)
        {
            //preDropScript = handManager;
            handManager.AddNoteAt(note, this); //'this' = dropped on Note
        }
     }

    public void SetPreDropScript(IDrop dropScript)
    {
        preDropScript = dropScript;
        preDropScriptGO = dropScript.getGameObject();
    }

    public void SetEmpathy(int val)
    {
        modifiedValue = baseValue + val;
        valueText.text = modifiedValue.ToString();
    }

    public void SetCharisma(int val)
    {
        modifiedValue = baseValue + val;
        valueText.text = modifiedValue.ToString();
    }

    public void SetDedication(bool used)
    {
        if (used)
        {
            modifiedValue = baseValue + 1;
        }
        else
        {
            modifiedValue = baseValue;
        }
        valueText.text = modifiedValue.ToString();
    }

    public void RemoveOldNote(Note note)
    {
        HandManager handManager = this.transform.parent.parent.GetComponent<HandManager>();
        if (handManager != null) handManager.RemoveNote(note);
    }

    public GameObject getGameObject()
    {
        return gameObject;
    }
}
