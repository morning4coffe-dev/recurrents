﻿namespace ProjectSBS.Services.Items.Billing;

public interface IBillingService
{

    List<DateOnly> GetFuturePayments(DateOnly initialDate, Period periodType, int recurEvery, int numberOfPayments = 20);

    DateOnly GetNextBillingDate(DateOnly date, Period periodType, int recurEvery);
}