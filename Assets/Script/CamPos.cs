using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Tobii.Gaming;

public class CamPos : MonoBehaviour
{
    
    private List<Vector2> _points;
    [SerializeField] private int MAXPOINT = 20;
    [SerializeField] private int MAXNEED = 10;
    [SerializeField] private Text _camPos;
    [SerializeField] private Text _wordFind;
    [SerializeField] private RectTransform _hoverWord;
    
    private Camera _mainCam;
    private Vector3 position;

    private GameManager _manager;
    // Start is called before the first frame update
    void Start()
    {
        _manager = Object.FindObjectOfType<GameManager>();
        _mainCam = Camera.main;
        _points = new List<Vector2>();
        
        if (_manager.Debug)
        {
            _camPos.gameObject.SetActive(true);
            _wordFind.gameObject.SetActive(true);
            _hoverWord.gameObject.SetActive(true);
            GetComponent<TrailRenderer>().enabled = true;

        }
        else
        {
            _camPos.gameObject.SetActive(false);
            _wordFind.gameObject.SetActive(false);
            _hoverWord.gameObject.SetActive(false);
            GetComponent<TrailRenderer>().enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        position = Input.mousePosition;
        if(_manager.Gaze){
            
            GazePoint gazePoint = TobiiAPI.GetGazePoint();
            if(gazePoint.IsRecent())
                position = gazePoint.Screen;
        }
        position = _mainCam.ScreenToWorldPoint(position);
        Vector2 v2 = new Vector2(position.x, position.y);
        
        // Show Trail
        Vector2 toScreen = _mainCam.WorldToScreenPoint(position);
        _camPos.text = toScreen.ToString();
        transform.position = v2;
        
        // Stock position
        _points.Add(toScreen);
        if(_points.Count > MAXPOINT) _points.RemoveAt(0);
        
        Dictionary<Word,float> find = new Dictionary<Word, float>();
        
        for (int i = 0; i < _points.Count; ++i)
        { 
            Vector2 pos = _points[i];
            /*
            Dictionary<string, float> findForPoint = gestor.DistWordInPos(pos);
            foreach (string word in findForPoint.Keys)
            {
                if (!find.ContainsKey(word)) find[word] = 0;
                find[word] += findForPoint[word];
            }
            */
            Word wordInPos = _manager.Gestor.WordInPos(pos);
            if (!find.ContainsKey(wordInPos)) find[wordInPos] = 0;
            ++find[wordInPos];
            
        }

        Word maxWord = new Word();
        float maxIndex = 0;
        foreach (Word word in find.Keys)
        {
            if (find[word] > maxIndex && find[word] > MAXNEED)
            {
                maxWord = word;
                maxIndex = find[word];
            }
        }

        _hoverWord.localPosition = Vector3.zero;
        _hoverWord.position = new Vector3(maxWord.pos.x + maxWord.pos.width/2,maxWord.pos.y + maxWord.pos.height/2,0);
        _hoverWord.sizeDelta = new Vector2(maxWord.pos.width, maxWord.pos.height);
        _wordFind.text = maxWord.text + " : " + maxIndex;

    }
}
