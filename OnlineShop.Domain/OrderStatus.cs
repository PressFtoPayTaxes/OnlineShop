namespace OnlineShop.Domain
{
    public enum OrderStatus
    {
        WaitForPayment,
        Paid,
        Cancelled,
        Delivered,
        Finished
    }
}