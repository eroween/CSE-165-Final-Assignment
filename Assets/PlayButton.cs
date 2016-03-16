using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayButton : StylusButton
{
    public override void select()
    {
        SceneManager.LoadScene("copy_menu");
    }
}
