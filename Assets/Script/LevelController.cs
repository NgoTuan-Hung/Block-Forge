using System;
using System.Collections.Generic;
using System.Linq;
using BlockBlast.Shadow;
using BlockBlast.Util;
using UnityEngine;

namespace BlockBlast
{
    public class LevelController : MonoBehaviour
    {
        private LevelData _levelData;
        private DataController dataController;
        private GameObject _cellPrefab;
        private GameObject _board;
        private Shape _draggingShape;
        private Camera _camera;
        private float _mouseZ;
        private Rect _bound;
        private Vector3 _pointerPos;
        private Cell[][] _cells;
        private Vector2 _origin;
        private HashSet<int> _clearRowsCheck,
            _clearColsCheck;
        public List<int> _clearRows,
            _clearCols;
        private List<Shape> _shapes,
            _genShapesThisTurn;
        private int _movesLeft,
            _movePerTurn;
        public event Action OnGameLose;
        private ShapeShadow _shadow;
        private bool _isFit;
        private List<GameObject> _lineHightlights;
        private GameObject _lineHightlightPrefab;
        private List<int> _rowCounts,
            _colCounts;
        private List<int> _rowAdds,
            _colAdds;
        private EffectController _effectController;

        public void Init(
            LevelData levelData,
            DataController data,
            GameObject cellPrefab,
            Camera camera
        )
        {
            _levelData = levelData;
            dataController = data;
            _shapes = levelData.BaseShapes;
            _cellPrefab = cellPrefab;
            _board = new GameObject("Board");
            _camera = camera;
            _mouseZ = -_camera.transform.position.z;
            _bound = new(
                -_levelData.BoardSizeX / 2,
                -_levelData.BoardSizeY / 2,
                _levelData.BoardSizeX,
                _levelData.BoardSizeY
            );
            _clearRowsCheck = new();
            _clearColsCheck = new();
            _clearRows = new();
            _clearCols = new();
            _movePerTurn = 3;
            _movesLeft = _movePerTurn;
            OnGameLose = () => { };
            _genShapesThisTurn = new();
            _shadow = dataController.Shadow;
            _lineHightlights = new();
            _lineHightlightPrefab = dataController.RowHightlight;
            _effectController = gameObject.AddComponent<EffectController>();
            GenerateBoard();
            ShowShapesForPlayer(_movePerTurn);
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start() { }

        void GenerateBoard()
        {
            _cells = new Cell[_levelData.BoardSizeX][];
            _origin = new Vector2(
                -_levelData.BoardSizeX / 2 + 0.5f,
                _levelData.BoardSizeY / 2 - 0.5f
            );
            _rowCounts = new();
            _colCounts = new();
            _rowAdds = new();
            _colAdds = new();

            for (int i = 0; i < _levelData.BoardSizeY; i++)
            {
                _cells[i] = new Cell[_levelData.BoardSizeX];
                for (int j = 0; j < _levelData.BoardSizeX; j++)
                {
                    Cell cell = new(
                        Instantiate(
                            _cellPrefab,
                            new Vector3(_origin.x + j, _origin.y - i),
                            Quaternion.identity
                        ),
                        _board.transform
                    );
                    _cells[i][j] = cell;

                    if (_colCounts.Count < _levelData.BoardSizeX)
                    {
                        _colCounts.Add(0);
                        _colAdds.Add(0);
                    }
                }

                _rowCounts.Add(0);
                _rowAdds.Add(0);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                var hit = Physics2D.Raycast(
                    _camera.ScreenToWorldPoint(Input.mousePosition.WithZ(_mouseZ)),
                    Vector3.forward
                );
                if (hit)
                {
                    _draggingShape = GameManager
                        .IdentityController.GetIdentity(hit.collider.GetHashCode())
                        .GetComponent<Shape>();
                    _draggingShape.Busy();

                    _shadow.ChangeShape(_draggingShape);
                }
            }

            if (Input.GetMouseButton(0))
            {
                _isFit = false;
                if (_draggingShape)
                {
                    _pointerPos = _camera.ScreenToWorldPoint(Input.mousePosition.WithZ(_mouseZ));
                    _draggingShape.transform.position =
                        _pointerPos
                        + (
                            _draggingShape.transform.position
                            - _draggingShape.Origin.View.transform.position
                        );

                    _isFit = CheckFit(_draggingShape, _pointerPos);
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (_draggingShape)
                {
                    if (_isFit)
                        FitToBoard(_draggingShape);
                    else
                        _draggingShape.Idle();

                    _shadow.Hide();
                    HideLineHightlights();
                    _draggingShape = null;
                }
            }
        }

        /// <summary>
        /// Check if the shape fit on the board
        /// and place the shadow
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        bool CheckFit(Shape shape, Vector2 pos)
        {
            if (_bound.Contains(_pointerPos))
            {
                var index = GetCellIndex(pos);
                var cell = _cells[index.x][index.y];

                if (IsShapeFit(shape, index))
                {
                    _shadow.Place(cell);
                    CheckClearLines(shape);
                    return true;
                }
            }

            _shadow.Hide();
            HideLineHightlights();
            return false;
        }

        Vector2Int GetCellIndex(Vector2 pos)
        {
            pos = new((float)Math.Floor(pos.x) + 0.5f, (float)Math.Floor(pos.y) + 0.5f);
            return new(Mathf.RoundToInt(_origin.y - pos.y), Mathf.RoundToInt(pos.x - _origin.x));
        }

        void FitToBoard(Shape shape)
        {
            shape.Parts.ForEach(part =>
            {
                _cells[part.FitIndex.x][part.FitIndex.y].SetPart(part);
                _rowCounts[part.FitIndex.x] += 1;
                _colCounts[part.FitIndex.y] += 1;
            });

            _genShapesThisTurn.Remove(shape);
            shape.CleanParts();
            shape.DestroyShape();
            ClearLinesIfAny();

            _movesLeft--;
            if (_movesLeft <= 0)
            {
                ShowShapesForPlayer(_movePerTurn);
                _movesLeft = _movePerTurn;
            }
            else
                CheckLoseCondition();
        }

        bool IsShapeFit(Shape shape, Vector2Int cellIndex)
        {
            bool fit = true;
            shape.Parts.ForEach(part =>
            {
                var fitIndex = cellIndex + (part.Index - shape.Origin.Index);
                if (!IsIndexValid(fitIndex) || !_cells[fitIndex.x][fitIndex.y].IsBlank())
                {
                    fit = false;
                    return;
                }
                else
                    part.PrepareForFit(fitIndex);
            });

            return fit;
        }

        bool IsShapeFitAny(Shape shape)
        {
            for (int i = 0; i < _levelData.BoardSizeY; i++)
            {
                for (int j = 0; j < _levelData.BoardSizeX; j++)
                {
                    if (IsShapeFit(shape, new Vector2Int(i, j)))
                        return true;
                }
            }

            return false;
        }

        void CheckPotentialLines(Shape shape)
        {
            _clearRowsCheck.Clear();
            _clearColsCheck.Clear();
            shape.Parts.ForEach(part =>
            {
                _clearRowsCheck.Add(part.FitIndex.x);
                _clearColsCheck.Add(part.FitIndex.y);
                _rowAdds[part.FitIndex.x] += 1;
                _colAdds[part.FitIndex.y] += 1;
            });
        }

        void CheckClearLines(Shape shape)
        {
            CheckPotentialLines(shape);
            _clearRows.Clear();
            _clearCols.Clear();
            foreach (var row in _clearRowsCheck)
            {
                if (_rowCounts[row] + _rowAdds[row] == _levelData.BoardSizeX)
                    _clearRows.Add(row);

                _rowAdds[row] = 0;
            }

            foreach (var col in _clearColsCheck)
            {
                if (_colCounts[col] + _colAdds[col] == _levelData.BoardSizeY)
                    _clearCols.Add(col);

                _colAdds[col] = 0;
            }

            PlaceLineHightlights();
        }

        void PlaceLineHightlights()
        {
            var total = _clearRows.Count + _clearCols.Count;

            if (_lineHightlights.Count > total)
            {
                for (int i = total; i < _lineHightlights.Count; i++)
                {
                    _lineHightlights[i].gameObject.SetActive(false);
                }
            }

            int index = 0;

            foreach (var row in _clearRows)
            {
                if (index >= _lineHightlights.Count)
                {
                    var lineHightlight = Instantiate(_lineHightlightPrefab);
                    _lineHightlights.Add(lineHightlight);
                }

                _lineHightlights[index].SetActive(true);
                _lineHightlights[index].transform.position = new Vector3(0, _origin.y - row, 0);
                _lineHightlights[index].transform.rotation = Quaternion.Euler(0, 0, 0);
                index++;
            }

            foreach (var col in _clearCols)
            {
                if (index >= _lineHightlights.Count)
                {
                    var lineHightlight = Instantiate(_lineHightlightPrefab);
                    _lineHightlights.Add(lineHightlight);
                }

                _lineHightlights[index].SetActive(true);
                _lineHightlights[index].transform.position = new Vector3(_origin.x + col, 0, 0);
                _lineHightlights[index].transform.rotation = Quaternion.Euler(0, 0, 90);
                index++;
            }
        }

        void HideLineHightlights() =>
            _lineHightlights.ForEach(lineHightlight => lineHightlight.SetActive(false));

        void ClearLinesIfAny()
        {
            _clearRows.ForEach(row => ClearRow(row));
            _clearCols.ForEach(col => ClearCol(col));
        }

        void ClearRow(int row)
        {
            for (int i = 0; i < _levelData.BoardSizeX; i++)
            {
                _cells[row][i].Clear();
                _colCounts[i]--;
            }
            _rowCounts[row] = 0;
        }

        void ClearCol(int col)
        {
            for (int i = 0; i < _levelData.BoardSizeY; i++)
            {
                _cells[i][col].Clear();
                _rowCounts[i]--;
            }
            _colCounts[col] = 0;
        }

        bool IsIndexValid(Vector2Int index)
        {
            return index.x >= 0
                && index.x < _levelData.BoardSizeX
                && index.y >= 0
                && index.y < _levelData.BoardSizeY;
        }

        List<Shape> GenerateShapesForPlayer(int count)
        {
            _shapes = _shapes.Shuffle().ToList();

            var shapePrefabs = new List<Shape>();

            int gen = 0;
            for (int i = 0; i < _shapes.Count; i++)
            {
                if (gen < count)
                {
                    var shape = _shapes[i].GetComponent<Shape>();
                    if (IsShapeFitAny(shape))
                    {
                        shapePrefabs.Add(shape);
                        gen++;
                    }
                }
                else
                    break;
            }

            int index = 0;
            while (gen < count)
            {
                if (!shapePrefabs.Contains(_shapes[index]))
                {
                    shapePrefabs.Add(_shapes[index]);
                    gen++;
                }
                index++;
            }

            return shapePrefabs;
        }

        void ShowShapesForPlayer(int count)
        {
            var shapePrefabs = GenerateShapesForPlayer(count);
            _genShapesThisTurn.Clear();

            Vector3 origin = new(-3f, -5.5f);
            for (int i = 0; i < shapePrefabs.Count; i++)
            {
                _genShapesThisTurn.Add(
                    Instantiate(shapePrefabs[i].gameObject)
                        .GetComponent<Shape>()
                        .Setup(origin + new Vector3(3f * i, 0))
                );
            }
        }

        void CheckLoseCondition()
        {
            bool lose = true;
            foreach (var shape in _genShapesThisTurn)
            {
                if (IsShapeFitAny(shape))
                {
                    lose = false;
                    break;
                }
            }

            if (lose)
                OnGameLose();
        }

        public void SelfDestroy()
        {
            _genShapesThisTurn.ForEach(shape => shape.DestroyShape());
            Destroy(_effectController);
            Destroy(_board);
            Destroy(this);
        }
    }
}
