using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineUpgrade : MonoBehaviour
{
    public Mine mine;

    private void OnMouseDown()
    {
        mine.Upgrade();
    }
}
