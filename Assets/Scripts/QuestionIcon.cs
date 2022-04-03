using EchoUtilities;
using UnityEngine;

public class QuestionIcon : MonoBehaviour
{
    private bool isShowingTooltips;
    [SerializeField] GameObject TooltipHolder;
    public void ToggleTooltips()
    {
        isShowingTooltips = !isShowingTooltips;
        TooltipHolder.SetActive(isShowingTooltips);
    }
}
