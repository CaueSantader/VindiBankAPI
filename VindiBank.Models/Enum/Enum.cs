namespace VindiBank.Models.Enum
{
    public enum MetodoAPI
    {
        createAccount,
        getAccounts,
        accountDeactivation,
        accountTransfer
    }

    public enum PersonType
    {
        Individual,    // CPF
        Business       // CNPJ
    }
}
