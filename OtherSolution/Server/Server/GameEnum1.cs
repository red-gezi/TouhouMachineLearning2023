////public enum AgainstModeType
////{
////    Story,//故事模式
////    Practice,//练习模式

////    Casual,//休闲模式
////    Rank,//天梯模式
////    Arena,//竞技场模式
////}
//public enum NetAcyncType
//{
//    Init,
//    FocusCard,
//    PlayCard,
//    SelectRegion,
//    SelectUnites,
//    SelectLocation,
//    SelectProperty,
//    SelectBoardCard,
//    ExchangeCard,
//    RoundStartExchangeOver,
//    Pass,
//    Surrender
//}
////需要在客户端同步更新
//public enum UpdateType
//{
//    Name,
//    UnlockTitles,
//    PrefixTitle,
//    SuffixTitle,
//    Decks,
//    UseDeckNum,
//    Stage,
//    LastLoginTime,
//}
//public enum PlayerOperationType
//{
//    PlayCard,//出牌
//    DisCard,//弃牌
//    Pass,//过牌
//}
//public enum SelectOperationType
//{
//    SelectProperty,//选择属性
//    SelectUnite,//选择单位
//    SelectRegion,//选择对战区域
//    SelectLocation,//选择位置坐标
//    SelectBoardCard,//从面板中选择卡牌
//    SelectExchangeOver,//选择换牌完毕
//}
//public enum CardState
//{
//    None,//默认空状态
//    Seal,//封印 清楚所有附加值和附加状态，并使能力无法生效
//    Invisibility,//隐身 无法作为主动选择目标
//    Pry,//窥探 强制卡牌正面，持续一回合
//    Cover,//覆盖 强制显示卡牌背面，满足条件后翻转
//    /// <summary>
//    /// 禁锢 无法打出
//    /// </summary>
//    Close,//禁锢 无法打出
//    Fate,//命运 位于自动选择目标列表中时会被优先选择
//    Lurk,//潜伏（间谍）部署至对方场上
//    Furor,//狂暴 
//    Docile,//温顺
//    Poisoning,//中毒 回合结束时给与自身1点伤害
//    Rely,//凭依 替换目标卡牌某种类型效果
//    Water,//水
//    Fire,//火
//    Wind,//风
//    Soil,//土
//    Hold, //驻守 小局结束时不进入墓地并取消此状态
//    Congealbounds,//结界 抵消一次伤害
//    Forbidden,//禁足 无法被移动
//    Apothanasia,//延命 点数为0时增益自身一点
//    Black,//黑
//    White,//白
//}
///// <summary>
///// 卡牌附加值类型     ！！！！不要改变顺序，有新的再后面追加，服务端和翻译表格需要同步更新！！！！
///// </summary>
//public enum CardField
//{
//    None,//默认空状态
//    Timer,//计时
//    Inspire,//鼓舞
//    Chain,//连锁
//    Energy,//能量
//    Shield,//护盾 抵消等量伤害
//    Pary,//祈祷 
//}
//public enum VariationType
//{
//    None,
//    Reverse,//逆转
//}
//public enum Camp
//{
//    Neutral,
//    Taoism,
//    Shintoism,
//    Buddhism,
//    Technology
//}