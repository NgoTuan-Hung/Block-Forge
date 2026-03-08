using UnityEngine;

namespace BlockBlast
{
    public class Cell
    {
        public GameObject View { get; private set; }
        public Part Part { get; private set; }

        public Cell(GameObject view, Transform parent)
        {
            this.View = view;
            SetParent(parent);
        }

        public void SetParent(Transform parent)
        {
            View.transform.parent = parent;
        }

        public bool IsBlank() => Part == null;

        public void SetPart(Part part)
        {
            Part = part;
            part.SetCell(this);
        }

        public void Explode()
        {
            if (!IsBlank())
            {
                Part.Explode();
                Part = null;
            }
        }
    }
}
