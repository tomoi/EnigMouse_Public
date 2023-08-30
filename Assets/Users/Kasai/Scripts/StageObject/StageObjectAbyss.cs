using Cysharp.Threading.Tasks;
using UnityEngine;
using UniRx;

public class StageObjectAbyss : BaseStageObject
{
    /// <summary>
    /// 移動できるようにする場合はtrueに変更
    /// </summary>
    private bool _isPassable = false;
    private int _PassableStartTurn = 0;
    public StageObjectAbyss(Vector2 position, int stageCreateAnimationIndex) : base(position, stageCreateAnimationIndex)
    {

    }

    public override bool HitMagic(MagicType type, Vector2 direction, out StageObjectType stageObjectType)
    {
        if (MagicType.Wind == type)
        {
            //通れるターンをセット
            _PassableStartTurn = GameManager.Instance.Turn;
            //1ターンのみ通行可能に変更
            _isPassable = true;
            
            var position = transform.position;
            position.y += 1f;

            //エフェクトを再生
            //ここでは適当な値StageObject_Noneを使用
            //第二引数、第三引数に座標と回転を入れる
            //回転には基本的にPrefab自身の回転情報を使うQuaternion.identityを入れる
            //もし指定の方向に回転したいときのみQuaternion.Euler()などで計算して使用する
            EffectManager.Instance.PlayEffect(EffectType.StageObject_None, position, Quaternion.identity);
        }
        stageObjectType = StageObjectType.Abyss;
        return false;
    }

    /// <summary>
    /// ネズミが移動可能なマスか判定して返す
    /// </summary>
    /// <returns></returns>
    public override bool isValidMove()
    {
        return _isPassable;
    }
    /// <summary>
    /// ネズミが今移動したら死亡するか判定して返す
    /// </summary>
    /// <returns></returns>
    public override bool isMovedDeath()
    {
        return !_isPassable;
    }

    public override async UniTask EndTurn()
    {
        //ターン終了時にターンが_PassableStartTurnを越していたら通れなくする
        if (_PassableStartTurn < GameManager.Instance.Turn)
        {
            _isPassable = false;
        }
        await UniTask.Yield();
    }
    public override async UniTask MoveToCell()
    {
        if (!_isPassable)
        {
            await Mouse.Instance.Death();
        }
    }
}