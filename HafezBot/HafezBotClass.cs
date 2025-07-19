using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace HafezBot;

public class HafezBotClass
{
    private readonly TelegramBotClient _bot;
    private readonly string _url;
    private readonly CancellationToken _cancellationToken;

    // Dictionary for labels
    private readonly Dictionary<string, string> _labels = new()
    {
        { "IR_COIN_1G", "سکه ۱ گرمی" },
        { "IR_COIN_QUARTER", "ربع سکه" },
        { "IR_COIN_EMAMI", "سکه امامی" },
        { "IR_COIN_BAHAR","سکه بهار آزادی"},
        { "XAUUSD","انس طلا"},
        { "IR_COIN_HALF","نیم سکه"},
        { "IR_GOLD_18K", "طلای ۱۸ عیار" },
         { "IR_GOLD_24K","طلای ۲۴ عیار"},
        { "BTC", "بیتکوین" },
        { "USDT", "تتر" },
        { "ETH","اتریوم"},
        { "USD", "دلار آمریکا" },
        { "CAD", "دلار کانادا" },
        { "EUR", "یورو" },
        { "AED", "درهم امارات" }
    };

    public HafezBotClass(string token, string MarketApiUrl, CancellationToken cancellationToken)
    {
        _bot = new TelegramBotClient(token, cancellationToken: cancellationToken);
        _url = MarketApiUrl;
        _cancellationToken = cancellationToken;

        _bot.OnMessage += OnMessage;
        _bot.OnUpdate += OnUpdate;
        _bot.OnError += OnError;
    }

    public async Task StartAsync()
    {
        var me = await _bot.GetMe();
        Console.WriteLine($"@{me.Username} is running...");
    }

    private async Task OnMessage(Message msg, UpdateType _)
    {
        if (msg.Text == "/start")
        {
            await ShowMainMenu(msg.Chat.Id);
        }
    }

    private async Task OnUpdate(Update update)
    {
        if (update.Type == UpdateType.CallbackQuery)
        {
            var chatId = update.CallbackQuery.Message.Chat.Id;
            var data = update.CallbackQuery.Data;

            switch (data)
            {
                case "get_fal":
                    await ShowFal(chatId);
                    break;

                case "main_menu":
                    await ShowMainMenu(chatId);
                    break;

                case "gold_menu":
                    await ShowGoldMenu(chatId);
                    break;

                case "crypto_menu":
                    await ShowCryptoMenu(chatId);
                    break;

                case "currency_menu":
                    await ShowCurrencyMenu(chatId);
                    break;

                case "gold_1g":
                    await HandleMarketDataRequest(chatId, "IR_COIN_1G");
                    break;
                case "gold_quarter":
                    await HandleMarketDataRequest(chatId, "IR_COIN_QUARTER");
                    break;
                case "gold_half":
                    await HandleMarketDataRequest(chatId, "IR_COIN_HALF");
                    break;
                case "gold_full":
                    await HandleMarketDataRequest(chatId, "IR_COIN_EMAMI");
                    break;
                case "gold_bahar":
                    await HandleMarketDataRequest(chatId, "IR_COIN_BAHAR");
                    break;
                case "gold_18":
                    await HandleMarketDataRequest(chatId, "IR_GOLD_18K");
                    break;
                case "gold_24":
                    await HandleMarketDataRequest(chatId, "IR_GOLD_24K");
                    break;
                case "gold_ons":
                    await HandleMarketDataRequest(chatId, "XAUUSD");
                    break;
                case "btc":
                    await HandleMarketDataRequest(chatId, "BTC");
                    break;
                case "usdt":
                    await HandleMarketDataRequest(chatId, "USDT");
                    break;
                case "Ethereum":
                    await HandleMarketDataRequest(chatId, "ETH");
                    break;
                case "usd":
                    await HandleMarketDataRequest(chatId, "USD");
                    break;
                case "cad":
                    await HandleMarketDataRequest(chatId, "CAD");
                    break;
                case "eur":
                    await HandleMarketDataRequest(chatId, "EUR");
                    break;
                case "aed":
                    await HandleMarketDataRequest(chatId, "AED");
                    break;

                default:
                    await _bot.SendMessage(chatId, "گزینه نامشخص");
                    break;
            }
        }
    }

    private async Task HandleMarketDataRequest(long chatId, string symbol)
    {
        try
        {
            var response = await FetchMarketDataAsync(_url);
            MarketItems? dataItem = null;
            dataItem = response?.Gold?.FirstOrDefault(x => x.symbol.Equals(symbol, StringComparison.OrdinalIgnoreCase))
            ?? response?.Currency?.FirstOrDefault(x => x.symbol.Equals(symbol, StringComparison.OrdinalIgnoreCase))
            ?? response?.CryptoCurrency?.FirstOrDefault(x => x.symbol.Equals(symbol, StringComparison.OrdinalIgnoreCase));

            if (dataItem != null)
            {
                await _bot.SendMessage(chatId,
                    $"{dataItem.name}\nقیمت: {dataItem.price} {dataItem.unit}\n🕒 {dataItem.date} {dataItem.time}");
            }
            else
            {
                await _bot.SendMessage(chatId, $"❌ اطلاعات {GetLabel(symbol)} یافت نشد.");
            }
        }
        catch (Exception ex)
        {
            await _bot.SendMessage(chatId, $"❌ خطا: {ex.Message}");
        }
    }

