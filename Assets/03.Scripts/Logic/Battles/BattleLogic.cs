using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Logic.Pathfind;

namespace Logic
{
    public class BattleLogic
    {
        // 상수
        readonly List<Position2> SpawnPoint = new List<Position2>{
            new Position2(-3, 0),
            new Position2(-1, 0),
            new Position2(1, 0),
            new Position2(3, 0)
        };
        public readonly int ExCostGaugePerCount = 100000;     // 코스트 갯수 1개를 증가시키기 위해 필요한 충전량.

        public BattleData BattleData;

        // 전투 씬 상태
        public int BaseLogicTickrate = 30;
        public BattleSceneState BattleState;
        public int LeftAllyStriker;
        public int LeftEnemyStriker;
        public int CurWave;

        // 관리중인 엔티티들(로직)
        public CharacterGroup CharactersLogic = new CharacterGroup();      // 아군
        public CharacterGroup EnemiesLogic = new CharacterGroup();          // 적군
        public List<ObstacleLogic> ObstaclesLogic = new List<ObstacleLogic>();
        public List<BulletLogic> BulletsActive = new List<BulletLogic>();
        
        // 리스트 순회 도중 삭제할 것들 임시 저장
        List<CharacterLogic> _charactersToRemove = new List<CharacterLogic>();
        List<CharacterLogic> _enemiesToRemove = new List<CharacterLogic>();
        List<ObstacleLogic> _obstaclesToRemove = new List<ObstacleLogic>();
        List<BulletLogic> _bulletsToRemove = new List<BulletLogic>();

        // EX 스킬 관련
        public int ExCostCount = 0;         // 현재 코스트 갯수. Ex 스킬 사용에 필요.
        public int ExCostRecharging = 0;    // 현재 코스트 충전량. 이 값이 최대치가 되면 ExCostCount가 1 증가.
        public int ExCostRegen = 0;         // 틱 당 코스트 회복량. 캐릭터 코스트 회복량의 총합.
        public List<CharacterLogic> skillCardHand = new List<CharacterLogic>();       // 패. 최대 3장
        public LinkedList<CharacterLogic> skillCardDeck = new LinkedList<CharacterLogic>(); // 덱. 패에 들고 있지 않은 모든 스킬카드.

        // 길찾기 담당
        public NavigationSystem NavigationSystem;
        
        // 전투 이벤트들
        public Action OnBattleBegin;
        public delegate void CharacterInstanceEvent(CharacterLogic characterLogic);
        public CharacterInstanceEvent OnAllySpawn;
        public CharacterInstanceEvent OnEnemySpawn;
        public CharacterInstanceEvent OnAllyDie;
        public CharacterInstanceEvent OnEnemyDie;
        public delegate void BulletInstacneEvent(BulletLogic bulletLogic);
        public BulletInstacneEvent OnBulletSpawned;
        public BulletInstacneEvent OnBulletExpired;
        public delegate void ObstacleInstacneEvent(ObstacleLogic obstacleLogic);
        public ObstacleInstacneEvent OnObstacleSpawned;
        public ObstacleInstacneEvent OnObstacleDestroyed;
        

