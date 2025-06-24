using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem; // InputSystemの名前空間
using UnityEngine.EventSystems;
using System;


[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(NPCManager))]
public class PlayerController : MonoBehaviour
{
    NPCManager m_npcManager;
    DeathHandler m_deathHandler;
    Rigidbody2D m_rb;
    public bool CanAttack => !IsEquipmentMenuOpen && !DragSystem.Instance.IsDragging; // アタックできるかを表す。使ってないが，例えばFireの返り値から次打てる時までfalseにすることで処理を軽くできたり，敵の攻撃でfalseにすることで攻撃ができなくさせたりできる。
    bool IsEquipmentMenuOpen => UIManager.Instance.GetEquipmentUI().IsOpen; // 装備画面開いてるかどうか。trueならFireできない。装備画面を開いている間もFireできる設定なら，falseにならない。
    public bool m_autoFire = false;

    float m_rightInput;
    float m_leftInput;
    float m_upInput;
    float m_downInput;

    Vector2 m_currentVelocity;
    Vector2 m_nextVelocity;
    /// <summary>
    /// MaxSpeedに指数関数的に収束するときの底。1/Time.deltaTimeより大きいと発散する。
    /// </summary>
    [SerializeField] float m_acceleration = 0.1f;
    [SerializeField] float m_initMoveSpeed;
    [SerializeField] float m_maxMoveSpeed;
    [SerializeField] float m_initJumpSpeed;
    [SerializeField] float m_maxJumpSpeed;

    // [SerializeField] bool IsGrounded;

    Vector2 m_mousePosition;
    List<Item> m_items;

    // GameManagerでインスタンス化された.inputActionsでwasd, 矢印キーなどを観測する。子のコンポネントがEnableされたときにinputeActions.Player.Move.performedと.canceledが起きたときにOnMoveが実行されるよう登録され，Disableされたときにそれらの登録を取り消す。OnMoveでは移動キーの入力がmovementInputに入れられる。movementInputを使用して操作を定義する。
    SpeedHandler m_speedHandler;
    void Awake()
    {
        Init();
    }

    public void Init()
    {
        RegisterInput(true);

        m_rb = GetComponent<Rigidbody2D>();
        m_npcManager = GetComponent<NPCManager>();
        m_deathHandler = GetComponent<DeathHandler>();

        if (TryGetComponent(out m_speedHandler))
        {
            SetMoveSpeed(m_speedHandler.Speed);
            SetJumpSpeed(m_speedHandler.Speed);
            SetSpeedChangeCallback(true);
        }
    }

    public void SetMoveSpeed(float speed)
    {
        m_initMoveSpeed = speed;
        m_maxMoveSpeed = speed * 1.5f;
    }

    public void SetJumpSpeed(float speed)
    {
        m_initJumpSpeed = speed;
        m_maxJumpSpeed = speed * 1.5f;
    }

    private void OnDestroy()
    {
        RegisterInput(false);

        SetSpeedChangeCallback(false);
    }

