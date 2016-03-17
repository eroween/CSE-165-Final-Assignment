using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EarthController : PlanetController
{

    public TextMesh information;
    public TextMesh title;
    public GameObject picture;

    public Texture2D image;
    public TextAsset text_asset;

    private TextMesh textMesh;
    private Dictionary<char, float> dict = new Dictionary<char, float>();

    private Sprite sprite;
    private string text = "";

    void Start()
    {
        textMesh = information;
        Rect rec = new Rect(0, 0, image.width, image.height);
        sprite = Sprite.Create(image, rec, new Vector2(0.5f, 0.5f), 400.0f);
        text = FitToWidth(250.0f, text_asset.text);
    }

    public override void select()
    {
        information.text = text;
        picture.GetComponent<SpriteRenderer>().sprite = sprite;
        title.text = "Earth";
        ZoomController.selected = this;
    }

    public string FitToWidth(float wantedWidth, string oldText)
    {
        string result = "";
        string[] lines = oldText.Split('\n');

        foreach (string line in lines)
        {
            result += wrapLine(line, wantedWidth);
            result += "\n";
        }
        return result;
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
                charWidth = textMesh.GetComponent<Renderer>().bounds.size.x * 12.0f;
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
