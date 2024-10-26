namespace Flvt.Infrastructure.Data.DataModels;

internal interface IDataModel<out TDomainModel>
{
    Type GetDomainModelType();
    TDomainModel ToDomainModel();
}