    private async Task<MarketResponse?> FetchMarketDataAsync(string url)
    {
        using var http = new HttpClient();
        var response = await http.GetAsync(url, _cancellationToken);
        var json = await response.Content.ReadAsStringAsync(_cancellationToken);
        return JsonSerializer.Deserialize<MarketResponse>(json);
    }

    async Task ShowMainMenu(long chatId)
    {
        var menu = new InlineKeyboardMarkup(new[]
        {
        new[] { InlineKeyboardButton.WithCallbackData("🟡 طلا", "gold_menu") },
        new[] { InlineKeyboardButton.WithCallbackData("🪙 رمز ارزها", "crypto_menu") },
        new[] { InlineKeyboardButton.WithCallbackData("💵 واحد های پول", "currency_menu") },
        new[] { InlineKeyboardButton.WithCallbackData("📜 گرفتن فال حافظ", "get_fal") }
    });

        await _bot.SendMessage(chatId, "سلام رفیق! کدام یک از خدمات زیر رو میخوای ؟", replyMarkup: menu);
    }

    async Task ShowGoldMenu(long chatId)
    {
        var menu = new InlineKeyboardMarkup(new[]
        {
        new[] { InlineKeyboardButton.WithCallbackData("سکه ۱ گرمی", "gold_1g") },
        new[] { InlineKeyboardButton.WithCallbackData("ربع سکه", "gold_quarter") },
         new[] { InlineKeyboardButton.WithCallbackData("نیم سکه", "gold_half") },
        new[] { InlineKeyboardButton.WithCallbackData("سکه امامی", "gold_full") },
        new[] { InlineKeyboardButton.WithCallbackData("سکه بهار آزادی", "gold_bahar") },
        new[] { InlineKeyboardButton.WithCallbackData("طلای ۱۸ عیار", "gold_18") },
        new[] { InlineKeyboardButton.WithCallbackData("طلای ۲۴ عیار", "gold_24") },
        new[] { InlineKeyboardButton.WithCallbackData("انس طلا", "gold_ons") },
        new[] { InlineKeyboardButton.WithCallbackData("↩️ بازگشت", "main_menu") }

    });

        await _bot.SendMessage(chatId, "یکی از موارد طلا را انتخاب کنید:", replyMarkup: menu);
    }

    async Task ShowCryptoMenu(long chatId)
    {
        var menu = new InlineKeyboardMarkup(new[]
        {
        new[] { InlineKeyboardButton.WithCallbackData("بیتکوین", "btc") },
        new[] { InlineKeyboardButton.WithCallbackData("تتر", "usdt") },
        new[] { InlineKeyboardButton.WithCallbackData("اتریوم", "Ethereum") },
        new[] { InlineKeyboardButton.WithCallbackData("↩️ بازگشت", "main_menu") }
    });

        await _bot.SendMessage(chatId, "یکی از رمز ارزها را انتخاب کنید:", replyMarkup: menu);
    }

    async Task ShowCurrencyMenu(long chatId)
    {
        var menu = new InlineKeyboardMarkup(new[]
        {
        new[] { InlineKeyboardButton.WithCallbackData("دلار آمریکا", "usd") },
        new[] { InlineKeyboardButton.WithCallbackData("دلار کانادا", "cad") },
        new[] { InlineKeyboardButton.WithCallbackData("یورو", "eur") },
        new[] { InlineKeyboardButton.WithCallbackData("درهم امارات", "aed") },
        new[] { InlineKeyboardButton.WithCallbackData("↩️ بازگشت", "main_menu") }
    });

        await _bot.SendMessage(chatId, "یکی از ارزها را انتخاب کنید:", replyMarkup: menu);
    }

    private Task OnError(Exception exception, HandleErrorSource source)
    {
        Console.WriteLine(exception);
        return Task.CompletedTask;
    }
    private string GetLabel(string key) =>
        _labels.TryGetValue(key, out var label) ? label : "نامشخص";
    async Task ShowFal(long chatId)
    {
        using var http = new HttpClient();
        var response = await http.GetAsync("https://localhost:7150/api/fal", _cancellationToken);
        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync(_cancellationToken);
            var result = JsonSerializer.Deserialize<FaalResponse>(json);
            await _bot.SendMessage(chatId, $"📜 *شعر:*\n{result?.Poem}\n\n📖 *تعبیر:*\n{result?.Interpretation}");
        }
        else
        {
            await _bot.SendMessage(chatId, "هیچ فالی پیدا نشد.");
        }
    }

}

