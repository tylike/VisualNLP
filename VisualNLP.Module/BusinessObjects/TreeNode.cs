using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.General;
using DevExpress.Xpo;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.ComponentModel;
using System.Drawing;
using DevExpress.Xpo.Metadata;
namespace VisualNLP.Module.BusinessObjects;

// ...
public class ColorValueConverter : ValueConverter
{
    public override Type StorageType
    {
        get { return typeof(Int32); }
    }
    public override object ConvertToStorageType(object value)
    {
        if (!(value is Color)) return null;
        Color color = (Color)value;
        return color.IsEmpty ? -1 : color.ToArgb();
    }
    public override object ConvertFromStorageType(object value)
    {
        if (!(value is Int32)) return null;
        Int32 argbCode = Convert.ToInt32(value);
        return argbCode == -1 ? Color.Empty : Color.FromArgb(argbCode);
    }
}

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

    [Size(-1)]
    public string JSON
    {
        get => GetPropertyValue<string>(nameof(JSON));
        set => SetPropertyValue(nameof(JSON), value);
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
[Appearance("OnlyOneItem",BackColor ="Red", Criteria ="ItemCount == 1",TargetItems ="*")]
public partial class TreeNode
{
    public TreeNode(Session s) : base(s)
    {

    }
    /// <summary>
    /// 后缀标点符号
    /// </summary>
    public string Suffix
    {
        get=>GetPropertyValue<string>(nameof(Suffix));
        set=> SetPropertyValue(nameof(Suffix), value);
    }
    //public string Prefix
    //{
    //    get=> GetPropertyValue<string>(nameof(Prefix));
    //    set=>SetPropertyValue(nameof(Prefix), value);
    //}
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
            if (RefVocab != null)
                return RefVocab.Text + Suffix;

            if (!string.IsNullOrEmpty(Text))
                return Text + Suffix;
            return string.Join(" ", Items.Select(t => t.Name))+Suffix;// $"{Type}.{this.Oid}";
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

    public Vocab RefVocab
    {
        get { return GetPropertyValue<Vocab>(nameof(RefVocab)); }
        set { SetPropertyValue(nameof(RefVocab), value); }
    }
    //public List<object> Items { get; } = new List<object>();

    public string Partern
    {
        get
        {
            if (Items.Count > 0)
                return "(" + string.Join("+", Items.Select(t => $"{t.Partern}")) + ")";
            return Type;
        }
    }
}

[NavigationItem]
[XafDisplayName("词表")]
[XafDefaultProperty(nameof(Title))]
public class Vocab : XPObject
{
    public Vocab(Session s) : base(s)
    {

    }

    [ValueConverter(typeof(ColorValueConverter))]
    public Color Color 
    {
        get =>GetPropertyValue<Color>(nameof(Color)); 
        set => SetPropertyValue(nameof(Color), value);
    }

    public string Title
    {
        get
        {
            return Category.GetType().Name + "." + CaptionHelper.GetClassCaption(Category.GetType().FullName) + " " + Text;
        }
    }

    public string Text
    {
        get { return GetPropertyValue<string>(nameof(Text)); }
        set { SetPropertyValue(nameof(Text), value); }
    }

    public SentencePartCategory Category
    {
        get { return GetPropertyValue<SentencePartCategory>(nameof(Category)); }
        set { SetPropertyValue(nameof(Category), value); }
    }
}