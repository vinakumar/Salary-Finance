namespace FullStack.Domain.Catalog.Classification.Hierarchy.Taxonomy;

/// <summary>
/// A node in the product taxonomy tree.
/// Demonstrates deep namespace nesting for client generation testing.
/// </summary>
public sealed class ProductTaxonomyNode
{
    /// <summary>The node identifier.</summary>
    public int NodeId { get; set; } = 100;

    /// <summary>The display label for this node.</summary>
    public string Label { get; set; } = "Uncategorized";

    /// <summary>The depth level in the taxonomy tree.</summary>
    public int Depth { get; set; } = 0;

    /// <summary>Whether this node is a leaf (has no children).</summary>
    public bool IsLeaf { get; set; } = true;

    /// <summary>The child nodes in the taxonomy hierarchy.</summary>
    public IList<ProductTaxonomyNode> Children { get; set; } = [];
}
