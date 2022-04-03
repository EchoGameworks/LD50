using EchoUtilities;
using UnityEngine;

public class ButtonNextStep : MonoBehaviour
{
    [SerializeField] private CapabilityStore CharismaStore;
    [SerializeField] private CapabilityStore DedicationStore;
    [SerializeField] private CapabilityStore EmpathyStore;
    [SerializeField] private CapabilityStore IntelligenceStore;

    [SerializeField] private DayDrop Monday;
    [SerializeField] private DayDrop Tuesday;
    [SerializeField] private DayDrop Wednesday;
    [SerializeField] private DayDrop Thursday;
    [SerializeField] private DayDrop Friday;

    [SerializeField] private GameObject bgActive;
    [SerializeField] private GameObject bgInactive;

    public bool IsActive;
    public void ButtonClicked()
    {
        if (!IsActive) return;
        CharismaStore.CommitCurrentValue();
        DedicationStore.CommitCurrentValue();
        EmpathyStore.CommitCurrentValue();
        IntelligenceStore.CommitCurrentValue();

        Monday.MoveActiveToUsed();
        Tuesday.MoveActiveToUsed();
        Wednesday.MoveActiveToUsed();
        Thursday.MoveActiveToUsed();
        Friday.MoveActiveToUsed();

        GameManager.instance.MoveToEventState();
        DeactivateSilent();
    }

    public void PointerEnter()
    {
        if (!IsActive) return;
        LeanTween.scale(gameObject, Vector3.one * 1.3f, 0.3f).setEaseInOutCirc();
    }

    public void PointerExit()
    {
        LeanTween.scale(gameObject, Vector3.one, 0.3f).setEaseInOutCirc();
    }

    public void Activate()
    {
        IsActive = true;
        bgActive.SetActive(true);
        LeanTween.scale(bgActive, Vector3.one * 1.3f, 0.2f).setEaseInOutCubic().setLoopPingPong(1);
        bgInactive.SetActive(false);
    }

    public void DeactivateSilent()
    {
        IsActive = false;
        bgActive.SetActive(false);
        bgInactive.SetActive(true);
    }

    public void Deactivate()
    {
        IsActive = false;
        bgActive.SetActive(false);
        bgInactive.SetActive(true);
        LeanTween.scale(bgInactive, Vector3.one * 1.3f, 0.2f).setEaseInOutCubic().setLoopPingPong(1);
    }
}
