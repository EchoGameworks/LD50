using EchoUtilities;
using UnityEngine;
using UnityEngine.UI;
using static Reward;

public class Measure : MonoBehaviour
{
    [SerializeField] RewardType Reward;

    [SerializeField] int maxValue = 100;
    [SerializeField] int currentValue = 20;

    [SerializeField] Image circleImage;
    void Start()
    {

        
    }

    public void StartNewGame()
    {

        currentValue = Random.Range(40, 60);
#if !UNITY_EDITOR
        currentValue = Random.Range(40, 60);
#endif
        circleImage.fillAmount = (float)currentValue / maxValue;
    }

    void Update()
    {
        
    }

    public void CalculateRatio()
    {
        if(currentValue / maxValue != circleImage.fillAmount)
        {
            circleImage.fillAmount = (float)currentValue / maxValue;
            LeanTween.scale(circleImage.gameObject, Vector3.one * 1.2f, 0.2f).setEaseInOutCirc().setLoopPingPong(1);
        }
        
    }

    public void AddValue(int val)
    {
        currentValue += val;
        if (currentValue < 0) GameManager.instance.GameOver();
        if(currentValue > maxValue) currentValue = maxValue;

        CalculateRatio();
    }
}
