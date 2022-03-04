using Message;

namespace Base;

public static class ItemHelper
{
    public static ItemType GetItemType(int itemId)
    {
        return (ItemType) (itemId / 100000);
    }

    public static CurrencyType ToCurrency(this int itemId)
    {
        A.Ensure(GetItemType(itemId) == ItemType.Currency, code: Code.ItemNotCurrency);
        return (CurrencyType) itemId;
    }
}