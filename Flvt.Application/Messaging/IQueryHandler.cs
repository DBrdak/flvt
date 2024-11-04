using Flvt.Domain.Primitives.Responses;
using MediatR;

namespace Flvt.Application.Messaging;

public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>;