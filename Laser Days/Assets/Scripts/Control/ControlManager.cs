using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlManager : MonoBehaviour
{

    public static ControlManager CM;

    public KeyCode jump { get; set; }
    public KeyCode forward { get; set; }
    public KeyCode backward { get; set; }
    public KeyCode left { get; set; }
    public KeyCode right { get; set; }
    public KeyCode pickup { get; set; }
    public KeyCode flip { get; set; }
    public KeyCode select { get; set; }
    public KeyCode pause { get; set; }
    public KeyCode submit { get; set; }

    void Awake()
    {
        forward = (KeyCode) System.Enum.Parse(typeof(KeyCode),
            PlayerPrefs.GetString("forwardKey", "W"));
        
        backward = (KeyCode) System.Enum.Parse(typeof(KeyCode),
            PlayerPrefs.GetString("backwardKey", "S"));
        
        left = (KeyCode) System.Enum.Parse(typeof(KeyCode),
            PlayerPrefs.GetString("leftKey", "A"));
        
        right = (KeyCode) System.Enum.Parse(typeof(KeyCode),
            PlayerPrefs.GetString("rightKey", "D"));
        
        jump = (KeyCode) System.Enum.Parse(typeof(KeyCode),
            PlayerPrefs.GetString("jumpKey", "Space"));
        
        pickup = (KeyCode) System.Enum.Parse(typeof(KeyCode),
            PlayerPrefs.GetString("pickupKey", "E"));
        
        flip = (KeyCode) System.Enum.Parse(typeof(KeyCode),
            PlayerPrefs.GetString("flipKey", "Mouse0"));
        
        select = (KeyCode) System.Enum.Parse(typeof(KeyCode),
            PlayerPrefs.GetString("selectKey", "Mouse1"));
        
        pause = (KeyCode) System.Enum.Parse(typeof(KeyCode),
            PlayerPrefs.GetString("pauseKey", "Escape"));
        
        submit = (KeyCode) System.Enum.Parse(typeof(KeyCode),
            PlayerPrefs.GetString("submitKey", "Return"));
    }
}
