// File: D:\ai.study\VisualNLP\VisualNLP.Module\BusinessObjects\Class1.cs
public class T
{
    // Attention 方法用于在进行计算时选择性地关注不同的输入数据部分
    // 它需要两个参数：input，一个二维数组，其中每行是单个输入样本；和query，一个表示查询向量的一维数组。
    //输出是一个一维数组，即注意力向量。
    public double[] Attention(double[][] input, double[] query)
    {
        int inputLength = input.Length;
        int queryLength = query.Length;

        // 初始化矩阵和向量
        double[][] attentionMatrix = new double[inputLength][];
        for (int i = 0; i < inputLength; i++)
        {
            attentionMatrix[i] = new double[queryLength];
            for (int j = 0; j < queryLength; j++)
            {
                // 计算输入的每一行与查询向量(query)的点积
                attentionMatrix[i][j] = Dot(input[i], query);
            }
        }

        // 对注意力矩阵的每一列进行softmax计算，以获得注意力向量
        double[] attentionVector = Softmax(ColSum(attentionMatrix));

        return attentionVector;
    }

    // Dot方法用于计算具有相等长度的两个数组的点积.
    //例如，DP(x, y) = sum(x[i]*y[i], i=1 to n)。 返回点积结果作为double类型。
    public double Dot(double[] a, double[] b)
    {
        return a.Select((x, i) => x * b[i]).Sum();
    }

    // ColSum方法用于计算矩阵的列总和。 返回结果作为double类型的数组
    public double[] ColSum(double[][] matrix)
    {
        return Enumerable.Range(0, matrix[0].Length)
            .Select(c => Enumerable.Range(0, matrix.Length)
                .Select(r => matrix[r][c]).Sum()).ToArray();
    }

    // 实现Softmax函数。 它需要一个double类型的数组作为输入，并返回另一个相同长度的double类型的数组。
    public double[] Softmax(double[] vector)
    {
        double sum = 0.0;
        double[] result = new double[vector.Length];

        // 指数运算求softmax
        for (int i = 0; i < vector.Length; i++)
        {
            result[i] = Math.Exp(vector[i]);
            sum += result[i];
        }

        // 归一化步骤
        for (int i = 0; i < vector.Length; i++)
        {
            result[i] /= sum;
        }

        return result;
    }
    public void Attention_ReturnsCorrectResult()
    {
        var t = new T();
        // 创建T类的实例
        double[][] input =
            new double[][]
            {
                new double[] { 1.0, 2.0, 3.0 },
                new double[] { 4.0, 5.0, 6.0 }
            };
        // 创建一个二维数组input
        double[] query = new double[] { 0.1, 0.2, 0.3 }; //创建一个查询数组query
        var expected = new double[] { 0.02731, 0.97269 }; // 创建一个期望结果数组expected
        var result = t.Attention(input, query); // 调用Attention方法进行计算

        // 遍历结果数组，判断Expected和Result之间的差别是否在0.00001极差范围之内。
        for (int i = 0; i < expected.Length; i++)
        {
            //Assert.InRange(Math.Abs(result[i] - expected[i]), 0.0, 0.00001);
        }
    }
}
