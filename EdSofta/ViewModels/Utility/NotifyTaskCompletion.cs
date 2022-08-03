using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdSofta.ViewModels.Utility
{
    public delegate void TaskCompletedHandler();
    public sealed class NotifyTaskCompletion<TResult> : INotifyPropertyChanged
    {
        //public event TaskCompletedHandler OnTaskCompleted;
        public EventHandler<TaskCompletedEventArgs> EventTaskCompleted;

        public NotifyTaskCompletion(Task<TResult> task)
        {
            Task = task;
            if (!task.IsCompleted)
            {
                var _ = WatchTaskAsync(task);
            }
        }

        public NotifyTaskCompletion(Task<TResult> task, EventHandler<TaskCompletedEventArgs> handler)
        {
            Task = task;
            EventTaskCompleted += handler;

            if (task.IsCompleted)
            {
                OnTaskCompleted();
            }

            if (!task.IsCompleted)
            {
                var _ = WatchTaskAsync(task);
            }

            

        }
        private async Task WatchTaskAsync(Task task)
        {
            try
            {
                await task;
                if (task.IsCompleted)
                {
                    OnTaskCompleted();
                }
            }
            catch(Exception e)
            {
                var message = e.Message;
            }
            var propertyChanged = PropertyChanged;
            if (propertyChanged == null) return;

            propertyChanged(this, new PropertyChangedEventArgs("Status"));
            propertyChanged(this, new PropertyChangedEventArgs("IsCompleted"));
            propertyChanged(this, new PropertyChangedEventArgs("IsNotCompleted"));
            if (task.IsCanceled)
            {
                propertyChanged(this, new PropertyChangedEventArgs("IsCanceled"));
            }
            else if (task.IsFaulted)
            {
                propertyChanged(this, new PropertyChangedEventArgs("IsFaulted"));
                propertyChanged(this, new PropertyChangedEventArgs("Exception"));
                propertyChanged(this,
                  new PropertyChangedEventArgs("InnerException"));
                propertyChanged(this, new PropertyChangedEventArgs("ErrorMessage"));
            }
            else
            {
                propertyChanged(this,
                  new PropertyChangedEventArgs("IsSuccessfullyCompleted"));
                propertyChanged(this, new PropertyChangedEventArgs("Result"));
            }

            
        }

        public void OnTaskCompleted()
        {
            var tmpEvent = EventTaskCompleted;
            tmpEvent?.Invoke(this, new TaskCompletedEventArgs()
            {
               Tag = ""
            });
        }

        public Task<TResult> Task { get; private set; }
        public TResult Result =>
            (Task.Status == TaskStatus.RanToCompletion) ?
                Task.Result : default(TResult);

        public TaskStatus Status => Task.Status;
        public bool IsCompleted => Task.IsCompleted;
        public bool IsNotCompleted => !Task.IsCompleted;

        public bool IsSuccessfullyCompleted =>
            Task.Status ==
            TaskStatus.RanToCompletion;

        public bool IsCanceled => Task.IsCanceled;
        public bool IsFaulted => Task.IsFaulted;
        public AggregateException Exception => Task.Exception;

        public Exception InnerException =>
            Exception?.InnerException;

        public string ErrorMessage =>
            InnerException?.Message;

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
