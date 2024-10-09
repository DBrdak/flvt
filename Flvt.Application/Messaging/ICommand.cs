using Flvt.Domain.Primitives.Responses;
using MediatR;

namespace Flvt.Application.Messaging;

public interface ICommand : IRequest<Result>, IBaseCommand;

public interface ICommand<TReponse> : IRequest<Result<TReponse>>, IBaseCommand;

public interface IBaseCommand;