using Amazon.DynamoDBv2.DocumentModel;
using Flvt.Domain.Filters;
using Flvt.Domain.Primitives.Advertisements;
using Flvt.Domain.Subscribers;
using Flvt.Infrastructure.Data.DataModels.Exceptions;
using Flvt.Infrastructure.Data.DataModels.Filters;
using System.Reflection;
using System.Xml.Linq;

namespace Flvt.Infrastructure.Data.DataModels.Subscribers;

internal sealed class SubscriberDataModel : IDataModel<Subscriber>
{
    public string Email { get; init; }
    public string Tier { get; init; }
    public string CountryCode { get; init; }
    public IEnumerable<string> Filters { get; init; }

    public SubscriberDataModel(Subscriber original)
    {
        Email = original.Email.Value;
        Tier = original.Tier.Value;
        CountryCode = original.Country.Code;
        Filters = original.Filters;
    }
    public SubscriberDataModel(Document doc)
    {
        Email = doc[nameof(Email)];
        Tier = doc[nameof(Tier)];
        CountryCode = doc[nameof(Country)];
        Filters = doc[nameof(Filters)].AsArrayOfString();
    }

    public static SubscriberDataModel FromDomainModel(Subscriber domainModel) => new(domainModel);

    public static SubscriberDataModel FromDocument(Document document) => new(document);

    public Type GetDomainModelType() => typeof(Subscriber);

    public Subscriber ToDomainModel()
    {
        var email = Domain.Subscribers.Email.Create(Email).Value;
        var country = Country.Create(CountryCode).Value;
        var tier = SubscribtionTier.Create(Tier).Value;

        return Activator.CreateInstance(
                   typeof(Subscriber),
                   BindingFlags.Instance | BindingFlags.NonPublic,
                   null,
                   [
                       email,
                       country,
                       tier,
                       Filters
                   ],
                   null) as Subscriber ??
               throw new DataModelConversionException(typeof(Subscriber));
    }
}