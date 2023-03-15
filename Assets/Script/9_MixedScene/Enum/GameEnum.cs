namespace TouhouMachineLearningSummary.GameEnum
{
    public enum FirstTurn { PlayerFirst, OpponentFirst, Random }
    public enum AgainstModeType
    {
        Story,//����ģʽ
        Practice,//��ϰģʽ

        Casual,//����ģʽ
        Rank,//����ģʽ
        Arena,//������ģʽ
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
        ////////////////////////////////////////////////�ƶ�/////////////////////////////////////////
        /// <summary>
        /// ����
        /// </summary>
        Generate,
        /// <summary>
        /// ��ȡ
        /// </summary>
        Draw,
        /// <summary>
        /// ���
        /// </summary>
        Play,
        /// <summary>
        /// ����
        /// </summary>
        Reback,
        /// <summary>
        /// ����
        /// </summary>
        Deploy,
        /// <summary>
        /// ����
        /// </summary>
        Discard,
        /// <summary>
        /// ����
        /// </summary>
        Dead,
        /// <summary>
        /// ����
        /// </summary>
        Revive,
        /// <summary>
        /// λ��
        /// </summary>
        Move,
        /// <summary>
        /// ��϶
        /// </summary>
        Banish,
        /// <summary>
        /// �ٻ�
        /// </summary>
        Summon,
        ////////////////////////////////////////////////����/////////////////////////////////////////
        /// <summary>
        /// ��ֵ
        /// </summary>
        Set,
        /// <summary>
        /// ����
        /// </summary>
        Gain,
        /// <summary>
        /// �˺�
        /// </summary>
        Hurt,
        /// <summary>
        /// ����
        /// </summary>
        Cure,
        /// <summary>
        /// ����
        /// </summary>
        Reset,
        /// <summary>
        /// �ݻ�
        /// </summary>
        Destory,
        /// <summary>
        /// ǿ��
        /// </summary>
        Strengthen,
        /// <summary>
        /// ����
        /// </summary>
        Weak,
        /// <summary>
        /// ��ת
        /// </summary>
        Reverse,
        /// <summary>
        /// �������ӣ�ֻ�гɹ����������仯ʱ�Żᴥ��
        /// </summary>
        Increase,
        /// <summary>
        /// �������٣�ֻ�гɹ����������仯ʱ�Żᴥ��
        /// </summary>
        Decrease,
        ////////////////////////////////////////////////״̬/////////////////////////////////////////
        StateAdd,
        StateClear,
        ////////////////////////////////////////////////�ֶ�/////////////////////////////////////////
        FieldSet,
        FieldChange,
        ////////////////////////////////////////////////Ʒ��/////////////////////////////////////////
        /// <summary>
        /// �ᴿ
        /// </summary>
        /// /// <summary>
        /// ħ��
        /// </summary>
        ////////////////////////////////////////////////�׶�/////////////////////////////////////////
        RoundStart,
        RoundEnd,
        TurnStart,
        TurnEnd,
        Pass,
    }
    /// <summary>
    /// ���Ƹ���״̬����   ����������Ҫ�ı�˳�����µ��ٺ���׷�ӣ�����˺ͷ�������Ҫͬ�����£�������
    /// </summary>
    public enum CardState
    {
        None,//Ĭ�Ͽ�״̬
        Seal,//��ӡ ������и���ֵ�͸���״̬����ʹ�����޷���Ч
        Invisibility,//���� �޷���Ϊ����ѡ��Ŀ��
        Pry,//��̽ ǿ�ƿ������棬����һ�غ�
        Cover,//���� ǿ����ʾ���Ʊ��棬����������ת
        /// <summary>
        /// ���� �޷����
        /// </summary>
        Close,//���� �޷����
        Fate,//���� λ���Զ�ѡ��Ŀ���б���ʱ�ᱻ����ѡ��
        Lurk,//Ǳ����������������Է�����
        Furor,//�� 
        Docile,//��˳
        Poisoning,//�ж� �غϽ���ʱ��������1���˺�
        Rely,//ƾ�� �滻Ŀ�꿨��ĳ������Ч��
        Water,//ˮ
        Fire,//��
        Wind,//��
        Soil,//��
        Hold, //פ�� С�ֽ���ʱ������Ĺ�ز�ȡ����״̬
        Congealbounds,//��� ����һ���˺�
        Forbidden,//���� �޷����ƶ�
        Apothanasia,//���� ����Ϊ0ʱ��������һ��
        Black,//��
        White,//��
    }
    /// <summary>
    /// ���Ƹ���ֵ����     ����������Ҫ�ı�˳�����µ��ٺ���׷�ӣ�����˺ͷ�������Ҫͬ�����£�������
    /// </summary>
    public enum CardField
    {
        None,//Ĭ�Ͽ�״̬
        Timer,//��ʱ
        Inspire,//����
        Chain,//����
        Energy,//����
        Shield,//���� ���������˺�
        Pary,//�� 
    }
    /// <summary>
    /// �������
    /// </summary>
    public enum VariationType
    {
        None,
        Reverse,//��ת
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

        NoGold,//ͭ��+����
        GoldAndLeader//��+���俨
    }

    public enum CardBoardMode
    {
        Temp,//Ĭ��״̬���޷������������Թر�
        Select,//���ѡ��ģʽ
        ExchangeCard,//���γ鿨ģʽ
        ShowOnly,//�ް�ť���޷�����ģʽ
        ShowOnlyAndHide//�ް�ť���޷�����ģʽ
    }
    public enum CardBoardType
    {
        Actual,//ʵ��ģʽ������ӵ��ʵ��Ŀ��Ƽ�������
        Vitual,//����ģʽ,���ز����ڵĿ��Ƽ�������
        Temp,//��ʱģʽ�����������ʱ�򿪵Ŀ��Ƽ�������
    }
    //��������п��ƿɼ���
    public enum BoardCardVisible
    {
        AlwaysShow,
        AlwaysHide,
        FromCard
    }
    public enum CardTag
    {
        /// <summary>
        /// ����
        /// </summary>
        SpellCard,
        /// <summary>
        /// 
        /// </summary>
        Variation,
        /// <summary>
        /// ��е
        /// </summary>
        Machine,
        /// <summary>
        /// ����
        /// </summary>
        Fairy,
        /// <summary>
        /// ����
        /// </summary>
        Object,
        /// <summary>
        /// ����
        /// </summary>
        Tool,
        /// <summary>
        /// ���ּ�
        /// </summary>
        Yokai,
        /// <summary>
        /// �漣
        /// </summary>
        Miracle,
    }
    public enum Orientation
    {
        /// <summary>
        /// �Ե�ǰ�غϷ���Ϊ���ӽǵ��ҷ�����
        /// </summary>
        My,
        /// <summary>
        /// �Ե�ǰ�غϷ���Ϊ���ӽǵĶԷ�����
        /// </summary>
        Op,
        /// <summary>
        /// ˫������
        /// </summary>
        All,
        /// <summary>
        /// �Կͻ����ӽǷ���Ϊ���ӽǵ��Ϸ�����
        /// </summary>
        Up,
        /// <summary>
        /// �Կͻ����ӽǷ���Ϊ���ӽǵ��·�����
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
    //��������Ҫͬ������
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