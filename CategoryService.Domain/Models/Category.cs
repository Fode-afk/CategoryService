using CategoryService.Domain.DomainEvents;
using CategoryService.Domain.Errors;
using CategoryService.Domain.Primitives;
using CategoryService.Domain.RequestData;
using CategoryService.Domain.ValueObjects;
using migApp.Shared.Results;
using static migApp.Shared.Results.ResultFactory;

namespace CategoryService.Domain.Models;

public sealed class Category : AggregateRoot
{
    private Category() : base(Guid.Empty) { }

    private Category(
        Guid id,
        CategoryName name,
        Slug slug,
        SeoMetadata seoMetadata,
        ImageUrl? image,
        Guid? parentCategoryId,
        int sortOrder,
        bool isActive,
        DateTimeOffset createdAt) : base(id)
    {
        Name = name;
        Slug = slug;
        SeoMetadata = seoMetadata;
        Image = image;
        ParentCategoryId = parentCategoryId;
        SortOrder = sortOrder;
        IsActive = isActive;
        CreatedAt = createdAt;
    }

    public CategoryName Name { get; private set; }
    public Slug Slug { get; private set; }
    public SeoMetadata SeoMetadata { get; private set; }
    public ImageUrl? Image { get; private set; }

    public Guid? ParentCategoryId { get; private set; }
    public bool IsRoot => !ParentCategoryId.HasValue;

    public int SortOrder { get; private set; }
    public bool IsActive { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public DateTimeOffset? DeletedAt { get; private set; }

    public static IResult<Category> Create(
        CategoryCreationData data,
        Guid? parentCategoryId,
        int sortOrder,
        bool isActive,
        DateTimeOffset now)
    {
        var category = new Category(
            Guid.NewGuid(),
            data.Name, 
            data.Slug, 
            data.SeoMetadata,
            data.Image,
            parentCategoryId, 
            sortOrder,
            isActive,
            now);

        category.RaiseDomainEvent(new CategoryCreatedDomainEvent(
            category.Id,
            category.Name,
            category.Slug,
            category.IsActive,
            category.Version));

        return Ok(category);
    }

    public static IResult<Category> CreateWithId(
        Guid id,
        CategoryCreationData data,
        Guid? parentCategoryId,
        int sortOrder,
        bool isActive,
        DateTimeOffset now)
    {
        var category = new Category(
            id, 
            data.Name,
            data.Slug,
            data.SeoMetadata,
            data.Image,
            parentCategoryId, 
            sortOrder,
            isActive, 
            now);

        category.RaiseDomainEvent(new CategoryCreatedDomainEvent(
            category.Id,
            category.Name,
            category.Slug,
            category.IsActive, 
            category.Version));

        return Ok(category);
    }

    public IResult UpdateInfo(
        CategoryUpdateInfoData data,
        DateTimeOffset now)
    {
        if (data.Name == Name &&
            data.Slug == Slug &&
            data.SeoMetadata == SeoMetadata &&
            data.Image == Image)
            return Ok();

        Name = data.Name;
        Slug = data.Slug;
        SeoMetadata = data.SeoMetadata;
        Image = data.Image;
        UpdatedAt = now;

        IncreaseVersion();

        RaiseDomainEvent(new CategoryInfoUpdatedDomainEvent(
            Id,
            Name,
            Slug,
            Version));

        return Ok();
    }

    public IResult MoveTo(
        Guid? newParentCategoryId,
        DateTimeOffset now)
    {
        if (newParentCategoryId == Id)
            return Fail(CategoryErrors.CannotBeOwnParent());

        if (!ParentCategoryId.HasValue && newParentCategoryId.HasValue is false)
            return Ok();

        if (ParentCategoryId == newParentCategoryId)
            return Ok();

        ParentCategoryId = newParentCategoryId;
        UpdatedAt = now;

        IncreaseVersion();

        RaiseDomainEvent(new CategoryMovedDomainEvent());

        return Ok();
    }

    public IResult Reorder(int sortOrder, DateTimeOffset now)
    {
        if (SortOrder == sortOrder)
            return Ok();

        SortOrder = sortOrder;
        UpdatedAt = now;

        IncreaseVersion();

        RaiseDomainEvent(new CategoryReorderedDomainEvent());

        return Ok();
    }

    public IResult Activate(DateTimeOffset now)
    {
        if (IsActive)
            return Ok();

        IsActive = true;
        UpdatedAt = now;

        IncreaseVersion();

        RaiseDomainEvent(new CategoryActivatedDomainEvent(
            Id,
            IsActive,
            Version));

        return Ok();
    }

    public IResult Deactivate(DateTimeOffset now)
    {
        if (!IsActive)
            return Ok();

        IsActive = false;
        UpdatedAt = now;

        IncreaseVersion();

        RaiseDomainEvent(new CategoryDeactivatedDomainEvent(
            Id,
            IsActive,
            Version));

        return Ok();
    }

    public IResult Delete(DateTimeOffset now)
    {
        if (DeletedAt != null)
            return Ok();

        DeletedAt = now;
        IsActive = false;
        UpdatedAt = now;

        IncreaseVersion();

        RaiseDomainEvent(new CategoryDeletedDomainEvent(Id));

        return Ok();
    }
}