// Copyright (c) Rodel. All rights reserved.

namespace RodelAgent.UI.ViewModels;

/// <summary>
/// 视图模型基类.
/// </summary>
public abstract class ViewModelBase : ObservableObject
{
    /// <summary>
    /// 为异步命令添加错误回调.
    /// </summary>
    /// <param name="handler">错误回调.</param>
    /// <param name="commands">命令集.</param>
    protected static void AttachExceptionHandlerToAsyncCommand(Action<Exception> handler, params IAsyncRelayCommand[] commands)
    {
        foreach (var command in commands)
        {
            command.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(AsyncRelayCommand.ExecutionTask) &&
                    ((IAsyncRelayCommand)s).ExecutionTask is Task task &&
                    task.Exception is AggregateException exception)
                {
                    exception.Handle(ex =>
                    {
                        handler(ex);
                        return true;
                    });
                }
            };
        }
    }

    /// <summary>
    /// 为异步命令的 <see cref="AsyncRelayCommand.IsRunning"/> 属性添加回调.
    /// </summary>
    /// <param name="handler">回调.</param>
    /// <param name="commands">命令集合.</param>
    protected static void AttachIsRunningToAsyncCommand(Action<bool> handler, params IAsyncRelayCommand[] commands)
    {
        foreach (var command in commands)
        {
            command.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(AsyncRelayCommand.IsRunning))
                {
                    handler(command.IsRunning);
                }
            };
        }
    }
}

/// <summary>
/// 数据视图模型基类.
/// </summary>
/// <typeparam name="TData">数据类型.</typeparam>
public abstract partial class ViewModelBase<TData> : ViewModelBase
    where TData : class
{
    [ObservableProperty]
    private TData _data;

    /// <summary>
    /// Initializes a new instance of the <see cref="ViewModelBase{TData}"/> class.
    /// </summary>
    protected ViewModelBase(TData data) => Data = data;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is ViewModelBase<TData> @base && EqualityComparer<TData>.Default.Equals(Data, @base.Data);

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Data);
}
