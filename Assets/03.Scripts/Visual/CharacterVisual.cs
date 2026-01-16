using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Logic;
// using DamageNumbersPro;


// TODO: Replace DamageNumbersPro
public class CharacterVisual : MonoBehaviour
{
    public CharacterLogic CharacterLogic;

    // 적 허수아비용 강제 초기화
    [SerializeField] bool _forceInitializeWithUnity;
    [ShowIf("_forceInitializeWithUnity")]
    [SerializeField] Position2 _initPosition;
    [ShowIf("_forceInitializeWithUnity")]
    [SerializeField] int _initHP;

    // [SerializeField] DamageNumber _damageNumberPrefab;
    Vector3 _positionBeforeFrame;

    void Awake()
    {
        // 디버그용: CharacterLogic의 일부 필드를 CharacterVisual의 것으로 덮어쓰기
        if (_forceInitializeWithUnity)
        {
            CharacterLogic.SetPosition(_initPosition);
            CharacterLogic.SetHP(_initHP);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _positionBeforeFrame = transform.position;
        CharacterLogic.OnAttack += LookAtEnemy;
        CharacterLogic.OnUseNormalSkill += LookAtEnemy;
        CharacterLogic.OnUseExSkill += LookAtEnemy;
        CharacterLogic.OnReload += DisplayReloadMessage;
        CharacterLogic.OnShoulderWeapon += DisplayShoulderMessage;
        CharacterLogic.OnUnshoulderWeapon += DisplayUnshoulderMessage;
        CharacterLogic.OnCharacterTakeDamage += DisplayDamageNumber;
        CharacterLogic.OnDie += DestroyVisual;
    }

    // Update is called once per frame
    void Update()
    {
        var targetPosition = ToVector3(CharacterLogic.Position);
        
        if (CharacterLogic.IsMoving)
        {
            Vector3 positionCurrentFrame = targetPosition;
            // 이동 방향 바라보기
            transform.LookAt(targetPosition);
            _positionBeforeFrame = positionCurrentFrame;
        }
        
        // 부드러운 이동을 위한 좌표 보간
        Vector3 velocity = targetPosition - _positionBeforeFrame;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, 0.02f);

    }

    void LookAtEnemy()
    {
        CharacterLogic enemy = CharacterLogic.CurrentTarget;
        if(enemy == null)
        {
            return;
        }
        Position2 targetLogicPosition = enemy.Position;
        Vector3 targetWorldPosition = new Vector3(targetLogicPosition.x, 0, targetLogicPosition.y);
        transform.LookAt(targetWorldPosition);
    }

    void DisplayReloadMessage()
    {
        // _damageNumberPrefab.Spawn(transform.position, "Reloaded");
    }

    void DisplayShoulderMessage()
    {
        // _damageNumberPrefab.Spawn(transform.position, "Shouldered Weapon");
    }

    void DisplayUnshoulderMessage()
    {
        // _damageNumberPrefab.Spawn(transform.position, "Unshouldered Weapon");
    }

    void DisplayDamageNumber(int damage, bool isCritical, AttackType attackType, ArmorType armorType)
    {
        // Debug.Log("데미지 넘버 스폰");
        // if (_damageNumberPrefab != null)
        // {
        //     _damageNumberPrefab.Spawn(transform.position, damage);
        // }
    }

    void DestroyVisual()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        // 사거리 원
        Gizmos.color = Color.yellow;
        // Gizmos.DrawWireSphere(transform.position, attackRange);

        // 이동 목표 위치
        Gizmos.color = Color.green;
        // Gizmos.DrawSphere(CharacterLogic.moveDest, 0.1f);

        // 공격 대상
        if (CharacterLogic.CurrentTarget != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(new Vector3(CharacterLogic.Position.x, 0, CharacterLogic.Position.y), 0.1f);
        }
    }

    private Vector3 ToVector3(Position2 pos)
    {
        return new Vector3(pos.x, 0, pos.y);
    }
}
