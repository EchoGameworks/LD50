using EchoUtilities;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameEvent : MonoBehaviour
{
    //[SerializeField] string EventName;

    //[SerializeField] TextMeshProUGUI eventText;

    [SerializeField] Transform RequirementsHolder;
    [SerializeField] Transform RewardHolder;

    [SerializeField] public List<Requirement> Requirements;
    [SerializeField] public List<Reward> Rewards;

    public int weekIntro;
    public bool hasCrisis;

    public bool IsActive;

    private void Start()
    {
        SetupReqReward();
    }

    [Button]
    private void SetupReqReward()
    {
        Requirements = new List<Requirement>();
        Rewards = new List<Reward>();

        foreach(Transform t in RequirementsHolder)
        {
            if (t != null) Requirements.Add(t.GetComponent<Requirement>());
        }

        foreach(Transform t in RewardHolder)
        {
            if(t != null) Rewards.Add(t.GetComponent<Reward>());
        }        

        bool rewardHasCrisis = false;
        foreach(Reward r in Rewards)
        {
            if(r.IsCrisis) rewardHasCrisis = true;
        }
        hasCrisis = rewardHasCrisis;
    }

    public void ClearRequirementProgress()
    {
        foreach(Requirement r in Requirements)
        {
            r.ResetCurrentPoints();
        }
    }

    public void PointerEnter()
    {
        if(GameManager.instance.CurrentStore != null)
        {
            LeanTween.scale(gameObject, Vector3.one * 1.1f, 0.2f).setEaseInOutCirc();
            GameManager.instance.CurrentEvent = this;
        }
        
    }

    public void PointerExit()
    {
        LeanTween.scale(gameObject, Vector3.one, 0.2f).setEaseInOutCirc();
        GameManager.instance.CurrentEvent = null;
    }

    public void CheckComplete()
    {
        print("checking complete");
        bool isComplete = true;
        foreach(Requirement requirement in Requirements)
        {
            if (!requirement.IsComplete) isComplete = false;
        }

        if (isComplete)
        {
            foreach(Reward reward in Rewards)
            {
                if (!reward.IsCrisis) reward.UpdateMeasure();
            }
            GameManager.instance.RemoveEvent(this);
        }
    }

}
