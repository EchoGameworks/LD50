using EchoUtilities;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class GameManager : MonoBehaviour
{
    public Note CurrentNote;
    public IDrop CurrentDrop;
    public CapabilityStore CurrentStore;
    public GameEvent CurrentEvent;
    public List<GameEvent> GameEvents;
    [SerializeField] List<GameObject> availableGameEventGOs;
    List<GameEvent> availableGameEvents;
    [SerializeField] Transform eventHolder;
    [SerializeField] Transform availableEventHolder;

    public int FundsRaised;
    [SerializeField] private TextMeshProUGUI fundsRaisedText;
    public static GameManager instance;
    public int WeekCount;
    public enum GameState { Plan, Event }
    public GameState CurrentGameState;
    [SerializeField]
    private List<DayDrop> Days;

    [SerializeField] private CapabilityStore CharismaStore;
    [SerializeField] private CapabilityStore DedicationStore;
    [SerializeField] private CapabilityStore EmpathyStore;
    [SerializeField] private CapabilityStore IntelligenceStore;

    [SerializeField] private List<Measure> measures; 

    public List<DayDrop> DayList;

    private bool confirmedUsedDedicationLastRound;
    private bool tentativeUsedDedication;

    [SerializeField] private TextMeshProUGUI weekText;
    [SerializeField] private DrawDeck deck;
    [SerializeField] ButtonNextStep nextEventButton;

    [SerializeField] GameObject gameoverCanvas;
    [SerializeField] TextMeshProUGUI gameoverText;

    [SerializeField] HandManager handManager;
    private Controls ctrl;

    void Awake()
    {
        //CurrentLevelContainer = null;
        //Singleton
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        ctrl = new Controls();
        ctrl.Enable();

        ctrl.Player.Restart.performed += Restart_performed;
    }

    private void Restart_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        StartNewGame();
    }

    private void Start()
    {
        availableGameEvents = new List<GameEvent>();
        foreach(GameObject go in availableGameEventGOs)
        {
            availableGameEvents.Add(go.GetComponent<GameEvent>());
        }
        StartNewGame();
    }

    public void StartNewGame()
    {
        GameEvents = new List<GameEvent>();
        gameoverCanvas.SetActive(false);
        FundsRaised = 0;
        foreach (Measure m in measures)
        {
            m.StartNewGame();
        }
        PopulateEvents();
        CharismaStore.ResetStartingScore();
        DedicationStore.ResetStartingScore();
        EmpathyStore.ResetStartingScore();
        IntelligenceStore.ResetStartingScore();
        
        foreach(DayDrop d in DayList)
        {
            d.StartNewGame();
        }

        handManager.StartNewGame();
        deck.StartNewGame();
        
        WeekCount = 0;
        MoveToNextWeek();
    }

    void PopulateEvents()
    {
        int countEvents = GameEvents.Count;
        for(int i = 0; i < 4 - countEvents; i++)
        {
            AddEvent();
        }
    }

    public void AddEvent()
    {
        if(availableGameEvents == null || availableGameEvents.Count == 0)
        {
            availableGameEvents = new List<GameEvent>();
            for (int i = 0; i < availableEventHolder.childCount - 1; i++)
            {
                availableGameEvents.Add(availableEventHolder.GetChild(i).GetComponent<GameEvent>());
            }
        }

        GameEvent ge;
        bool hasCrisisEvent = GameEvents.Any(o => o.hasCrisis);
        if (!hasCrisisEvent)
        {
            List<GameEvent> crisisEvents = availableGameEvents.Where(o => o.hasCrisis && o.weekIntro <= WeekCount && !o.IsActive).ToList();
            ge = crisisEvents[Random.Range(0, crisisEvents.Count - 1)];
        }
        else
        {
            if(WeekCount > 6)
            {
                List<GameEvent> normalEvents = availableGameEvents.Where(o => o.weekIntro <= WeekCount && !o.IsActive).ToList();
                ge = normalEvents[Random.Range(0, normalEvents.Count - 1)];
            }
            else
            {
                //Easy Mode
                List<GameEvent> normalEvents = availableGameEvents.Where(o => !o.hasCrisis && o.weekIntro <= WeekCount && !o.IsActive).ToList();
                ge = normalEvents[Random.Range(0, normalEvents.Count - 1)];
            }

        }

        GameEvents.Add(ge);
        availableGameEvents.Remove(ge);
        //ge.gameObject.transform.localScale = Vector3.zero;
        ge.ClearRequirementProgress();
        ge.IsActive = true;
        for (int i = 0; i < eventHolder.childCount; i++)
        {
            if (eventHolder.GetChild(i).childCount == 0)
            {
                ge.transform.position = eventHolder.GetChild(i).position;
                ge.transform.SetParent(eventHolder.GetChild(i));
                //LeanTween.scale(ge.gameObject, Vector3.one, 0.3f).setEaseInOutCirc();
                break;
            }
        }
    }

    public void RemoveEvent(GameEvent e)
    {
        LeanTween.scale(e.gameObject, Vector3.zero, 0.3f).setEaseInOutCirc()
            .setOnComplete(() =>
            {
                e.transform.SetParent(availableEventHolder, true);
                e.IsActive = false;
                GameEvents.Remove(e);
                PopulateEvents();
            });

    }

    public void SetCurrentNote(Note note)
    {
        CurrentNote = note;
    }

    public void SetCurrentDrop(IDrop drop)
    {
        CurrentDrop = drop;
    }

    public void UpdatePlan()
    {
        //Empathy Check
        int countEmpathy = Days.Count(o => o.CurrentNote != null && o.CurrentNote.Capability == Note.Capabilities.Empathy) - 1;
        //print("empathy count: " + countEmpathy);
        for (int i = 0; i < Days.Count; i++)
        {
            if (Days[i].CurrentNote != null && Days[i].CurrentNote.Capability == Note.Capabilities.Empathy) Days[i].CurrentNote.SetEmpathy(countEmpathy);
        }

        //Charisma Check
        for (int i = 0; i < Days.Count; i++)
        {
            if (Days[i].CurrentNote != null && Days[i].CurrentNote.Capability == Note.Capabilities.Charisma)
            {
                int prevIndex = i - 1;
                int nextIndex = i + 1;
                int deduction = 0;
                if(prevIndex >= 0 && Days[prevIndex].CurrentNote != null && Days[prevIndex].CurrentNote.Capability == Note.Capabilities.Charisma) deduction--;        
                if(nextIndex < Days.Count && Days[nextIndex].CurrentNote != null && Days[nextIndex].CurrentNote.Capability == Note.Capabilities.Charisma) deduction--;
                Days[i].CurrentNote.SetCharisma(deduction);
            } 
        }

        //Dedication Check        
        tentativeUsedDedication = false;
         for (int i = 0; i < Days.Count; i++)
        {
            if (Days[i].CurrentNote != null && Days[i].CurrentNote.Capability == Note.Capabilities.Dedication)
            {
                tentativeUsedDedication = true;
                Days[i].CurrentNote.SetDedication(confirmedUsedDedicationLastRound);                
            }
        }

        CharismaStore.ResetTempScore();
        DedicationStore.ResetTempScore();
        EmpathyStore.ResetTempScore();
        IntelligenceStore.ResetTempScore();

        for (int i = 0; i < Days.Count; i++)
        {
            if (Days[i].CurrentNote != null && Days[i].CurrentNote.Capability == Note.Capabilities.Charisma) CharismaStore.AddScore(Days[i].CurrentNote.modifiedValue);
            if (Days[i].CurrentNote != null && Days[i].CurrentNote.Capability == Note.Capabilities.Dedication) DedicationStore.AddScore(Days[i].CurrentNote.modifiedValue);
            if (Days[i].CurrentNote != null && Days[i].CurrentNote.Capability == Note.Capabilities.Empathy) EmpathyStore.AddScore(Days[i].CurrentNote.modifiedValue);
            if (Days[i].CurrentNote != null && Days[i].CurrentNote.Capability == Note.Capabilities.Intelligence) IntelligenceStore.AddScore(Days[i].CurrentNote.modifiedValue);
        }

        CharismaStore.UpdateProgress();
        DedicationStore.UpdateProgress();
        EmpathyStore.UpdateProgress();
        IntelligenceStore.UpdateProgress();

        bool canGoToPlan = true;
        foreach(DayDrop d in DayList)
        {
            if (d.CurrentNote == false) canGoToPlan = false;
        }

        if (canGoToPlan)
        {
            if(!nextEventButton.IsActive)  nextEventButton.Activate();
        }
        else
        {
            if(nextEventButton.IsActive) nextEventButton.Deactivate();
        }
            
    }

    public void MoveToEventState()
    {
        CurrentGameState = GameManager.GameState.Event;
        int maxUnusedCount = 0;
        foreach(DayDrop d in DayList)
        {
            if(d.UsedNotes.Count > maxUnusedCount) maxUnusedCount = d.UsedNotes.Count;
        }
        List<DayDrop> maxUnusedDays = DayList.Where(o => o.UsedNotes.Count == maxUnusedCount).ToList();
        foreach(DayDrop d in maxUnusedDays)
        {
            d.NextButton.GetComponent<DiscardStep>().Activate();
        }
    }

    public void MoveToNextWeek()
    {
        confirmedUsedDedicationLastRound = tentativeUsedDedication;
        foreach(GameEvent ge in GameEvents)
        {
            foreach(Reward r in ge.Rewards)
            {
                if (r.IsCrisis) r.UpdateMeasure();
            }
        }
        foreach (DayDrop d in DayList)
        {
            d.NextButton.GetComponent<DiscardStep>().Deactivate();
        }
        CharismaStore.ResetStartingScore();
        DedicationStore.ResetStartingScore();
        EmpathyStore.ResetStartingScore();
        //IntelligenceStore.ResetStartingScore(); //Not needed due to bonus
        WeekCount++;
        weekText.text = "Week " + WeekCount.ToString();
        LeanTween.scale(weekText.gameObject, Vector3.one * 1.3f, 0.2f).setEaseInOutCubic().setLoopOnce();
        deck.DrawCards();
        CurrentGameState = GameState.Plan;
    }

    public void AddFunds(int value)
    {
        FundsRaised += value;
        fundsRaisedText.text = "Funds Raised: \n $" + FundsRaised + " million";
        LeanTween.scale(fundsRaisedText.gameObject, Vector3.one * 1.4f, 0.2f).setEaseInOutCubic().setLoopPingPong(1);
    }

    public void GameOver()
    {
        gameoverCanvas.SetActive(true);
        gameoverText.text = "<size=60>Game Over!</size> \n Your Unicorn Startup is worthless! Look on the bright side - you  were able to raise $" + FundsRaised + " million. Only a bit shy of that $1 billion. \n Press backspace to restart the game";
        print("gameover");
    }
}
