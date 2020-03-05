using System.Collections;
using System.Collections.Generic;
using Tobii.Gaming;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum FocusChoice
    {
        Gaze, Mouse
    }
    [SerializeField] private FocusChoice _focus = FocusChoice.Gaze;
    [SerializeField] private TextGestor _gestor;
    [SerializeField] private bool _debug;
    
    public bool Gaze => _focus == FocusChoice.Gaze;
    public TextGestor Gestor => _gestor;
    public bool Debug => _debug;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
