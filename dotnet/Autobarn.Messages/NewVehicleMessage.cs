namespace Autobarn.Messages;
public class NewVehicleMessage {
	public string Registration { get; set; } = String.Empty;

	/// <summary>The make of car, e.g. Ford, Volkswagen, Ferrari</summary>
	public string Make { get; set; } = String.Empty;
	public string Model { get; set; } = String.Empty;

	public string Color { get; set; } = String.Empty;
	public int Year { get; set; }

	public DateTimeOffset ListedAt { get; set; }

	public override string ToString()
		=> $"{Registration} ({Make} {Model}, {Color}, {Year}) - listed at {ListedAt:O}";

	public NewVehiclePriceMessage WithPrice(int price, string currencyCode)
		=> new () {
			Color = this.Color,
			Year = this.Year,
			ListedAt = this.ListedAt,
			Make = this.Make,
			Model = this.Model,
			Registration = this.Registration,
			CurrencyCode = currencyCode,
			Price = price
		};
}

public class NewVehiclePriceMessage : NewVehicleMessage {
	public int Price { get; set; }
	public string CurrencyCode { get; set; } = String.Empty;

}
