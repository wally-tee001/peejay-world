using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Stripe;
using Stripe.Checkout;

namespace peejayworld_mvc.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly IConfiguration _config;
        private readonly ILogger<CheckoutController> _logger;

        public CheckoutController(IConfiguration config, ILogger<CheckoutController> logger)
        {
            _config = config;
            _logger = logger;
        }

        public IActionResult Index()
        {
            // Temporary: return plain content to isolate view rendering errors
            return Content("Checkout view disabled for debugging");
        }

        [HttpPost]
        public async Task<IActionResult> CreateSession(decimal amount)
        {
            // amount expected in major units (e.g., 19.99)
            var currency = _config["Stripe:Currency"] ?? "usd";
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(amount * 100m),
                            Currency = currency,
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = "Peejay's World Order"
                            }
                        },
                        Quantity = 1
                    }
                },
                Mode = "payment",
                SuccessUrl = Url.Action("Success", "Checkout", null, Request.Scheme) + "?session_id={CHECKOUT_SESSION_ID}",
                CancelUrl = Url.Action("Cancel", "Checkout", null, Request.Scheme)
            };

            var service = new SessionService();
            var session = await service.CreateAsync(options);

            return Redirect(session.Url);
        }

        public IActionResult Success(string session_id)
        {
            ViewData["SessionId"] = session_id;
            return View();
        }

        public IActionResult Cancel()
        {
            return View();
        }

        [HttpPost]
        [Route("checkout/webhook")]
        public async Task<IActionResult> Webhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var sigHeader = Request.Headers["Stripe-Signature"].ToString();
            var webhookSecret = _config["Stripe:WebhookSecret"] ?? System.Environment.GetEnvironmentVariable("STRIPE_WEBHOOK_SECRET");

            try
            {
                var stripeEvent = EventUtility.ConstructEvent(json, sigHeader, webhookSecret);
                if (stripeEvent.Type == Events.CheckoutSessionCompleted)
                {
                    var session = stripeEvent.Data.Object as Session;
                    _logger.LogInformation("Checkout session completed: {SessionId}", session?.Id);

                    // Record payment in a simple payments table (create if missing) using raw SQL.
                    try
                    {
                        var connString = _config.GetConnectionString("DefaultConnection");
                        var password = _config["ConnectionStrings:DefaultPassword"] ?? System.Environment.GetEnvironmentVariable("PGPASSWORD") ?? System.Environment.GetEnvironmentVariable("DB_PASSWORD");
                        if (!string.IsNullOrWhiteSpace(password) && !connString.Contains("Password=", StringComparison.OrdinalIgnoreCase))
                        {
                            connString += $"Password={password};";
                        }

                        using (var conn = new Npgsql.NpgsqlConnection(connString))
                        {
                            await conn.OpenAsync();
                            var createSql = @"CREATE TABLE IF NOT EXISTS payments (
                                id SERIAL PRIMARY KEY,
                                session_id TEXT,
                                amount BIGINT,
                                currency TEXT,
                                paid_at TIMESTAMPTZ,
                                raw JSONB
                            );";
                            using (var cmd = conn.CreateCommand())
                            {
                                cmd.CommandText = createSql;
                                await cmd.ExecuteNonQueryAsync();
                            }

                            using (var insert = conn.CreateCommand())
                            {
                                insert.CommandText = "INSERT INTO payments(session_id, amount, currency, paid_at, raw) VALUES(@s, @a, @c, @p, @r)";
                                insert.Parameters.AddWithValue("@s", session?.Id ?? "");
                                insert.Parameters.AddWithValue("@a", session?.AmountTotal ?? 0L);
                                insert.Parameters.AddWithValue("@c", session?.Currency ?? "");
                                insert.Parameters.AddWithValue("@p", DateTime.UtcNow);
                                insert.Parameters.AddWithValue("@r", NpgsqlTypes.NpgsqlDbType.Jsonb, json);
                                await insert.ExecuteNonQueryAsync();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to record payment in DB");
                    }
                }

                return Ok();
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Webhook construct failed");
                return BadRequest();
            }
        }
    }
}

