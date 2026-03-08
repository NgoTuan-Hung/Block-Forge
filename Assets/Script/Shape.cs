using System.Collections.Generic;
using BlockBlast;
using UnityEngine;

namespace BlockBlast
{
    public class Shape : MonoBehaviour
    {
        public List<Part> Parts { get; private set; }
        public Part Origin { get; private set; }
        private Collider2D _collider2D;

        private void OnValidate()
        {
            Init();
        }

        void Init()
        {
            List<GameObject> children = new List<GameObject>();
            foreach (Transform child in transform)
            {
                children.Add(child.gameObject);
            }

            Parts = new List<Part>();
            foreach (GameObject child in children)
            {
                Parts.Add(new(child));
            }

            Origin = Parts[0];
        }

        private void Awake()
        {
            GetComponents();
            Init();
            ChangePartsColor();
        }

        private void ChangePartsColor()
        {
            var color = Random.ColorHSV(
                0f,
                1f, // Hue range
                0.8f,
                1f, // High saturation
                0.8f,
                1f // High brightness
            );
            Parts.ForEach(part => part.ChangeColor(color));
        }

        private void Start()
        {
            GameManager.IdentityController.AddIdentity(_collider2D.GetHashCode(), gameObject);
        }

        void GetComponents()
        {
            _collider2D = GetComponent<Collider2D>();
        }

        public void DestroyShape()
        {
            GameManager.IdentityController.RemoveIdentity(_collider2D.GetHashCode());
            Destroy(gameObject);
        }
    }
}
