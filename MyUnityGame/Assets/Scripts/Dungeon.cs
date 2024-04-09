using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class Dungeon : MonoBehaviour
{
    #region variables
    public float camMoveDuration;

    int rngRoom;

    public GameObject firstRoom;
    public GameObject connectorBtwRooms;
    public GameObject[] rooms;
    public Texture2D[] enemyMaps;

    public GameObject enemyPrefab;

    bool areEnemiesInRoom = false;
    bool hasStartedMovingCam = false;
    bool hasBeenInCon = false;
    bool roomDown = false;
    bool roomUp = false;


    public GameObject player;
    public Camera cam;
    EnemySpawner enemyGen = new EnemySpawner();

    List<GameObject> spawnedRooms = new List<GameObject>();
    GameObject currentRoom;
    GameObject currentCon;

    ConnectorComp conComp;
    RoomComp roomComp;
    #endregion

    class EnemySpawner
    {
        GameObject enemyPrefab;
        Texture2D enemyMap;
        Transform room;
        bool isRot;

        public void DrawEnemies(Texture2D map, Transform roomTrans, bool isRotated, GameObject enemy)
        {
            isRot = isRotated;
            room = roomTrans;
            enemyMap = map;
            enemyPrefab = enemy;

            for (int x = 0; x < enemyMap.width; x++)
            {
                for (int y = 0; y < enemyMap.height; y++)
                {
                    CheckTile(x, y);
                }
            }
        }
        private void CheckTile(int x, int y)
        {
            Color pixelColor;
            if (isRot)
            {
                pixelColor = enemyMap.GetPixel(x, y);
            }
            else
            {
                pixelColor = enemyMap.GetPixel(y, x);
            }

            if (pixelColor.a == 0)
            {
                return;
            }
            else
            {
                Instantiate(enemyPrefab, room.TransformPoint(x - 9, y - 9, 0f), Quaternion.identity);
            }
        }
    }
    
    static class RoomChildren
    {
        public static Transform spawnPoint;

        public static GameObject door; 
        public static GameObject backdoor;

        // first room children are defined in the start function
        public static void UpdateChildren(GameObject room)
        {
            door = room.transform.Find("Door").gameObject;
            backdoor = room.transform.Find("Backdoor").gameObject;
        }
    }

    static class ConChildren
    {
        public static Transform camPos;
        public static Transform nextRoomPos;

        public static void UpdateChildren(GameObject con)
        {
            camPos = con.transform.Find("CamPos");
            nextRoomPos = con.transform.Find("NextRoomPos");
        }
    }

    void Start()
    {
        // spawn first room
        currentRoom = Instantiate(firstRoom, Vector3.zero, Quaternion.identity);
        RoomChildren.door = currentRoom.transform.Find("Door").gameObject;
        RoomChildren.spawnPoint = currentRoom.transform.Find("SpawnPoint");
        spawnedRooms.Add(currentRoom);

        
        player.transform.position = RoomChildren.spawnPoint.position; // teleport the player to the spawn point
        enemyGen.DrawEnemies(enemyMaps[0], currentRoom.transform, IsNextRoomRotated(), enemyPrefab); // spawn enemies
        cam.transform.position = currentRoom.transform.position;
        areEnemiesInRoom = false;
        roomComp = currentRoom.GetComponent<RoomComp>();
    }

    void Update()
    {
        if (currentCon != null)
        {
            // if the player has left the connector
            if (roomComp.playerTouching && !conComp.playerTouching && hasBeenInCon)
            { 
                if (cam.transform.position != currentRoom.transform.position || !hasStartedMovingCam)
                {
                    StartCoroutine(MoveCam(currentRoom.transform.position));
                    hasStartedMovingCam = true;
                }
                if (currentRoom.transform.Find("Backdoor") != null)
                {
                    RoomChildren.backdoor.SetActive(true);
                }
                enemyGen.DrawEnemies(enemyMaps[Random.Range(0, enemyMaps.Length)], currentRoom.transform, IsNextRoomRotated(), enemyPrefab); //spawn enemies

                hasBeenInCon = false;
                areEnemiesInRoom = false;
            }
        }
        if (roomComp.enemies.Count == 0)
        {
            if (!roomComp.playerTouching && conComp.playerTouching && !hasBeenInCon) // when the player is in the connector
            {
                hasBeenInCon = true;
                NextRoom();
                if (cam.transform.position != ConChildren.camPos.position || !hasStartedMovingCam)
                {
                    StartCoroutine(MoveCam(ConChildren.camPos.position));
                    hasStartedMovingCam = true;
                }
                if (roomComp.playerTouching && currentRoom.transform.Find("Backdoor") != null)
                {
                    RoomChildren.backdoor.SetActive(true);
                }
            }
            if (!areEnemiesInRoom) // spawn connector if havent already
            {
                SpawnCon();
            }
        }
        else if (roomComp.enemies.Count > 0 && areEnemiesInRoom)
        {
            Destroy(currentCon);
            RoomChildren.door.SetActive(true);
            areEnemiesInRoom = false;
        }
    }
    bool IsNextRoomRotated()
    {
        if (currentRoom != null)
        {
            if (RoomChildren.door.transform.position.y != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }
    void SpawnCon()
    {
        if (RoomChildren.door.transform.position.y < currentRoom.transform.position.y) // next room is under
        {
            currentCon = Instantiate(connectorBtwRooms, RoomChildren.door.transform.position, Quaternion.Euler(0, 0, -90));
            roomDown = true;
            roomUp = false;
        }
        else if (RoomChildren.door.transform.position.y > currentRoom.transform.position.y) // next room is above
        {
            currentCon = Instantiate(connectorBtwRooms, RoomChildren.door.transform.position, Quaternion.Euler(0, 0, 90));
            roomDown = false;
            roomUp = true;
        }
        else // next room is forward
        {
            currentCon = Instantiate(connectorBtwRooms, RoomChildren.door.transform.position, Quaternion.identity);
            roomDown = false;
            roomUp = false;
        }

        ConChildren.UpdateChildren(currentCon);
        RoomChildren.door.gameObject.SetActive(false);
        conComp = currentCon.GetComponent<ConnectorComp>();

        hasBeenInCon = false;
        areEnemiesInRoom = true;
    }
    void NextRoom()
    {
        rngRoom = Random.Range(0, rooms.Length);

        if (roomUp)
        {
            while (rooms[rngRoom].name.Contains("LURoom"))
            {
                rngRoom = Random.Range(0, rooms.Length);
            }
        }
        else if (roomDown)
        {
            while (rooms[rngRoom].name.Contains("LDRoom"))
            {
                rngRoom = Random.Range(0, rooms.Length);
            }
        }

        if (ConChildren.nextRoomPos.position.y < currentCon.transform.position.y)
        {
            currentRoom = Instantiate(rooms[rngRoom], ConChildren.nextRoomPos.position, Quaternion.Euler(0, 0, 270));
        }
        else if (ConChildren.nextRoomPos.position.y > currentCon.transform.position.y)
        {
            currentRoom = Instantiate(rooms[rngRoom], ConChildren.nextRoomPos.position, Quaternion.Euler(0, 0, 90));
        }
        else
        {
            currentRoom = Instantiate(rooms[rngRoom], ConChildren.nextRoomPos.position, Quaternion.identity);
        }

        RoomChildren.UpdateChildren(currentRoom);
        roomUp = false;
        roomDown = false;
        spawnedRooms.Add(currentRoom);
        roomComp = currentRoom.GetComponent<RoomComp>();
    }
    IEnumerator MoveCam(Vector3 targetPosition) //moves the camera from one position to the other smoothly
    {
        Vector3 startPos = cam.transform.position;
        float elapsedTime = 0f;
        float smoothLerp = 0f;
        while (smoothLerp < 1f)
        {
            smoothLerp = Mathf.SmoothStep(0.0f, 1.0f, elapsedTime / camMoveDuration);
            cam.transform.position = Vector3.Lerp(startPos, targetPosition, smoothLerp);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        cam.transform.position = targetPosition;
        hasStartedMovingCam = false;
    }
}