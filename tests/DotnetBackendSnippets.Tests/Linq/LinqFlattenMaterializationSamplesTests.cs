using DotnetBackendSnippets.Linq;

namespace DotnetBackendSnippets.Tests.Linq;

public sealed partial class LinqReverseLookupSamplesTests
{
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
