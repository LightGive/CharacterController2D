using UnityEngine;
using System.Collections;

/// <summary>
/// キャラクターコントローラー２D
/// アタッチした時以下を追加
/// ・Animator
/// ・Rigidbody2D
/// ・BoxCollider
/// ・SpriteRenderer
/// </summary>
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class CharacterController2D : MonoBehaviour
{
    [Header("GROUND_LAYER")]
    public LayerMask groundLayer;
    [Header("CHARACTER_IMAGE_DIR")]
    public CharacterDir charaImageDir;

    [Header("ANIMATION_NAME")]
    public string animNameJump = "Jump";
    public string animNameIdle = "Idle";
    public string animNameWalk = "Walk";

    [Header("CHARACTER_STATUS")]
    public float charaScale = 1.0f;
    public float charaHead = 1.0f;
    public float charaFoot = -1.0f;
    public float charaWidth = 1.0f;
    public float charaGravityScale = 1.0f;
    public float charaMoveSpeed = 5.0f;
    public float charaJumpScale = 10.0f;
    public float charaCeilingBouness = -1.0f;
    public CharacterDir charaDir = CharacterDir.Right;

    [Header("KEY_CONFIG")]
    public bool isInput = true;

    private Rigidbody2D rigid = null;
    private Animator animator = null;
    private bool isDeath = false;

    /// <summary>
    /// 開始処理
    /// </summary>
    void Start()
    {
        Init();
    }

    /// <summary>
    /// 初期化
    /// </summary>
    void Init()
    {
        if (rigid == null)
            rigid = this.gameObject.GetComponent<Rigidbody2D>();
        if (animator == null)
            animator = this.gameObject.GetComponent<Animator>();

        //初期のキャラクターの向きを合わせる
        if (transform.localScale.x > 0)
            charaDir = CharacterDir.Right;
        else if (transform.localScale.x < 0)
            charaDir = CharacterDir.Left;

        rigid.freezeRotation = true;
        rigid.gravityScale = charaGravityScale;
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    void Update()
    {
        if (isDeath)
            return;

        if (isInput)
            InputKey();

        Animation();
    }

    /// <summary>
    /// シーンビューにキャラ頭の線と床の線を表示させる
    /// </summary>
    void OnDrawGizmos()
    {
        if (Application.isPlaying)
            return;

        Gizmos.color = Color.blue;
        Vector3 floorA = transform.position + new Vector3(-(charaWidth / 2), charaFoot);
        Vector3 floorB = transform.position + new Vector3((charaWidth / 2), charaFoot);
        Gizmos.DrawLine(floorA, floorB);

        Vector3 ceilingA = transform.position + new Vector3(-(charaWidth / 2), charaHead);
        Vector3 ceilingB = transform.position + new Vector3((charaWidth / 2), charaHead);
        Gizmos.DrawLine(ceilingA, ceilingB);
    }

    /// <summary>
    /// キー入力
    /// </summary>
    void InputKey()
    {
        if (Input.GetButtonDown("Jump"))
            Jump();
            
        //速度に緩急が出るように(お好み)
        //Move(Input.GetAxis("Horizontal"));
        Move(Input.GetAxisRaw("Horizontal"));

    }


    /// <summary>
    /// アニメーション制御
    /// </summary>
    void Animation()
    {
        if (!isGranded())
            animator.Play(animNameJump);
        else if (rigid.velocity.x == 0.0f)
            animator.Play(animNameIdle);
        else
            animator.Play(animNameWalk);
    }

    /// <summary>
    /// ジャンプした時
    /// </summary>
    public void Jump()
    {
        if (isGranded())
        {
            var nowVec = rigid.velocity;
            rigid.velocity = new Vector3(nowVec.x, charaJumpScale);
        }
    }

    /// <summary>
    /// 移動
    /// </summary>
    public void Move(float rightSpeed)
    {
        if (rightSpeed > 0)
            charaDir = CharacterDir.Right;
        else if (rightSpeed < 0)
            charaDir = CharacterDir.Left;
        var dir = rightSpeed * charaMoveSpeed;


        if (charaImageDir == CharacterDir.Right)
            transform.localScale = new Vector3((float)charaDir * charaScale, charaScale);
        else
            transform.localScale = new Vector3(-(float)charaDir * charaScale, charaScale);

        rigid.velocity = new Vector2(dir, rigid.velocity.y);

        //頭に当たったとき、下に向かって力を加える
        if (isCeiling())
            rigid.velocity = new Vector2(rigid.velocity.x, charaCeilingBouness);
    }


    /// <summary>
    /// 地面についているかどうか
    /// </summary>
    /// <returns></returns>
    public bool isGranded()
    {
        Vector3 floorA = transform.position + new Vector3(-(charaWidth / 2), charaFoot);
        Vector3 floorB = transform.position + new Vector3((charaWidth / 2), charaFoot);
        bool hit = Physics2D.Linecast(floorA, floorB, groundLayer);
        Debug.Log("Hit::" + hit);
        return hit;
    }

    /// <summary>
    /// 頭が当たったかどうか
    /// </summary>
    /// <returns></returns>
    public bool isCeiling()
    {
        Vector3 ceilingA = transform.position + new Vector3(-(charaWidth / 2), charaHead);
        Vector3 ceilingB = transform.position + new Vector3((charaWidth / 2), charaHead);
        bool hit = Physics2D.Linecast(ceilingA, ceilingB, groundLayer);
        return hit;
    }

    /// <summary>
    /// 死んだとき
    /// </summary>
    public void Death()
    {
        isDeath = true;
    }
}

/// <summary>
/// CharactorSpriteの向き
/// </summary>
public enum CharacterDir // ほかのクラスからの利用を考えて分離, キャラクターの向きに利用するために各要素に1,-1を設定
{
    Right = 1,
    Left = -1
}
