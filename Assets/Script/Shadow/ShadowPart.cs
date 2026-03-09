using UnityEngine;

namespace BlockBlast.Shadow
{
    public class ShadowPart : MonoBehaviour
    {
        private SpriteRenderer _renderer;

        private void Awake()
        {
            _renderer = transform.Find("BG").GetComponent<SpriteRenderer>();
        }

        public void Copy(Part part)
        {
            transform.localPosition = part.View.transform.localPosition;
            var color = part.GetColor();
            _renderer.color = new(color.r, color.g, color.b, 0.5f);
            if (!gameObject.activeSelf)
                gameObject.SetActive(true);
        }
    }
}
