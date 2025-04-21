namespace MyApi.Models;

public class Order{
    public int Id {get;set;}
    public int ProductId {get;set;}
    public string? Description {get;set;}
    public int Quantity {get;set;}
    public decimal Amount {get;set;}
    public decimal Price {get;set;}
    public decimal Total {get;set;}
    public decimal Interest {get;set;}
    public DateTime OrderDate {get;set;}
}