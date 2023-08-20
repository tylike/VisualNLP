using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Office.NumberConverters;
using DevExpress.Persistent.Base;
using DevExpress.PivotGrid.Design;
using DevExpress.XtraPrinting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Python.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using VisualNLP.Module.BusinessObjects;

namespace VisualNLP.Module.Controllers
{
    public static class JTokenExtendesion
    {
        public static bool IsString(this JToken token, string str)
        {
            return token is JValue jv && object.Equals(jv.Value, str);
        }

        public static string GetString(this JToken token)
        {
            if (token is JValue jv)
                return jv.Value as string;
            return null;
        }

        public static bool IsPU(this JToken node)
        {
            return node.GetPU() != null;
        }

        public static string GetPU(this JToken node)
        {
            if (node is JArray arr1)
            {
                if (arr1.Count == 2 && arr1.First.IsString("PU"))
                {
                    if (arr1.Last is JArray arr2 && arr2.Count == 1)
                    {
                        if (arr2.First is JValue jv && jv.Value is string pu)
                            return pu;
                    }
                }
            }
            return null;
        }

    }

    public class SplitSentenceViewController : ObjectViewController<ObjectView, TextDocument>
    {
        public SplitSentenceViewController()
        {
            var action = new SimpleAction(this, "分词", null);
            action.Execute += Action_Execute;
            var createCategory = new SimpleAction(this, "创建分类", null);
            createCategory.Execute += CreateCategory_Execute;
            var fixdata = new SimpleAction(this, "修复数据", null);
            fixdata.Execute += Fixdata_Execute;
            var getJson = new SimpleAction(this, "分词->JSON", null);
            getJson.Execute += GetJson_Execute;

            var parseJson = new SimpleAction(this, "Parse JSON", null);
            parseJson.Execute += ParseJson_Execute;
        }

        private void ParseJson_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            this.vocabs = ObjectSpace.GetObjects<Vocab>().ToList();
            this.categories = ObjectSpace.GetObjects<SentencePartCategory>().ToList();
            if (!categories.Any())
            {
                CreateCategory();
            }


            var json = (JObject)JsonConvert.DeserializeObject(ViewCurrentObject.JSON);
            var con = json["con"];
            //var con = json["con"];
            var treeNode = ObjectSpace.CreateObject<TreeNode>();
            Access(con, treeNode);
            ViewCurrentObject.Root = treeNode;
            ObjectSpace.CommitChanges();
        }

        List<Vocab> vocabs { get; set; }
        List<SentencePartCategory> categories { get; set; }
        Vocab FindOrCreate(string category, string text)
        {
            var find = vocabs.FirstOrDefault(t => t.Text == text && t.Category.GetType().Name == category);
            if (find == null)
            {
                find = ObjectSpace.CreateObject<Vocab>();
                find.Text = text;
                find.Category = categories.FirstOrDefault(t => t.GetType().Name == category);
                vocabs.Add(find);
            }
            return find;
        }

        void Access(JToken token, TreeNode treeNode)
        {
            if (token is JArray array)
            {
                //只有一个子级节点,
                if (array.Count == 1)
                {
                    //是最终词
                    if (token.First is JValue jv)
                    {
                        if (jv.Value is string txt)
                        {
                            //Debug.Write(jv.Value);
                            //处理最终词;
                            treeNode.Text = txt;
                            var find = FindOrCreate(treeNode.Type, txt);
                            treeNode.RefVocab = find;
                            //tn.Text = txt.Text;

                            return;
                        }
                    }
                    //是子级
                    Access(token.First, treeNode);

                    return;
                }

                //有两个子级节点,第一个值是类型,后面的是数组.
                if (array.Count == 2)
                {
                    if (array.First is JValue jv1 && jv1.Value is string type)
                    {
                        treeNode.Type = type;
                        //var newNode = ObjectSpace.CreateObject<TreeNode>();
                        Access(array.Last, treeNode);
                        return;
                    }
                    var pu = array.Last.GetPU();
                    if (pu != null)
                    {
                        Access(array.First, treeNode);
                        treeNode.Suffix += pu;
                        return;
                    }
                }



                TreeNode pre = null;

                foreach (var item in token)
                {
                    var pu = item.GetPU();
                    if (pu == null || pre == null)
                    {
                        var newNode = ObjectSpace.CreateObject<TreeNode>();
                        newNode.Index = treeNode.Items.Count;
                        Access(item, newNode);
                        treeNode.Items.Add(newNode);
                        pre = newNode;
                    }
                    else
                    {
                        pre.Suffix += " " + pu;
                    }
                }

                return;
            }
            if (token is JObject)
            {

            }

            if (token is IList)
            {

            }
        }

