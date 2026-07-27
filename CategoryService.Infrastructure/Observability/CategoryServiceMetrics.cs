using CategoryService.Application.Interfaces.Metrics;
using System.Diagnostics.Metrics;

namespace CategoryService.Infrastructure.Observability;

public sealed class CategoryServiceMetrics : IDisposable, ICategoryMetrics
{
    public const string MeterName = "CategoryService";
    private readonly Meter _meter;

    private readonly Counter<long> _categoriesCreated;
    private readonly Counter<long> _categoriesUpdated;
    private readonly Counter<long> _categoriesMoved;
    private readonly Counter<long> _categoriesReordered;
    private readonly Counter<long> _categoriesActivated;
    private readonly Counter<long> _categoriesDeactivated;
    private readonly Counter<long> _categoriesCascadeActivated;
    private readonly Counter<long> _categoriesCascadeDeactivated;
    private readonly Counter<long> _categoriesDeleted;
    private readonly Counter<long> _categoriesCascadeDeleted;
    private int _activeCategoriesCount_value;
    private readonly ObservableGauge<int> _activeCategoriesCount;

    private readonly Counter<long> _cacheHits;
    private readonly Counter<long> _cacheMisses;
    private readonly Counter<long> _cacheInvalidations;

    private readonly Counter<long> _handlerErrors;
    private readonly Histogram<double> _handlerDuration;

    public CategoryServiceMetrics()
    {
        _meter = new Meter(MeterName);

        _categoriesCreated = _meter.CreateCounter<long>(
            "category.created");

        _categoriesUpdated = _meter.CreateCounter<long>(
            "category.updated");

        _categoriesMoved = _meter.CreateCounter<long>(
           "category.moved");

        _categoriesReordered = _meter.CreateCounter<long>(
           "category.reordered");

        _categoriesActivated = _meter.CreateCounter<long>(
           "category.activated");

        _categoriesDeactivated = _meter.CreateCounter<long>(
           "category.deactivated");

        _categoriesCascadeActivated = _meter.CreateCounter<long>(
           "category.cascade.activated");

        _categoriesCascadeDeactivated = _meter.CreateCounter<long>(
           "category.cascade.deactivated");

        _categoriesDeleted = _meter.CreateCounter<long>(
            "category.deleted");

        _categoriesCascadeDeleted = _meter.CreateCounter<long>(
            "category.cascade.deleted");

        _categoriesCascadeDeactivated = _meter.CreateCounter<long>(
           "category.cascade.deactivated");

        _activeCategoriesCount = _meter.CreateObservableGauge(
            "category.active",
            () => _activeCategoriesCount_value);

        _cacheHits = _meter.CreateCounter<long>(
            "category.cache.hits");

        _cacheMisses = _meter.CreateCounter<long>(
            "category.cache.misses");

        _cacheInvalidations = _meter.CreateCounter<long>(
            "category.cache.invalidations",
            description: "Number of RemoveByTag operations triggered");

        _handlerErrors = _meter.CreateCounter<long>(
            "category.handlers.errors");

        _handlerDuration = _meter.CreateHistogram<double>(
            "category.handlers.duration",
            unit: "ms");
    }

    public void RecordCategoryCreated() => _categoriesCreated.Add(1);

    public void RecordCategoryUpdated() => _categoriesUpdated.Add(1);

    public void RecordCategoryMoved() => _categoriesMoved.Add(1);

    public void RecordCategoryReordered() => _categoriesReordered.Add(1);

    public void RecordCategoryActivated() => _categoriesActivated.Add(1);

    public void RecordCategoryDeactivated() => _categoriesDeactivated.Add(1);

    public void RecordCascadeActivation(int affectedCount) => 
        _categoriesCascadeActivated.Add(affectedCount);

    public void RecordCascadeDeactivation(int affectedCount) =>
        _categoriesCascadeDeactivated.Add(affectedCount);

    public void RecordCategoryDeleted() => _categoriesDeleted.Add(1);

    public void RecordCascadeDeletion(int affectedCount) =>
        _categoriesCascadeDeleted.Add(affectedCount);

    public void SetActiveCategoriesCount(int count) =>
        _activeCategoriesCount_value = count;

    public void RecordCacheHitOrMiss(string cacheKeyPrefix, bool wasHit)
    {
        if (wasHit)
            _cacheHits.Add(1, new KeyValuePair<string, object?>("key_prefix", cacheKeyPrefix));
        else
            _cacheMisses.Add(1, new KeyValuePair<string, object?>("key_prefix", cacheKeyPrefix));
    }

    public void RecordCacheInvalidation(string tag) =>
        _cacheInvalidations.Add(1, new KeyValuePair<string, object?>("tag_type", tag));

    public void RecordHandlerError(string handlerName, string handlerType) =>
        _handlerErrors.Add(1,
            new KeyValuePair<string, object?>("handler", handlerName),
            new KeyValuePair<string, object?>("type", handlerType));

    public void RecordHandlerDuration(double ms, string handlerName, string handlerType) =>
        _handlerDuration.Record(ms,
            new KeyValuePair<string, object?>("handler", handlerName),
            new KeyValuePair<string, object?>("type", handlerType));

    public void Dispose() => _meter.Dispose();
}