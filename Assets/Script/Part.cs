using UnityEngine;

namespace BlockBlast
{
    public class Part
    {
        public GameObject View { get; private set; }
        private SpriteRenderer _border,
            _bg;
        public Vector2Int Index { get; private set; }
        public Vector2Int FitIndex { get; private set; }

        public Part(GameObject view)
        {
            this.View = view;
            _border = view.GetComponent<SpriteRenderer>();
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

        public void Clear()
        {
            Object.Destroy(View);
        }

        public void ChangeColor(Color color) => _bg.color = color;

        public Color GetColor() => _bg.color;

        public void Idle()
        {
            _border.sortingLayerName = "Part";
            _bg.sortingLayerName = "Part";
        }

        public void Busy()
        {
            _border.sortingLayerName = "BusyPart";
            _bg.sortingLayerName = "BusyPart";
        }
    }
}
