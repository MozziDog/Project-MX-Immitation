using UnityEngine;
using Logic;
// using DamageNumbersPro;


// TODO: Replace DamageNumbersPro
public class CharacterVisual : MonoBehaviour
{
    public CharacterLogic Logic;
    
    // [SerializeField] DamageNumber _damageNumberPrefab;
    Vector3 _positionBeforeFrame;
    
    // Start is called before the first frame update
    void Start()
    {
        _positionBeforeFrame = transform.position;
        Logic.OnAttack += LookAtEnemy;
        Logic.OnUseNormalSkill += LookAtEnemy;
        Logic.OnUseExSkill += LookAtEnemy;
        Logic.OnReload += OnReloaded;
        Logic.OnShoulderWeapon += OnShoulder;
        Logic.OnUnshoulderWeapon += OnUnshoulder;
        Logic.OnCharacterTakeDamage += DisplayDamageNumber;
        Logic.OnDie += DestroyVisual;
    }

    // Update is called once per frame
    void Update()
    {
        var targetPosition = ToVector3(Logic.Position);
        
        if (Logic.IsMoving)
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
        CharacterLogic enemy = Logic.CurrentTarget;
        if(enemy == null)
        {
            return;
        }
        Position2 targetLogicPosition = enemy.Position;
        Vector3 targetWorldPosition = new Vector3(targetLogicPosition.x, 0, targetLogicPosition.y);
        transform.LookAt(targetWorldPosition);
    }

    void OnReloaded()
    {
        // _damageNumberPrefab.Spawn(transform.position, "Reloaded");
    }

    void OnShoulder()
    {
        // _damageNumberPrefab.Spawn(transform.position, "Shouldered Weapon");
    }

    void OnUnshoulder()
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
        Gizmos.DrawWireSphere(transform.position, Logic.AttackRange);

        // 이동 목표 위치
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(ToVector3(Logic.MoveDest), 0.1f);

        // 공격 대상
        if (Logic.CurrentTarget != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(new Vector3(Logic.Position.x, 0, Logic.Position.y), 0.1f);
        }
    }

    private Vector3 ToVector3(Position2 pos)
    {
        return new Vector3(pos.x, 0, pos.y);
    }
}
