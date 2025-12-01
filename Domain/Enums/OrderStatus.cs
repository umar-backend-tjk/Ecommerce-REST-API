namespace Domain.Enums;

public enum OrderStatus
{
    New = 1,
    PendingPayment = 2,
    Paid = 3,
    Processing = 4,
    Shipped = 5,
    Completed = 6,
    Cancelled = 7
}