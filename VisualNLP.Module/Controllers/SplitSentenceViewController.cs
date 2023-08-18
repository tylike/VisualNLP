using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using Python.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualNLP.Module.BusinessObjects;

namespace VisualNLP.Module.Controllers
{

    public class SplitSentenceViewController : ObjectViewController<DetailView, TextDocument>
    {
        public SplitSentenceViewController()
        {
            var action = new SimpleAction(this, "分词", null);
            action.Execute += Action_Execute;
            var createCategory = new SimpleAction(this, "创建分类", null);
            createCategory.Execute += CreateCategory_Execute;
            var fixdata = new SimpleAction(this, "修复数据", null);
            fixdata.Execute += Fixdata_Execute;
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
            PSetup.Setup();
            var categories = ObjectSpace.GetObjects<SentencePartCategory>().ToList();
            using (Py.GIL())
            {
                dynamic hanlp = Py.Import("hanlp");
                dynamic tok = hanlp.load(hanlp.pretrained.mtl.CLOSE_TOK_POS_NER_SRL_DEP_SDP_CON_ELECTRA_SMALL_ZH);
                //var txt = @"北京动物园的历史可追溯到清朝光绪三十二年（1906年）。当时被称为“万牲园” [3] 。它的前身是清农工商部农事试验场，试验场是在原乐善园、继园（又称“三贝子花园”）和广善寺、惠安寺旧址上所建。初衷是为学习西方先进经验，“开通风气，振兴农业”。 [4-5]建成后的农事试验场占地面积约71公顷，除有不少精美的建筑物外，还设有试验室、农器室、肥料室、蚕室、温室、农夫住宅等。对各类农作物分为五大宗进行试验，分别为“谷麦试验、蚕桑试验、蔬菜试验、果木试验、花卉试验。附设的动物园面积仅占1.5公顷（即今北京动物园东南隅），最初展览的动物是南洋大臣兼两江总督的端方，自德国购回的部分禽兽及全国各地抚督送献清朝政府的动物约百余种。植物室内建有温室，并展出各种奇花异卉。光绪三十四年（1908年）6月16日，农事试验场全部竣工，开放接待游人。门票售价为铜元八枚，孩童、跟役减半。如需到动物园和植物园参观要另买票，动物园票价为铜元八枚，植物园票价为铜元四枚。在农事试验场的正门外东西两旁，各设有小屋一间；东边一间是售票处，有两个窗口，分别为南窗、西窗；南窗卖男客票，为白色；西窗卖女客票，为红色。西边一间是寄存物件处，如有大件、要件不便带入场中的，可以寄存此处。游人入场由东门进，分男左女右。进门后，有人验票、剪票。 [3]由于农事试验场所处之地，交通十分便利，又是京师第一座集动物、植物为一体的，带有公园性质的农事试验场，故开办之初，人流不断，十分热闹，甚至慈禧、光绪也两次来园观赏。农事试验场之名，很快盛传京师。 [4]";
                var txt = ViewCurrentObject.Content;
                //                    @"北京动物园的历史可追溯到清朝光绪三十二年（1906年）。
                //当时被称为“万牲园” [3] 。
                //它的前身是清农工商部农事试验场，试验场是在原乐善园、继园（又称“三贝子花园”）和广善寺、惠安寺旧址上所建。
                //初衷是为学习西方先进经验，“开通风气，振兴农业”。 
                //";
                var rst = tok(txt);
                var con = rst["con"];
                var treeNode = GetObject(con[0]);
                ViewCurrentObject.Root = treeNode;
                ObjectSpace.CommitChanges();

            }


            TreeNode GetObject(dynamic node)
            {
                var tn = ObjectSpace.CreateObject<TreeNode>();
                tn.Type = node._label;
                tn.Category = categories.FirstOrDefault(t => t.GetType().Name == tn.Type);
                //var rst = new List<object>();
                int index = 0;

                //子级只有一个字符串
                if (node.__len__() == 1)
                {
                    if (node[0] is PyObject pys && pys != null && pys.GetPythonType().Name == "str")
                    {
                        tn.Text = node[0].As<string>();
                        return tn;
                    }
                    else
                    {
                        //throw new Exception("错误");
                    }
                }

                foreach (var item in node)
                {
                    var child = GetObject(item);
                    child.Index = index;
                    tn.Items.Add(child);
                    index++;
                }
                return tn;
            }
        }
    }
}
