namespace Marketplace.Domain.Exceptions;

// أي مخالفة لقواعد العمل الأساسية (مش validation بسيط زي "الإيميل فاضي")
public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}

public class EmailAlreadyExistsException : DomainException
{
    public EmailAlreadyExistsException(string email)
        : base($"An account with email '{email}' already exists.") { }
}

public class InvalidCredentialsException : DomainException
{
    public InvalidCredentialsException()
        : base("Invalid email or password.") { }
}

public class InvalidRefreshTokenException : DomainException
{
    public InvalidRefreshTokenException()
        : base("Refresh token is invalid or expired.") { }
}

public class ProductNotFoundException : DomainException
{
    public ProductNotFoundException(Guid id)
        : base($"Product with id '{id}' was not found, or does not belong to you.") { }
}

public class SkuAlreadyExistsException : DomainException
{
    public SkuAlreadyExistsException(string sku)
        : base($"A product with SKU '{sku}' already exists.") { }
}

public class SellerNotApprovedException : DomainException
{
    public SellerNotApprovedException()
        : base("Your seller account is not approved yet. You cannot add products until an admin approves your store.") { }
}

public class CartEmptyException : DomainException
{
    public CartEmptyException()
        : base("Your cart is empty. Add items before checking out.") { }
}

public class InsufficientStockException : DomainException
{
    public InsufficientStockException(string productName, int available, int requested)
        : base($"Insufficient stock for '{productName}'. Available: {available}, requested: {requested}.") { }
}

public class InvalidCouponException : DomainException
{
    public InvalidCouponException(string reason) : base($"Coupon is not valid: {reason}") { }
}

public class DuplicateReviewException : DomainException
{
    public DuplicateReviewException()
        : base("You have already reviewed this product. You can edit your existing review instead.") { }
}

public class PaymentAlreadyExistsException : DomainException
{
    public PaymentAlreadyExistsException()
        : base("A payment has already been initiated for this order.") { }
}

public class OrderNotPayableException : DomainException
{
    public OrderNotPayableException(string reason) : base($"This order cannot be paid: {reason}") { }
}

public class CategorySlugExistsException : DomainException
{
    public CategorySlugExistsException(string slug) : base($"A category with slug '{slug}' already exists.") { }
}

public class CouponCodeExistsException : DomainException
{
    public CouponCodeExistsException(string code) : base($"A coupon with code '{code}' already exists.") { }
}
