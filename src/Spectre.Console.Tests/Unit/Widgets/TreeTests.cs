namespace Spectre.Console.Tests.Unit;

[ExpectationPath("Widgets/Tree")]
public class TreeTests
{
    [Fact]
    [Expectation("Render")]
    public Task Should_Render_Tree_Correctly()
    {
        // Given
        var console = new TestConsole();

        var tree = new Tree(new Text("Root node")).Guide(TreeGuide.DoubleLine);

        var nestedChildren = Enumerable.Range(0, 10).Select(x => new Text($"multiple\nline {x}"));
        var child2 = new TreeNode(new Text("child2"));
        var child2Child = new TreeNode(new Text("child2-1"));
        child2.AddNode(child2Child);
        child2Child.AddNode(new TreeNode(new Text("Child2-1-1\nchild")));
        var child3 = new TreeNode(new Text("child3"));
        var child3Child = new TreeNode(new Text("single leaf\nmultiline"));
        child3Child.AddNode(new TreeNode(new Calendar(2021, 01)));
        child3.AddNode(child3Child);

        tree.AddNode("child1").AddNodes(nestedChildren);
        tree.AddNode(child2);
        tree.AddNode(child3);
        tree.AddNode("child4");

        // When
        console.Write(tree);

        // Then
        return Verifier.Verify(console.Output);
    }

    [Fact]
    [Expectation("Render_NoChildren")]
    public Task Should_Render_Tree_With_No_Child_Nodes_Correctly()
    {
        // Given
        var console = new TestConsole();
        var tree = new Tree(new Text("Root node"));

        // When
        console.Write(tree);

        // Then
        return Verifier.Verify(console.Output);
    }

    [Fact]
    public void Should_Throw_If_Tree_Contains_Cycles()
    {
        // Given
        var console = new TestConsole();

        var child2 = new TreeNode(new Text("child 2"));
        var child3 = new TreeNode(new Text("child 3"));
        var child1 = new TreeNode(new Text("child 1"));
        child1.AddNodes(child2, child3);
        var root = new TreeNode(new Text("Branch Node"));
        root.AddNodes(child1);
        child2.AddNode(root);

        var tree = new Tree("root node");
        tree.AddNodes(root);

        // When
        var result = Record.Exception(() => console.Write(tree));

        // Then
        result.ShouldBeOfType<CircularTreeException>();
    }

    [Fact]
    [Expectation("Render_NoChildren_OfCollapsed")]
    public void Should_Render_Tree_With_No_Child_Of_Collapsed_Nodes_Correctly()
    {
        // Given
        var console = new TestConsole();
        var tree = new Tree(new Text("Root node"));
        var node1 = new TreeNode(new Text("Node level 1"));
        node1.AddNode(new TreeNode(new Text("Node level 2")));
        tree.AddNode(node1);
        node1.Expanded = false;

        // When
        console.Write(tree);

        // Then
        console.Output.SplitLines()
            .Select(x => x.Trim())
            .ToArray()
            .ShouldBeEquivalentTo(new[]
            {
                "Root node",
                "└── Node level 1",
                string.Empty,
            });
    }

    [Fact]
    [Expectation("Render_NoChildren_IfRouteCollapsed")]
    public void Should_Render_Tree_With_No_Child_If_Route_Collapsed_Correctly()
    {
        // Given
        var console = new TestConsole();
        var tree = new Tree(new Text("Root node"));
        var node1 = new TreeNode(new Text("Node level 1"));
        node1.AddNode(new TreeNode(new Text("Node level 2")));
        tree.AddNode(node1);
        tree.Expanded = false;

        // When
        console.Write(tree);

        // Then
        console.Output.SplitLines()
            .Select(x => x.Trim())
            .ToArray()
            .ShouldBeEquivalentTo(new[]
            {
                "Root node",
                string.Empty,
            });
    }

    [Fact]
    [Expectation("Render_Dotted")]
    public Task Should_Render_Tree_With_Dotted_Guide_Correctly()
    {
        // Given
        var console = new TestConsole();

        var tree = new Tree(new Text("Root node")).Guide(TreeGuide.Dotted);

        var child1 = new TreeNode(new Text("child1"));
        var child1Grandchild1 = new TreeNode(new Text("grandchild1"));
        var child1Grandchild2 = new TreeNode(new Text("grandchild2\nmultiline"));
        child1.AddNodes(child1Grandchild1, child1Grandchild2);

        var child2 = new TreeNode(new Text("child2"));
        var child2Grandchild = new TreeNode(new Text("grandchild"));
        var child2GreatGrandchild = new TreeNode(new Text("great-grandchild"));
        child2Grandchild.AddNode(child2GreatGrandchild);
        child2.AddNode(child2Grandchild);

        tree.AddNodes(child1, child2);

        // When
        console.Write(tree);

        // Then
        return Verifier.Verify(console.Output);
    }

    [Fact]
    [Expectation("Render_Dotted_DeepNesting")]
    public Task Should_Render_Tree_With_Dotted_Guide_And_Deep_Nesting_Correctly()
    {
        // Given
        var console = new TestConsole();

        var tree = new Tree(new Text("Root")).Guide(TreeGuide.Dotted);

        var level1 = new TreeNode(new Text("Level 1"));
        var level2 = new TreeNode(new Text("Level 2"));
        var level3 = new TreeNode(new Text("Level 3"));
        var level4a = new TreeNode(new Text("Level 4a"));
        var level4b = new TreeNode(new Text("Level 4b"));
        var level4c = new TreeNode(new Text("Level 4c"));

        level3.AddNodes(level4a, level4b, level4c);
        level2.AddNode(level3);
        level1.AddNode(level2);
        tree.AddNode(level1);

        // When
        console.Write(tree);

        // Then
        return Verifier.Verify(console.Output);
    }
}