        public void Init(BattleData battleData)
        {
            this.BattleData = battleData;
            BattleState = BattleSceneState.InBattle;
            
            // 장애물 스폰
            for (int i = 0; i < battleData.Obstacles.Count; i++)
            {
                var pos = battleData.ObstaclePosition[i];
                var rot = battleData.ObstacleRotation[i];
                SpawnObstacle( battleData.Obstacles[i], pos, rot);
            }
            
            // Pathfinder 초기화
            NavigationSystem = new NavigationSystem(10, 100, ObstaclesLogic);
            
            // battleData에 기록된 캐릭터들을 스폰
            // 아군
            for (int i = 0; i < battleData.Characters.Count; i++)
            {
                if (battleData.Characters[i] != null) 
                    SpawnCharacter( 
                        battleData.Characters[i], 
                        battleData.CharacterStats[i], 
                        SpawnPoint[i] 
                    );
            }

            // 첫번째 웨이브 스폰
            CurWave = 0;
            var firstWave = battleData.EnemyWaves[0];
            for(int i=0; i<firstWave.Count; i++)
                SpawnEnemy( 
                    firstWave.Enemies[i], 
                    firstWave.EnemyStats[i], 
                    firstWave.EnemyPositions[i]
                );
            // 남은 적 수 계산
            foreach(var wave in battleData.EnemyWaves)
                LeftEnemyStriker += wave.Count;

            // 코스트 회복량 산정
            foreach(var character in CharactersLogic)
                ExCostRegen += character.CostRegen;

            // 스킬카드 덱 구성 & 최대 3장 드로우
            foreach(int i in Enumerable.Range(0, CharactersLogic.Count).OrderBy(x => UnityEngine.Random.Range(0,1)))
                skillCardDeck.AddLast(CharactersLogic[i]);
            
            int numberToDraw = Mathf.Min(skillCardDeck.Count, 3);
            for (int i=0; i< numberToDraw; i++)
                DrawSkillCard();
            
            // 이벤트 구독
            OnAllyDie += RemoveSkillCardFromDeck;
            OnAllyDie += UpdateExCostRegen;
            OnAllyDie += CheckGameLose;
            OnEnemyDie += CheckGameWin;

            // 초기화 완료 후, 게임 루프 시작 전에 이벤트 호출
            OnBattleBegin?.Invoke();
        }

        public void Tick()
        {
            // 코스트 회복
            if(ExCostCount < 10)
            {
                ExCostRecharging += ExCostRegen;
                if(ExCostRecharging > ExCostGaugePerCount)
                {
                    ExCostRecharging -= ExCostGaugePerCount;
                    ExCostCount++;
                }
            }

            // 관리중인 객체들을 모두 틱
            foreach(var character in CharactersLogic)
            {
                character.Tick();
            }
            // TODO: 적군 Tick 활성확
            // foreach (var enemy in EnemiesLogic)
            // {
            //     enemy.Tick();
            // }
            foreach(var bullet in BulletsActive)
            {
                bullet.Tick();
            }

            // 삭제해야할 엔티티 정리
            RemoveInactiveEntities();
        }
        
        /// 다음 웨이브로 넘어가기 전에 아군 행동 기다리기
        public bool GetIfSomeAllyDoingAction()
        {
            bool answer = false;
            foreach(var character in CharactersLogic)
            {
                answer |= character.IsDoingSomeAction;
            }
            return answer;
        }

        private void DrawSkillCard()
        {
            skillCardHand.Add(skillCardDeck.First.Value);
            skillCardDeck.RemoveFirst();
        }

        private void RemoveSkillCardFromDeck(CharacterLogic toRemove)
        {
            // 삭제할 카드가 패에 있다면, 삭제하고 (가능하다면) 드로우
            if(skillCardHand.Remove(toRemove))
            {
                if(skillCardDeck.Count > 0)
                {
                    DrawSkillCard();
                }
            }
            else
            {
                skillCardDeck.Remove(toRemove);
            }
        }

        public void TryUseSkillCard(int index)
        {
            // ex 스킬 사용 가능한지 먼저 체크
            if(skillCardHand.Count <= index)
            {
                Debug.LogError("해당 위치에는 스킬카드가 없음!");
                return;
            }
            CharacterLogic character = skillCardHand[index];
            if(!character.CanUseExSkill)
            {
                Debug.LogWarning("장애물을 뛰어넘는 중에는 Ex 스킬을 사용할 수 없음");
                return;
            }
            if(ExCostCount < character.ExSkillCost)
            {
                Debug.Log("EX스킬 사용을 위한 코스트가 충분하지 않음!");
                return;
            }

            ExCostCount -= character.ExSkillCost;
            character.TriggerExSkill();

            // 스킬 카드를 덱의 맨 밑으로 넣고 한 장 드로우
            skillCardHand.Remove(character);
            skillCardDeck.AddLast(character);
            DrawSkillCard();

            return;
        }

        private CharacterLogic SpawnCharacter(CharacterData characterData, 
                                    CharacterStatData characterStat, Position2 spawnPos)
        {
            CharacterLogic newCharacter = new CharacterLogic(this, characterData, characterStat);
            newCharacter.SetPosition(spawnPos);

            CharactersLogic.Add(newCharacter);
            OnAllySpawn?.Invoke(newCharacter);
            return newCharacter;
        }

