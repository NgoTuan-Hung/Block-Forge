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
        private Vector3 _idlePos;
        public static Vector3 IdleScale = new Vector3(0.5f, 0.5f, 1f);

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

        public Shape Setup(Vector3 idlePos)
        {
            _idlePos = idlePos;
            Idle();
            return this;
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

        public void Idle()
        {
            transform.position = _idlePos;
            transform.localScale = IdleScale;
            Parts.ForEach(part => part.Idle());
        }

        public void Busy()
        {
            transform.localScale = Vector3.one;
            Parts.ForEach(part => part.Busy());
        }

        public void CleanParts()
        {
            Parts.ForEach(part => part.Idle());
        }
    }
}
