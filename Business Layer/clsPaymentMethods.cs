using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Models.DTO;

namespace Business_Layer;

public class clsPaymentMethods : BaseService

{
    public clsPaymentMethods(IConfiguration config) : base(config) { }

    public async Task<int?> CheckIfCardExist(PurchaseGameDTO request)
    {
        return await new Data_Access_Layer.PaymentMethodsData(_config)
            .CheckIfCardExist(request);
    }

    public async Task<int?> CreateNewPaymentMethod(
    int userId,
    PurchaseGameDTO request)
    {
        var data = await new Data_Access_Layer.PaymentMethodsData(_config)
            .CreateNewPaymentMethod(userId, request);
        if (data == null)
            throw new ArgumentException("Card is not created is the data base");
        return data;
    }

    public async Task<List<CardDTO>?> GetUserCardsAsync(int userId)
    {
        return await new Data_Access_Layer.PaymentMethodsData(_config)
            .GetUserCardsAsync(userId);
    }
    public async Task<bool?> ChangeDefaultCardAsync(int cardId, int currentUserId)
    {
        return await new Data_Access_Layer.PaymentMethodsData(_config).
            ChangeDefaultCardAsync(cardId, currentUserId);
    }
}
