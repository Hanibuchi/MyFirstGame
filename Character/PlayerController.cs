using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem; // InputSystemの名前空間
using UnityEngine.EventSystems;
using System;


namespace MyGame
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(NPCManager))]
    public class PlayerController : MonoBehaviour
    {
        NPCManager npcManager;
        Rigidbody2D rb;
        public bool CanAttack = true; // アタックできるかを表す。使ってないが，例えばFireの返り値から次打てる時までfalseにすることで処理を軽くできたり，敵の攻撃でfalseにすることで攻撃ができなくさせたりできる。
        bool IsEquipmentMenuOpen => UIManager.Instance.EquipmentMenuManager.IsOpen; // 装備画面開いてるかどうか。trueならFireできない。装備画面を開いている間もFireできる設定なら，falseにならない。
        public bool autoFire = false;

        float rightInput;
        float leftInput;
        float upInput;
        float downInput;

        Vector2 currentVelocity;
        Vector2 nextVelocity;
        /// <summary>
        /// MaxSpeedに指数関数的に収束するときの底。1/Time.deltaTimeより大きいと発散する。
        /// </summary>
        [SerializeField] float acceleration = 0.1f;
        [SerializeField] float initMoveSpeed;
        public float InitMoveSpeed { get => initMoveSpeed; private set => initMoveSpeed = value; }
        [SerializeField] float maxMoveSpeed;
        public float MaxMoveSpeed { get => maxMoveSpeed; private set => maxMoveSpeed = value; }
        [SerializeField] float initJumpSpeed;
        public float InitJumpSpeed { get => initJumpSpeed; private set => initJumpSpeed = value; }
        [SerializeField] float maxJumpSpeed;
        public float MaxJumpSpeed { get => maxJumpSpeed; private set => maxJumpSpeed = value; }

        // [SerializeField] bool IsGrounded;

        Vector2 mousePosition;
        List<Item> items;

        // GameManagerでインスタンス化された.inputActionsでwasd, 矢印キーなどを観測する。子のコンポネントがEnableされたときにinputeActions.Player.Move.performedと.canceledが起きたときにOnMoveが実行されるよう登録され，Disableされたときにそれらの登録を取り消す。OnMoveでは移動キーの入力がmovementInputに入れられる。movementInputを使用して操作を定義する。

        void Awake()
        {
            Init();
        }

        public void Init()
        {
            RegisterInput(true);

            rb = GetComponent<Rigidbody2D>();
            npcManager = GetComponent<NPCManager>();

            SetMoveSpeed(npcManager.CurrentSpeed);
            SetJumpSpeed(npcManager.CurrentSpeed);
            SetSpeedChangeCallback(true);
        }

        public void SetMoveSpeed(float speed)
        {
            InitMoveSpeed = speed;
            MaxMoveSpeed = speed * 1.5f;
        }

        public void SetJumpSpeed(float speed)
        {
            InitJumpSpeed = speed;
            MaxJumpSpeed = speed * 1.5f;
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
                MyInputSystem.GameInputs.UI.EquipmentMenu.performed += OnOpenEquipmentMenu;
                MyInputSystem.GameInputs.Player.SelectItemUp.performed += OnScrollWheel;
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
                MyInputSystem.GameInputs.UI.EquipmentMenu.performed -= OnOpenEquipmentMenu;
                MyInputSystem.GameInputs.Player.SelectItemUp.performed -= OnScrollWheel;
            }
        }

        /// <summary>
        /// npcManagerのOnSpeedChangeにSetMoveSpeedとSetJumpSpeedを登録・登録解除する。正しく使用されているならば，isRegisterがtrueでもfalseでも実行されるコードの行数は同じで，このメソッドの呼び出される回数は偶数になるはず。
        /// </summary>
        /// <param name="isRegister">trueなら登録，falseなら登録解除</param>
        void SetSpeedChangeCallback(bool isRegister)
        {
            if (npcManager == null)
            {
                Debug.LogWarning("npcManager is null");
                return;
            }
            if (isRegister)
            {
                npcManager.OnCurrentSpeedChanged += SetMoveSpeed;
                npcManager.OnCurrentSpeedChanged += SetJumpSpeed;
            }
            else
            {
                npcManager.OnCurrentSpeedChanged -= SetMoveSpeed;
                npcManager.OnCurrentSpeedChanged -= SetJumpSpeed;
            }
        }

        void OnRightMove(InputAction.CallbackContext context)
        {
            if (npcManager.IsDead)
                return;
            rightInput = context.ReadValue<float>();
        }

        void OnLeftMove(InputAction.CallbackContext context)
        {
            if (npcManager.IsDead)
                return;
            leftInput = context.ReadValue<float>();
        }

        void OnJump(InputAction.CallbackContext context)
        {
            if (npcManager.IsDead)
                return;
            upInput = context.ReadValue<float>();
        }

        void OnDown(InputAction.CallbackContext context)
        {
            if (npcManager.IsDead)
                return;
            downInput = context.ReadValue<float>();
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
                Fire();
                autoFire = true;
            }
            else if (context.canceled)
                autoFire = false;
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

        public void OnOpenEquipmentMenu(InputAction.CallbackContext context)
        {
            ToggleEquipmentMenu();
        }

        public void OnScrollWheel(InputAction.CallbackContext context)
        {
            npcManager.AddSelectedSlotNumber((int)math.sign(context.ReadValue<Vector2>().y));
        }

        // void OnShowValue(InputAction.CallbackContext context)
        // {
        //     Debug.Log($"Value: {context.ReadValue<Vector2>()}");
        // }

        void Update()
        {
            // if (MyInputSystem.GameInputs.Player.Move.IsPressed())
            Move();

            if (autoFire)
                Fire();
        }

        // 移動。左右の速度の大きさが初速未満なら初速に，それ以上かつ最大速度未満なら最大速度へ漸近，最大速度以上なら操作できないようにする。
        void Move()
        {
            currentVelocity = rb.velocity;
            nextVelocity = currentVelocity;

            HorizontalMove();
            VerticalMove();//
            rb.velocity = nextVelocity;
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
            var horizontalInput = rightInput - leftInput;
            nextVelocity.x = math.lerp(currentVelocity.x, MaxMoveSpeed * horizontalInput, acceleration);
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
            var verticalInput = upInput - downInput;
            if (verticalInput > 0)
                nextVelocity.y = math.lerp(currentVelocity.y, MaxJumpSpeed * verticalInput, acceleration);
        }

        void Fire()
        {
            if (!CanAttack || IsEquipmentMenuOpen)
                return;

            mousePosition = GameManager.Utility.GetMousePos();
            items = (List<Item>)npcManager.Items;
            if (items != null && items.Count > 0)
            {
                npcManager.SetSelectedSlotNumber(0);
                npcManager.Fire(mousePosition);
                // transform.GetChild(0).GetComponent<Animator>().SetTrigger("Attack");
            }
            else
            {
                // Debug.LogWarning("npcManager.Items is null or empty");
            }
        }

        public void Throw()
        {
            mousePosition = GameManager.Utility.GetMousePos();
            npcManager.ThrowItem(mousePosition);
        }

        /// <summary>
        /// アイテムをピックアップする
        /// </summary>
        public void PickUpItem()
        {
            npcManager.PickupItem();
        }

        /// <summary>
        /// NPCと会話する
        /// </summary>
        public void InteractNPC()
        {
            npcManager.InteractNPC();
        }

        public void ToggleEquipmentMenu()
        {
            UIManager.Instance.EquipmentMenuManager.ToggleEquipmentMenu();

            if (IsEquipmentMenuOpen) // 開けるとき
            {
                if (true) // もし設定で編集中のFireがoffなら
                    autoFire = false;

                DragSystem.Instance.OnOpenEquipmentMenu();
            }
            else // 閉じるとき
            {
                DragSystem.Instance.OnCloseEquipmentMenu();
            }
        }
    }
}
