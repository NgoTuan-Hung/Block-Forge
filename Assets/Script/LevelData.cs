using System.Collections.Generic;
using UnityEngine;

namespace BlockBlast
{
    [CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObject/LevelData")]
    public class LevelData : ScriptableObject
    {
        public int BoardSizeX;
        public int BoardSizeY;
        public int CellSize = 1;
        public List<Shape> BaseShapes;
    }
}
