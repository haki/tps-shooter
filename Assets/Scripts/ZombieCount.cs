using UnityEngine;
using UnityEngine.UI;

public class ZombieCount : MonoBehaviour
{
    public static ZombieCount Instance { get; set; }

    public float remainingZombies;

    [SerializeField] private GameObject enemies;

    private float _totalZombies;
    private Text _text;

    private void Start()
    {
        _totalZombies = enemies.transform.childCount;
        remainingZombies = _totalZombies;
        _text = GetComponent<Text>();

        _text.text = remainingZombies + " / " + _totalZombies;
    }

    public void CountDown()
    {
        remainingZombies--;
        _text.text = remainingZombies + " / " + _totalZombies;

        if (remainingZombies == 0)
        {
            var totalWin = PlayerPrefs.GetInt("Win");
            totalWin++;
            PlayerPrefs.SetInt("Win", totalWin++);

            GameManager.Instance.ShowFinalPanel(true);
        }
    }
}