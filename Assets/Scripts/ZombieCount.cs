using UnityEngine;
using UnityEngine.UI;

public class ZombieCount : MonoBehaviour
{
    // Instance
    public static ZombieCount Instance { get; private set; }
    
    // References
    [SerializeField] private GameObject enemies;
    
    // Private variables
    private float _remainingZombies;
    private float _totalZombies;
    private Text _text;

    private void Start()
    {
        Instance = this;
        _totalZombies = enemies.transform.childCount;
        _remainingZombies = _totalZombies;
        _text = GetComponent<Text>();

        _text.text = _remainingZombies + " / " + _totalZombies;
    }

    public void CountDown()
    {
        _remainingZombies--;
        _text.text = _remainingZombies + " / " + _totalZombies;

        if (_remainingZombies == 0)
        {
            var totalWin = PlayerPrefs.GetInt("Win");
            totalWin++;
            PlayerPrefs.SetInt("Win", totalWin++);

            GameManager.Instance.ShowFinalPanel(true);
        }
    }
}