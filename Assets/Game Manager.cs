using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject boxPrefab;
    public GameObject ClearTarget;
    public GameObject ClearText;
    public GameObject WallPrefab;
    public TMP_Text moveCounterText;
    int[,] map;
    GameObject[,] field;
    GameObject instance;
    int moveCount;
    bool isGameCleared = false;

    void ResetGame()
    {
        moveCount = 0; // resetCount
        UpdateMoveCounterText(); // 更新
        isGameCleared = false; // resetClear
                               // Clear the field


        for (int y = 0; y < field.GetLength(0); y++)
        {
            for (int x = 0; x < field.GetLength(1); x++)
            {
                if (field[y, x] != null)
                {
                    Destroy(field[y, x]);
                    field[y, x] = null;
                }
            }
        }

        // Reset the map and field
        Start();
        ClearText.SetActive(false);
    }


    bool MoveNumber(Vector2Int movefrom, Vector2Int moveto, bool isPlayerMove = false)
    {
        if (moveto.y < 0 || moveto.y >= field.GetLength(0))
            return false;

        if (moveto.x < 0 || moveto.x >= field.GetLength(1))
            return false;
        if (map[moveto.y, moveto.x] == 4)
            return false;

        if (field[moveto.y, moveto.x] != null && field[moveto.y, moveto.x].tag == "Box") {
            Vector2Int velocity = moveto - movefrom;
            bool success = MoveNumber(moveto, moveto + velocity);
            if (!success) { return false; }
        }

        field[moveto.y, moveto.x] = field[movefrom.y, movefrom.x];
        field[movefrom.y, movefrom.x] = null;

        Move move = field[moveto.y, moveto.x].GetComponent<Move>();
        move.MoveTo(new Vector3(moveto.x, -1 * moveto.y, 0));
        if (isPlayerMove)
        {
            moveCount++; //Count++
            UpdateMoveCounterText(); // 更新
        }

        return true;
    }


    bool IsClear()
    {
        List<Vector2Int> goals = new List<Vector2Int>();
        for (int y = 0; y < map.GetLength(0); y++)
        {
            for (int x = 0; x < map.GetLength(1); x++)
            {

                if (map[y, x] == 3)
                {
                    goals.Add(new Vector2Int(x, y));
                }
            }
        }

        for (int i=0;i<goals.Count;i++)
        {
            GameObject f = field[goals[i].y, goals[i].x];
            if (f == null || f.tag != "Box")
            {
                return false;
            }

        }
        return true;
    }


    Vector2Int GetPlayerIndex()
    {
        for (int y = 0; y < field.GetLength(0); y++)
        {
            for (int x = 0; x < field.GetLength(1); x++)
            {
                GameObject obj = field[y, x];

                if (obj != null && obj.tag == "Player")
                {
                    return new Vector2Int(x, y);
                }   // プレイヤーを見つけた
            }
        }

        return new Vector2Int(-1, -1);  // 見つからなかった
    }

    void PrintArray()
    {
        string debugText = "";

        for (int y = 0; y < map.GetLength(0); y++)
        {
            for (int x = 0; x < map.GetLength(1); x++)
            {
                debugText += map[y, x].ToString() + ",";
            }

            debugText += "\n";
        }

        Debug.Log(debugText);
    }

    void Start()
    {
        map = new int[,]
        {
            { 4,4, 4, 4, 4, 4,4,},
            {4, 1, 0, 0, 0, 0,4,},
            {4, 0, 2, 3, 2, 0,4,},
            {4, 0, 0, 0, 0, 0, 4},
            {4, 0, 3, 2, 3, 0, 4,},
            {4, 0, 0, 0, 0, 0, 4},
             { 4,4, 4, 4, 4, 4,4,},

        };
        moveCount = 0; // 初始化MoveCount
        PrintArray();

        field = new GameObject[
            map.GetLength(0),
            map.GetLength(1)
        ];

        for (int y = 0; y < map.GetLength(0); y++)
        {
            for (int x = 0; x < map.GetLength(1); x++)
            {
                if (map[y, x] == 1)
                {
                    GameObject instance =
                        Instantiate(playerPrefab,
                        new Vector3(x, -1 * y, 0),
                        Quaternion.identity);
                    field[y, x] = instance; // プレイヤーを保存しておく
                    // break;  // プレイヤーは１つだけなので抜ける
                }   // プレイヤーを出す
                else if (map[y, x] == 2)
                {
                    GameObject instance =
                        Instantiate(boxPrefab,
                        new Vector3(x, -1 * y, 0),
                        Quaternion.identity);
                    field[y, x] = instance; // 箱を保存しておく
                }   // 箱を出す
                else if (map[y, x] == 3)
                {
                    GameObject instance =
                        Instantiate(ClearTarget,
                        new Vector3(x, -1 * y, 0.5f),
                        Quaternion.identity);
                }   // CLEAR場所を出す
                else if (map[y, x] == 4)
                {
                    GameObject instance =
                        Instantiate(WallPrefab,
                        new Vector3(x, -1 * y, 0),
                        Quaternion.identity);
                }   // 壁を出す
            }
        }
        UpdateMoveCounterText();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitGame();
        }

       
            if (Input.GetKeyDown(KeyCode.R))
            {
                ResetGame();
            }
        if (isGameCleared)
        {
            return; // CLEAR中
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            var playerPostion = GetPlayerIndex();
            MoveNumber(playerPostion, playerPostion + Vector2Int.right, true);
            PrintArray();
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            var playerPostion = GetPlayerIndex();
            MoveNumber(playerPostion, playerPostion + Vector2Int.left, true);
            PrintArray();
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            var playerPostion = GetPlayerIndex();
            MoveNumber(playerPostion, playerPostion - Vector2Int.up, true);
            PrintArray();
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            var playerPostion = GetPlayerIndex();
            MoveNumber(playerPostion, playerPostion - Vector2Int.down, true);
            PrintArray();

        }

        if (IsClear())
        {
            ClearText.SetActive(true);
            isGameCleared = true;
        }
    }
    void UpdateMoveCounterText()
    {
        moveCounterText.text = "Moves: " + moveCount;
    }

    void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}