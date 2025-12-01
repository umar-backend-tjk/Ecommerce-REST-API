using Application.Interfaces;

namespace Infrastructure.Helpers;

public static class OrderNumberGenerator
{
    public static async Task<string> GenerateOrderNumberAsync(IOrderRepository orderRepository)
    {
        var today = DateTime.UtcNow;
        var year = today.Year;
        var month = today.Month;

        var lastOrderNumber = await orderRepository.GetLastOrderNumberAsync(year, month);
        
        var sequence = 1;
        if (!string.IsNullOrEmpty(lastOrderNumber))
        {
            var lastSequencePart = lastOrderNumber.Split('-').Last();
            if (int.TryParse(lastSequencePart, out var lastSequence))
            {
                sequence = lastSequence + 1;
            }
        }

        if (sequence > 999999)
        {
            throw new InvalidOperationException("Order sequence overflow for current month");
        }

        return $"ORD-{year}-{month:00}-{sequence:000000}";
    }
}