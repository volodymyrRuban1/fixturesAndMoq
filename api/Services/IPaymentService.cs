﻿namespace Services 
{
    public interface IPaymentService 
    {
        bool Charge(double total, ICard card);
    }
}