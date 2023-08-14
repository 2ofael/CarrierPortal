using CarrierPortal.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace CarrierPortal.Controllers
{
    public class PaymentsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }




        [HttpPost]
        public IActionResult CreatePaymentIntent()
        {
            StripeConfiguration.ApiKey = "sk_test_51Neu2iIuE0SL7TqEtXVVnRTiZ5uHAsJgZBsMl9oUE8TZQeGUqcZ6xPBKXS9b3g23I2suY3NopmaC8GUc365x6CHU005yfcGuf6"; // Replace with your actual Stripe secret key

            var options = new PaymentIntentCreateOptions
            {
                Amount = 2000, // Amount in cents
                Currency = "usd",
                PaymentMethodTypes = new List<string>
        {
            "card",
        },
            };

            var service = new PaymentIntentService();
            var paymentIntent = service.Create(options);

            return Json(new { ClientSecret = paymentIntent.ClientSecret });
        }

        [HttpPost]
        public IActionResult ProcessPayment([FromBody] ProcessPaymentModel model)
        {
            StripeConfiguration.ApiKey = "sk_test_51Neu2iIuE0SL7TqEtXVVnRTiZ5uHAsJgZBsMl9oUE8TZQeGUqcZ6xPBKXS9b3g23I2suY3NopmaC8GUc365x6CHU005yfcGuf6"; // Replace with your Stripe secret key

            var service = new PaymentIntentService();
            var paymentIntent = service.Get(model.PaymentIntentId);

            try
            {
                if (paymentIntent.Status == "succeeded")
                {
                    // Payment already succeeded, return a JSON response
                    return Json(new { success = true });
                }
                else if (paymentIntent.Status == "requires_payment_method")
                {
                    // Attempt to confirm the payment again
                    paymentIntent = service.Confirm(paymentIntent.Id);

                    if (paymentIntent.Status == "succeeded")
                    {
                        // Payment successfully confirmed, return a JSON response
                        return Json(new { success = true });
                    }
                    else
                    {
                        // Payment confirmation failed, return a JSON response
                        return Json(new { success = false });
                    }
                }
                else
                {
                    // Payment failed, return a JSON response
                    return Json(new { success = false });
                }
            }
            catch (StripeException ex)
            {
                // Handle any Stripe API exceptions
                return Json(new { success = false, error = ex.Message });
            }
        }






    }


}
