using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;


namespace VisualNLP.Module.BusinessObjects;
public interface 禁用词表
{

}
public interface 最小单位
{

}

[XafDisplayName("成份分类")]
[NavigationItem]
public abstract class SentencePartCategory : XPObject
{
    static Dictionary<string, Type> dict;
    public static Dictionary<string, Type> TypeDictionary
    {
        get
        {
            if (dict == null)
            {
                var b = typeof(SentencePartCategory);
                var a = b.Assembly.GetTypes().Where(t => !t.IsAbstract && b.IsAssignableFrom(t));
                dict = new Dictionary<string, Type>();
                foreach (var item in a)
                {
                    dict.Add(item.Name, item);
                }

            }
            return dict;
        }
    }

    // Base class definition
    public SentencePartCategory(Session s) : base(s)
    {

    }

    public string Name { get => this.GetType().Name; }
    public string Text { get => CaptionHelper.GetClassCaption(this.GetType().FullName); }
}


// adjective phrase
// 形容词短语，以形容词为中心词
// 举例:不完全、大型
[XafDisplayName("形容词短语")]
public class ADJP : SentencePartCategory
{
    public ADJP(Session s) : base(s)
    {

    }
}

// adverbial phrase headed by AD (adverb)
// 副词短语，以副词为中心词
// 非常、很
[XafDisplayName("副词短语")]
public class ADVP : SentencePartCategory
{
    public ADVP(Session s) : base(s)
    {
    }
}

// classifier phrase
// 由量词构成的短语
// 系列、大批
[XafDisplayName("分类短语")]
public class CLP : SentencePartCategory
{
    public CLP(Session s) : base(s)
    {
    }
}

// clause headed by C (complementizer)
// 从句，通过带补语（如“的”、“吗”等）
// 张三喜欢李四吗？
[XafDisplayName("从句")]
public class CP : SentencePartCategory
{
    public CP(Session s) : base(s)
    {
    }
}

// phrase formed by ‘‘XP + DEG’’
// 结构为XP + DEG(的)的短语，其中XP可以是ADJP、DP、QP、PP等等，用于修饰名词短语。
// 大型的、前几年的、五年的、在上海的
[XafDisplayName("XXX的")]
public class DNP : SentencePartCategory
{
    public DNP(Session s) : base(s)
    {
    }
}

// determiner phrase
// 限定词短语，通常由限定词和数量词构成
// 这三个、任何
[XafDisplayName("限定词短语")]
public class DP : SentencePartCategory
{
    public DP(Session s) : base(s)
    {
    }
}

// phrase formed by ‘‘XP + DEV’’
// 结构为XP+地的短评，用于修饰动词短语VP
// 心情失落地、大批地
[XafDisplayName("XXX地")]
public class DVP : SentencePartCategory
{
    public DVP(Session s) : base(s)
    {
    }
}

// fragment
// 片段
// (完）
[XafDisplayName("片段")]
public class FRAG : SentencePartCategory
{
    public FRAG(Session s) : base(s)
    {
    }
}

// interjection
// 插话，感叹语
// 哈哈、切
[XafDisplayName("感叹语")]
public class INTJ : SentencePartCategory
{
    public INTJ(Session s) : base(s)
    {
    }
}

// simple clause headed by I (INFL)
// 简单子句或句子，通常不带补语（如“的”、“吗”等）
// 张三喜欢李四。
[XafDisplayName("句子")]
public class IP : SentencePartCategory
{
    public IP(Session s) : base(s)
    {
    }
}

// phrase formed by ‘‘XP + LC’’
// 用于表本地点+方位词（LC)的短语
// 生活中、田野上
[XafDisplayName("地点方位")]
public class LCP : SentencePartCategory
{
    public LCP(Session s) : base(s)
    {
    }
}

// list marker
// 列表短语，包括标点符号
// 一.
[XafDisplayName("列表短语")]
public class LST : SentencePartCategory
{
    public LST(Session s) : base(s)
    {
    }
}

