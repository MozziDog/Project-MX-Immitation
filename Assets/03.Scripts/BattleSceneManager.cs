using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Logic;

public class BattleSceneManager : MonoBehaviour
{
    [Title("전투 초기화 정보")]
    public BattleData BattleData;
    public BattleLogic BattleLogic;
    public CharacterPrefabDatabase CharacterViewDatabase;
    public CharacterVisual EnemyPrefab;
    public ObstacleVisual ObstaclePrefab;
    public BulletVisual BulletPrefab;

    [Title("전투 씬 상태")]
    [SerializeField] int _logicTickPerSecond = 30;

    [Title("관리중인 비주얼 엔티티들")]
    public List<CharacterVisual> CharacterVisuals = new List<CharacterVisual>();
    public List<CharacterVisual> EnemyVisuals = new List<CharacterVisual>();
    public List<ObstacleVisual> ObstacleVisuals = new List<ObstacleVisual>();
    public List<BulletVisual> BulletVisuals = new List<BulletVisual>();

    // 캐릭터 비주얼 추가/삭제 이벤트
    public delegate void BattleEvent(BattleLogic battleLogic);
    public BattleEvent OnBattleBegin;
    public BattleEvent OnBattleEnd;
    public delegate void CharacterVisualEvent(CharacterVisual visual);
    public CharacterVisualEvent OnCharacterVisualSpawn;
    public CharacterVisualEvent OnCharacterVisualDestroy;
    public CharacterVisualEvent OnEnemyVisualSpawn;
    public CharacterVisualEvent OnEnemyVisualDestroy;
    public delegate void BulletVisualEvent(BulletVisual visual);
    public BulletVisualEvent OnBulletVisualSpawn;
    public BulletVisualEvent OnBulletVisualDestroy;
    public delegate void ObstacleVisualEvent(ObstacleVisual visual);
    public ObstacleVisualEvent OnObstacleVisualSpawn;
    public ObstacleVisualEvent OnObstacleVisualDestroy;

    // Start is called before the first frame update
    private void Start()
    {
        BattleLogic = new BattleLogic();

        // 이벤트 등록
        BattleLogic.OnAllySpawn += SpawnCharacterVisual;
        BattleLogic.OnAllyDie += DestroyCharacterVisual;
        BattleLogic.OnEnemySpawn += SpawnEnemyVisual;
        BattleLogic.OnEnemyDie += DestroyEnemyVisual;
        BattleLogic.OnBulletSpawned += SpawnBulletVisual;
        BattleLogic.OnBulletExpired += DestroyBulletVisual;
        BattleLogic.OnObstacleSpawned += SpawnObstacleVisual;
        BattleLogic.OnObstacleDestroyed += DestroyObstacleVisual;
        

        // 전투 시작
        BattleLogic.Init(BattleData);
        OnBattleBegin?.Invoke(BattleLogic);
        StartCoroutine(GameCoroutine(BattleData));
    }

    private void SpawnCharacterVisual(CharacterLogic newCharacter)
    {
        // 캐릭터(비주얼) 생성
        var visual = Instantiate(CharacterViewDatabase.CharacterViews[newCharacter.Name]);

        // Visual 초기화 진행
        visual.transform.position = Position2ToVector3(newCharacter.Position);
        visual.CharacterLogic = newCharacter;
        CharacterVisuals.Add(visual);
        
        OnCharacterVisualSpawn?.Invoke(visual);
    }

    private void DestroyCharacterVisual(CharacterLogic deadCharacter)
    {
        // 대상 CharacterVisual 찾기
        var visual = CharacterVisuals.Find((ch) => { return ch.CharacterLogic == deadCharacter; });

        OnCharacterVisualDestroy?.Invoke(visual);
        Destroy(visual.gameObject);
    }

    private void SpawnEnemyVisual(CharacterLogic newEnemy)
    {
        // 적(비주얼) 생성
        var enemyVisualObject = Instantiate(EnemyPrefab);
        var enemyVisualComponent = enemyVisualObject.GetComponent<CharacterVisual>();

        // 최소한의 초기화만 진행
        enemyVisualObject.transform.position = Position2ToVector3(newEnemy.Position);
        enemyVisualComponent.CharacterLogic = newEnemy;
        EnemyVisuals.Add(enemyVisualComponent);
        
        OnCharacterVisualSpawn?.Invoke(enemyVisualComponent);
    }

    private void DestroyEnemyVisual(CharacterLogic deadEnemy)
    {
        // 대상 CharacterVisual 찾기
        var visual = EnemyVisuals.Find((ch) => { return ch.CharacterLogic == deadEnemy; });

        OnEnemyVisualDestroy?.Invoke(visual);
        Destroy(visual.gameObject);
    }

    private void SpawnBulletVisual(BulletLogic newBullet)
    {
        // 새 총알 오브젝트를 생성하고 Bullet 로직과 연결
        var bulletObject = Instantiate(BulletPrefab);
        var visual = bulletObject.GetComponent<BulletVisual>();
        visual.BulletLogic = newBullet;
        BulletVisuals.Add(visual);
        
        OnBulletVisualSpawn?.Invoke(visual);
    }

    private void DestroyBulletVisual(BulletLogic expiredBullet)
    {
        // 대응하는 bulletVisual 찾기
        var visual = BulletVisuals.Find(
                            (bu) => { return bu.BulletLogic == expiredBullet; });
        
        OnBulletVisualDestroy?.Invoke(visual);
        Destroy(visual.gameObject);
    }

    private void SpawnObstacleVisual(ObstacleLogic newObstacle)
    {
        var visual = Instantiate(ObstaclePrefab);
        visual.ObstacleLogic = newObstacle;
        visual.transform.position = Position2ToVector3(newObstacle.Position);
        visual.transform.localScale = Position2ToVector3(newObstacle.Scale) + Vector3.up; // (x, 1, y)
        visual.transform.rotation = Quaternion.Euler(0, 0, -newObstacle.Rotation);  // Unity 왼손 좌표계 고려
        ObstacleVisuals.Add(visual);
        
        OnObstacleVisualSpawn?.Invoke(visual);
    }

    private void DestroyObstacleVisual(ObstacleLogic destroyedObstacle)
    {
        var visual = ObstacleVisuals.Find(
                        ob => { return ob.ObstacleLogic == destroyedObstacle; });
        OnObstacleVisualDestroy?.Invoke(visual);
        Destroy(visual.gameObject);
    }

    private IEnumerator GameCoroutine(BattleData battleData)
    {
        // 게임 루프 진행
        while(BattleLogic.BattleState == BattleSceneState.InBattle)
        {
            BattleLogic.Tick();
            yield return new WaitForSeconds(1f / _logicTickPerSecond);
        }
        Debug.Log("게임 오버");
        if(OnBattleEnd != null)
        {
            OnBattleEnd(BattleLogic);
        }
        yield break;
    }

    private Vector3 Position2ToVector3(Position2 logicPosition)
    {
        return new Vector3(logicPosition.x, 0, logicPosition.y);
    }

    public void OnClickSkillCard(int index)
    {
        BattleLogic.TryUseSkillCard(index);
    }
}
