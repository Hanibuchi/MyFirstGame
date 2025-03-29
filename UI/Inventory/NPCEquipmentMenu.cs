using System;
using System.Collections;
using System.Collections.Generic;
using MyGame;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

namespace MyGame
{
    public class NPCEquipmentMenu : UI, INPCStatusUI
    {
        NPCManager npcManager; // 後々MobManagerにしたいが，アイテムの処理などがだるく時間がかかる
        [SerializeField] protected Image npcImage;
        [SerializeField] protected TextMeshProUGUI npcName;
        [SerializeField] protected TextMeshProUGUI npcLevel;
        [SerializeField] protected TextMeshProUGUI npcJob;
        [SerializeField] protected TextMeshProUGUI hpTMP;
        [SerializeField] protected Slider hpSlider;
        [SerializeField] protected TextMeshProUGUI mpTMP;
        [SerializeField] protected Slider mpSlider;
        [SerializeField] protected GameObject equipmentSlotsFrame;
        public List<EquipmentSlot> equipmentSlots;

        /// <summary>
        /// NPCEquipmentMenuを初期化し，ステータスが表示されるよう登録する。
        /// </summary>
        /// <param name="equipmentMenu"></param>
        public void RegisterStatus(NPCManager newNPCManager)
        {
            npcManager = newNPCManager;

            GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1); // なぜかscaleが(0,0)になるため対症療法的にこうしてる。

            GetStatusDisplayComponents();

            UpdateNPCImage(newNPCManager.NPCImage);
            UpdateNPCName(newNPCManager.gameObject.name);
            UpdateNPCLevel(newNPCManager.CurrentLevel);
            UpdateNPCJob(newNPCManager.Job);
            UpdateCurrentHP(newNPCManager.CurrentHP);
            UpdateMaxtHP(newNPCManager.CurrentMaxHP);
            UpdateCurrentMP(newNPCManager.CurrentMP);
            UpdateMaxtMP(newNPCManager.CurrentMaxMP);

            newNPCManager.OnNPCImageChanged += UpdateNPCImage;
            newNPCManager.OnCurrentLevelChanged += UpdateNPCLevel;
            newNPCManager.OnJobChanged += UpdateNPCJob;
            newNPCManager.OnCurrentHPChanged += UpdateCurrentHP;
            newNPCManager.OnCurrentMaxHPChanged += UpdateMaxtHP;
            newNPCManager.OnCurrentMPChanged += UpdateCurrentMP;
            newNPCManager.OnCurrentMaxMPChanged += UpdateMaxtMP;

            name = npcManager.gameObject.name + "EquipMentMenu";
        }

        /// <summary>
        /// ステータスを入力するコンポネントを取得 // なおGUIで設定できるため必要なかった。
        /// </summary>
        protected void GetStatusDisplayComponents()
        {
            // GameObject UpperSection = transform.GetChild(0).gameObject;
            // npcImage = UpperSection.transform.GetChild(0).GetChild(0).GetComponent<Image>();
            // GameObject statusFrame = UpperSection.transform.GetChild(1).gameObject;
            // GameObject basicStatus = statusFrame.transform.GetChild(0).gameObject;
            // npcName = basicStatus.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
            // npcLevel = basicStatus.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();
            // npcJob = basicStatus.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>();
            // GameObject hpFrame = statusFrame.transform.GetChild(1).gameObject;
            // hpSlider = hpFrame.transform.GetChild(0).gameObject.GetComponent<Slider>();
            // hpTMP = hpFrame.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();
            // GameObject mpFrame = statusFrame.transform.GetChild(2).gameObject;
            // mpSlider = mpFrame.transform.GetChild(0).gameObject.GetComponent<Slider>();
            // mpTMP = mpFrame.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();

            // EquipmentSlotたちのOnItemChangedにNPCManagerから装備アイテムを変更するメソッドを追加する
            equipmentSlotsFrame = transform.GetChild(1).GetChild(0).GetChild(0).gameObject;
            if (equipmentSlotsFrame != null)
            {
                equipmentSlots = new List<EquipmentSlot>(equipmentSlotsFrame.GetComponentsInChildren<EquipmentSlot>());
                if (equipmentSlots != null)
                    for (int i = 0; i < equipmentSlots.Count; i++)
                    {
                        equipmentSlots[i].npcManager = npcManager;
                        equipmentSlots[i].id = i;
                    }
            }
            else
                Debug.LogWarning("equipmentSlotsFrame moved");
        }

        void OnDestroy()
        {
            // アイテムのUIも削除する。
            UnregisterStatus();
        }

        public override void OnRelease()
        {
            base.OnRelease();
            UnregisterStatus();
            UIManager.Instance.EquipmentMenuManager.RemoveMember(this);
        }

        /// <summary>
        /// 登録を解除する。
        /// </summary>
        /// <param name="equipmentMenu"></param>
        public void UnregisterStatus()
        {
            if (npcManager == null)
                return;
            npcManager.OnNPCImageChanged -= UpdateNPCImage;
            npcManager.OnCurrentLevelChanged -= UpdateNPCLevel;
            npcManager.OnJobChanged -= UpdateNPCJob;
            npcManager.OnCurrentHPChanged -= UpdateCurrentHP;
            npcManager.OnCurrentMaxHPChanged -= UpdateMaxtHP;
            npcManager.OnCurrentMPChanged -= UpdateCurrentMP;
            npcManager.OnCurrentMaxMPChanged -= UpdateMaxtMP;
        }

        public void UpdateNPCImage(Sprite newNPCImage)
        {
            if (npcImage != null)
            {
                npcImage.sprite = newNPCImage;
            }
            else
            {
                Debug.LogWarning("Init() might be not done");
            }
        }

        public void UpdateNPCName(string newNPCName)
        {
            if (npcName != null)
            {
                npcName.text = newNPCName;
            }
            else
            {
                Debug.LogWarning("Init() might be not done");
            }
        }

        public void UpdateNPCLevel(ulong newLevel)
        {
            if (npcLevel != null)
            {
                npcLevel.text = "Lv. " + newLevel.ToString("F0");
            }
            else
            {
                Debug.LogWarning("Init() might be not done");
            }
        }

        public void UpdateNPCJob(Jobs newNPCJob)
        {
            if (npcJob != null)
            {
                npcJob.text = newNPCJob.ToString();
            }
            else
            {
                Debug.LogWarning("Init() might be not done");
            }
        }

        public void UpdateCurrentHP(float currentHP)
        {
            if (hpSlider != null && mpSlider != null)
            {
                hpSlider.value = currentHP;
                hpTMP.text = $"HP {currentHP:F0}";
            }
            else
            {
                Debug.LogWarning("Init() might be not done");
            }
        }
        public void UpdateMaxtHP(float maxHP)
        {
            if (hpSlider != null && mpSlider != null)
            {
                hpSlider.maxValue = maxHP;
            }
            else
            {
                Debug.LogWarning("Init() might be not done");
            }
        }

        public void UpdateCurrentMP(float currentMP)
        {
            if (mpSlider != null && mpSlider != null)
            {
                mpSlider.value = currentMP;
                mpTMP.text = $"MP {currentMP:F0}";
            }
            else
            {
                Debug.LogWarning("Init() might be not done");
            }
        }
        public void UpdateMaxtMP(float maxMP)
        {
            if (mpSlider != null && mpSlider != null)
            {
                mpSlider.maxValue = maxMP;
            }
            else
            {
                Debug.LogWarning("Init() might be not done");
            }
        }
    }
}