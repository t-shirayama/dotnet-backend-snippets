using DotnetBackendSnippets.Linq;

namespace DotnetBackendSnippets.Tests.Linq;

// テスト補助: LINQ Reverse Lookup Samples の共有データを定義する。
public sealed partial class LinqReverseLookupSamplesTests
{
    // テスト意図: Select Many With Parent / Skips Null Children And Keeps Parent Values を確認する。
    [Fact]
    public void SelectManyWithParent_SkipsNullChildrenAndKeepsParentValues()
    {
        var carts = new[]
        {
            new Cart("cart-1", [new CartItem("book"), new CartItem("pen")]),
            new Cart("cart-2", null),
            new Cart("cart-3", [new CartItem("mug")]),
        };

        var result = LinqReverseLookupSamples.SelectManyWithParent(
            carts,
            cart => cart.Items,
            (cart, item) => $"{cart.Id}:{item.Sku}");

        Assert.Equal(["cart-1:book", "cart-1:pen", "cart-3:mug"], result);
    }
}
