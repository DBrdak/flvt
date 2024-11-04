using Flvt.Domain.Primitives.Responses;
using MediatR;

namespace Flvt.Application.Messaging;

public interface IQuery<TReponse> : IRequest<Result<TReponse>>, IBaseQuery;

public interface IBaseQuery;