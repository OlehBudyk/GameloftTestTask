using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameHandler : MonoBehaviour
{
    public GameObject plate;
    public Transform cam;

    private const int Width = 4;
    private const int Height = 4;
    private const int PlatesCount = 16;
    private const string EmptyPlateName = "0";

    private float timer;
    private bool timeStarted = false;
    private GameObject timeTextHandler;
    private GameObject winnerTextHandler;


    private GameObject[,] positions = new GameObject[Width, Height];
    private GameObject emptyPlateRef;

    int[] Shuffle()
    {
        var shuffledPlates = new int[PlatesCount] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };

        bool isGenerated = false;
        do
        {
            //-----------------shuffling---------------------------------------
            for (int i = 0; i < PlatesCount; i++)
            {
                int firstIndex = UnityEngine.Random.Range(0, PlatesCount - 1);
                int secondIndex = UnityEngine.Random.Range(0, PlatesCount - 1);

                int tempValue = shuffledPlates[firstIndex];
                shuffledPlates[firstIndex] = shuffledPlates[secondIndex];
                shuffledPlates[secondIndex] = tempValue;
            }

            //-------------------------------------------------------------------
            int zeroIndex = ArrayUtility.IndexOf(shuffledPlates, 0);

            int zeroRow = 0;
            if (zeroIndex <= 3)
                zeroRow = 1;
            else if (zeroIndex > 3 && zeroIndex <= 7)
                zeroRow = 2;
            else if (zeroIndex > 7 && zeroIndex <= 11)
                zeroRow = 3;
            else if (zeroIndex > 11 && zeroIndex <= 15)
                zeroRow = 4;

            //--------------------------------------------------------------------

            int sum = 0;

            for (int i = 0; i < PlatesCount - 1; i++)
            {
                int currValue = shuffledPlates[i];

                if (currValue != 0)
                    for (int j = i + 1; j <= PlatesCount - 1; j++)
                    {
                        int nextValue = shuffledPlates[j];

                        if (currValue > nextValue && nextValue != 0)
                            sum++;
                    }
            }

            sum += zeroRow;

            if (sum % 2 == 0)
                isGenerated = true;
        }
        while (!isGenerated);

        return shuffledPlates;
    }

    void SetTimeText(GameObject textHandler, float currTime, string prependText)
    {
        string time = Mathf.Floor(currTime / 60).ToString("00") + ":" + Mathf.FloorToInt(currTime % 60).ToString("00");

        textHandler.GetComponent<Text>().text = prependText + time;
    }

    void Update()
    {
        if (timeStarted)
        {
            timer += Time.deltaTime;

            SetTimeText(timeTextHandler, timer, "Played time: ");
        }
    }

    void Start()
    {
        var shuffledPlates = Shuffle();

        int index = 0;
        for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
            {
                positions[x, y] = Create(shuffledPlates[index].ToString(), y, 3 - x);

                index++;
            }

        cam.transform.position = new Vector3((float)Width / 2 - 0.5f, (float)Height / 2 - 0.5f, -10);

        winnerTextHandler = GameObject.FindGameObjectWithTag("WinnerText");
        timeTextHandler = GameObject.FindGameObjectWithTag("TimeText");

        SetTimeText(timeTextHandler, 0, "Played time: ");
    }

    GameObject Create(string name, int x, int y)
    {
        GameObject currentPlate = Instantiate(plate, new Vector3(x, y), Quaternion.identity);
        PlateHandler plateHandler = currentPlate.GetComponent<PlateHandler>();
        plateHandler.name = name;

        if (name == EmptyPlateName)
            emptyPlateRef = currentPlate;

        plateHandler.SetX(x);
        plateHandler.SetY(y);

        plateHandler.Activate();

        return currentPlate;
    }

    bool IsEmptyPlate(int x, int y)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height)
            return false;

        return positions[x, y].name == EmptyPlateName;
    }

    bool IsMovePossible(PlateHandler plate)
    {
        int x = 3 - plate.GetY();
        int y = plate.GetX();

        if (IsEmptyPlate(x + 1, y))
            return true;

        if (IsEmptyPlate(x - 1, y))
            return true;

        if (IsEmptyPlate(x, y + 1))
            return true;

        if (IsEmptyPlate(x, y - 1))
            return true;

        return false;
    }

    public void GameStarted()
    {
        timeStarted = true;
    }

    public void TryMove(PlateHandler plate)
    {
        if (IsMovePossible(plate))
        {
            Move(plate);

            if (IsGameComplet())
                CompletGame();
        }
    }

    void CompletGame()
    {
        timeStarted = false;

        timeTextHandler.GetComponent<Text>().enabled = false;
        winnerTextHandler.GetComponent<Text>().enabled = true;
        SetTimeText(winnerTextHandler, timer, "You win! Time is: ");
    }

    bool IsGameComplet()
    {
        int index = 0;
        for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
            {
                if (index == 15)
                    return true;

                if (positions[x, y].name != Convert.ToString(index + 1))
                    return false;

                index++;
            }

        return true;
    }

    void Move(PlateHandler currPlateHandler)
    {
        int currPlateX = 3 - currPlateHandler.GetY();
        int currPlateY = currPlateHandler.GetX();
        string currPlateName = currPlateHandler.name;

        PlateHandler emptyPlateHandler = emptyPlateRef.GetComponent<PlateHandler>();
        int emptyPlateX = 3 - emptyPlateHandler.GetY();
        int emptyPlateY = emptyPlateHandler.GetX();
        string emptyPlateName = emptyPlateHandler.name;


        Destroy(positions[emptyPlateX, emptyPlateY]);
        Destroy(positions[currPlateX, currPlateY]);

        positions[emptyPlateX, emptyPlateY] = Create(currPlateName, emptyPlateY, 3 - emptyPlateX);
        positions[currPlateX, currPlateY] = Create(emptyPlateName, currPlateY, 3 - currPlateX);
    }
}
