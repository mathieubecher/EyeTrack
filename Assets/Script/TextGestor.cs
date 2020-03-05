using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct Word
{
    private static int ID = 0;
    public int id;
    public string text;
    public Rect pos;
    
    public Word(string text, Vector2 topleft, Vector2 rightbottom)
    {
        id = ID;
        ++ID;
        this.text = text;
        this.pos = new Rect(topleft.x,topleft.y, Mathf.Abs(rightbottom.x - topleft.x), Mathf.Abs(rightbottom.y - topleft.y));
    }
}
public class TextGestor : MonoBehaviour
{

    [SerializeField] private Text text;
    private List<Word> wordsPos;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<Text>();
        wordsPos = new List<Word>();
        //ConstructWords();
    }


    // Update is called once per frame
    void Update()
    {
        if(wordsPos.Count == 0 && text.text != "" && text.cachedTextGenerator.characterCount > 1) ConstructWords();
        Vector3 v3 = Input.mousePosition;
        
        Vector2 v2 = new Vector2(v3.x, v3.y);
        
        
    }

    public Word WordInPos(Vector2 ScreenPos)
    {
        Word wordFound = new Word("",new Vector2(0,0), new Vector2(0,0));
        foreach (Word word in wordsPos)
        {
            if(word.pos.Contains(ScreenPos)) wordFound = word;
        }

        return wordFound;
    }

    public Dictionary<string, float> DistWordInPos(Vector2 ScreenPos)
    {
        Dictionary<string, float> dict = new Dictionary<string, float>();

        foreach (Word word in wordsPos)
        {
            dict[word.text] = DistancePointToRectangle(ScreenPos, word.pos);
        }

        return dict;
    }

    void ConstructWords() {
        Debug.Log("Construct");
        wordsPos = new List<Word>();

        var textGen = text.cachedTextGenerator;

        var chars = textGen.characters;
        var lines = textGen.lines;
        
        // Create var for word
        bool beginWord = true;
        Vector2 locUpperLeft = new Vector2();
        Vector2 locBottomRight = new Vector2();
        string word = "";
        for (int i = 0; i < textGen.characterCount && i < text.text.Length; ++i)
        {
            var line = lines.First(lineInfo => i >= lineInfo.startCharIdx);
            if (i == 0)
            {
                locUpperLeft = chars[i].cursorPos - new Vector2(0, line.height);
            }
            if (!IsChar(text.text[i]) && beginWord)
            {
                beginWord = false;
                AddWord(word, locUpperLeft, locBottomRight);

            }
            else if (IsChar(text.text[i]) && !beginWord)
            {
                word = "";
                beginWord = true;
                locUpperLeft = chars[i].cursorPos - new Vector2(0, line.height);
                locBottomRight = new Vector2(locUpperLeft.x + chars[i].charWidth, locUpperLeft.y + line.height);
            }
            else if(beginWord)
            {
                Vector2 locUpperLeftActual = chars[i].cursorPos - new Vector2(0, line.height);
                locBottomRight = new Vector2(locUpperLeftActual.x + chars[i].charWidth, locUpperLeftActual.y + line.height);   
            }

            word += text.text[i];
        }
        if(beginWord)
        {
            AddWord(word, locUpperLeft, locBottomRight);
        }
    }


    bool IsChar(char c)
    {
        return !(c == ' ' || c == ','  || c == '.'  || c == '?'  || c == '!'  || c == '(' || c == ')' || c == '{' || c == '}' || c == '"' || c == ';' );
    }

    void AddWord(string word, Vector2 upperLeft, Vector2 bottomRight)
    {
        Vector3 worldUpperLeft = transform.TransformPoint(upperLeft);
        Vector3 worldBottomRight = transform.TransformPoint(bottomRight);
        wordsPos.Add(new Word(word, worldUpperLeft, worldBottomRight));
    }
    
    public static float DistancePointToRectangle(Vector2 point, Rect rect) {
        //  Calculate a distance between a point and a rectangle.
        //  The area around/in the rectangle is defined in terms of
        //  several regions:
        //
        //  O--x
        //  |
        //  y
        //
        //
        //        I   |    II    |  III
        //      ======+==========+======   --yMin
        //       VIII |  IX (in) |  IV
        //      ======+==========+======   --yMax
        //       VII  |    VI    |   V
        //
        //
        //  Note that the +y direction is down because of Unity's GUI coordinates.
 
        if (point.x < rect.xMin) { // Region I, VIII, or VII
            if (point.y < rect.yMin) { // I
                Vector2 diff = point - new Vector2(rect.xMin, rect.yMin);
                return diff.magnitude;
            }
            else if (point.y > rect.yMax) { // VII
                Vector2 diff = point - new Vector2(rect.xMin, rect.yMax);
                return diff.magnitude;
            }
            else { // VIII
                return rect.xMin - point.x;
            }
        }
        else if (point.x > rect.xMax) { // Region III, IV, or V
            if (point.y < rect.yMin) { // III
                Vector2 diff = point - new Vector2(rect.xMax, rect.yMin);
                return diff.magnitude;
            }
            else if (point.y > rect.yMax) { // V
                Vector2 diff = point - new Vector2(rect.xMax, rect.yMax);
                return diff.magnitude;
            }
            else { // IV
                return point.x - rect.xMax;
            }
        }
        else { // Region II, IX, or VI
            if (point.y < rect.yMin) { // II
                return rect.yMin - point.y;
            }
            else if (point.y > rect.yMax) { // VI
                return point.y - rect.yMax;
            }
            else { // IX
                return 0f;
            }
        }
    }
}
