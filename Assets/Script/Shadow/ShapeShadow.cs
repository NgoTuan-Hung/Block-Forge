using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BlockBlast.Shadow
{
    public class ShapeShadow : MonoBehaviour
    {
        private List<ShadowPart> _parts = new();
        private Shape _shape;

        private void Awake()
        {
            _parts = GetComponentsInChildren<ShadowPart>().ToList();
        }

        public void ChangeShape(Shape shape)
        {
            _shape = shape;
            if (_parts.Count > shape.Parts.Count)
            {
                for (int i = shape.Parts.Count; i < _parts.Count; i++)
                {
                    _parts[i].gameObject.SetActive(false);
                }
            }

            for (int i = 0; i < shape.Parts.Count; i++)
            {
                if (i >= _parts.Count)
                {
                    var part = Instantiate(GameManager.Data.ShadowPart, transform)
                        .GetComponent<ShadowPart>();
                    _parts.Add(part);
                }

                _parts[i].Copy(shape.Parts[i]);
            }
        }

        public void Place(Cell cell)
        {
            gameObject.SetActive(true);
            transform.position =
                cell.View.transform.position
                + _shape.transform.position
                - _shape.Origin.View.transform.position;
        }

        public void Hide() => gameObject.SetActive(false);
    }
}
