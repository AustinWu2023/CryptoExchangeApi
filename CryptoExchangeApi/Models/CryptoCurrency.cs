using System;
using System.Collections.Generic;

namespace CryptoExchangeApi.Models;

public partial class CryptoCurrency
{
    public int Id { get; set; }

    public string CurrencyCode { get; set; } = null!;

    public string CurrencyName { get; set; } = null!;

    public decimal ExchangeRate { get; set; }

    public DateTime UpdatedAtUtc { get; set; }
}
