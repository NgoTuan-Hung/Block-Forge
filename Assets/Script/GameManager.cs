using UnityEngine;

namespace BlockBlast
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        private Camera _camera;

        [SerializeField]
        private DataController _data;
        public static DataController Data => Instance._data;
        private LevelController _levelController;

        [SerializeField]
        private UIController _uIController;
        private IdentityController _identityController;
        public static IdentityController IdentityController => Instance._identityController;

        private void Awake()
        {
            Instance = this;
            _camera = Camera.main;
            InitControllers();
        }

        void RestartGame()
        {
            _levelController.SelfDestroy();
            InitControllers();
        }

        void InitControllers()
        {
            _levelController = gameObject.AddComponent<LevelController>();
            _levelController.Init(_data.LevelData, _data, _data.CellPrefab, _camera);
            _uIController.Init(_levelController);
            _uIController.LoseButton.onClick.AddListener(RestartGame);
            _identityController = new();
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start() { }

        // Update is called once per frame
        void Update() { }
    }
}