        private void GetJson_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var categories = ObjectSpace.GetObjects<SentencePartCategory>().ToList();
            var vocabs = ObjectSpace.GetObjects<Vocab>().ToList();

            PSetup.Setup();

            using (Py.GIL())
            {
                //from hanlp_restful import HanLPClient
                //HanLP = HanLPClient('https://www.hanlp.com/api', auth = None, language = 'zh') # auth不填则匿名，zh中文，mul多语种
                //dynamic hanlp = Py.Import("hanlp_restful");
                var scope = Py.CreateScope();
                dynamic parse = scope.Exec(
$@"from hanlp_restful import HanLPClient
HanLP = HanLPClient('https://www.hanlp.com/api', auth = None, language = 'zh')

def runParse(str):
    return HanLP.parse(str,skip_tasks = 'tok/fine')

"
);
                var rst = parse.runParse(ViewCurrentObject.Content);
                ViewCurrentObject.JSON = rst.ToString();
                ObjectSpace.CommitChanges();

            }
        }

        private void Fixdata_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var categories = ObjectSpace.GetObjects<SentencePartCategory>().ToList();

            var wait = ObjectSpace.GetObjectsQuery<TreeNode>().Where(t => t.Category == null);
            foreach (var tn in wait)
            {
                tn.Category = categories.FirstOrDefault(t => t.GetType().Name == tn.Type);
            }
            ObjectSpace.CommitChanges();
        }

        private void CreateCategory_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            CreateCategory();
        }

        private void CreateCategory()
        {
            var list = ObjectSpace.GetObjects<SentencePartCategory>().ToArray();

            foreach (var x in SentencePartCategory.TypeDictionary)
            {
                if (list.Any(t => t.GetType().FullName == x.GetType().FullName))
                {
                    continue;
                }
                var ins = ObjectSpace.CreateObject(x.Value);
            }
            ObjectSpace.CommitChanges();
        }

        private void Action_Execute(object sender, SimpleActionExecuteEventArgs e)
        {

            var categories = ObjectSpace.GetObjects<SentencePartCategory>().ToList();
            var vocabs = ObjectSpace.GetObjects<Vocab>().ToList();

            PSetup.Setup();

            using (Py.GIL())
            {
                //from hanlp_restful import HanLPClient
                //HanLP = HanLPClient('https://www.hanlp.com/api', auth = None, language = 'zh') # auth不填则匿名，zh中文，mul多语种
                //dynamic hanlp = Py.Import("hanlp_restful");
                var scope = Py.CreateScope();
                dynamic parse = scope.Exec(
$@"from hanlp_restful import HanLPClient
HanLP = HanLPClient('https://www.hanlp.com/api', auth = None, language = 'zh')

def runParse(str):
    return HanLP.parse(str,skip_tasks = 'tok/fine')

"
);
                var rst = parse.runParse(ViewCurrentObject.Content);
                //var http = new hanlp.HanLPClient("", auth = null, language = "zh");


                //dynamic hanlp = Py.Import("hanlp");
                //dynamic tok = hanlp.load(hanlp.pretrained.mtl.CLOSE_TOK_POS_NER_SRL_DEP_SDP_CON_ELECTRA_SMALL_ZH);
                ////var txt = @"北京动物园的历史可追溯到清朝光绪三十二年（1906年）。当时被称为“万牲园” [3] 。它的前身是清农工商部农事试验场，试验场是在原乐善园、继园（又称“三贝子花园”）和广善寺、惠安寺旧址上所建。初衷是为学习西方先进经验，“开通风气，振兴农业”。 [4-5]建成后的农事试验场占地面积约71公顷，除有不少精美的建筑物外，还设有试验室、农器室、肥料室、蚕室、温室、农夫住宅等。对各类农作物分为五大宗进行试验，分别为“谷麦试验、蚕桑试验、蔬菜试验、果木试验、花卉试验。附设的动物园面积仅占1.5公顷（即今北京动物园东南隅），最初展览的动物是南洋大臣兼两江总督的端方，自德国购回的部分禽兽及全国各地抚督送献清朝政府的动物约百余种。植物室内建有温室，并展出各种奇花异卉。光绪三十四年（1908年）6月16日，农事试验场全部竣工，开放接待游人。门票售价为铜元八枚，孩童、跟役减半。如需到动物园和植物园参观要另买票，动物园票价为铜元八枚，植物园票价为铜元四枚。在农事试验场的正门外东西两旁，各设有小屋一间；东边一间是售票处，有两个窗口，分别为南窗、西窗；南窗卖男客票，为白色；西窗卖女客票，为红色。西边一间是寄存物件处，如有大件、要件不便带入场中的，可以寄存此处。游人入场由东门进，分男左女右。进门后，有人验票、剪票。 [3]由于农事试验场所处之地，交通十分便利，又是京师第一座集动物、植物为一体的，带有公园性质的农事试验场，故开办之初，人流不断，十分热闹，甚至慈禧、光绪也两次来园观赏。农事试验场之名，很快盛传京师。 [4]";
                //var txt = ViewCurrentObject.Content;
                ////                    @"北京动物园的历史可追溯到清朝光绪三十二年（1906年）。
                ////当时被称为“万牲园” [3] 。
                ////它的前身是清农工商部农事试验场，试验场是在原乐善园、继园（又称“三贝子花园”）和广善寺、惠安寺旧址上所建。
                ////初衷是为学习西方先进经验，“开通风气，振兴农业”。 
                ////";
                //var rst = tok(txt);
                ViewCurrentObject.JSON = rst.ToString();

                var con = rst["con"];
                var child = con.__len__();

                var treeNode = GetObject(con);

                ViewCurrentObject.Root = treeNode;
                ObjectSpace.CommitChanges();

            }
            Vocab FindOrCreate(string category, string text)
            {
                var find = vocabs.FirstOrDefault(t => t.Text == text && t.Category.GetType().Name == category);
                if (find == null)
                {
                    find = ObjectSpace.CreateObject<Vocab>();
                    find.Text = text;
                    find.Category = categories.FirstOrDefault(t => t.GetType().Name == category);
                    vocabs.Add(find);
                }
                return find;
            }

            TreeNode GetObject(dynamic node)
            {
                var lbl = "Root";
                if (node is PyObject py1 && py1.HasAttr("_label"))
                {
                    lbl = node._label;
                }
                else
                {
                    lbl = "Root";
                }

                var tn = ObjectSpace.CreateObject<TreeNode>();
                tn.Type = lbl;
                tn.Category = categories.FirstOrDefault(t => t.GetType().Name == tn.Type);
                //var rst = new List<object>();
                int index = 0;

                if (node.__len__() == 1)
                {
                    (bool IsText, string Text) txt = GetEndString(node[0]);
                    //子级只有一个字符串
                    if (txt.IsText)
                    {
                        var find = FindOrCreate(tn.Type, txt.Text);
                        tn.RefVocab = find;
                        //tn.Text = txt.Text;
                        return tn;

                    }
                    else
                    {
                        //子级只有一个不可分的元素“词”
                        var n = node[0];
                        if (n.__len__() == 1)
                        {
                            (bool IsText, string Text) t1 = GetEndString(n[0]);
                            if (t1.IsText)
                            {
                                string label = n._label;
                                var find = FindOrCreate(label, t1.Text);
                                tn.RefVocab = find;
                                return tn;
                            }
                        }
                        //throw new Exception("错误");
                    }
                }

                TreeNode pre = null;

                foreach (var item in node)
                {
                    var pu = item.GetPU();
                    if (pu == null || pre == null)
                    {
                        var child = GetObject(item);
                        child.Index = index;
                        tn.Items.Add(child);
                        index++;
                        pre = child;
                    }
                    else
                    {
                        pre.Suffix += " " + pu;
                    }
                }
                return tn;
            }
        }

        (bool IsText, string Text) GetEndString(dynamic obj)
        {
            if (obj is PyObject pys && pys != null && pys.GetPythonType().Name == "str")
            {
                return (IsText: true, Text: pys.As<string>());
            }
            return (IsText: false, Text: null);
        }
    }
}
