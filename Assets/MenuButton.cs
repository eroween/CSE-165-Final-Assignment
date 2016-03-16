using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MenuButton : StylusButton
{
    public override void select()
    {
        SceneManager.LoadScene("menu");
    }
}
