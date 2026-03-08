using UnityEngine;

namespace BlockBlast
{
    public class Part
    {
        public GameObject View { get; private set; }
        private SpriteRenderer _bg;
        public Vector2Int Index { get; private set; }
        public Vector2Int FitIndex { get; private set; }

        public Part(GameObject view)
        {
            this.View = view;
            _bg = view.transform.Find("BG").GetComponent<SpriteRenderer>();
            var localPos = view.transform.localPosition;
            Index = new(-Mathf.RoundToInt(localPos.y), Mathf.RoundToInt(localPos.x));
        }

        public void PrepareForFit(Vector2Int fitIndex)
        {
            FitIndex = fitIndex;
        }

        public void SetCell(Cell cell)
        {
            View.transform.parent = cell.View.transform;
            View.transform.localPosition = Vector3.zero;
        }

        public void Explode()
        {
            Object.Destroy(View);
        }

        public void ChangeColor(Color color) => _bg.color = color;
    }
}
