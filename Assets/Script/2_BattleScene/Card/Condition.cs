namespace TouhouMachineLearningSummary.GameEnum
{
    /// <summary>
    /// 用于判断该卡牌效果片段是否触发的条件
    /// </summary>
    public enum Condition
    {
        /// <summary>
        /// 默认效果触发条件（位于战场，自身未死亡，且未被封印时触发）
        /// </summary>
        Default,
        OnBattle,
        OnHand,
        OnDeck,
        OnUse,
        OnGrave,
        /// <summary>
        /// 位于当前回合所属方区域
        /// </summary>
        OnMyRegion,
        /// <summary>
        /// 位于当前回合所属方对方区域
        /// </summary>
        OnOpRegion,
        /// <summary>
        /// 封印
        /// </summary>
        Seal,
        /// <summary>
        /// 未被封印
        /// </summary>
        NotSeal,
        /// <summary>
        /// 死亡
        /// </summary>
        Dead,
        /// <summary>
        /// 未死亡
        /// </summary>
        NotDead
    }
}