    int semafo;
    void RegisterInput(bool doRegist)
    {
        if (doRegist)
        {
            if (semafo >= 1)
                return;
            semafo++;
            // Moveアクションのイベントにコールバックを登録
            MyInputSystem.GameInputs.Player.Right.performed += OnRightMove;
            MyInputSystem.GameInputs.Player.Right.canceled += OnRightMove;

            MyInputSystem.GameInputs.Player.Left.performed += OnLeftMove;
            MyInputSystem.GameInputs.Player.Left.canceled += OnLeftMove;

            MyInputSystem.GameInputs.Player.Jump.performed += OnJump;
            MyInputSystem.GameInputs.Player.Jump.canceled += OnJump;

            MyInputSystem.GameInputs.Player.Down.performed += OnDown;
            MyInputSystem.GameInputs.Player.Down.canceled += OnDown;

            MyInputSystem.GameInputs.Player.Fire.performed += OnLeftClick;
            MyInputSystem.GameInputs.Player.Fire.canceled += OnLeftClick;
            MyInputSystem.GameInputs.Player.Throw.performed += OnRightClick;
            MyInputSystem.GameInputs.Player.Interact.performed += OnInteract;
            MyInputSystem.GameInputs.Player.SelectItemUp.performed += OnScrollWheel;

            OnEquipmentMenuOpenEventHandler += OnEquipmentMenuOpen;
        }
        else
        {
            semafo = Math.Max(0, semafo - 1);
            // Moveアクションのイベントへのコールバックの登録を取り消す
            MyInputSystem.GameInputs.Player.Right.performed -= OnRightMove;
            MyInputSystem.GameInputs.Player.Right.canceled -= OnRightMove;

            MyInputSystem.GameInputs.Player.Left.performed -= OnLeftMove;
            MyInputSystem.GameInputs.Player.Left.canceled -= OnLeftMove;

            MyInputSystem.GameInputs.Player.Jump.performed -= OnJump;
            MyInputSystem.GameInputs.Player.Jump.canceled -= OnJump;

            MyInputSystem.GameInputs.Player.Down.performed -= OnDown;
            MyInputSystem.GameInputs.Player.Down.canceled -= OnDown;

            MyInputSystem.GameInputs.Player.Fire.performed -= OnLeftClick;
            MyInputSystem.GameInputs.Player.Fire.canceled -= OnLeftClick;
            MyInputSystem.GameInputs.Player.Throw.performed -= OnRightClick;
            MyInputSystem.GameInputs.Player.Interact.performed -= OnInteract;
            MyInputSystem.GameInputs.Player.SelectItemUp.performed -= OnScrollWheel;

            OnEquipmentMenuOpenEventHandler -= OnEquipmentMenuOpen;
        }
    }

    /// <summary>
    /// npcManagerのOnSpeedChangeにSetMoveSpeedとSetJumpSpeedを登録・登録解除する。正しく使用されているならば，isRegisterがtrueでもfalseでも実行されるコードの行数は同じで，このメソッドの呼び出される回数は偶数になるはず。
    /// </summary>
    /// <param name="isRegister">trueなら登録，falseなら登録解除</param>
    void SetSpeedChangeCallback(bool isRegister)
    {
        if (m_speedHandler == null)
        {
            Debug.Log("m_speedHandler is null");
            return;
        }
        if (isRegister)
        {
            m_speedHandler.OnSpeedChanged += SetMoveSpeed;
            m_speedHandler.OnSpeedChanged += SetJumpSpeed;
        }
        else
        {
            m_speedHandler.OnSpeedChanged -= SetMoveSpeed;
            m_speedHandler.OnSpeedChanged -= SetJumpSpeed;
        }
    }

    void OnRightMove(InputAction.CallbackContext context)
    {
        if (m_deathHandler.IsDead)
            return;
        m_rightInput = context.ReadValue<float>();
    }

    void OnLeftMove(InputAction.CallbackContext context)
    {
        if (m_deathHandler.IsDead)
            return;
        m_leftInput = context.ReadValue<float>();
    }

    void OnJump(InputAction.CallbackContext context)
    {
        if (m_deathHandler.IsDead)
            return;
        m_upInput = context.ReadValue<float>();
    }

    void OnDown(InputAction.CallbackContext context)
    {
        if (m_deathHandler.IsDead)
            return;
        m_downInput = context.ReadValue<float>();
    }

    // void OnStop(InputAction.CallbackContext context)
    // {
    //     currentVelocity = rb.velocity;
    //     if (math.abs(currentVelocity.x) < MaxMoveSpeed)
    //         rb.velocity = new Vector2(0, currentVelocity.y);
    // }

