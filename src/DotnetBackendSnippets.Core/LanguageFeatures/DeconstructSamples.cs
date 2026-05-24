namespace DotnetBackendSnippets.LanguageFeatures;

/// <summary>
/// 分解宣言と Deconstruct メソッドの使い方を示します。
/// </summary>
public static class DeconstructSamples
{
    /// <summary>
    /// 顧客名を分解して表示用の氏名に整形します。
    /// </summary>
    /// <param name="customer">顧客名。</param>
    /// <returns>姓と名を並べた文字列。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="customer"/> が null です。</exception>
    public static string FormatCustomer(CustomerName customer)
    {
        ArgumentNullException.ThrowIfNull(customer);

        var (lastName, firstName) = customer;

        return $"{lastName} {firstName}";
    }

    /// <summary>
    /// レコードを分解して商品コード文字列に整形します。
    /// </summary>
    /// <param name="product">商品コード。</param>
    /// <returns>カテゴリと番号を連結した文字列。</returns>
    public static string FormatRecordProduct(ProductCode product)
    {
        var (category, number) = product;

        return $"{category}-{number:0000}";
    }
}

/// <summary>
/// 姓名を Deconstruct で分解できる顧客名です。
/// </summary>
public sealed class CustomerName
{
    /// <summary>
    /// <see cref="CustomerName"/> クラスの新しいインスタンスを作成します。
    /// </summary>
    /// <param name="firstName">名。</param>
    /// <param name="lastName">姓。</param>
    public CustomerName(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }

    /// <summary>
    /// 名を取得します。
    /// </summary>
    /// <value>名。</value>
    public string FirstName { get; }

    /// <summary>
    /// 姓を取得します。
    /// </summary>
    /// <value>姓。</value>
    public string LastName { get; }

    /// <summary>
    /// 顧客名を姓と名に分解します。
    /// </summary>
    /// <param name="lastName">分解された姓。</param>
    /// <param name="firstName">分解された名。</param>
    public void Deconstruct(out string lastName, out string firstName)
    {
        lastName = LastName;
        firstName = FirstName;
    }
}

/// <summary>
/// カテゴリと番号で構成する商品コードです。
/// </summary>
/// <param name="Category">カテゴリ。</param>
/// <param name="Number">番号。</param>
public sealed record ProductCode(string Category, int Number);
