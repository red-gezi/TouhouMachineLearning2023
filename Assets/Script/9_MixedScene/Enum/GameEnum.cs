namespace TouhouMachineLearningSummary.GameEnum
{
    public enum FirstTurn { PlayerFirst, OpponentFirst, Random }
    public enum AgainstModeType
    {
        Story,//故事模式
        Practice,//练习模式

        Casual,//休闲模式
        Rank,//天梯模式
        Arena,//竞技场模式
    }
    public enum NotifyBoardMode
    {
        Ok,
        Ok_Cancel,
        Cancel,
        Input
    }
    public enum PageType
    {
        CardList,
        Image,
        Text
    }
    public enum TriggerTime
    {
        Before,
        When,
        After
    }
    public enum TriggerType
    {
        ////////////////////////////////////////////////移动/////////////////////////////////////////
        /// <summary>
        /// 生成
        /// </summary>
        Generate,
        /// <summary>
        /// 抽取
        /// </summary>
        Draw,
        /// <summary>
        /// 打出
        /// </summary>
        Play,
        /// <summary>
        /// 回手
        /// </summary>
        Reback,
        /// <summary>
        /// 部署
        /// </summary>
        Deploy,
        /// <summary>
        /// 丢弃
        /// </summary>
        Discard,
        /// <summary>
        /// 死亡
        /// </summary>
        Dead,
        /// <summary>
        /// 复活
        /// </summary>
        Revive,
        /// <summary>
        /// 位移
        /// </summary>
        Move,
        /// <summary>
        /// 间隙
        /// </summary>
        Banish,
        /// <summary>
        /// 召唤
        /// </summary>
        Summon,
        ////////////////////////////////////////////////点数/////////////////////////////////////////
        /// <summary>
        /// 置值
        /// </summary>
        Set,
        /// <summary>
        /// 增益
        /// </summary>
        Gain,
        /// <summary>
        /// 伤害
        /// </summary>
        Hurt,
        /// <summary>
        /// 治愈
        /// </summary>
        Cure,
        /// <summary>
        /// 重置
        /// </summary>
        Reset,
        /// <summary>
        /// 摧毁
        /// </summary>
        Destory,
        /// <summary>
        /// 强化
        /// </summary>
        Strengthen,
        /// <summary>
        /// 弱化
        /// </summary>
        Weak,
        /// <summary>
        /// 逆转
        /// </summary>
        Reverse,
        /// <summary>
        /// 点数增加，只有成功触发点数变化时才会触发
        /// </summary>
        Increase,
        /// <summary>
        /// 点数减少，只有成功触发点数变化时才会触发
        /// </summary>
        Decrease,
        ////////////////////////////////////////////////状态/////////////////////////////////////////
        StateAdd,
        StateClear,
        ////////////////////////////////////////////////字段/////////////////////////////////////////
        FieldSet,
        FieldChange,
        ////////////////////////////////////////////////品质/////////////////////////////////////////
        /// <summary>
        /// 提纯
        /// </summary>
        /// /// <summary>
        /// 魔怔
        /// </summary>
        ////////////////////////////////////////////////阶段/////////////////////////////////////////
        RoundStart,
        RoundEnd,
        TurnStart,
        TurnEnd,
        Pass,
    }
    /// <summary>
    /// 卡牌附加状态类型   ！！！！不要改变顺序，有新的再后面追加，服务端和翻译表格需要同步更新！！！！
    /// </summary>
    public enum CardState
    {
        None,//默认空状态
        Seal,//封印 清楚所有附加值和附加状态，并使能力无法生效
        Invisibility,//隐身 无法作为主动选择目标
        Pry,//窥探 强制卡牌正面，持续一回合
        Cover,//覆盖 强制显示卡牌背面，满足条件后翻转
        /// <summary>
        /// 禁锢 无法打出
        /// </summary>
        Close,//禁锢 无法打出
        Fate,//命运 位于自动选择目标列表中时会被优先选择
        Lurk,//潜伏（间谍）部署至对方场上
        Furor,//狂暴 
        Docile,//温顺
        Poisoning,//中毒 回合结束时给与自身1点伤害
        Rely,//凭依 替换目标卡牌某种类型效果
        Water,//水
        Fire,//火
        Wind,//风
        Soil,//土
        Hold, //驻守 小局结束时不进入墓地并取消此状态
        Congealbounds,//结界 抵消一次伤害
        Forbidden,//禁足 无法被移动
        Apothanasia,//延命 点数为0时增益自身一点
        Black,//黑
        White,//白
    }
    /// <summary>
    /// 卡牌附加值类型     ！！！！不要改变顺序，有新的再后面追加，服务端和翻译表格需要同步更新！！！！
    /// </summary>
    public enum CardField
    {
        None,//默认空状态
        Timer,//计时
        Inspire,//鼓舞
        Chain,//连锁
        Energy,//能量
        Shield,//护盾 抵消等量伤害
        Pary,//祈祷 
    }
    /// <summary>
    /// 异变类型
    /// </summary>
    public enum VariationType
    {
        None,
        Reverse,//逆转
    }

    public enum Camp
    {
        Neutral,
        Taoism,
        Shintoism,
        Buddhism,
        Technology
    }
    public enum GameRegion
    {
        Water,
        Fire,
        Wind,
        Soil,
        Leader,
        Hand,
        Used,
        Deck,
        Grave,
        Battle = 99,
        None = 100,
    }
    public enum BattleRegion
    {
        Water, Fire, Wind, Soil, All = 99, None = 100
    }

    public enum CardType
    {
        Unit,
        Special,
    }
    public enum CardFeature
    {
        LargestPointUnits,
        LowestPointUnits,
        LargestRankUnits,
        LowestRankUnits,
        NotZero,
    }
    public enum CardRank
    {
        Leader,
        Gold,
        Silver,
        Copper,

        NoGold,//铜卡+银卡
        GoldAndLeader//金卡+领袖卡
    }

    public enum CardBoardMode
    {
        Temp,//默认状态，无法操作，但可以关闭
        Select,//多次选择模式
        ExchangeCard,//单次抽卡模式
        ShowOnly,//无按钮，无法操作模式
        ShowOnlyAndHide//无按钮，无法操作模式
    }
    public enum CardBoardType
    {
        Actual,//实际模式，加载拥有实体的卡牌集合数据
        Vitual,//虚拟模式,加载不存在的卡牌集合数据
        Temp,//临时模式，加载玩家临时打开的卡牌集合数据
    }
    //卡牌面板中卡牌可见性
    public enum BoardCardVisible
    {
        AlwaysShow,
        AlwaysHide,
        FromCard
    }
    public enum CardTag
    {
        /// <summary>
        /// 符卡
        /// </summary>
        SpellCard,
        /// <summary>
        /// 
        /// </summary>
        Variation,
        /// <summary>
        /// 机械
        /// </summary>
        Machine,
        /// <summary>
        /// 妖精
        /// </summary>
        Fairy,
        /// <summary>
        /// 物体
        /// </summary>
        Object,
        /// <summary>
        /// 道具
        /// </summary>
        Tool,
        /// <summary>
        /// 妖怪迹
        /// </summary>
        Yokai,
        /// <summary>
        /// 奇迹
        /// </summary>
        Miracle,
    }
    public enum Orientation
    {
        /// <summary>
        /// 以当前回合方作为主视角的我方区域
        /// </summary>
        My,
        /// <summary>
        /// 以当前回合方作为主视角的对方区域
        /// </summary>
        Op,
        /// <summary>
        /// 双方区域
        /// </summary>
        All,
        /// <summary>
        /// 以客户端视角方作为主视角的上方区域
        /// </summary>
        Up,
        /// <summary>
        /// 以客户端视角方作为主视角的下方区域
        /// </summary>
        Down,
    }
    public enum Territory { My, Op, All }
    public enum Language
    {
        Ch,
        Tc,
        En,
        geyu
    }
    public enum CardPointType
    {
        green,
        red,
        white
    }
    //服务器需要同步更新
    public enum NetAcyncType
    {
        Init,
        FocusCard,
        PlayCard,
        DisCard,
        SelectRegion,
        SelectUnits,
        SelectLocation,
        SelectProperty,
        SelectBoardCard,
        ExchangeCard,
        RoundStartExchangeOver,
        Pass,
        Surrender
    }
    public enum PracticeLeader
    {
        Reimu_Hakurei,
        Sanae_Kotiya,

        Mononobe_no_Futo,
        Kaku_Seiga,

        Hijiri_Byakuren,
        Koishi_Komeiji,

        Nitori_Kawasiro,
        Kaguya_Houraisan,

        Cirno,
        Remilia_Scarlet,
        Kijin_Seija,
    }
}