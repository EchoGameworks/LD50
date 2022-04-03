using EchoUtilities;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class Reward : MonoBehaviour
{
    public enum RewardType { Profit, Morale, Fame, Technology, Board }

    [SerializeField] private RewardType CompleteReward;
    [SerializeField] private int Value;
    [SerializeField] TextMeshProUGUI progressText;

    public bool IsCrisis;
    [SerializeField] GameObject crisisIcon;

    Measure measure;

    private void Start()
    {
        measure = GameObject.FindGameObjectWithTag(CompleteReward.ToString()).GetComponent<Measure>();
        UpdateProgress();
    }

    [Button]
    public void UpdateProgress()
    {
        crisisIcon.SetActive(IsCrisis);
        if(IsCrisis) Value = -Mathf.Abs(Value);
        if (Value > 0)
        {
            progressText.text = "+" + Value.ToString();
        }
        else
        {
            progressText.text = Value.ToString();
        }
    }

    public void UpdateMeasure()
    {
        measure.AddValue(Value);
        if (CompleteReward == RewardType.Profit && !IsCrisis) GameManager.instance.AddFunds(Value);
    }
}
