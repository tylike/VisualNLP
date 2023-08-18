using System;
using edu.stanford.nlp.pipeline;
using java.util.Properties;
using System.IO;
using System.Text;
using java.io;

class PG
{
    static void Run(string[] args)
    {
        var props = new Properties();
        props.setProperty("annotators", "tokenize,ssplit,pos,lemma,ner,parse,depparse");
        props.setProperty("parse.model", "edu/stanford/nlp/models/lexparser/chinesePCFG.ser.gz");
        props.setProperty("tokenize.language", "zh");
        var pipeline = new StanfordCoreNLP(props);
        var text = "这是一个用中文写的例子。";
        var annotation = new Annotation(text);
        pipeline.annotate(annotation);

        using (var stream = new MemoryStream())
        {
            pipeline.jsonPrint(annotation, new PrintWriter(stream));
            var json = Encoding.UTF8.GetString(stream.ToArray());
            Console.WriteLine(json);
        }
    }
}

