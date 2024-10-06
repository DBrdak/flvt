using Flvt.Domain.Primitives;
using MediatR;

namespace Flvt.Application.Messaging;

public interface ICommand : IRequest<Result>, IBaseCommand;

public interface ICommand<TReponse> : IRequest<Result<TReponse>>, IBaseCommand;

public interface IBaseCommand;