// some particles
// 其他小品词
// 所、而、来、去
[XafDisplayName("其他小品词")]
public class MSP : SentencePartCategory, 最小单位
{
    public MSP(Session s) : base(s)
    {
    }
}

// common noun
// 名词
// HanLP、技术
[XafDisplayName("名词")]
public class NN : SentencePartCategory, 最小单位
{
    public NN(Session s) : base(s)
    {
    }
}

// noun phrase
// 名词短语，中心词通常为名词
// 美好生活、经济水平
[XafDisplayName("名词短语")]
public class NP : SentencePartCategory
{
    public NP(Session s) : base(s)
    {
    }
}

// preposition phrase
// 介词短语，中心词通常为介词
// 在北京、据报道
[XafDisplayName("介词短语")]
public class PP : SentencePartCategory
{
    public PP(Session s) : base(s)
    {
    }
}

//PRN
//parenthetical
//插入语
//，（张三说)，
[XafDisplayName("插入语")]
public class PRN : SentencePartCategory
{
    public PRN(Session s) : base(s)
    {
    }
}

// quantifier phrase
// 量词短语
// 三个、五百辆
[XafDisplayName("量词短语")]
public class QP : SentencePartCategory
{
    public QP(Session s) : base(s)
    {
    }
}

// root node
// 根节点
// 根节点
[XafDisplayName("根节点")]
public class ROOT : SentencePartCategory
{
    public ROOT(Session s) : base(s)
    {
    }
}

// unidentical coordination phrase
// 不对称的并列短语，指并列词两侧的短语类型不致
// (养老、医疗）保险
[XafDisplayName("不对称的并列短语")]
public class UCP : SentencePartCategory
{
    public UCP(Session s) : base(s)
    {
    }
}

// coordinated verb compound
// 复合动词
// 出版发行
[XafDisplayName("复合动词")]
public class VCD : SentencePartCategory
{
    public VCD(Session s) : base(s)
    {
    }
}

// verb compounds formed by VV + VC
// VV + VC形式的动词短语
// 看作是
[XafDisplayName("VV+VC动词短语")]
public class VCP : SentencePartCategory
{
    public VCP(Session s) : base(s)
    {
    }
}

// verb compounds formed by A-not-A or A-one-A
// V不V形式的动词短语
// 能不能、信不信
[XafDisplayName("V不V-动词短语")]
public class VNV : SentencePartCategory
{
    public VNV(Session s) : base(s)
    {
    }
}

// verb phrase
// 动词短语，中心词通常为动词
// 完成任务、努力工作
[XafDisplayName("动词短语")]
public class VP : SentencePartCategory
{
    public VP(Session s) : base(s)
    {
    }
}

// potential form V-de-R or V-bu-R
// V不R、V得R形式的动词短语
// 打不赢、打得过
[XafDisplayName("V不R、V得R动词短语")]
public class VPT : SentencePartCategory
{
    public VPT(Session s) : base(s)
    {
    }
}

// verb resultative compound
// 动补结构短语
// 研制成功、降下来
[XafDisplayName("动补短语")]
public class VRD : SentencePartCategory
{
    public VRD(Session s) : base(s)
    {
    }
}

// verb compounds formed by a modifier + a head
// 修饰语+中心词构成的动词短语
// 拿来支付、仰头望去
[XafDisplayName("修饰语+中心词动词短语")]
public class VSB : SentencePartCategory, 最小单位
{
    public VSB(Session s) : base(s)
    {
    }
}

//adjective phrase
//形容词短语,以形容词为中心词
//举例:不完全、大型
[XafDisplayName("形容词短语,以形容词为中心词")]
public class AD : SentencePartCategory, 最小单位
{
    public AD(Session s) : base(s)
    {

    }
}

//adverb
//副词
//举例:仍然、很、大大、约
[XafDisplayName("副词")]
public class ADV : SentencePartCategory
{
    public ADV(Session s) : base(s)
    {

    }
}

