using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.General;
using DevExpress.Xpo;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.ComponentModel;

namespace VisualNLP.Module.BusinessObjects;
[NavigationItem]
public class TextDocument : XPObject
{
    public TextDocument(Session s) : base(s)
    {

    }

    public string Title
    {
        get { return GetPropertyValue<string>(nameof(Title)); }
        set { SetPropertyValue(nameof(Title), value); }
    }
    [Size(-1)]
    public string Content
    {
        get { return GetPropertyValue<string>(nameof(Content)); }
        set { SetPropertyValue(nameof(Content), value); }
    }
    public TreeNode Root
    {
        get { return GetPropertyValue<TreeNode>(nameof(Root)); }
        set { SetPropertyValue(nameof(Root), value); }
    }

    public XPCollection<TreeNode> Nodes => Root?.Items;

}

[NavigationItem]
public partial class TreeNode : XPObject, ITreeNode
{

    public int ItemCount => Items.Count;

    public string ClrType => this.GetType().Name;

    [Association]
    public TreeNode Parent
    {
        get { return GetPropertyValue<TreeNode>(nameof(Parent)); }
        set { SetPropertyValue(nameof(Parent), value); }
    }
    [Association, DevExpress.Xpo.Aggregated]
    public XPCollection<TreeNode> Items
    {
        get => GetCollection<TreeNode>();
    }

    public int Index
    {
        get { return GetPropertyValue<int>(nameof(Index)); }
        set { SetPropertyValue(nameof(Index), value); }
    }

    ITreeNode ITreeNode.Parent { get => this.Parent; }
    IBindingList ITreeNode.Children { get => this.Items; }
}

public partial class TreeNode
{
    public TreeNode(Session s) : base(s)
    {

    }

    public string CategoryName
    {
        get
        {
            if (Category != null)
                return CaptionHelper.GetClassCaption(Category.GetType().FullName);
            return null;
        }
    }

    public string Name
    {
        get
        {
            if (!string.IsNullOrEmpty(Text))
                return Text;
            return string.Join(" ", Items.Select(t => t.Name));// $"{Type}.{this.Oid}";
        }
    }

    public string Type { get; set; }

    public SentencePartCategory Category
    {
        get
        {
            var rst = GetPropertyValue<SentencePartCategory>(nameof(Category));

            return rst;
        }
        set { SetPropertyValue(nameof(Category), value); }
    }


    public string Text
    {
        get { return GetPropertyValue<string>(nameof(Text)); }
        set { SetPropertyValue(nameof(Text), value); }
    }


    //public List<object> Items { get; } = new List<object>();

}
