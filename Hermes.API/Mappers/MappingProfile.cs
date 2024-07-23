using AutoMapper;
using Hermes.Application.DTOs;
using Hermes.Domain.Entities;

namespace Hermes.API.Mappers;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Address Mapping
        CreateMap<AddressDto, Address>().ReverseMap();

        // Category Mappings
        CreateMap<Category, CategoryDto>()
            .ForMember(dest => dest.ParentCategoryName, 
                       opt => opt.MapFrom(src => src.ParentCategory.Name));
        CreateMap<CreateCategoryDto, Category>();
        CreateMap<UpdateCategoryDto, Category>();

        // Cart Mappings
        CreateMap<Cart, CartDto>();
        CreateMap<CartItem, CartItemDto>();

        // Coupon Mappings
        CreateMap<Coupon, CouponDto>().ReverseMap();
        CreateMap<CreateCouponDto, Coupon>();
        CreateMap<UpdateCouponDto, Coupon>();

        // Order Mappings
        CreateMap<Order, OrderDto>()
            .ForMember(dest => dest.ShippingAddress, opt => opt.MapFrom(src => src.ShippingAddress))
            .ForMember(dest => dest.BillingAddress, opt => opt.MapFrom(src => src.BillingAddress));

        CreateMap<OrderItem, OrderItemDto>().ReverseMap();
        CreateMap<CreateOrderDto, Order>()
            .ForMember(dest => dest.ShippingAddress, opt => opt.MapFrom(src => src.ShippingAddress))
            .ForMember(dest => dest.BillingAddress, opt => opt.MapFrom(src => src.BillingAddress));

        // Product Mappings
        CreateMap<Product, ProductDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));
        CreateMap<Product, ProductDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
            .ForMember(dest => dest.Reviews, opt => opt.MapFrom(src => src.Reviews));
        CreateMap<CreateProductDto, Product>();
        CreateMap<UpdateProductDto, Product>();

        // Product Variant Mappings
        CreateMap<ProductVariant, ProductVariantDto>().ReverseMap();
        CreateMap<CreateProductVariantDto, ProductVariant>();
        CreateMap<UpdateProductVariantDto, ProductVariant>();

        // Product Variant Option Mappings
        CreateMap<ProductVariantOption, ProductVariantOptionDto>().ReverseMap();

        // Review Mappings
        CreateMap<Review, ReviewDto>().ReverseMap();
        CreateMap<CreateReviewDto, Review>();
        
        // User Mappings
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address));
        CreateMap<UserDto, User>()
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address));
    }
}