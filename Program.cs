using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

if (args.Length != 0 && !File.Exists(args[0]))
{
    Console.Error.WriteLine("指定されたファイル\"{0}\"が見つかりませんでした。\n", args[0]);
    PrintUsage();
    return -1;
}

try
{

    string text;
    using (TextReader reader = GetReader(args))
    {
        text = reader.ReadToEnd();
    }

    if (text == null || text.Length == 0)
    {
        return -127;
    }

    SyntaxTree tree = CSharpSyntaxTree.ParseText(text);
    SyntaxNode root = tree.GetRoot();
    var rewriter = new RemoverRewriter();
    var newRoot = rewriter.Visit(root);
    Console.Out.WriteLine(newRoot.ToFullString());
    return 0;
}
catch (Exception ex)
{
    Console.Error.WriteLine("予期しないエラーが発生しました。");
    Console.Error.WriteLine(ex.ToString());
    return -255;
}

void PrintUsage()
{
    Console.Error.WriteLine(@"CommentRemover: C#コメント除去コマンド

Usage: CommentRemover.exe <コメント除去対象のC#ソースコードパス>
");
}

TextReader GetReader(string[] param)
{
    if (param.Length != 0)
    {
        TextReader reader = new StreamReader(param[0]);
        return reader;
    }
    else
    {
        TextReader reader = Console.In;
        return reader;
    }

}

internal class RemoverRewriter : CSharpSyntaxRewriter
{

    public override SyntaxTrivia VisitTrivia(SyntaxTrivia trivia)
    {
        switch(trivia.Kind())
        {
            case SyntaxKind.SingleLineCommentTrivia:
            case SyntaxKind.MultiLineCommentTrivia:
            case SyntaxKind.SingleLineDocumentationCommentTrivia:
            case SyntaxKind.MultiLineDocumentationCommentTrivia:
                return default;
        }
        return base.VisitTrivia(trivia);
    }

}
