// 修改自：https://benchmarksgame-team.pages.debian.net/benchmarksgame/program/binarytrees-csharpcore-2.html
// 许可: https://benchmarksgame-team.pages.debian.net/benchmarksgame/license.html

using BenchmarkDotNet.Attributes;

#nullable disable

public class BinaryTrees
{
    private const int MinDepth = 4;
    private const int NoTasks = 4;

    private static readonly TextWriter Log = TextWriter.Null;

    [Benchmark]
    public void Run()
    {
        const int maxDepth = 10;
        // int maxDepth = args.Length == 0
        //     ? 10
        //     : Math.Max(MinDepth + 2, int.Parse(args[0]));
        

        Log.WriteLine(string.Concat("stretch tree of depth ", maxDepth + 1,
            "\t check: ", TreeNode.bottomUpTree(maxDepth + 1).itemCheck()));

        TreeNode longLivedTree = TreeNode.bottomUpTree(maxDepth);

        var results = new string[(maxDepth - MinDepth) / 2 + 1];

        for (var i = 0; i < results.Length; i++)
        {
            int depth = i * 2 + MinDepth;
            int n = (1 << (maxDepth - depth + MinDepth)) / NoTasks;
            var tasks = new Task<int>[NoTasks];
            for (var t = 0; t < tasks.Length; t++)
            {
                tasks[t] = Task.Run(() =>
                {
                    var check = 0;
                    for (int i = n; i > 0; i--)
                        check += TreeNode.bottomUpTree(depth).itemCheck();
                    return check;
                });
            }

            int check = tasks[0].Result;
            for (var t = 1; t < tasks.Length; t++)
                check += tasks[t].Result;
            results[i] = string.Concat(n * NoTasks, "\t trees of depth ",
                depth, "\t check: ", check);
        }

        for (var i = 0; i < results.Length; i++)
            Log.WriteLine(results[i]);

        Log.WriteLine(string.Concat("long lived tree of depth ", maxDepth,
            "\t check: ", longLivedTree.itemCheck()));
    }

    private class TreeNode
    {
        private readonly TreeNode left, right;

        internal TreeNode(TreeNode left, TreeNode right)
        {
            this.left = left;
            this.right = right;
        }

        internal static TreeNode bottomUpTree(int depth)
        {
            if (depth > 0)
            {
                return new TreeNode(
                    bottomUpTree(depth - 1),
                    bottomUpTree(depth - 1));
            }

            return new TreeNode(null, null);
        }

        internal int itemCheck()
        {
            if (left == null) return 1;
            return 1 + left.itemCheck() + right.itemCheck();
        }
    }
}