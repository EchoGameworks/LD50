using EchoUtilities;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using static Note;

public class Requirement : MonoBehaviour
{
    [SerializeField] public Capabilities Capability;
    [SerializeField] private int TotalCost;
    [SerializeField] private int CurrentPoints;

    [SerializeField] TextMeshProUGUI progressText;

    public bool IsComplete;
    private void Start()
    {
        UpdateProgress();
    }

    public void ResetCurrentPoints()
    {
        CurrentPoints = 0;
    }

    public void AddPoints(int points)
    {
        CurrentPoints += points;
        UpdateProgress();
    }

    public void RemovePoints(int points)
    {
        CurrentPoints -= points;
        UpdateProgress();
    }

    [Button]
    public void UpdateProgress()
    {
        string progressRatioText = CurrentPoints.ToString() + "/" + TotalCost.ToString();
        if (CurrentPoints >= TotalCost)
        {
            progressRatioText = "<b>" + progressRatioText + "</b>";
            IsComplete = true;
        }
        progressText.text = progressRatioText;
        LeanTween.scale(progressText.gameObject, Vector3.one * 1.1f, 0.3f).setEaseInOutCubic().setLoopPingPong(1);
    }
}
