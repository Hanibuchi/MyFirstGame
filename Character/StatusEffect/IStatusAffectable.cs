using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public interface IStatusAffectable
{
    public List<Status> StatusList { get; }

    /// <summary>
    /// 動くとInvoke(Velocity.magnitude)されるようにする
    /// </summary>
    public Action<float> MoveAction { get; set; }

    public void SetMoveAction(Action<float> action)
    {
        MoveAction = action;
    }

    public Status AddStatus(StatusData data)
    {
        return AddStatus(data.ID, data.Type, data.Remaining);
    }
    public Status AddStatus(Status.StatusID id)
    {
        return AddStatus(id, Status.StatusType.Default, 0);
    }
    /// <summary>
    /// ステータスを追加。追加できなかったらnullを返す。
    /// </summary>
    /// <param name="id"></param>
    /// <param name="type"></param>
    /// <param name="remaining"></param>
    /// <returns></returns>
    public Status AddStatus(Status.StatusID id, Status.StatusType type, float remaining)
    {
        if (!CanAddStatus(id, type, remaining)) return null;

        Status status = type switch
        {
            Status.StatusType.Wet => AddWetStatus(id, remaining),
            Status.StatusType.Duration => AddDurationStatus(id, remaining),
            _ => AddStatusDefault(id),
        };
        switch (id)
        {
            case Status.StatusID.PowerBoost:
                OnStatusPowerBoost(status);
                break;
        }
        return status;
    }

    Status AddStatusDefault(Status.StatusID id)
    {
        var status = new Status();
        status.Add(id, this);
        return status;
    }

    DurationStatus AddDurationStatus(Status.StatusID id, float duration)
    {
        if (this is MonoBehaviour monoBehaviour)
        {
            var status = new DurationStatus();
            monoBehaviour.StartCoroutine(status.Add(id, duration, this));
            return status;
        }
        return null;
    }

    WetStatus AddWetStatus(Status.StatusID id, float wetLevel)
    {
        var status = new WetStatus();
        // Debug.Log("WetStatus was OnAdded");
        status.Add(id, wetLevel, this);
        return status;
    }

    /// <summary>
    /// 追加できるかどうかを返す。追加せず時間を延長するとかの処理もここでする。
    /// </summary>
    /// <param name="id"></param>
    /// <param name="type"></param>
    /// <param name="remaining"></param>
    /// <returns></returns>
    public bool CanAddStatus(Status.StatusID id, Status.StatusType type, float remaining)
    {
        bool canAdd = true;
        switch (id)
        {
            case Status.StatusID.PowerBoost:
                if (type == Status.StatusType.Duration)
                {
                    // 3つ以上だと追加せずに一番Durationが小さいもののDurationをremainingにする。
                    if (GetStatusCount(id, type) >= 3)
                    {
                        DurationStatus minDurationStatus = (DurationStatus)GetStatusList(id, type).Aggregate((min, next) => ((DurationStatus)next).Duration < ((DurationStatus)min).Duration ? next : min);
                        minDurationStatus.SetDuration(remaining);
                        canAdd = false;
                    }
                }
                else
                if (type == Status.StatusType.Wet)
                {
                    // 1つ以上だと追加せずにすでにあるもののWetLevelをremainingにする。
                    if (GetStatusCount(id, type) >= 1)
                    {
                        WetStatus wetStatus = (WetStatus)GetStatusList(id, type).First();
                        wetStatus.SetWetLevel(remaining);
                        canAdd = false;
                    }
                }
                else
                if (type == Status.StatusType.Default)
                {
                    // 1つ以上だと追加しない
                    if (GetStatusCount(id, type) >= 1)
                    {
                        canAdd = false;
                    }
                }
                break;

            default: break;
        }

        return canAdd;
    }

    public int GetStatusCount(Status.StatusID id, Status.StatusType type)
    {
        return StatusList.Count(status => status.ID == id && status.Type == type);
    }
    public List<Status> GetStatusList(Status.StatusID id, Status.StatusType type)
    {
        return StatusList.FindAll(status => status.ID == id && status.Type == type);
    }

    ////////////////////////////////////////////////////////////
    /// 以下に個々のステータス実行時に行う処理を記述する
    public void OnStatusPowerBoost(Status status);
}