//aspect marker
//动态助词
//举例:了、着、过
[XafDisplayName("动态助词")]
public class AS : SentencePartCategory, 最小单位
{
    public AS(Session s) : base(s)
    {

    }
}

//bǎ in ba-construction
//把字句
//当“把”、“将”出现在结构“NP0 + BA + NP1+VP”时的词性
//举例:把、将
[XafDisplayName("把字句")]
public class BA : SentencePartCategory
{
    public BA(Session s) : base(s)
    {

    }
}

//coordinating conjunction
//并列连接词
//举例:与、和、或者、还是
[XafDisplayName("并列连接词")]
public class CC : SentencePartCategory, 最小单位
{
    public CC(Session s) : base(s)
    {

    }
}

//cardinal number
//概数词
//举例:一百、好些、若干  
[XafDisplayName("概数词")]
public class CD : SentencePartCategory
{
    public CD(Session s) : base(s)
    {

    }
}

//subordinating conjunction
//从属连词
//举例:如果、那么、就
[XafDisplayName("从属连词")]
public class CS : SentencePartCategory
{
    public CS(Session s) : base(s)
    {

    }
}

//de as a complementizer or a nominalizer
//补语成分“的”
//当“的”或“之”作补语标记或名词化标记时的词性,其结构为:S/VP DEC {NP},如,喜欢旅游的大学生
[XafDisplayName("补语成分“的”")]
public class DEC : SentencePartCategory, 最小单位
{
    public DEC(Session s) : base(s)
    {

    }
}

//de as a genitive marker and an associative marker  
//属格“的”
//当“的”或“之”作所有格时的词性,其结构为:NP/PP/JJ/DT DEG {NP}, 如,他的车、经济的发展
[XafDisplayName("属格“的”")]
public class DEG : SentencePartCategory, 最小单位
{
    public DEG(Session s) : base(s)
    {

    }
}

//resultative de, de in V-de const and V-de-R
//表结果的“得” 
//当“得”出现在结构“V-得-R”时的词性,如,他跑得很快
[XafDisplayName("表结果的“得”")]
public class DER : SentencePartCategory
{
    public DER(Session s) : base(s)
    {

    }
}

//manner de, de before VP
//表方式的“地”
//当“地”出现在结构“X-地-VP”时的词性,如,高兴地说
[XafDisplayName("表方式的“地”")]
public class DEV : SentencePartCategory
{
    public DEV(Session s) : base(s)
    {

    }
}

//determiner
//限定词
//举例:这、那、该、每、各  
[XafDisplayName("限定词")]
public class DT : SentencePartCategory, 最小单位
{
    public DT(Session s) : base(s)
    {

    }
}

//for words like “etc.”
//表示省略
//举例:等、等等
[XafDisplayName("表示省略")]
public class ETC : SentencePartCategory, 最小单位
{
    public ETC(Session s) : base(s)
    {

    }
}

//emoji
//表情符
//举例:)
[XafDisplayName("表情符")]
public class EM : SentencePartCategory
{
    public EM(Session s) : base(s)
    {

    }
}

//foreign words
//外来语
//举例:卡拉、A型
[XafDisplayName("外来语")]
public class FW : SentencePartCategory
{
    public FW(Session s) : base(s)
    {

    }
}

//interjection
//句首感叹词
//举例:啊
[XafDisplayName("句首感叹词")]
public class IJ : SentencePartCategory
{
    public IJ(Session s) : base(s)
    {

    }
}

//other noun-modifier
//其他名词修饰语
//举例:共同、新
[XafDisplayName("其他名词修饰语")]
public class JJ : SentencePartCategory, 最小单位
{
    public JJ(Session s) : base(s)
    {

    }
}

