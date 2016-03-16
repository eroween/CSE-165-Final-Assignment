using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Wrap : MonoBehaviour {

    private TextMesh textMesh;
    private Dictionary<char, float> dict = new Dictionary<char, float>();

    // Use this for initialization
    void Start () {
        textMesh = GetComponent<TextMesh>();
        FitToWidth(250.0f);
	}
	
	// Update is called once per frame
	void Update () {
	
	}


    public void FitToWidth(float wantedWidth)
    {
        string oldText = textMesh.text;
        textMesh.text = "";

        string[] lines = oldText.Split('\n');

        foreach (string line in lines)
        {
            textMesh.text += wrapLine(line, wantedWidth);
            textMesh.text += "\n";
        }
    }
    private string wrapLine(string s, float w)
    {
        // need to check if smaller than maximum character length, really...
        if (w == 0 || s.Length <= 0) return s;

        char c;
        char[] charList = s.ToCharArray();

        float charWidth = 0;
        float wordWidth = 0;
        float currentWidth = 0;

        string word = "";
        string newText = "";
        string oldText = textMesh.text;

        for (int i = 0; i < charList.Length; i++)
        {
            c = charList[i];

            if (dict.ContainsKey(c))
            {
                charWidth = (float)dict[c];
            }
            else {
                textMesh.text = "" + c;
                charWidth = GetComponent<Renderer>().bounds.size.x;
                dict.Add(c, charWidth);
                //here check if max char length
            }

            if (c == ' ' || i == charList.Length - 1)
            {
                if (c != ' ')
                {
                    word += c.ToString();
                    wordWidth += charWidth;
                }

                if (currentWidth + wordWidth < w)
                {
                    currentWidth += wordWidth;
                    newText += word;
                }
                else {
                    currentWidth = wordWidth;
                    newText += word.Replace(" ", "\n");
                }

                word = "";
                wordWidth = 0;
            }

            word += c.ToString();
            wordWidth += charWidth;
        }

        textMesh.text = oldText;
        return newText;
    }
}
