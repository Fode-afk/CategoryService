namespace CategoryService.Application.Interfaces.Metrics;

public interface ICategoryMetrics
{
    void RecordCategoryCreated();
    void RecordCategoryUpdated();
    void RecordCategoryMoved();
    void RecordCategoryReordered();
    void RecordCategoryActivated();
    void RecordCategoryDeactivated();
    void RecordCascadeActivation(int affectedCount);
    void RecordCascadeDeactivation(int affectedCount);
    void RecordCategoryDeleted();
    void RecordCascadeDeletion(int affectedCount);
    void SetActiveCategoriesCount(int count);
    void RecordCacheHitOrMiss(string cacheKeyPrefix, bool wasHit);
    void RecordCacheInvalidation(string tag);
    void RecordHandlerError(string handlerName, string handlerType);
    void RecordHandlerDuration(double ms, string handlerName, string handlerType);
}