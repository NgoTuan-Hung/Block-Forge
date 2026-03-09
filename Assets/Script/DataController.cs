using System;
using BlockBlast.Shadow;
using UnityEngine;

namespace BlockBlast
{
    [Serializable]
    public class DataController
    {
        public LevelData LevelData;
        public GameObject CellPrefab;
        public ShapeShadow Shadow;
        public GameObject ShadowPart;
        public GameObject RowHightlight;
    }
}
