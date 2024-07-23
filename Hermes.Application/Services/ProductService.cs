using AutoMapper;
using Hermes.Application.DTOs;
using Hermes.Application.Exceptions;
using Hermes.Application.Interfaces;
using Hermes.Domain.Entities;
using Hermes.Domain.Interfaces;

namespace Hermes.Application.Services;

public class ProductService(IUnitOfWork unitOfWork, IInventoryService inventoryService, IMapper mapper)
    : IProductService
{
    /// <summary>
    /// Retrieves a paged collection of all products.
    /// </summary>
    /// <param name="page">The page number to retrieve. Defaults to 1.</param>
    /// <param name="pageSize">The number of items per page. Defaults to 10.</param>
    /// <returns>A PagedResult object containing the retrieved products and pagination information.</returns>
    public async Task<PagedResult<ProductDto>> GetAllProductsAsync(int page, int pageSize)
    {
        var query = unitOfWork.Products.GetAllAsync();
        var skip = (page - 1) * pageSize;
        var productsQuery = query.Skip(skip).Take(pageSize);
        var products = await unitOfWork.Products.ExecuteQueryAsync(productsQuery);
        var totalCount = query.Count();

        return new PagedResult<ProductDto>
        {
            Items = mapper.Map<IEnumerable<ProductDto>>(products),
            CurrentPage = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    /// <summary>
    /// Retrieves a specific product by its ID.
    /// </summary>
    /// <param name="productId">The ID of the product to retrieve.</param>
    /// <returns>A ProductDto object representing the specified product, or null if no product with the given ID is found.</returns>
    public async Task<ProductDto?> GetProductByIdAsync(int productId)
    {
        var product = await unitOfWork.Products.GetByIdAsync(productId);
        if (product == null)
        {
            throw new NotFoundException("Product not found.");
        }

        return mapper.Map<ProductDto>(product);
    }

    /// <summary>
    /// Retrieves a paged collection of products associated with a specific category.
    /// </summary>
    /// <param name="categoryId">The ID of the category to filter products by.</param>
    /// <param name="page">The page number to retrieve. Defaults to 1.</param>
    /// <param name="pageSize">The number of items per page. Defaults to 10.</param>
    /// <returns>A PagedResult object containing the retrieved products and pagination information.</returns>
    public async Task<PagedResult<ProductDto>> GetProductsByCategoryAsync(int categoryId, int page, int pageSize)
    {
        if (!await unitOfWork.Categories.ExistsAsync(categoryId))
            throw new NotFoundException("Category not found.");

        var query = unitOfWork.Products.GetProductsByCategoryAsync(categoryId);
        var skip = (page - 1) * pageSize;
        var productsQuery = query.Skip(skip).Take(pageSize);
        var products = await unitOfWork.Products.ExecuteQueryAsync(productsQuery);
        return new PagedResult<ProductDto>
        {
            Items = mapper.Map<IEnumerable<ProductDto>>(products),
            CurrentPage = page,
            PageSize = pageSize,
            TotalCount = query.Count()
        };
    }

    /// <summary>
    /// Searches for products based on specified criteria.
    /// </summary>
    /// <param name="searchTerm">The search term to use for filtering.</param>
    /// <param name="page">The page number to retrieve. Defaults to 1.</param>
    /// <param name="pageSize">The number of items per page. Defaults to 10.</param>
    /// <param name="categoryId">Optional category ID to filter by.</param>
    /// <param name="minPrice">Optional minimum price filter.</param>
    /// <param name="maxPrice">Optional maximum price filter.</param>
    /// <param name="tags">Optional list of tags to filter by.</param>
    /// <param name="seller">Optional seller name to filter by.</param>
    /// <param name="sortBy">Optional field to sort by.</param>
    /// <param name="sortOrder">Optional sort order (ascending or descending).</param>
    /// <returns>A PagedResult object containing the retrieved products and pagination information.</returns>
    public async Task<PagedResult<ProductDto>> SearchProductsAsync(string searchTerm, int page, int pageSize,
        int? categoryId = null, decimal? minPrice = null,
        decimal? maxPrice = null, List<string>? tags = null, string? seller = null, string? sortBy = null,
        string? sortOrder = null)
    {
        var query = unitOfWork.Products.SearchProductsAsync(searchTerm, categoryId, minPrice, maxPrice, tags, seller,
            sortBy, sortOrder);
        var skip = (page - 1) * pageSize;
        var productsQuery = query.Skip(skip).Take(pageSize);
        var products = await unitOfWork.Products.ExecuteQueryAsync(productsQuery);
        return new PagedResult<ProductDto>
        {
            Items = mapper.Map<IEnumerable<ProductDto>>(products),
            CurrentPage = page,
            PageSize = pageSize,
            TotalCount = query.Count()
        };
    }

    /// <summary>
    /// Retrieves a paged collection of the top-selling products.
    /// </summary>
    /// <param name="pageSize">The number of items per page. Defaults to 10.</param>
    /// <returns>A PagedResult object containing the retrieved products and pagination information.</returns>
    public async Task<PagedResult<ProductDto>> GetTopSellingProductsAsync(int pageSize)
    {
        var query = unitOfWork.Products.GetTopSellingProductsAsync(pageSize);
        var products = await unitOfWork.Products.ExecuteQueryAsync(query);
        return new PagedResult<ProductDto>
        {
            Items = mapper.Map<IEnumerable<ProductDto>>(products),
            CurrentPage = 1,
            PageSize = pageSize,
            TotalCount = pageSize
        };
    }

    /// <summary>
    /// Retrieves a paged collection of the latest products.
    /// </summary>
    /// <param name="pageSize">The number of items per page. Defaults to 10.</param>
    /// <returns>A PagedResult object containing the retrieved products and pagination information.</returns>
    public async Task<PagedResult<ProductDto>> GetLatestProductsAsync(int pageSize)
    {
        var query = unitOfWork.Products.GetLatestProductsAsync(pageSize);
        var products = await unitOfWork.Products.ExecuteQueryAsync(query);
        return new PagedResult<ProductDto>
        {
            Items = mapper.Map<IEnumerable<ProductDto>>(products),
            CurrentPage = 1,
            PageSize = pageSize,
            TotalCount = pageSize
        };
    }

    /// <summary>
    /// Retrieves detailed information for a specific product.
    /// </summary>
    /// <param name="productId">The ID of the product to retrieve details for.</param>
    /// <returns>The retrieved ProductDto object, or null if no matching product is found.</returns>
    public async Task<ProductDto?> GetProductDetailsAsync(int productId)
    {
        var product = await unitOfWork.Products
            .GetByIdAsync(productId);

        if (product == null)
        {
            throw new NotFoundException("Product not found.");
        }

        var productDetailsDto = mapper.Map<ProductDto>(product);

        productDetailsDto.Variants =
            mapper.Map<List<ProductVariantDto>>(await unitOfWork.Products.GetProductVariantsAsync(productId));
        productDetailsDto.Reviews =
            mapper.Map<List<ReviewDto>>(await unitOfWork.Reviews.GetReviewsByProductAsync(productId));

        return productDetailsDto;
    }

    /// <summary>
    /// Creates a new product.
    /// </summary>
    /// <param name="productDto">The CreateProductDto object containing the product data to create.</param>
    /// <returns>A ProductDto object representing the newly created product.</returns>
    public async Task<ProductDto?> CreateProductAsync(CreateProductDto productDto)
    {
        var product = mapper.Map<Product>(productDto);
        await unitOfWork.Products.AddAsync(product);
        foreach (var variant in product.Variants)
        {
            await inventoryService.CreateInventoryForVariantAsync(variant.Id, variant.Quantity);
        }

        return mapper.Map<ProductDto>(product);
    }

    /// <summary>
    /// Updates an existing product.
    /// </summary>
    /// <param name="productId">The ID of the product to update.</param>
    /// <param name="productDto">The UpdateProductDto object containing the updated product data.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task UpdateProductAsync(int productId, UpdateProductDto productDto)
    {
        var product = await unitOfWork.Products.GetByIdAsync(productId);
        if (product == null)
        {
            throw new NotFoundException("Product not found.");
        }

        mapper.Map(productDto, product);
        await unitOfWork.Products.UpdateAsync(product);
        foreach (var variant in product.Variants)
        {
            await inventoryService.UpdateQuantityAsync(variant.Id, variant.Quantity);
        }
    }

    /// <summary>
    /// Deletes an existing product.
    /// </summary>
    /// <param name="productId">The ID of the product to delete.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task DeleteProductAsync(int productId)
    {
        var product = await unitOfWork.Products.GetByIdAsync(productId);
        if (product == null)
        {
            throw new NotFoundException("Product not found.");
        }

        await unitOfWork.Products.DeleteAsync(product);
    }

    /// <summary>
    /// Retrieves a product based on its associated product variant ID.
    /// </summary>
    /// <param name="variantId">The ID of the product variant associated with the product to retrieve.</param>
    /// <returns>The retrieved ProductDto object, or null if no matching product is found.</returns>
    public async Task<ProductDto?> GetProductByVariantIdAsync(int variantId)
    {
        var query = unitOfWork.Products.FindAsync(p => p.Variants.Any(v => v.Id == variantId));
        var product = await unitOfWork.Products.ExecuteQuerySingleAsync(query);
        if (product == null)
        {
            throw new NotFoundException("Product not found.");
        }

        return mapper.Map<ProductDto>(product);
    }

    /// <summary>
    /// Retrieves a collection of product variants associated with a specific product.
    /// </summary>
    /// <param name="productId">The ID of the product to retrieve variants for.</param>
    /// <returns>An IEnumerable of ProductVariantDto objects representing the product's variants.</returns>
    public async Task<IEnumerable<ProductVariantDto>> GetProductVariantsAsync(int productId)
    {
        return mapper.Map<IEnumerable<ProductVariantDto>>(await unitOfWork.Products.GetProductVariantsAsync(productId));
    }

    /// <summary>
    /// Retrieves a specific product variant by its ID.
    /// </summary>
    /// <param name="variantId">The ID of the product variant to retrieve.</param>
    /// <returns>A ProductVariantDto object representing the specified product variant, or null if no variant with the given ID is found.</returns>
    public async Task<ProductVariantDto?> GetProductVariantByIdAsync(int variantId)
    {
        var variant = await unitOfWork.Products.GetProductVariantByIdAsync(variantId);
        if (variant == null)
        {
            throw new NotFoundException("Product variant not found.");
        }

        return mapper.Map<ProductVariantDto>(variant);
    }

    /// <summary>
    /// Creates a new product variant for a specific product.
    /// </summary>
    /// <param name="productId">The ID of the product to create a variant for.</param>
    /// <param name="variantDto">The CreateProductVariantDto object containing the variant data to create.</param>
    /// <returns>A ProductVariantDto object representing the newly created variant.</returns>
    public async Task<ProductVariantDto?> CreateProductVariantAsync(int productId, CreateProductVariantDto variantDto)
    {
        var product = await unitOfWork.Products.GetByIdAsync(productId);
        if (product == null)
        {
            throw new NotFoundException("Product not found.");
        }

        var existingVariant = product.Variants.FirstOrDefault(v => v.SKU == variantDto.SKU);
        if (existingVariant != null)
            throw new BadRequestException("Product variant with the same SKU already exists.");

        var variant = mapper.Map<ProductVariant>(variantDto);
        variant.ProductId = productId;
        await unitOfWork.Products.AddProductVariantAsync(variant);

        return mapper.Map<ProductVariantDto>(variant);
    }

    /// <summary>
    /// Updates an existing product variant.
    /// </summary>
    /// <param name="variantId">The ID of the product variant to update.</param>
    /// <param name="variantDto">The UpdateProductVariantDto object containing the updated variant data.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task UpdateProductVariantAsync(int variantId, UpdateProductVariantDto variantDto)
    {
        var variant = await unitOfWork.Products.GetProductVariantByIdAsync(variantId);
        if (variant == null)
        {
            throw new NotFoundException("Product variant not found.");
        }

        mapper.Map(variantDto, variant);

        if (variantDto.Options != null)
        {
            foreach (var optionDto in variantDto.Options)
            {
                var option = variant.Options.FirstOrDefault(o => o.Name == optionDto.Name);
                if (option == null)
                {
                    option = mapper.Map<ProductVariantOption>(optionDto);
                    option.ProductVariantId = variantId;
                    variant.Options.Add(option);
                }
                else
                {
                    mapper.Map(optionDto, option);
                }
            }
        }

        await unitOfWork.Products.UpdateProductVariantAsync(variant);
    }

    /// <summary>
    /// Deletes an existing product variant.
    /// </summary>
    /// <param name="variantId">The ID of the product variant to delete.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task DeleteProductVariantAsync(int variantId)
    {
        var variant = await unitOfWork.Products.GetProductVariantByIdAsync(variantId);
        if (variant == null)
        {
            throw new NotFoundException("Product variant not found.");
        }

        await unitOfWork.Products.DeleteProductVariantAsync(variant);
    }
}