//bèi in long bei-const  
//长句式表被动
//当“被”、“叫”、“给”出现在结构“NP0 + LB + NP1+ VP”结构时 的词性,如,他被我训了一顿
[XafDisplayName("长句式表被动")]
public class LB : SentencePartCategory
{
    public LB(Session s) : base(s)
    {

    }
}

//localizer
//方位词
//举例:前、旁、到、在内、以来、为止
[XafDisplayName("方位词")]
public class LC : SentencePartCategory, 最小单位
{
    public LC(Session s) : base(s)
    {

    }
}

//measure word
//量词
//举例:个、群、公里
[XafDisplayName("量词")]
public class M : SentencePartCategory, 最小单位
{
    public M(Session s) : base(s)
    {

    }
}

//noise that characters are written in the wrong order
//噪声
//举例:事/NOI 类/NOI 各/NOI 故/NOI
[XafDisplayName("噪声")]
public class NOI : SentencePartCategory
{
    public NOI(Session s) : base(s)
    {

    }
}

//proper noun
//专有名词
//举例:北京、乔丹、微软
[XafDisplayName("专有名词")]
public class NR : SentencePartCategory, 最小单位
{
    public NR(Session s) : base(s)
    {

    }
}

//temporal noun
//时间名词
//举例:一月、汉朝、当今
[XafDisplayName("时间名词")]
public class NT : SentencePartCategory
{
    public NT(Session s) : base(s)
    {

    }
}

//ordinal number  
//序数词
//举例:第一百
[XafDisplayName("序数词")]
public class OD : SentencePartCategory
{
    public OD(Session s) : base(s)
    {

    }
}

//onomatopoeia
//象声词
//举例:哗哗、呼、咯吱
[XafDisplayName("象声词")]
public class ON : SentencePartCategory
{
    public ON(Session s) : base(s)
    {

    }
}

//preposition e.g., “from” and “to”
//介词
//举例:从、对、根据
[XafDisplayName("介词")]
public class P : SentencePartCategory, 最小单位
{
    public P(Session s) : base(s)
    {

    }
}

//pronoun  
//代词
//举例:我、这些、其、自己
[XafDisplayName("代词")]
public class PN : SentencePartCategory, 最小单位
{
    public PN(Session s) : base(s)
    {

    }
}

//punctuation
//标点符号
//举例:?、。、;
[XafDisplayName("标点符号")]
public class PU : SentencePartCategory, 最小单位
{
    public PU(Session s) : base(s)
    {

    }
}


//bèi in short bei-const
//短句式表被动 
//当“被”、“给”出现在NP0 +SB+ VP结果时的词性,如,他被训了 一顿
[XafDisplayName("短句式表被动")]
public class SB : SentencePartCategory, 最小单位
{
    public SB(Session s) : base(s)
    {

    }
}

//sentence final particle
//句末助词
//举例:吧、呢、啊、啊
[XafDisplayName("句末助词")]
public class SP : SentencePartCategory
{
    public SP(Session s) : base(s)
    {

    }
}

//web address
//网址
//举例:www.hankcs.com
[XafDisplayName("网址")]
public class URL : SentencePartCategory
{
    public URL(Session s) : base(s)
    {

    }
}

//predicative adjective
//表语形容词
//举例:雪白、厉害
[XafDisplayName("表语形容词")]
public class VA : SentencePartCategory, 最小单位
{
    public VA(Session s) : base(s)
    {

    }
}

//copula, be words
//系动词
//举例:是、为、非
[XafDisplayName("系动词")]
public class VC : SentencePartCategory, 最小单位
{
    public VC(Session s) : base(s)
    {

    }
}

//yǒu as the main verb
//动词有无
//举例:有、没有、无
[XafDisplayName("动词有无")]
public class VE : SentencePartCategory, 最小单位
{
    public VE(Session s) : base(s)
    {

    }
}

//other verb
//其他动词
//举例:可能、要、走、喜欢  
[XafDisplayName("其他动词")]
public class VV : SentencePartCategory,最小单位
{
    public VV(Session s) : base(s)
    {

    }
}



