using GoodStuff.CartApi.Presentation.Controllers;

namespace GoodStuff.CartApi.Presentation.Extensions;

public static partial class LoggerExtension
{
    [LoggerMessage(LogLevel.Warning, "Validation failed while adding item to cart {userId}")]
    public static partial void LogValidationFailedWhileAddingItemToCartUserId(this ILogger<CartController> logger, Exception ex, string userId);

    [LoggerMessage(LogLevel.Error, "Unexpected error while adding item to cart {userId}")]
    public static partial void LogUnexpectedErrorWhileAddingItemToCartUserId(this ILogger<CartController> logger, Exception ex, string userId);

    [LoggerMessage(LogLevel.Warning, "Validation failed while retrieving cart {userId}")]
    public static partial void LogValidationFailedWhileRetrievingCartUserid(this ILogger<CartController> logger, Exception ex, string userId);

    [LoggerMessage(LogLevel.Error, "Unexpected error while retrieving cart {userId}")]
    public static partial void LogUnexpectedErrorWhileRetrievingCartUserid(this ILogger<CartController> logger, Exception ex, string userId);
}