    public void OnLeftClick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Fire(); // 応答性に差が出るらしいからここでもFire()する。（というか普通にこれないとクリックしただけじゃFire()されない可能性がある。
            m_autoFire = true;
        }
        else if (context.canceled)
            m_autoFire = false;
    }

    public void OnRightClick(InputAction.CallbackContext context)
    {
        Throw();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        PickUpItem();
        InteractNPC();
    }

    public void OnScrollWheel(InputAction.CallbackContext context)
    {
    }

    // void OnShowValue(InputAction.CallbackContext context)
    // {
    //     Debug.Log($"Value: {context.ReadValue<Vector2>()}");
    // }

    void Update()
    {
        // if (MyInputSystem.GameInputs.Player.Move.IsPressed())
        Move();

        if (m_autoFire)
            Fire();
    }

    // 移動。左右の速度の大きさが初速未満なら初速に，それ以上かつ最大速度未満なら最大速度へ漸近，最大速度以上なら操作できないようにする。
    void Move()
    {
        m_currentVelocity = m_rb.velocity;
        m_nextVelocity = m_currentVelocity;

        HorizontalMove();
        VerticalMove();//
        m_rb.velocity = m_nextVelocity;
    }

    // void HorizontalMove()
    // {
    //     var horizontalInput = rightInput - leftInput;

    //     if (math.abs(currentVelocity.x) < InitMoveSpeed)
    //         nextVelocity.x = InitMoveSpeed * horizontalInput;
    //     else if (math.abs(currentVelocity.x) < MaxMoveSpeed)
    //     {
    //         nextVelocity.x = math.lerp(currentVelocity.x, MaxMoveSpeed * horizontalInput, acceleration * Time.deltaTime);
    //     }

    //     // キャラクターの向きを変更。アニメーションで実装したい。
    //     // if (horizontalInput > 0)
    //     // {
    //     //     transform.localScale = new Vector3(math.abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    //     //     // transform.localScale = new Vector3(1, 1, 1);
    //     // }
    //     // else if (horizontalInput < 0)
    //     // {
    //     //     transform.localScale = new Vector3(-math.abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    //     //     transform.localScale = new Vector3(-1, 1, 1);
    //     // }
    // }

    void HorizontalMove()
    {
        var horizontalInput = m_rightInput - m_leftInput;
        m_nextVelocity.x = math.lerp(m_currentVelocity.x, m_maxMoveSpeed * horizontalInput, m_acceleration);
    }

    // void HorizontalMove3()
    // {
    //     var horizontalInput = rightInput - leftInput;
    //     nextVelocity.x = MaxMoveSpeed * horizontalInput;
    // }

    // void VerticalMove()
    // {
    //     var verticalInput = upInput - downInput;

    //     if (verticalInput > 0)
    //     {
    //         if (currentVelocity.y < InitJumpSpeed)
    //             nextVelocity.y = InitJumpSpeed;
    //         else if (currentVelocity.y < MaxJumpSpeed)
    //             nextVelocity.y = math.lerp(currentVelocity.y, MaxJumpSpeed, acceleration * Time.deltaTime);
    //     }
    // }


    void VerticalMove()
    {
        var verticalInput = m_upInput - m_downInput;
        if (verticalInput > 0)
            m_nextVelocity.y = math.lerp(m_currentVelocity.y, m_maxJumpSpeed * verticalInput, m_acceleration);
    }

    void Fire()
    {
        if (!CanAttack)
            return;

        m_mousePosition = GameManager.Utility.GetMousePos();
        // m_items = (List<Item>)m_npcManager.GetComp;
        if (m_items != null && m_items.Count > 0)
        {
            // m_npcManager.SetSelectedSlotNumber(0);
            // m_npcManager.Fire(m_mousePosition);
            // transform.GetChild(0).GetComponent<Animator>().SetTrigger("Attack");
        }
        else
        {
            // Debug.LogWarning("npcManager.Items is null or empty");
        }
    }

    public void Throw()
    {
        m_mousePosition = GameManager.Utility.GetMousePos();
        // m_npcManager.ThrowItem(m_mousePosition);
    }

    /// <summary>
    /// アイテムをピックアップする
    /// </summary>
    public void PickUpItem()
    {
        if (TryGetComponent(out ItemUser itemUser))
            itemUser.PickupItem();
    }

    /// <summary>
    /// NPCと会話する
    /// </summary>
    public void InteractNPC()
    {
        m_npcManager.InteractNPC();
    }

    public static Action OnEquipmentMenuOpenEventHandler;
    public void OnEquipmentMenuOpen()
    {
        if (true) // もし設定で編集中のFireがoffなら
            m_autoFire = false;
    }
}