        private CharacterLogic SpawnEnemy(CharacterData enemyData, 
                                    CharacterStatData enemyStat, Position2 spawnPos)
        {
            CharacterLogic newEnemy = new CharacterLogic(this, enemyData, enemyStat);
            newEnemy.SetPosition(spawnPos);
            
            EnemiesLogic.Add(newEnemy);
            OnEnemySpawn?.Invoke(newEnemy);
            return newEnemy;
        }

        private ObstacleLogic SpawnObstacle(ObstacleData obstacleData, 
                                    Position2 position, float rotationDeg)
        {
            ObstacleLogic newObstacle = new ObstacleLogic(obstacleData, position, rotationDeg);
            
            ObstaclesLogic.Add(newObstacle);
            OnObstacleSpawned?.Invoke(newObstacle);
            return newObstacle;
        }

        /// <summary>
        /// Character에 의해 스폰된 총알을 관리 대상에 추가
        /// </summary>
        public void AddBullet(BulletLogic bullet)
        {
            BulletsActive.Add(bullet);
            bullet.Init(this);
            
            OnBulletSpawned?.Invoke(bullet);
        }
        
        /// 총알 수명 다했을 때 없애기
        public void RemoveExpiredBullet(BulletLogic expiredBullet)
        {
            _bulletsToRemove.Add(expiredBullet);
            if(OnBulletExpired != null)
            {
                OnBulletExpired(expiredBullet);
            }
        }

        // Tick 안의 루프에서 엔티티가 삭제/추가되면 안되므로
        // 삭제해야 할 엔티티들은 모아뒀다가 Tick 사이에 처리
        private void RemoveInactiveEntities()
        {
            if (_charactersToRemove.Count > 0)
            {
                for (int i = _charactersToRemove.Count - 1; i >= 0; i--)
                {
                    CharactersLogic.Remove(_charactersToRemove[i]);
                }
                _charactersToRemove.Clear();
            }
            if (_enemiesToRemove.Count > 0)
            {
                for(int i = _enemiesToRemove.Count - 1; i>=0; i--)
                {
                    EnemiesLogic.Remove(_enemiesToRemove[i]);
                }
                _enemiesToRemove.Clear();
            }
            if (_bulletsToRemove.Count > 0)
            {
                for (int i = _bulletsToRemove.Count - 1; i >= 0; i--)
                {
                    BulletsActive.Remove(_bulletsToRemove[i]);
                }
                _bulletsToRemove.Clear();
            }
        }

        /// 아군 적군 상관없이 임의의 캐릭터 사망 시 이벤트
        public void RemoveDeadCharacter(CharacterLogic deadCharacter)
        {
            // 아군일 경우
            if(CharactersLogic.Contains(deadCharacter))
            {
                OnAllyDie?.Invoke(deadCharacter);
                _charactersToRemove.Add(deadCharacter);
            }
            // 적군일 경우
            else if(EnemiesLogic.Contains(deadCharacter))
            {
                OnEnemyDie?.Invoke(deadCharacter);
                _enemiesToRemove.Add(deadCharacter);
            }
        }

        private void UpdateExCostRegen(CharacterLogic deadCharacter)
        {
            ExCostRegen -= deadCharacter.CostRegen;
        }

        private void CheckGameLose(CharacterLogic deadCharacter)
        {
            LeftAllyStriker--;
            if (LeftAllyStriker <= 0)
                BattleState = BattleSceneState.lose;
        }

        private void CheckGameWin(CharacterLogic deadEnemy)
        {
            LeftEnemyStriker--;
            if (LeftEnemyStriker <= 0)
                BattleState = BattleSceneState.win;
            else
            {
                // 다음 웨이브 스폰
                CurWave++;
                var nextWave = BattleData.EnemyWaves[CurWave];
                for(int i=0; i<nextWave.Count; i++)
                    SpawnEnemy( 
                        nextWave.Enemies[i], 
                        nextWave.EnemyStats[i], 
                        nextWave.EnemyPositions[i]
                    );
            }
        }
    }
}