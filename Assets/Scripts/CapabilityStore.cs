using EchoUtilities;
using Sirenix.OdinInspector;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Note;

public class CapabilityStore : MonoBehaviour
{
    [SerializeField] Capabilities Capability;
    [SerializeField] int startingValue;
    [SerializeField] int currentValue;

    [SerializeField] TextMeshProUGUI valueText;
    [SerializeField] Image background;

    private Canvas canvas;
    private Vector3 startLocation;

    private float nonInteractTimer = 0f;

    private bool hasTentativeChanges;

    [SerializeField] private bool canDrag;
    private void Start()
    {
        canvas = GameObject.FindGameObjectWithTag("Board Canvas").GetComponent<Canvas>();
        startLocation = this.transform.position;
        UpdateProgress();
    }

    private void Update()
    {
        if(nonInteractTimer > 0f) nonInteractTimer--;
    }

    [Button]
    public void UpdateProgress()
    {
        if (startingValue != currentValue)
        {
            string newText = "<b>" + currentValue.ToString() + "<b>";
            if(currentValue > startingValue)
            {
                newText = "<color=#94D377>" + newText + "</color>";
            }
 
            valueText.text = newText;
        }
        else
        {
            valueText.text = startingValue.ToString();
        }
        
    }

    public void CommitCurrentValue()
    {
        LeanTween.scale(gameObject, Vector3.one * 1.3f, 0.3f).setEaseInOutCirc().setLoopPingPong(1);
        startingValue = currentValue;
        UpdateProgress();
        canDrag = true;
        
    }

    public void ResetTempScore()
    {
        currentValue = 0;
    }

    public void ResetStartingScore()
    {
        startingValue = 0;
        currentValue = 0;
        UpdateProgress();
    }

    public void AddScore(int val)
    {
        currentValue += val;
    }
    public void DragStart(BaseEventData data)
    {
        if (nonInteractTimer > 0 || !canDrag) return;
        if (GameManager.instance.CurrentGameState != GameManager.GameState.Event) return;
        //EchoLogger.Log(this, "Drag Start");
        GameManager.instance.CurrentStore = this;
        background.raycastTarget = false;
        startLocation = this.transform.position;
        // LeanTween.moveLocalY(holder, 0f, 0.2f).setEaseInOutCirc();
        LeanTween.scale(background.gameObject, Vector3.one, 0.2f).setEaseInOutCirc();
    }

    public void DragHandler(BaseEventData data)
    {
        if (!canDrag) return;
        if (GameManager.instance.CurrentGameState != GameManager.GameState.Event) return;
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
        if (GameManager.instance.CurrentGameState != GameManager.GameState.Event) return;
        nonInteractTimer = 0.3f;
        if (GameManager.instance.CurrentEvent != null && GameManager.instance.CurrentEvent.Requirements.Where(o => o.Capability == Capability).Any())
        {
            print("capability is in requirements");
            Requirement req = GameManager.instance.CurrentEvent.Requirements.Where(o => o.Capability == Capability).FirstOrDefault();
            req.AddPoints(startingValue);
            startingValue = 0;
            currentValue = 0;
            UpdateProgress();
            GameManager.instance.CurrentEvent.CheckComplete();
            GameManager.instance.CurrentStore = null;
            LeanTween.move(gameObject, startLocation, 0.3f).setEaseInOutCirc();
        }
        else
        {
            GameManager.instance.CurrentStore = null;
            LeanTween.move(gameObject, startLocation, 0.3f).setEaseInOutCirc();
        }

        background.raycastTarget = true;
    }

    public void PointerEnter(BaseEventData data)
    {
        if (!canDrag) return;
        if (GameManager.instance.CurrentGameState != GameManager.GameState.Event) return;
        //if (dragToEnterBufferTimer > 0f) return;

        //LeanTween.moveLocalY(holder, 10f, 0.2f).setEaseInOutCirc();
        LeanTween.scale(background.gameObject, Vector3.one * 1.2f, 0.2f).setEaseInOutCirc();
        //EchoLogger.Log(this, "Pointer Enter");
    }

    public void PointerExit(BaseEventData data)
    {
        //LeanTween.moveLocalY(holder, 0f, 0.2f).setEaseInOutCirc();
        LeanTween.scale(background.gameObject, Vector3.one, 0.2f).setEaseInOutCirc();
        GameManager.instance.SetCurrentDrop(null);
        //EchoLogger.Log(this, "Pointer Exit");
    }


}
