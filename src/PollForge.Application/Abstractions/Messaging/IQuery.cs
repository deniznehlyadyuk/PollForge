using MediatR;
using PollForge.SharedKernel;

namespace PollForge.Application.Abstractions.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>;
