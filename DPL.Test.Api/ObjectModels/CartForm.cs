using System;
namespace DPL.Test.Api.ObjectModels
{
	public class CartForm
	{
		public CartForm()
		{
		}
		public Guid ProductID { get; set; }
		public string Name { get; set; }
		public decimal Amount { get; set; }
		public decimal QTY { get; set; }

	}
}

