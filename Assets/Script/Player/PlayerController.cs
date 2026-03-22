using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static AudioManager;
using static GameManager;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb2D;
    private Collider2D _playerCollider;
    private Collider2D[] _playerColliders;
    private Vector3 _initialPosition;

    private InputAction moveAction;
    private InputAction jumpAction;

    public float speed = 5f;
    public float jumpSpeed = 10f;

    [SerializeField] private Transform visualTransform;
    [SerializeField] private float pushMoveDuration = 0.15f;
    [SerializeField] private float pushLeapHeight = 0.25f;
    [SerializeField] private LayerMask pushObstacleLayer = ~0;
    [SerializeField] private float pushBlockCheckRadius = 0.2f;

    private bool _isPushing;
    private float _pushElapsed;
    private Vector3 _pushStartPosition;
    private Vector3 _pushTargetPosition;
    private Vector3 _visualBaseLocalPosition;
    private Vector3 _visualPushStartOffset;

    [HideInInspector]
    public float convayorS;

    private float time = 0.5f;
    private bool superPowerOneShot = false;

    [HideInInspector]
    public bool onGround = false;
    [SerializeField]
    private LayerMask groundLayer;

   // Animator animator;

    [HideInInspector]
    public bool useSuperPower;

    [HideInInspector]
    public bool dontMove = false;//動けない状態

    [HideInInspector]
    public Vector3 moveValue;

    private void OnEnable()
    {
        MouseManager.BlockPlaced += OnBlockPlaced;
    }

    private void OnDisable()
    {
        MouseManager.BlockPlaced -= OnBlockPlaced;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb2D = GetComponentInChildren<Rigidbody2D>();
        _playerCollider = GetComponentInChildren<Collider2D>();
        _playerColliders = GetComponentsInChildren<Collider2D>();
        _initialPosition = transform.position;
        if (visualTransform == null && transform.childCount > 0)
        {
            visualTransform = transform.GetChild(0);
        }
        if (visualTransform != null)
        {
            _visualBaseLocalPosition = visualTransform.localPosition;
        }

        //animator = GetComponentInChildren<Animator>();

        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
        //time = 0;
        dontMove = false;
       // animator.SetBool("Die", false);
    }

    // Update is called once per frame
    void Update()
    {
        if (dontMove)
            return;

        if (_isPushing)
        {
            UpdatePushMotion();
            return;
        }

        // 移動処理
        moveValue = moveAction.ReadValue<Vector2>();

        if(convayorS != 0)
        {
            //コンベアベルトの影響を受ける
            moveValue.x += convayorS;
        }

        if (!useSuperPower)
        {
            rb2D.linearVelocity = new Vector2(moveValue.x * speed, rb2D.linearVelocity.y);
            if (moveValue.x != 0 && onGround)
            {
                time -= Time.deltaTime;
                if (time < 0) {
                    SetAudio(SE.playerMove);
                    time = 0.5f;
                }
            }else{
                time = 0.5f;
            }

            if (moveValue.x > 0)
            {
                //左向き
                transform.localScale = new Vector3(1, 1, 1);

            }
            else if (moveValue.x < 0)
            {
                //右向き
                transform.localScale = new Vector3(-1, 1, 1);

            }
            superPowerOneShot = true;
        }
       // animator.SetFloat("Speed", Mathf.Abs(moveValue.x));
        // ジャンプ処理
        if (jumpAction.WasPressedThisFrame()&&onGround)
        {
         //   animator.SetTrigger("Jump");
            SetAudio(SE.playerJumpLong);
            rb2D.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
        }

        // 地面にいるかどうかのチェック
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1f,groundLayer);
        Debug.DrawRay(transform.position, Vector2.down * 1f, Color.red);
        //Debug.Log(hit.collider);
        if (hit.collider != null)
        {
            onGround = true;
         //   animator.SetBool ("Onground", true);
            if (!onGround) {
                SetAudio(SE.playerGround);
            }
        }
        else
        {
            onGround = false;
          //  animator.SetBool("Onground", false);
        }
    }

    private void OnBlockPlaced(Vector2Int blockGridPos, Vector3 blockWorldPos)
    {
        if (GridChanager.Instance == null || dontMove)
        {
            return;
        }

        Vector2Int playerGridPos = GridChanager.Instance.GetGridPosition(transform.position);
        if (playerGridPos != blockGridPos)
        {
            return;
        }

        // マウスの移動方向に基づいて推奨される方向を決定
        Vector2 mouseMoveDir = MouseManager.GetMouseMoveDirection();
        Vector2Int preferredDirection = GetPreferredPushDirection(mouseMoveDir, blockWorldPos);

        // ブロック配置位置にプレイヤーがいる場合、プッシュ可能な方向を探す
        Vector2Int targetGridPos = ResolvePushTargetGrid(blockGridPos, preferredDirection);

        if (targetGridPos == blockGridPos)
        {
            // どの方向にも押し出せない場合はゲームオーバー
            PlayerDie();
            return;
        }

        // プレイヤーをターゲット位置に押す
        Vector3 targetWorld = GridChanager.Instance.GetWorldPosition(targetGridPos);
        targetWorld.z = transform.position.z;
        BeginPush(targetWorld);
        }

        /// <summary>
        /// プレイヤーの推奨プッシュ方向を決定します。
        /// マウス移動方向 → プレイヤーとブロックの相対位置 → 入力方向 の優先度で判定します。
        /// </summary>
    /// <param name="mouseMoveDirection">マウスの移動方向</param>
    /// <param name="blockWorldPos">ブロックのワールド座標</param>
    /// <returns>推奨される押し出し方向</returns>
    private Vector2Int GetPreferredPushDirection(Vector2 mouseMoveDirection, Vector3 blockWorldPos)
    {
        // マウスの移動方向から最適な方向を決定
        if (mouseMoveDirection.magnitude > 0.5f)
        {
            // マウスが左から右に移動 → 右へ押し出す
            if (Mathf.Abs(mouseMoveDirection.x) >= Mathf.Abs(mouseMoveDirection.y))
            {
                if (mouseMoveDirection.x > 0f) return Vector2Int.right;
                if (mouseMoveDirection.x < 0f) return Vector2Int.left;
            }
            // マウスが下から上に移動 → 上へ押し出す
            else
            {
                if (mouseMoveDirection.y > 0f) return Vector2Int.up;
                if (mouseMoveDirection.y < 0f) return Vector2Int.down;
            }
        }

        // マウスが移動していない場合、プレイヤーとブロックの相対位置から方向を判定
        Vector2 delta = (Vector2)(transform.position - blockWorldPos);

        if (Mathf.Abs(delta.x) >= Mathf.Abs(delta.y))
        {
            if (delta.x > 0f) return Vector2Int.right;
            if (delta.x < 0f) return Vector2Int.left;
        }
        else
        {
            if (delta.y > 0f) return Vector2Int.up;
            if (delta.y < 0f) return Vector2Int.down;
        }

        // プレイヤーの入力方向で判定
        if (moveValue.x > 0f) return Vector2Int.right;
        if (moveValue.x < 0f) return Vector2Int.left;

        // デフォルトは右方向
        return Vector2Int.right;
    }

    /// <summary>
    /// プッシュ先のグリッド位置を決定します。
    /// 推奨方向から順に候補位置をチェックし、最初に塞がっていない位置を返します。
    /// </summary>
    /// <param name="originGridPos">ブロックが配置されたグリッド位置</param>
    /// <param name="preferredDirection">推奨される押し出し方向</param>
    /// <returns>プッシュ先のグリッド位置</returns>
    private Vector2Int ResolvePushTargetGrid(Vector2Int originGridPos, Vector2Int preferredDirection)
    {
        // 推奨方向を最優先、その後は右→左→上→下の順で候補を試す
        Vector2Int[] directions = new[]
        {
            preferredDirection,
            Vector2Int.right,
            Vector2Int.left,
            Vector2Int.up,
            Vector2Int.down
        };

        for (int i = 0; i < directions.Length; i++)
        {
            if (directions[i] == Vector2Int.zero)
            {
                continue;
            }

            Vector2Int candidate = originGridPos + directions[i];
            Vector3 candidateWorld = GridChanager.Instance.GetWorldPosition(candidate);
            candidateWorld.z = transform.position.z;

            // 候補位置が塞がっていなければ、その方向に押し出す
            if (!IsBlocked(candidateWorld))
            {
                return candidate;
            }
        }

        return originGridPos;
    }

    /// <summary>
    /// 指定された世界座標が何かに塞がれているかをチェックします。
    /// プレイヤー自身とゴール位置は塞がっていないと判定します。
    /// </summary>
    /// <param name="worldPosition">チェックする世界座標</param>
    /// <returns>塞がっている場合true、空いている場合false</returns>
    private bool IsBlocked(Vector3 worldPosition)
    {
        // 指定座標の周辺にあるコライダーをすべて取得
        Collider2D[] hits = Physics2D.OverlapCircleAll(worldPosition, pushBlockCheckRadius, pushObstacleLayer);
        for (int i = 0; i < hits.Length; i++)
        {
            Collider2D hit = hits[i];
            if (hit == null)
            {
                continue;
            }

            // プレイヤー自身のコライダーは除外
            bool isPlayerSelf = false;
            if (_playerColliders != null)
            {
                for (int j = 0; j < _playerColliders.Length; j++)
                {
                    if (hit == _playerColliders[j])
                    {
                        isPlayerSelf = true;
                        break;
                    }
                }
            }

            if (isPlayerSelf)
            {
                continue;
            }

            // ゴール位置は塞がっていないと判定（空白扱い）
            if (hit.CompareTag("Goal"))
            {
                continue;
            }

            // IgnoreRayCastは当たり判定から除外
            if (hit.CompareTag("IgnoreRaycast"))
            {
                continue;
            }

            return true;
        }

        return false;
    }

    private void BeginPush(Vector3 targetWorld)
    {
        _isPushing = true;
        _pushElapsed = 0f;
        _pushStartPosition = transform.position;
        _pushTargetPosition = targetWorld;

        // 目標座標は即時に反映（判定座標を先に確定させて埋まりを防ぐ）
        transform.position = _pushTargetPosition;

        Debug.Log(transform.position);

        if (rb2D != null)
        {
            rb2D.linearVelocity = Vector2.zero;
        }

        if (visualTransform != null)
        {
            Vector3 worldOffset = _pushStartPosition - _pushTargetPosition;
            _visualPushStartOffset = new Vector3(worldOffset.x, worldOffset.y, 0f);
            visualTransform.localPosition = _visualBaseLocalPosition + _visualPushStartOffset;
        }
    }

    private void UpdatePushMotion()
    {
        _pushElapsed += Time.deltaTime;
        float t = pushMoveDuration <= 0f ? 1f : Mathf.Clamp01(_pushElapsed / pushMoveDuration);

        Debug.Log(gameObject.transform.position);
        if (visualTransform != null)
        {
            Vector3 from = _visualBaseLocalPosition + _visualPushStartOffset;
            Vector3 localPos = Vector3.Lerp(from, _visualBaseLocalPosition, t);
            localPos.y += Mathf.Sin(t * Mathf.PI) * pushLeapHeight;
            visualTransform.localPosition = localPos;
        }

        if (t >= 1f)
        {
            _isPushing = false;
            if (visualTransform != null)
            {
                visualTransform.localPosition = _visualBaseLocalPosition;
            }
        }
    }

    public void ReturnToInitialPosition()
    {
        _isPushing = false;
        transform.position = _initialPosition;

        if (rb2D != null)
        {
            rb2D.linearVelocity = Vector2.zero;
            rb2D.angularVelocity = 0f;
        }

        if (visualTransform != null)
        {
            visualTransform.localPosition = _visualBaseLocalPosition;
        }
    }

    public void PlayerDie()
    {
        if(dontMove)
            return;
        dontMove = true;
       // animator.SetBool("Die",true);
        SetAudio(SE.retry);
        //ゲーム終了処理
        StartCoroutine(GameEnd(1f,false));
    }
}