using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public Hero actor;
    public bool cameraFollows = true;
    public CameraBounds cameraBounds;


    [Header("Enemy Data")]
    public LevelData currentLevelData;
    private BattleEvent currentBattleEvent;
    private int nextEventIndex;
    public bool hasRemainingEvents;
    public List<GameObject> activeEnemeies;
    public Transform[] spawnPositions;
    public GameObject currentLevelBackground;
    public GameObject robotPrefab;
    
    private GameObject SpawnEnemy(EnemyData data)
    {
        GameObject enemyObj = Instantiate(robotPrefab);

        Vector3 position = spawnPositions[data.row].position;
        position.x = cameraBounds.activeCamera.transform.position.x + (data.offset * (cameraBounds.cameraHalfWidth + 1));
        enemyObj.transform.position = position;

        if (data.type == EnemyType.Robot)
        {
            enemyObj.GetComponent<Robot>().SetColor(data.color);
        }
        enemyObj.GetComponent<Enemy>().RegisterEnemy();

        return enemyObj;
    }

    [Header("Animations")]
    public Transform walkInStartTarget;
    public Transform walkInTarget;
    public Transform walkOutTarget;

    [Header("Level Data")]
    public LevelData[] levels;
    public static int currentLevel = 0;


    // Start is called before the first frame update
    void Start()
    {
        cameraBounds.SetXPosition(cameraBounds.minVisibleX);

        nextEventIndex = 0;
        StartCoroutine(LoadLevelData(levels[currentLevel]));

    }

    // Update is called once per frame
    void Update()
    {
        if(currentBattleEvent == null && hasRemainingEvents)
        {
            if(Mathf.Abs(currentLevelData.battleData[nextEventIndex].column-cameraBounds.activeCamera.transform.position.x) < 0.2f)
            {
                PlayBattleEvents(currentLevelData.battleData[nextEventIndex]);
            }
        }

        if(currentBattleEvent != null)
        {
            if(Robot.totalEnemies == 0)
            {
                CompleteCurrentEvent();
            }

        }
        if(cameraFollows)
        {
            cameraBounds.SetXPosition(actor.transform.position.x);
        }
    }

    ///
    /// Human Created Methods
    ///
    private void PlayBattleEvents(BattleEvent battleEventData)
    {
        currentBattleEvent = battleEventData;
        nextEventIndex++;

        cameraFollows = false;
        cameraBounds.SetXPosition(battleEventData.column);

        foreach(GameObject enemy in activeEnemeies)
        {
            Destroy(enemy);
        }
        activeEnemeies.Clear();
        Enemy.totalEnemies = 0;

        foreach(EnemyData enemyData in currentBattleEvent.enemies)
        {
            activeEnemeies.Add(SpawnEnemy(enemyData));
        }
    }

    private void CompleteCurrentEvent()
    {
        currentBattleEvent = null;

        cameraFollows = true;
        cameraBounds.CalculateOffset(actor.transform.position.x);
        hasRemainingEvents = currentLevelData.battleData.Count > nextEventIndex;

        if(!hasRemainingEvents)
        {
            StartCoroutine(HeroWalkOut());
        }
    }

    private IEnumerator LoadLevelData(LevelData data)
    {
        cameraFollows = false;
        currentLevelData = data;

        hasRemainingEvents = currentLevelData.battleData.Count > 0;
        activeEnemeies = new List<GameObject>();

        yield return null;

        cameraBounds.SetXPosition(cameraBounds.minVisibleX);

        if(currentLevelBackground != null)
        {
            Destroy(currentLevelBackground);
        }
        currentLevelBackground = Instantiate(currentLevelData.levelPrefab);
        cameraBounds.EnableBounds(false);
        actor.transform.position = walkInStartTarget.transform.position;
        yield return new WaitForSeconds(0.1f);
        actor.UseAutopilot(true);
        actor.AnimateTo(walkInTarget.transform.position, false, DidFinishIntro);

        cameraFollows = true;
    }


    public void DidFinishIntro()
    {
        actor.UseAutopilot(false);
        actor.controllable = true;
        cameraBounds.EnableBounds(true);
    }

    private IEnumerator HeroWalkOut()
    {
        cameraBounds.EnableBounds(false);
        cameraFollows = false;

        actor.UseAutopilot(true);
        actor.controllable = false;
        actor.AnimateTo(walkOutTarget.transform.position, true, DidFinishWalkout);
        yield return null;
    }

    public void DidFinishWalkout()
    {
        currentLevel++;
        if(currentLevel >= levels.Length)
        {
            Debug.Log("Game Completed!");
            SceneManager.LoadScene("Main Menu");
        } else
        {
            StartCoroutine(AnimateNextLevel());
        }

        cameraBounds.EnableBounds(true);
        cameraFollows = false;
        actor.UseAutopilot(false);
        actor.controllable = false;
    }

    private IEnumerator AnimateNextLevel()
    {
        yield return null;
        SceneManager.LoadScene("Game");
    }
}
