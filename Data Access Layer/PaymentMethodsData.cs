using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using GameZoneBack.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Models.Data.enums;
using Models.DTO;

namespace Data_Access_Layer;

public class PaymentMethodsData : BaseData
{
    public PaymentMethodsData(IConfiguration config) : base(config) { }

    public async Task<int?> CheckIfCardExist(PurchaseGameDTO request)
    {
        using var context = CreateDbContext();

        var expiryDate = DateOnly.FromDateTime(request.expiryDate);

        var paymentMethodId = await context.PaymentMethods
            .AsNoTracking()
            .Where(pm =>
                pm.CardHolder == request.cardHolder &&
                pm.CardNumber == request.cardNumber &&
                pm.ExpiryDate == expiryDate &&
                pm.CardTypeId == (int)request.cardType)
            .Select(pm => (int?)pm.PaymentMethodId)
            .FirstOrDefaultAsync();

        return paymentMethodId;
    }
    public async Task<int?> CreateNewPaymentMethod(
    int userId,
    PurchaseGameDTO request)
    {
        using var context = CreateDbContext();

        var personId = await context.Users
            .Where(u => u.UserId == userId)
            .Select(u => (int?)u.PersonId)
            .FirstOrDefaultAsync();

        if (!personId.HasValue)
            return null;

        var hasPaymentMethods = await context.PaymentMethods
            .AnyAsync(pm => pm.PersonId == personId.Value);

        var paymentMethod = new PaymentMethod
        {
            PersonId = personId.Value,
            CardHolder = request.cardHolder.Trim(),
            CardNumber = request.cardNumber.Trim(),
            ExpiryDate = DateOnly.FromDateTime(request.expiryDate),
            IsDefault = !hasPaymentMethods,
            CreatedAt = DateTime.UtcNow,
            CardTypeId = (int)request.cardType
        };

        await context.PaymentMethods.AddAsync(paymentMethod);

        var isCreated = await context.SaveChangesAsync() > 0;

        return isCreated
            ? paymentMethod.PaymentMethodId
            : null;
    }
    public async Task<List<CardDTO>?> GetUserCardsAsync(int userId)
    {
        using var context = CreateDbContext();

        var personId = await context.Users
            .Where(u => u.UserId == userId)
            .Select(u => (int?)u.PersonId)
            .FirstOrDefaultAsync();

        if (!personId.HasValue)
            return null;

        return await context.PaymentMethods.Where(pm => pm.PersonId == personId)
            .Select(res => new CardDTO
            {
                cardId = res.PaymentMethodId,
                cardHolder = res.CardHolder,
                cardNumber = "************" + res.CardNumber.Substring(res.CardNumber.Length - 4),
                cardType = (enCardType)res.CardTypeId,
                isDeafult = res.IsDefault,
                expiryDate = res.ExpiryDate,
            }).ToListAsync();
    }

    public async Task<bool?> ChangeDefaultCardAsync(int cardId, int currentUserId)
    {
        using var context = CreateDbContext();

        var personId = await context.Users
            .Where(u => u.UserId == currentUserId)
            .Select(u => (int?)u.PersonId)
            .FirstOrDefaultAsync();

        if (!personId.HasValue)
            return null;

        var card = await context.PaymentMethods
            .FirstOrDefaultAsync(pm =>
                pm.PaymentMethodId == cardId &&
                pm.PersonId == personId.Value);

        if (card == null)
            return null;

        // Already the default card
        if (card.IsDefault)
            return true;

        var defaultCard = await context.PaymentMethods
            .FirstOrDefaultAsync(pm =>
                pm.PersonId == personId.Value &&
                pm.IsDefault);

        card.IsDefault = true;

        if (defaultCard != null)
            defaultCard.IsDefault = false;

        return await context.SaveChangesAsync() > 0;
    }
}
