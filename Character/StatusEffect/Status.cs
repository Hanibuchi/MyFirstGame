using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

[Serializable]
public class Status
{
    [SerializeField] StatusID iD;
    public StatusID ID { get => iD; protected set => iD = value; }
    public virtual StatusType Type => StatusType.Default;
    public Action OnExpire { get; private set; }

    [NonSerialized]
    List<Status> StatusList;

    /// <summary>
    /// Statusでは返り値で追加できたか判断できるが，DurationStatusやWetStatusでは返り値がIEnumeratorのため追加できか判別できない。したがって判別したいときはこのメソッドを使う。
    /// </summary>
    /// <param name="statusList"></param>
    /// <returns></returns>
    protected virtual bool CanAdd(List<Status> statusList)
    {
        return true;
    }

    /// <summary>
    /// Statusを追加する際必ず実行しなければならない。1度だけ実行する
    /// </summary>
    /// <param name="statusList"></param>
    /// <returns></returns>
    public virtual bool Add(StatusID id, IStatusAffectable statusAffectable)
    {
        var statusList = statusAffectable.StatusList;
        if (!CanAdd(statusList))
            return false;

        ID = id;
        StatusList = statusList;
        StatusList.Add(this);

        return true;
    }

    /// <summary>
    /// このStatusを消すとき呼び出すメソッド。リストからの削除，Actionからの削除等このインスタンスの参照を消す操作はここで行う。これが実行された後はこのインスタンスはどこからも参照されていない状態になる。任意のタイミングで実行できる。
    /// </summary>
    public virtual void Expire()
    {
        StatusList.Remove(this);
        OnExpire?.Invoke();
    }

    public void RegisterExpireAction(Action action)
    {
        OnExpire += action; // 引数で渡されたメソッドをOnExpireに追加
    }

    public StatusData MakeData()
    {
        return FillData(new StatusData());
    }

    protected virtual StatusData FillData(StatusData data)
    {
        data.ID = ID;
        data.Type = Type;
        return data;
    }

    public virtual Status ApplyData(StatusData data)
    {
        ID = data.ID;
        return this;
    }

    public enum StatusID
    {
        PowerBoost,
    }

    public enum StatusType
    {
        Default,
        Wet,
        Duration,
    }
}

