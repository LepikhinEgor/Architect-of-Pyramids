using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformInformation {
    public int ID = 0;
    public int Score = 0;
    public int BlockMaterialNum = 0;
    public bool[] GoodEdgePositions = { false, false, false, false, false, false, };

    public PlatformInformation()
    {
        int ID = 0;
        int Score = 0;
        int BlockMaterialNum = 0;
        bool[] GoodEdgePositions = { false, false, false, false, false, false, };
    }

}
